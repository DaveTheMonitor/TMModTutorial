# Saving Data

## Prerequisites:
- [Basic Setup](./README.md)

You may want your mod to save some custom data on the world. We currently don't have anything useful to save, but this tutorial serves as a prerequisite to some other tutorials.

To save data, we'll want to use the `ITMPlugin.WorldSaved` method. Before we get started, there's a few things to keep in mind:

Total Miner saves data in a binary format. Technically you can save your data in any format, but binary is the smallest for file size, so we'll also save our data in a binary format.

Saving our data in a binary format simply means writing the binary data directly, rather than using a stucture like Json or XML. For example, if we were to save the byte `180` to a binary file, the result would be a binary file with the following binary data:

```
10110100
```

If you were to try to open this file with a text editor, you wouldn't see anything useful. This is because the file isn't saved as text, it contains the raw data we wrote to it.

Something very important to note about reading binary data is that we have to read it the exact same way we wrote it. If we miss anything, the game will likely either crash or we'll get useless data. Always check to make sure your read/write methods match!

Another thing to note is that we'll also want to write both the Total Miner version and the save version of our mod. To ensure backwards compatibility, we'll need both of these.

We'll pass the Total Miner version to various methods for reading TM data, and we'll use our save version to read the data differently for older versions so our mod will keep working on existing worlds.

## Writing Our Data

For now we'll just write the version numbers, but other tutorials will add data to this file.

In `ITMPlugin.WorldSaved`, we'll open a write stream for the `tutorialdata.dat` file so we can write to it. We do this using `StudioForge.Engine.Core.FileSystem.OpenWrite`.

Then, we'll write our data using a `BinaryWriter`.

**NOTE:** For an actual mod, you would likely want this file to have a different more unique name to guarantee there won't be any conflicts between mods, but for the purposes of this tutorial we're keeping the name simple.

```csharp
public void WorldSaved(int version)
{
    // Called when the world is saved.
    // Use ITMGame.World.WorldPath to get the world path if you
    // want to save files.
            
    // Opens or creates a new file named `tutorialdata.dat` in the
    // world folder to write data to.
    string file = Path.Combine(_game.World.WorldPath, "tutorialdata.dat");
    using FileStream stream = FileSystem.OpenWrite(file);

    // Creates a new BinaryWriter that writes to the stream we opened.
    using BinaryWriter writer = new BinaryWriter(stream);

    // Globals1.SaveVersion is the current save version for TM.
    writer.Write(Globals1.SaveVersion);

    // Here we'll write the current save version for our mod.
    // We'll want to increment this whenever we make changes to
    // our save format.
    writer.Write(0);
            
    // The file is saved when the writer is disposed, which
    // happens when this method ends because of the using statements.
    // So there is no need to manually save the file.
}
```

**NOTE:** *Do not forget the `using` statements for the stream and BinaryWriter!*

**NOTE:** We're using `StudioForge.Engine.Core.FileSystem` instead of `System.IO.File` because the world path isn't always a full path. The `FileSystem` methods handle that for us.

Now if you load a world with your mod and save it, you'll see a new file in the mod folder named `tutorialdata.dat` that's 8 bytes large.

For readability, let's move this saving logic to a new method:


## Reading Our Data

Now that we've written our data, we need to read it when the world is loaded. There is no `WorldLoaded` method we can use, so we'll instead use `InitializeGame`.

Let's add a new method called `ReadData` that will be called when we read our mod's data. This method will take the filepath to read from.

```csharp
private void ReadData(string file)
{

}
```

Now we'll call this method in `InitializeGame`:

```csharp
public void InitializeGame(ITMGame game)
{
    // Called once after all mods are initialized.
    // Add events to the game here (eg. item swing events)
    // and set a game field to use later.

    _game = game;

    ReadData(Path.Combine(game.World.WorldPath, "tutorialdata.dat"));
}
```

Because `InitializeGame` is called when a world is created for the first time, we need some way to know if we're loading an existing world or loading a new one, so we know whether or not to read the data. The easiest way to do this is by simply testing the date saved, which is always 0 for new worlds, and if the file exists. If the file doesn't exist, that means we're loading a world that has never been saved with this mod before.

