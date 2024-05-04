using System.Runtime.InteropServices;
using SavePandaWolongEditionCore.Helpers;

namespace SavePandaWolongEditionCore.Models.SaveData;

public class PcSaveDataFile(SaveDataDeencryptor deencryptor) : ISaveDataFile
{
    /// <summary>
    /// Header of the <see cref="PcSaveDataFile"/>.
    /// </summary>
    public PcSaveDataHeader Header { get; set; } = new();

    /// <summary>
    /// A sequence of bytes between Header and Data.
    /// </summary>
    public byte[] HeaderPadding { get; set; } = [];

    /// <summary>
    /// Data of the <see cref="PcSaveDataFile"/>.
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
        IsEncrypted = (uint)(Data[0] + Data[4] + Data[5] + Data[6] + Data[7]) != 0;
        return IsEncrypted;
    }

    /// <summary>
    /// Loads a '*.bin' archive of <see cref="PcSaveDataFile"/> type into the existing object.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public BoolResult SetFileData(string filePath)
    {
        try
        {
            using var fs = File.OpenRead(filePath);
            var result = TrySetFileData(fs);
            if (!result.Result) return result;
            CheckIfFileIsEncrypted();
            return new BoolResult(true);
        }
        catch { /* ignored */ }
        return new BoolResult(false, "Error on trying to open the file.");
    }

    /// <summary>
    /// Tries to set data of a <see cref="PcSaveDataFile"/> type based on Stream <paramref name="fs"/>.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    private BoolResult TrySetFileData(Stream fs)
    {
        using BinReader br = new(fs);
        try
        {
            // try to load header data into the Header
            Header = br.ReadStruct<PcSaveDataHeader>() ?? throw new NullReferenceException();

            // test Header Integrity
            var test = Header.CheckIntegrity();
            if (!test.Result) return test;

            // try to load the rest of the header content into the HeaderPadding
            HeaderPadding = br.ReadBytes(Marshal.SizeOf<PcSaveDataHeader>(), (int)(Header.DataOffset - Marshal.SizeOf<PcSaveDataHeader>()));
        }
        catch { return new BoolResult(false, "Invalid file header structure."); }

        try
        {
            // try to load data into the Data
            Data = br.ReadBytes(Header.DataOffset, (int)(fs.Length - Header.DataOffset));
        }
        catch { return new BoolResult(false, "Invalid file data structure."); }
        
        return new BoolResult(true);
    }

    /// <summary>
    /// Gets an existing object of a <see cref="PcSaveDataFile"/> type as byte array.
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<byte> GetFileData()
    {
        using MemoryStream ms = new();
        using BinWriter bw = new(ms);
        // write HEADER content
        bw.WriteStruct(Header);
        // write HEADER PADDING content
        bw.Write(HeaderPadding);
        // write DATA content
        bw.Write(Data);

        // return data
        return ms.ToArray();
    }

    /// <summary>
    /// Gets a header of <see cref="PcSaveDataFile"/> as byte array.
    /// </summary>
    /// <returns></returns>
    public ReadOnlySpan<byte> GetHeaderDataOnly()
    {
        using MemoryStream ms = new();
        using BinWriter bw = new(ms);
        // write HEADER content
        bw.WriteStruct(Header);
        // write HEADER PADDING content
        bw.Write(HeaderPadding);

        // return data
        return ms.ToArray();
    }

    /// <summary>
    /// Gets UserId.
    /// </summary>
    /// <returns></returns>
    public string GetUserId()
        => Header.SteamId.ToString();

    /// <summary>
    /// Sets UserId.
    /// </summary>
    /// <returns></returns>
    public void SetUserId(uint userId)
        => Header.SteamId = userId;
    
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
        if (deencryptMode == DeencryptMode.Decryption)
            Deencryptor.Decrypt(ref dataSpan);
        else
        {
            // sign file
            Header.DataChecksum = new byte[PcSaveDataHeader.ChecksumSize];
            Header.HeaderChecksum = new byte[PcSaveDataHeader.ChecksumSize];
            Span<byte> checksumContainer = Header.DataChecksum;
            Deencryptor.CalculateChecksum(ref checksumContainer, dataSpan);
            checksumContainer = Header.HeaderChecksum;
            Deencryptor.CalculateChecksum(ref checksumContainer, GetHeaderDataOnly());
            // encrypt
            Deencryptor.Encrypt(ref dataSpan);
        }
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
        SetUserId(userId);
        if (IsEncrypted) DecryptData();
        EncryptData();
    }

    /// <summary>
    /// Imports <paramref name="jsonData"/> into <see cref="Data"/>.
    /// </summary>
    /// <param name="jsonData"></param>
    /// <returns></returns>
    public BoolResult ImportJson(ReadOnlySpan<byte> jsonData)
    {
        if (IsEncrypted) DecryptData();
        Span<byte> dataSpan = Data;
        if (dataSpan.Length <= 8) return new BoolResult(false, "The input file length is too short.");
        dataSpan = dataSpan[8..];
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
        if (dataSpan.Length >= 8) dataSpan = dataSpan[8..];
        var eof = dataSpan.LastIndexOf<byte>(0x7D) + 1;
        return dataSpan[..eof];
    }

    /// <summary>
    /// Returns false if the file structure does not make sense.
    /// </summary>
    /// <returns></returns>
    public BoolResult CheckIntegrity()
        => Header.CheckIntegrity();
}