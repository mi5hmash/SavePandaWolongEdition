using SavePandaWolongEditionCore.Helpers;
using System.Text.RegularExpressions;
using SavePandaWolongEditionCore.Models.SaveData;
using static SavePandaWolongEditionCore.Helpers.IoHelpers;
using static SavePandaWolongEditionCore.Helpers.ISimpleMediator;
using static SavePandaWolongEditionCore.Helpers.SimpleLogger;
using System.Text.Json;

namespace SavePandaWolongEditionCore;

public class Core
{

    #region PATHS

    public static string RootPath => AppDomain.CurrentDomain.BaseDirectory;

    private const string BackupFolder = "_BACKUP";
    public static string BackupPath => Path.Combine(RootPath, BackupFolder);

    private const string JsonFile = "savedata.json";
    public static string JsonFilePath => Path.Combine(RootPath, JsonFile);

    private static string PathPattern => @$"\{Path.DirectorySeparatorChar}(\d+)\{Path.DirectorySeparatorChar}.+?\{Path.DirectorySeparatorChar}.+?\.(BIN|bin)$";

    #endregion

    #region BACKUP

    /// <summary>
    /// Determines if backup is enabled.
    /// </summary>
    public bool BackupEnabled { get; private set; } = true;

    public void SetBackupEnabled(bool boolean)
    {
        if (IsBusy) return;
        BackupEnabled = boolean;
    }

    public void ToggleBackupEnabled()
    {
        if (IsBusy) return;
        BackupEnabled ^= true;
    }

    /// <summary>
    /// Tries to make a backup of a file.
    /// </summary>
    /// <param name="simpleBackup"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static DialogAnswer MakeBackup(string filePath, SimpleBackup simpleBackup)
    {
        do
        {
            if (TryToMakeBackup(filePath, simpleBackup)) return DialogAnswer.Continue;
            // ask the user if they want to try again
            var dialogResult = _mediator.Ask($"""Failed to backup the file: "{filePath}".{Environment.NewLine}The file cannot be found, or a file with the same name exists in the target location and is used by another program.{Environment.NewLine}Would you like to try again?""", "Failed to backup the file", QuestionOptions.AbortRetryIgnore, DialogType.Exclamation);
            if (dialogResult == DialogAnswer.Retry) continue;
            return dialogResult;
        } while (true);

        static bool TryToMakeBackup(string filePath, SimpleBackup simpleBackup)
        {
            try
            {
                simpleBackup.Backup(filePath);
            }
            catch { return false; }
            return true;
        }
    }

    /// <summary>
    /// Opens the Backup directory.
    /// </summary>
    public static void OpenBackupDirectory()
        => OpenDirectory(BackupPath);

    /// <summary>
    /// Opens the directory where the json file was exported.
    /// </summary>
    public static void OpenJsonDirectory()
        => OpenDirectory(JsonFilePath);

    #endregion

    #region BUSY_LOCK

    public bool IsBusy { get; private set; }

    #endregion

    #region LOGGER

    private readonly SimpleLogger _logger;

    public void ActivateLogger() => _logger.NewLogFile();

    #endregion

    #region PROGRESS

    private readonly IProgress<string> _pText;
    private readonly IProgress<int> _pValue;

    /// <summary>
    /// Reports progress.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="value"></param>
    private void ReportProgress(string text, int value)
    {
        _pText.Report(text);
        _pValue.Report(value);
    }

    #endregion
    
    #region CONSTRUCTOR
    
    private static ISimpleMediator _mediator = null!;
    private readonly SaveDataDeencryptor _deencryptor = new();
    
    /// <summary>
    /// Constructs new <see cref="Core"/> class.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="pText"></param>
    /// <param name="pValue"></param>
    /// <param name="logger"></param>
    public Core(ISimpleMediator mediator, IProgress<string> pText, IProgress<int> pValue, SimpleLogger logger)
    {
        _mediator = mediator;
        _pText = pText;
        _pValue = pValue;
        _logger = logger;
        InputFilePath = "";
        InitializeComponent();
    }

    /// <summary>
    /// Initialize component.
    /// </summary>
    private static void InitializeComponent()
    {
        // create directories
        CreateDirectories();
    }

    #endregion

    #region IO

    /// <summary>
    /// Creates necessary directories.
    /// </summary>
    private static void CreateDirectories()
    {
        Directory.CreateDirectory(BackupPath);
    }

    /// <summary>
    /// Checks whether the file at the given path exists.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static bool DoesFileExists(string filePath)
    {
        if (File.Exists(filePath)) return true;
        _mediator.Inform($"""File: "{filePath}" does not exists.""", "Error", DialogType.Error);
        return false;
    }

