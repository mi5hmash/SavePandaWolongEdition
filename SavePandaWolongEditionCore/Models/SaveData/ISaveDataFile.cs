using SavePandaWolongEditionCore.Helpers;

namespace SavePandaWolongEditionCore.Models.SaveData;

public interface ISaveDataFile
{
    /// <summary>
    /// Data of the <see cref="ISaveDataFile"/>.
    /// </summary>
    byte[] Data { get; set; }

    /// <summary>
    /// Deencryptor instance.
    /// </summary>
    SaveDataDeencryptor Deencryptor { get; }

    /// <summary>
    /// Stores the encryption state of the current file.
    /// </summary>
    bool IsEncrypted { get; }

    /// <summary>
    /// Checks if the file is encrypted and sets <see cref="IsEncrypted"/>.
    /// </summary>
    /// <returns></returns>
    bool CheckIfFileIsEncrypted();

    /// <summary>
    /// Loads a '*.bin' archive of <see cref="ISaveDataFile"/> type into the existing object.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    BoolResult SetFileData(string filePath);

    /// <summary>
    /// Gets an existing object of a <see cref="ISaveDataFile"/> type as byte array.
    /// </summary>
    /// <returns></returns>
    ReadOnlySpan<byte> GetFileData();

    /// <summary>
    /// Gets UserId.
    /// </summary>
    /// <returns></returns>
    string GetUserId();

    /// <summary>
    /// Sets UserId.
    /// </summary>
    /// <returns></returns>
    void SetUserId(uint userId);

    /// <summary>
    /// Encrypts <see cref="Data"/>
    /// </summary>
    void EncryptData();

    /// <summary>
    /// Decrypts <see cref="Data"/>
    /// </summary>
    void DecryptData();

    /// <summary>
    /// Signs the file with a new signature.
    /// </summary>
    /// <param name="userId"></param>
    void ResignFile(uint userId);

    /// <summary>
    /// Cuts JSON out of the <see cref="Data"/>.
    /// </summary>
    /// <returns></returns>
    ReadOnlySpan<byte> ExportJson();

    /// <summary>
    /// Imports <paramref name="jsonData"/> into <see cref="Data"/>.
    /// </summary>
    /// <param name="jsonData"></param>
    /// <returns></returns>
    BoolResult ImportJson(ReadOnlySpan<byte> jsonData);

    /// <summary>
    /// Returns false if the file structure does not make sense.
    /// </summary>
    /// <returns></returns>
    BoolResult CheckIntegrity();
}