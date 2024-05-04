#pragma author Mi5hmasH
#pragma description Wo Long: Fallen Dynasty - SAVEDATA.BIN
#pragma endian little

#include <std/sys.pat>
#include <std/mem.pat>
#include <type/time.pat>

#define WOLONG_SYS_MAGIC    "WLNSYS"
#define WOLONG_USR_MAGIC    "WLNUSR"

fn findMagicLength() {
	for (u8 i = 0, $+i < sizeof(u64), i+=1) {
		if (std::mem::read_unsigned($+i, 1) == 0x0) {
			return i;
		}
	}
};

struct version {
    u8 unknown_1;
    u8 day;
    u8 month;
    u8 year;
};

struct file_data_header {
    version file_version;
    u8 _padding[4] [[color("555555")]];
};

struct file_header {
	char magic[findMagicLength()] [[color("33FF33")]];
	u8 _padding[sizeof(u64)-sizeof(magic)] [[color("555555")]];
	file_data_header decrypted_data_header [[color("FF7F0E")]];
	u32 steamID_32 [[color("1F77B4")]];
	u32 data_offset [[color("44FFFF")]];
	u32 data_length [[color("885500")]];
	u32 hero_lvl [[color("FF00AA")]];
	u32 hero_chapter [[color("FFFF33")]];
	u32 hero_battlement_image;
	u32 unknown_1;
	u32 unknown_2;
	type::time64_t file_creation_datetime;
	u32 unknown_3;
	u32 dataChecksum[8] [[color("FF0000")]];
	u32 headerChecksum[8] [[color("FF00AA")]];
};
file_header header @ 0x0;

struct file_data {
    file_data_header data_header [[color("FF7F0E")]];
    u8 data[header.data_length-sizeof(data_header)] [[color("FFFF33")]];  
};
file_data data @ header.data_offset;

// Assert file
std::assert(header.magic == WOLONG_SYS_MAGIC || header.magic == WOLONG_USR_MAGIC, "File is not a valid WOLONG SAVE DATA file!");

// Print current Chapter
fn printCurrentChapter(u32 a = header.hero_chapter) {
    if (a == 0) {
        return "Encounter";
    } else if (a == 1) {
        return "Delusion";
    } else if (a == 2) {
        return "Corruption";
    } else if (a == 3) {
        return "Tyranny";
    } else if (a == 4) {
        return "Fortitude";
    } else if (a == 5) {
        return "Malice";
    } else if (a == 6) {
        return "Ascension";
    } else {
        return "Unknown";
    }    
};

// Check filetype
if (header.magic == WOLONG_SYS_MAGIC) {
    std::print("Filetype is {:?}.", WOLONG_SYS_MAGIC);
} else if (header.magic == WOLONG_USR_MAGIC) {
    std::print("Filetype is {:?}.", WOLONG_USR_MAGIC);
    // Print Hero Level
    std::print("Hero LVL: {}", header.hero_lvl);
    // Print current Chapter
    std::print("Chapter: {}", printCurrentChapter());  
}

// Check encryption state
fn checkEncryptionState() {
    if (header.decrypted_data_header == data.data_header) {
        return "DECRYPTED";
    } else {
        return "ENCRYPTED";
    }
};
std::print("File is {}", checkEncryptionState());