    /// <summary>
    /// Tries to write an array of bytes to a file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileData"></param>
    /// <returns></returns>
    private static DialogAnswer WriteBytesToFile(string filePath, ReadOnlySpan<byte> fileData)
    {
        do
        {
            if (TryWriteAllBytes(filePath, fileData)) return DialogAnswer.Continue;
            // ask the user if they want to try again
            var dialogResult = _mediator.Ask($"""Failed to save the file: "{filePath}".{Environment.NewLine}It may be currently in use by another program.{Environment.NewLine}Would you like to try again?""", "Failed to save the file", QuestionOptions.AbortRetryIgnore, DialogType.Exclamation);
            if (dialogResult == DialogAnswer.Retry) continue;
            return dialogResult;
        } while (true);

        static bool TryWriteAllBytes(string fPath, ReadOnlySpan<byte> bytes)
        {
            try
            {
                File.WriteAllBytes(fPath, bytes.ToArray());
            }
            catch { return false; }
            return true;
        }
    }
    
    #endregion

    #region STEAM_ID

    public uint SteamId { get; private set; }

    /// <summary>
    /// Validates a <paramref name="steamId"/> string and sets the <see cref="SteamId"/> property.
    /// </summary>
    /// <param name="steamId"></param>
    /// <param name="verbose"></param>
    public void SetSteamId(string steamId, bool verbose = false)
    { 
        if (IsBusy) return;
        var result = ulong.TryParse(steamId, out var parsed);
        if (result) SteamId = (uint)parsed;
        if (!verbose) ReportProgress(result ? "The entered SteamID is correct." : "The entered SteamID was invalid.", 0);
    }

    /// <summary>
    /// Extracts SteamID from a directory path.
    /// </summary>
    /// <param name="directoryPath"></param>
    private void ExtractSteamIdFromPathIfValid(string directoryPath)
    {
        var match = Regex.Match(directoryPath, PathPattern);
        if (match.Success) SetSteamId(match.Groups[1].Value, true);
    }

    #endregion

    #region INPUT_DIRECTORY

    public string InputFilePath { get; private set; }

    /// <summary>
    /// Validates a <paramref name="inputPath"/> string and sets the <see cref="InputFilePath"/> property.
    /// </summary>
    /// <param name="inputPath"></param>
    public void SetInputFilePath(string inputPath)
    {
        if (IsBusy) return;

        // checking for forbidden directory
        if (IsForbiddenDirectory(BackupPath)) return;
        
        // checking if file exists
        var result = File.Exists(inputPath);
        if (result)
        {
            InputFilePath = inputPath;
            ExtractSteamIdFromPathIfValid(inputPath);
        }
        ReportProgress(result ? "The entered Input File Path is correct." : "The entered Input File Path was invalid.", 0);
        return;

        bool IsForbiddenDirectory(string path)
        {
            if (!inputPath.Contains(path)) return false;
            _mediator.Inform($"The entered path:\n\"{path}\", \ncannot be used as the Input File Path. \nThe path has not been updated.", "File placed in forbidden directory", DialogType.Exclamation);
            return true;
        }
    }

    #endregion

    #region OPERATIONS

    private delegate void OperationDelegate(ISaveDataFile saveDataFile);

    private enum OperationType
    {
        Encryption,
        Decryption,
        Resigning,
        JsonExport,
        JsonImport
    }

    public Task DecryptAsync()
        => ProcessAsyncOperation(OperationType.Decryption, Decrypt);

    public Task EncryptAsync()
        => ProcessAsyncOperation(OperationType.Encryption, Encrypt);

    public Task ResignAsync()
        => ProcessAsyncOperation(OperationType.Resigning, Resign);

    public Task ExportJsonAsync()
        => ProcessAsyncOperation(OperationType.JsonExport, Decrypt);

    public Task ImportJsonAsync()
        => ProcessAsyncOperation(OperationType.JsonImport, Encrypt);

    private async Task ProcessAsyncOperation(OperationType operationType, OperationDelegate operationDelegate)
    {
        if (IsBusy) return;
        IsBusy = true;

        _logger.Log(LogSeverity.Information, $"Provided Steam32_ID: {SteamId}");
        _logger.Log(LogSeverity.Information, "FileName | MD5_Checksum | IsEncrypted | UserID | Platform");
        await AsyncOperation(operationType, operationDelegate);
        _logger.Log(LogSeverity.Information, $"{operationType} complete.");
        IsBusy = false;
    }