```csharp
private void ReadData(string file)
{
    // We exit if the DateSaved is 0, because that means we're
    // creating a new world. We otherwise probably won't use this.
    if (_game.World.Header.DateSaved == 0)
    {
        return;
    }

    // We exit if the file doesn't exist, because that means we're
    // loading a world that has never been saved with this mod.
    if (!FileSystem.IsFileExist(file))
    {
        return;
    }
}
```

Now we'll read our data using a `BinaryReader`. Instead of using `FileSystem.OpenWrite` like when we saved the data, we'll instead use `FileSystem.OpenRead`. Since we only saved the TM and mod versions, we don't have much to read here, but we'll add more in other tutorials.

```csharp
private void ReadData(string file)
{
    // We exit if the DateSaved is 0, because that means we're
    // creating a new world. We otherwise probably won't use this.
    if (_game.World.Header.DateSaved == 0)
    {
        return;
    }

    // We exit if the file doesn't exist, because that means we're
    // loading a world that has never been saved with this mod.
    if (!FileSystem.IsFileExist(file))
    {
        return;
    }

    // Opens a read stream for the file.
    using Stream stream = FileSystem.OpenRead(file);

    // Creates a new BinaryReader to read from the stream we opened.
    using BinaryReader reader = new BinaryReader(stream);

    int tmVersion = reader.ReadInt32();
    int modVersion = reader.ReadInt32();

    // Add a notification for the TM and mod versions we saved.
    _game.AddNotification($"TM Version: {tmVersion}, Mod version: {modVersion}");
}
```

**NOTE:** *Do not forget the `using` statements for the stream and BinaryReader!*

Note that the `BinaryReader` does not use the aliases that C# uses for types. For example, what C# calls a `ushort`, the `BinaryReader` calls a `UInt16`. See [here](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types) for more info.

Hot reloading the mod after saving the world should now display a notification that shows the TM version and mod version we saved. Remember that you have to first save the world with the mod active or the notification won't show!

Now we're ready to start reading and writing custom data that our mod needs. Other tutorials may require this.

**NOTE:** Hot reloading the mod will reset all data to the point when it was last saved! If you want to make sure the data doesn't reset, save the world before hot reloading.

Here's the relevant methods we've changed:

`TutorialPlugin.cs`:

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.IO;

namespace TMModTutorial
{
    public sealed class TutorialPlugin : ITMPlugin
    {
        private ITMGame _game;

        public void InitializeGame(ITMGame game)
        {
            // Called once after all mods are initialized.
            // Add events to the game here (eg. item swing events)
            // and set a game field to use later.

            _game = game;

            ReadData(Path.Combine(game.World.WorldPath, "tutorialdata.dat"));
        }

        public void WorldSaved(int version)
        {
            // Called when the world is saved.
            // Use ITMGame.World.WorldPath to get the world path if you
            // want to save files.

            // Opens or creates a new file named `tutorialdata.dat` in the
            // world folder to write data to.
            string file = Path.Combine(_game.World.WorldPath, "tutorialdata.dat");
            using FileStream stream = FileSystem.OpenWrite(file);

            // Creates a new BinaryWriter that writes to the stream we opened.
            using BinaryWriter writer = new BinaryWriter(stream);

            // Globals1.SaveVersion is the current save version for TM.
            writer.Write(Globals1.SaveVersion);

            // Here we'll write the current save version for our mod.
            // We'll want to increment this whenever we make changes to
            // our save format.
            writer.Write(0);

            // The file is saved when the writer is disposed, which
            // happens when this method ends because of the using statements.
            // So there is no need to manually save the file.
        }

        private void ReadData(string file)
        {
            // We exit if the DateSaved is 0, because that means we're
            // creating a new world. We otherwise probably won't use this.
            if (_game.World.Header.DateSaved == 0)
            {
                return;
            }

            // We exit if the file doesn't exist, because that means we're
            // loading a world that has never been saved with this mod.
            if (!FileSystem.IsFileExist(file))
            {
                return;
            }

            // Opens a read stream for the file.
            using Stream stream = FileSystem.OpenRead(file);

            // Creates a new BinaryReader to read from the stream we opened.
            using BinaryReader reader = new BinaryReader(stream);

            int tmVersion = reader.ReadInt32();
            int modVersion = reader.ReadInt32();
        }
    }
}
```