using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.IO;

namespace TMModTutorial
{
    public sealed class TutorialPlugin : ITMPlugin
    {
        private ITMGame _game;

        public void Initialize(ITMPluginManager mgr, ITMMod mod)
        {
            // Called once when the mod is loaded.
            // Load any assets your mod needs (eg. textures) here.
        }

        public void InitializeGame(ITMGame game)
        {
            // Called once after all mods are initialized.
            // Add events to the game here (eg. item swing events)
            // and set a game field to use later.
            _game = game;

            ReadData(Path.Combine(game.World.WorldPath, "tutorialdata.dat"));
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

            // Add a notification for the TM and mod versions we saved.
            _game.AddNotification($"TM Version: {tmVersion}, Mod version: {modVersion}");
        }

        public void UnloadMod()
        {
            // Called when the mod is unloaded.
            // Unload/dispose any assets/resources here.
        }

        public object[] RegisterLuaFunctions(ITMScriptInstance si)
        {
            // Called when registering Lua functions to a script instance.
            // Return an array containing an instance of your Lua functions
            // class or an empty array (if you don't add any Lua functions) here.
            return Array.Empty<object>();
        }

        public void PlayerJoined(ITMPlayer player)
        {
            // Called when a player joins the game.
        }

        public void PlayerLeft(ITMPlayer player)
        {
            // Called when a player leaves the game.
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

        public void Callback(string data, GlobalPoint3D? p, ITMActor actor, ITMActor contextActor)
        {
            // A game-to-mod callback. Implement this to do something for the
            // ModCall script.
        }

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            // Called every rendered frame.
            // Draw custom UI or geometry here.
        }

        public bool HandleInput(ITMPlayer player)
        {
            // Called every frame.
            // React to user input here.
            // This method should return true if it does something in response
            // to user input, otherwise false.
            return false;
        }

        public void Update()
        {
            // Called every frame.
            // Implement any player-independent logic that needs to run every
            // frame here.
        }

        public void Update(ITMPlayer player)
        {
            // Called for each player every frame.
            // Implement any player-dependent logic that needs to run every
            // frame here.
        }
    }
}
