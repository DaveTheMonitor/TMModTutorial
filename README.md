# Total Miner C# Mod Tutorial

This guide will show you how to make a basic Total Miner mod and utilize the API to add basic functionality.

This guide is up to date as of Total Miner version `public v2.27.0.2574 3/21/2026`. If any part of this guide does not work, please let me know via a message in the [Discord Thread](https://discord.com/channels/259780503115137028/1221996845774147697) in the Total Miner Discord Server.

This guide assumes you have some basic knowledge of C#. If not, you'll need to learn C# first. There are many free resources online to help you get started.

This guide is also going to focus on the C# side of mods. Some basic XML setup will be included, but for the most part this guide assumes you either already know how XML mods work (where it's needed) or aren't planning to use it.

We will be using Visual Studio 2022 in this guide, but the setup for Visual Studio 2026 is mostly the same. If you have a different preferred editor, some parts of this guide may not apply the same way.

Your feedback is greatly appreciated! If you have any questions or something isn't explained clearly, let me know via a message in the [Discord Thread](https://discord.com/channels/259780503115137028/1221996845774147697) in the Total Miner Discord Server and I'll try to help. If needed, I can also update the guide to better explain it.

## Sections

- [Basic Setup](./BasicSetup.md)
  - IDE setup and project creation. Start here if you don't have a mod created already.
  - Prerequisites: None
- [Handling Input](./HandlingInput.md)
  - Perform actions when the player presses a key or button.
  - Prerequisites: [Basic Setup](./BasicSetup.md)
- [Custom Items](./CustomItems.md)
  - Access your XML-defined items with C# and add events that will be executed when the item is swung.
  - Prerequisites: [Basic Setup](./BasicSetup.md)
- [Saving Data](./SavingData.md)
  - Save custom data for the world that shouldn't reset when the world is reloaded.
  - Prerequisites: [Basic Setup](./BasicSetup.md)
- [Player Data](./PlayerData.md)
  - Give the player custom data, such as mana for swinging a staff.
  - Prerequisites: [Saving Data](./SavingData.md)
- [Lua Functions](./LuaFunctions.md)
  - Add custom Lua functions that can be used by scripts.
  - Prerequisites: [Basic Setup](./BasicSetup.md)