    private Task AsyncOperation(OperationType operationType, OperationDelegate operationDelegate)
    {
        return Task.Run(() =>
        {
            string message;

            // check if input file exists
            if (!DoesFileExists(InputFilePath))
            {
                message = $"File does not exits: {InputFilePath}";
                _logger.Log(LogSeverity.Error, message);
                ReportProgress(message, 0);
                return;
            }
            
            // try to load the file
            string platform;
            message = "Trying to load file...";
            ReportProgress(message, 0);
            ISaveDataFile saveDataFile = new PcSaveDataFile(_deencryptor);
            var result = saveDataFile.SetFileData(InputFilePath);
            if (result.Result) platform = "PC (STEAM)";
            else 
            {
                saveDataFile = new Ps4SaveDataFile(_deencryptor);
                result = saveDataFile.SetFileData(InputFilePath);
                if (result.Result) platform = "PS4";
                else
                {
                    message = result.Description;
                    _logger.Log(LogSeverity.Error, message);
                    ReportProgress(message, 100);
                    return;
                }
            }

            // log info about the input file
            _logger.LogDebug(LogSeverity.Information, $"{Path.GetFileName(InputFilePath)} | {Md5HashFromFile(InputFilePath)} | {saveDataFile.IsEncrypted} | {saveDataFile.GetUserId()} | {platform}");

            // check operation type and adjust to it
            DialogAnswer writeResult;
            switch (operationType)
            {
                case OperationType.JsonExport:
                    writeResult = WriteBytesToFile(JsonFilePath, saveDataFile.ExportJson());
                    message = writeResult switch
                    {
                        DialogAnswer.Continue => $"{operationType} done.",
                        _ => $"{operationType} operation was aborted."
                    };
                    _logger.Log(LogSeverity.Information, message);
                    ReportProgress(message, 100);
                    return;
                case OperationType.JsonImport:
                    byte[] compactJsonBytes;
                    try
                    {
                        // try to serialize the JSON document into a compact byte array
                        using var fs = File.OpenRead(JsonFilePath);
                        var jsonDoc = JsonDocument.Parse(fs);
                        using var ms = new MemoryStream();
                        using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false })) 
                            jsonDoc.RootElement.WriteTo(writer);
                        compactJsonBytes = ms.ToArray();
                    }
                    catch
                    {
                        message = $"{operationType} operation failed. Couldn't serialize the json file.";
                        _logger.Log(LogSeverity.Error, message);
                        ReportProgress(message, 0);
                        return;
                    }
                    var boolResult = saveDataFile.ImportJson(compactJsonBytes);
                    if (!boolResult.Result)
                    {
                        message = $"{operationType} operation failed. {boolResult.Description}";
                        _logger.Log(LogSeverity.Error, message);
                        ReportProgress(message, 0);
                        return;
                    }
                    break;
                case OperationType.Resigning:
                    if (platform == "PS4")
                    {
                        message = $"{operationType} operation failed. PS4 SaveData Files can't be resigned with this tool.";
                        _logger.Log(LogSeverity.Error, message);
                        ReportProgress(message, 100);
                        return;
                    }
                    break;
                case OperationType.Decryption:
                    if (!saveDataFile.IsEncrypted)
                    {
                        message = "The file is already decrypted.";
                        _logger.Log(LogSeverity.Warning, message);
                        ReportProgress(message, 100);
                        return;
                    }
                    break;
                case OperationType.Encryption:
                default:
                    if (saveDataFile.IsEncrypted)
                    {
                        message = "The file is already encrypted.";
                        _logger.Log(LogSeverity.Warning, message);
                        ReportProgress(message, 100);
                        return;
                    }
                    break;
            }

            // initialize backup
            SimpleBackup simpleBackup = new(BackupPath);
            if (BackupEnabled) simpleBackup.NewBackup();

            // run operation
            operationDelegate(saveDataFile);

            // backup file before overwriting it
            if (BackupEnabled)
            {
                var backupResult = MakeBackup(InputFilePath, simpleBackup);
                switch (backupResult)
                {
                    case DialogAnswer.Continue:
                        break;
                    default:
                        message = $"{operationType} operation was aborted.";
                        _logger.Log(LogSeverity.Information, message);
                        ReportProgress(message, 100);
                        return;
                }
            }
            
            // save file
            var fileData = saveDataFile.GetFileData();
            writeResult = WriteBytesToFile(InputFilePath, fileData);
            switch (writeResult)
            {
                case DialogAnswer.Continue:
                    break;
                default:
                    message = $"{operationType} operation was aborted.";
                    _logger.Log(LogSeverity.Information, message);
                    ReportProgress(message, 100);
                    return;
            }

            // log info about the output file
            _logger.LogDebug(LogSeverity.Information, $"{Path.GetFileName(InputFilePath)} | {Md5HashFromFile(InputFilePath)} | {saveDataFile.IsEncrypted} | {saveDataFile.GetUserId()} | {platform}");

            // finalize backup
            if (BackupEnabled) simpleBackup.FinalizeBackup();

            // toast
            message = $"{operationType} done.";
            _logger.Log(LogSeverity.Information, message);
            ReportProgress(message, 100);
        });
    }

    private static void Decrypt(ISaveDataFile saveDataFile)
        => saveDataFile.DecryptData();

    private static void Encrypt(ISaveDataFile saveDataFile)
        => saveDataFile.EncryptData();

    private void Resign(ISaveDataFile saveDataFile)
        => saveDataFile.ResignFile(SteamId);
    
    #endregion
}