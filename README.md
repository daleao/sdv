# Stardew Valley Mod Collection

This repository contains a collection of gameplay overhauls for **Stardew Valley**.  
Each project in the solution is a standalone mod focused on a specific gameplay mechanic, introducing overhauls, new systems, or both. In addition, the solution may include shared libraries used across multiple mods.

For detailed information, configuration options, and usage notes, see the README in each individual project folder.

---

## Technical Overview

- **Language:** C#
- **Framework:** .NET Framework 6.0 (as required by SMAPI)
- **Mod Loader:** [Stardew Modding API](https://smapi.io/) (SMAPI)

All mods are built to be used independently, unless otherwise stated in their respective documentation.

---

## Build Instructions (Generic)

1. Install **Stardew Valley**.
2. Install [**SMAPI**](https://smapi.io/).
3. Clone this repository.
4. Open *.csproj for the desired project(s).
5. Under `<!-- paths -->`, adjust the game path, mods path, and mod zip path to match your game install directory. Depending on your OS, this is usually one of the following:
   - Windows: `C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley`
   - macOS: `~/Library/Application Support/Steam/steamapps/common/Stardew Valley`
   - Linux: `~/.steam/steam/steamapps/common/Stardew Valley` (varies by distro and install method)
7. Open the solution in **Visual Studio**, **Rider** or another compatible C# IDE.
8. Restore NuGet packages if required.
9. Build the solution (or individual mod projects). The build output should automatically be copied into your mods path, as specified in step 5.
