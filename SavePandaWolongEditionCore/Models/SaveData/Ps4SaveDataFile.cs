using SavePandaWolongEditionCore.Helpers;

namespace SavePandaWolongEditionCore.Models.SaveData;

public class Ps4SaveDataFile(SaveDataDeencryptor deencryptor) : ISaveDataFile
{
    /// <summary>
    /// UserId.
    /// </summary>
    public uint UserId { get; set; }

    /// <summary>
    /// Data of the <see cref="Ps4SaveDataFile"/>.
    /// </summary>
    public byte[] Data { get; set; } = [];
    
    /// <summary>
    /// Deencryptor instance.
    /// </summary>
    public SaveDataDeencryptor Deencryptor { get; } = deencryptor;

    /// <summary>
    /// Stores the encryption state of the current file.
    /// </summary>
    public bool IsEncrypted { get; private set; }

    /// <summary>
    /// Checks if the file is encrypted and sets <see cref="IsEncrypted"/>.
    /// </summary>
    /// <returns></returns>
    public bool CheckIfFileIsEncrypted()
    {
        IsEncrypted = Data[8] != 0x7B && Data[9] != 0x22;
        return IsEncrypted;
    }

    /// <summary>
    /// Loads a '*.bin' archive of <see cref="Ps4SaveDataFile"/> type into the existing object.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public BoolResult SetFileData(string filePath)
    {
        try
        {
            Data = File.ReadAllBytes(filePath);
            CheckIfFileIsEncrypted();
            return new BoolResult(true);
        }
        catch { /* ignored */ }
        return new BoolResult(false, "Error on trying to open the file.");
    }

    /// <summary>
    /// Gets an existing object of a <see cref="Ps4SaveDataFile"/> type as byte array.
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<byte> GetFileData()
        => Data;

    /// <summary>
    /// Gets UserId.
    /// </summary>
    /// <returns></returns>
    public string GetUserId()
        => "0";

    /// <summary>
    /// Sets UserId.
    /// </summary>
    /// <returns></returns>
    public void SetUserId(uint userId)
        => UserId = userId;
    
    /// <summary>
    /// Specifies the mode for decrypting or encrypting data.
    /// </summary>
    private enum DeencryptMode
    {
        Decryption,
        Encryption
    }
    
    /// <summary>
    /// Encrypts or Decrypts <see cref="Data"/>.
    /// </summary>
    /// <param name="deencryptMode"></param>
    private void DeencryptData(DeencryptMode deencryptMode)
    {
        Span<byte> dataSpan = Data;
        var dataSpanSlice = dataSpan[8..];
        if (deencryptMode == DeencryptMode.Decryption)
            Deencryptor.Decrypt(ref dataSpanSlice);
        else
            Deencryptor.Encrypt(ref dataSpanSlice);
        CheckIfFileIsEncrypted();
    }

    /// <summary>
    /// Encrypts <see cref="Data"/>.
    /// </summary>
    public void EncryptData()
        => DeencryptData(DeencryptMode.Encryption);

    /// <summary>
    /// Decrypts <see cref="Data"/>.
    /// </summary>
    public void DecryptData()
        => DeencryptData(DeencryptMode.Decryption);

    /// <summary>
    /// Signs the file with a new signature.
    /// </summary>
    /// <param name="userId"></param>
    public void ResignFile(uint userId)
    {
        throw new InvalidOperationException("PS4 SaveData Files can't be resigned with this tool.");
    }

    /// <summary>
    /// Imports <paramref name="jsonData"/> into <see cref="Data"/>.
    /// </summary>
    /// <param name="jsonData"></param>
    public BoolResult ImportJson(ReadOnlySpan<byte> jsonData)
    {
        if (IsEncrypted) DecryptData();
        Span<byte> dataSpan = Data;
        if (dataSpan.Length <= 64) return new BoolResult(false, "The input file length is too short.");
        dataSpan = dataSpan[8..^56];
        if (jsonData.Length > dataSpan.Length) return new BoolResult(false, "The file to import is longer then the input file.");
        dataSpan.Clear();
        jsonData.CopyTo(dataSpan);
        return new BoolResult(true, "Data has been imported successfully.");
    }

    /// <summary>
    /// Cuts JSON out of the <see cref="Data"/>.
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<byte> ExportJson()
    {
        if (IsEncrypted) DecryptData();
        ReadOnlySpan<byte> dataSpan = Data;
        if (dataSpan.Length >= 8) dataSpan = dataSpan[8..^56];
        var eof = dataSpan.LastIndexOf<byte>(0x7D) + 1;
        return dataSpan[..eof];
    }

    /// <summary>
    /// Returns false if the file structure does not make sense.
    /// </summary>
    /// <returns></returns>
    public BoolResult CheckIntegrity()
    {
        if ((uint)(Data[0] + Data[4] + Data[5] + Data[6] + Data[7]) != 0) return new BoolResult(false, "Invalid file header!");
        return new BoolResult(true);
    }
}