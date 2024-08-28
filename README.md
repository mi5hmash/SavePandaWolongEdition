[![License: MIT](https://img.shields.io/badge/License-MIT-blueviolet.svg)](https://opensource.org/licenses/MIT)
[![Release Version](https://img.shields.io/github/v/tag/mi5hmash/SavePandaWolongEdition?label=Version)](https://github.com/mi5hmash/SavePandaWolongEdition/releases/latest)
[![Visual Studio 2022](https://custom-icon-badges.demolab.com/badge/Visual%20Studio%202022-5C2D91.svg?&logo=visual-studio&logoColor=white)](https://visualstudio.microsoft.com/)
[![.NET8](https://img.shields.io/badge/.NET%208-512BD4?logo=dotnet&logoColor=fff)](#)

> [!IMPORTANT]
> **This software is free and open source. If someone asks you to pay for it, it's likely a scam.**

# 🐼 SavePandaWolongEdition - What is it :interrobang:
This application can **resign WoLong SaveData files** with your own SteamID to **use any SaveData on your Steam Account**. It can also **decrypt and encrypt these SaveData files** so you can research them further.

#### Supported games:
* **[STEAM | PC]** Wo Long: Fallen Dynasty **(v1.304*)**
* **[STEAM | PC]** Wo Long: Fallen Dynasty Complete Edition **(v1.304*)**

**the most recent versions that I have tested.*

### About other platforms
#### **[PC | XBOX] MS Store \ GamePass**
SaveData files from games purchased in the Microsoft Store or rented under the GamePass service are additionally packed. SavePanda may work with these files after they have been unpacked, but I have not tested this.
#### **[PS4]**
SaveData files from PS4 are additionally packed and have a different signing method that I have not worked out, so they will not work after you put them back on the PS4 console.

However, you can convert PS4 SaveData files to a PC version of the game. To do so, you can load the ***APP.BIN*** file from the unpacked PS4 SaveData file into SavePanda, export the JSON data from it, and then import that JSON data into the PC SaveData file (***SAVEDATA.BIN***).

# :scream: Is it safe?
The short answer is: **No.** 
> [!CAUTION]
> If you unreasonably edit your SaveData files, you risk corrupting them or getting banned from playing online. In both cases, you will lose your progress.

> [!IMPORTANT]
> Always back up the files you intend to edit before editing them.

> [!IMPORTANT]
> Disable the Steam Cloud before you replace any SaveData files.

You have been warned, and now that you are completely aware of what might happen, you may proceed to the next chapter.

# :scroll: How to use this tool

<img src="https://github.com/mi5hmash/SavePandaWolongEdition/blob/main/.resources/images/MainWindow.png" alt="MainWindow"/>

## Setting the Input File Path
There are three ways to achieve this. The first one is to drop the SaveData file onto a TextBox **(1)** or a button **(4)**. Alternatively, you may use a button **(4)** to open a file-picker window and select it there. You can also type the path to the file in the **(1)** TextBox.

> [!TIP]
> The program will extract the Steam32_ID from the "Input File Path" TextBox **(1)**, if it ends with *"<steam_id>\\(SYSTEM)SAVEDATA0X\\SAVEDATA.BIN"*, and will fill the TextBox **(5)** for you.

## About Steam32 ID
It is a 32-bit representation of your 64-bit SteamID.

##### Example:
| 64-bit SteamID    | 32-bit SteamID |
|-------------------|----------------|
| 76561197960265729 | 1              |

> [!NOTE]
> Steam32 ID is also known as AccountID or Friend Code. 

> [!TIP]
You can use the calculator on [steamdb.info](https://steamdb.info/calculator/) to find your SteamID.

## Resigning files
If you want to resign your SaveData file so you can use it on another Steam Account, type in the Steam32_ID of that Steam Account into a TextBox **(5)**. Once it is typed in, press the **"Resign"** button **(6)**.

## Enabling SuperUser Mode

> [!WARNING]
> This mode is for advanced users only.

If you really need it, you can enable SuperUser mode by triple-clicking the version number label **(7)**.

## Exporting savedata.json

> [!IMPORTANT]  
> This button is visible only when the SuperUser Mode is Enabled. 

If you want to export JSON data from the SaveData file, press the **"Export JSON"** button **(10)**.
The ***savedata.json*** file will be exported to the program's root directory. You can navigate to it by pressing the **(11)** button.

## Importing savedata.json

> [!IMPORTANT]  
> This button is visible only when the SuperUser Mode is Enabled. 

If you want to import JSON data to the SaveData file, first ensure the ***savedata.json*** file is placed in the program's root directory, then press the **"Import JSON"** button **(12)**.

## Decrypting files

> [!IMPORTANT]  
> This button is visible only when the SuperUser Mode is Enabled. 

If you want to decrypt the SaveData file to read its content, press the **"Decrypt"** button **(8)**.

## Encrypting files

> [!IMPORTANT]  
> This button is visible only when the SuperUser Mode is Enabled. 

If you want to encrypt the decrypted SaveData file, press the **"Encrypt"** button **(9)**.

## Backup functionality
By default, the backup option is checked **(2)**. In this state, the application will back up files before each operation to the new folder inside the ***"SavePandaWolongEdition/_BACKUP/"*** directory. This app can create up to 3 zip archives.

## Open the Backup Directory
You may open the ***"SavePandaWolongEdition/_BACKUP/"*** directory in a new File Explorer window by pressing the button **(3)**.

# :fire: Issues
All the problems I've encountered during my tests have been fixed on the go. If you find any other issues (which I hope you won't) feel free to report them [there](https://github.com/mi5hmash/SavePandaWolongEdition/issues).

> [!TIP]
> This application creates a log file that may be helpful in troubleshooting.  
It can be found in the same directory as the executable file.

# :star: Sources
* https://github.com/tremwil/DS3SaveUnpacker
