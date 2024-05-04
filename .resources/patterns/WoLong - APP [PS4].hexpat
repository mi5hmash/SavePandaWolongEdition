#pragma author Mi5hmasH
#pragma description Wo Long: Fallen Dynasty - APP.BIN
#pragma endian little

#include <std/sys.pat>
#include <std/mem.pat>

struct version {
    u8 unknown_1;
    u8 day;
    u8 month;
    u8 year;
};

struct file_header {
    version file_version [[color("33FF33")]];
    u32 _padding [[color("555555")]];
};

struct file_footer {
    u32 dataChecksum[8] [[color("FF0000")]];
	u32 headerChecksum[8] [[color("FF00AA")]];
};

struct file_data {
    u8 data[std::mem::size()-sizeof(file_header)-sizeof(file_footer)] [[color("FFFF33")]];  
};

file_header header @ 0x0;
file_data data @ sizeof(file_header);
file_footer footer @ std::mem::size()-sizeof(file_footer);

// Assert file
std::assert(header.file_version.unknown_1 != 0 || header.file_version.day == 0 || header.file_version.month == 0 || header._padding == 0, "File is not a valid WOLONG SAVE DATA file!");

// Check encryption state
fn checkEncryptionState() {
    if ( data.data[0] == 0x7B && data.data[1] == 0x22) {
        return "DECRYPTED";
    } else {
        return "ENCRYPTED";
    }
};
std::print("File is {}", checkEncryptionState());