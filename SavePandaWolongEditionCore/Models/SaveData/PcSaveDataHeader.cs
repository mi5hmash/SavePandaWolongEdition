using System.Runtime.InteropServices;
using SavePandaWolongEditionCore.Helpers;

namespace SavePandaWolongEditionCore.Models.SaveData;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = SaveDataHeaderSize)]
public class PcSaveDataHeader
{
    public const int SaveDataHeaderSize = 0x7B;
    public const int ChecksumSize = 32;

    /// <summary>
    /// A magic signature of the file.
    /// </summary>
    public ulong Magic { get; set; } = 0x0;

    /// <summary>
    /// A header of a decrypted data.
    /// </summary>
    public ulong DecryptedDataHeader { get; set; } = 0x0;

    /// <summary>
    /// SteamID.
    /// </summary>
    public uint SteamId { get; set; } = 0x0;

    /// <summary>
    /// File data offset.
    /// </summary>
    public uint DataOffset { get; set; } = 0x100;

    /// <summary>
    /// Data length.
    /// </summary>
    public uint DataLength { get; set; } = 0x4E2010;

    /// <summary>
    /// Hero character Level.
    /// </summary>
    public uint HeroLevel { get; set; } = 0x0;

    /// <summary>
    /// Hero chapter ID.
    /// </summary>
    public uint HeroChapterId { get; set; } = 0x0;

    /// <summary>
    /// Battlement image ID.
    /// </summary>
    public uint HeroBattlementImageId { get; set; } = 0x0;

    /// <summary>
    /// Don't know what's there.
    /// </summary>
    public uint Unknown1 { get; set; } = 0x0;

    /// <summary>
    /// Don't know what's there.
    /// </summary>
    public uint Unknown2 { get; set; } = 0x0;

    /// <summary>
    /// The time of file creation in time64_t format.
    /// </summary>
    public long FileCreationTime64 { get; set; } = 0x0;

    /// <summary>
    /// Don't know what's there.
    /// </summary>
    public uint Unknown3 { get; set; } = 0x0;

    /// <summary>
    /// Data Checksum.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ChecksumSize)]
    public byte[] DataChecksum = new byte[ChecksumSize];

    /// <summary>
    /// Header Checksum.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ChecksumSize)]
    public byte[] HeaderChecksum = new byte[ChecksumSize];

    /// <summary>
    /// Create a parameter-less <see cref="PcSaveDataHeader"/>.
    /// </summary>
    public PcSaveDataHeader() { }
    
    public uint GetSaveDataFileType()
    {
        return Magic.NumberToAsciiString() switch
        {
            "WLNUSR" => 1,
            "WLNSYS" => 2,
            _ => 0
        };
    }

    /// <summary>
    /// Returns false if its <see cref="Magic"/> does not make sense.
    /// </summary>
    /// <returns></returns>
    public BoolResult CheckIntegrity()
    {
        if (GetSaveDataFileType() == 0) return new BoolResult(false, "Invalid file header!");
        return new BoolResult(true);
    }
}