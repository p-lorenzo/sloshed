# Sloshed

Sloshed is a Unity-based game prototype developed for the GameDev.js Jam 2025, designed around the theme of "balance". You control a character with drunken, ragdoll-like movement in a domestic environment filled with oversized furniture and challenges. The project leverages custom shaders, physics-based interactions, and procedural level design.

## 📦 Project Structure

- **Assets/**: Contains all game assets, including scripts, models, textures, and scenes.
- **Packages/**: Manages packages and dependencies used in the project.
- **ProjectSettings/**: Includes Unity project configuration settings.
- **.idea/**: Configuration files for development environments (e.g., JetBrains Rider).
- **.gitignore**: Specifies files and directories to be ignored by version control.
- **.vsconfig**: Configuration file for Visual Studio.

## 🛠️ Technologies Used

- **Game Engine**: Unity (Recommended version: Unity 6)
- **Editors**: Visual Studio, JetBrains Rider

## 🚀 Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/p-lorenzo/sloshed.git
   ```

2. Open the project using Unity (version 6 or later).

3. Import the required third-party assets listed below.

4. Open the main scene to test the game.

## 📦 Required Third-Party Assets (Not Included)

This project depends on the following Unity Asset Store plugins, which are **not included in the repository** due to licensing restrictions. You must purchase/import them manually after cloning the project.

- [PuppetMaster](https://assetstore.unity.com/packages/tools/physics/puppetmaster-48977) – for active ragdoll physics and character behavior
- [DunGen](https://assetstore.unity.com/packages/tools/level-design/dungen-15682) – for procedural level generation

### Import Instructions

1. After purchasing the assets from the Unity Asset Store, open the project in Unity.
2. Import the packages via the Asset Store or Unity Package Manager.
3. Verify that the following folders exist:
   - `Assets/RootMotion/` (PuppetMaster)
   - `Assets/DunGen/` (DunGen)

## 🤝 Contributors

- [p-lorenzo (Lorenzo Pesce)](https://github.com/p-lorenzo)
- [DaticaIT](https://github.com/DaticaIT)

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

Note: Third-party assets such as PuppetMaster and DunGen are **not** covered by this license. Refer to their respective publishers for licensing terms.
