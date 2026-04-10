using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.IO;

namespace TMModTutorial
{
    public sealed class TutorialPlugin : ITMPlugin
    {
        public TutorialPlayerDataManager PlayerData { get; private set; }
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

            PlayerData = new TutorialPlayerDataManager(game);

            // Make sure we read our save data before we initialize our
            // player data, but after we create the TutorialPlayerDataManager!
            ReadData(Path.Combine(game.World.WorldPath, "tutorialdata.dat"));

            // We initialize player data here so data exists for players
            // after a hot reload, as PlayerJoined isn't called for existing
            // players on  a hot reload. Without this, hot reloading the mod
            // will cause a crash.
            List<ITMPlayer> players = new List<ITMPlayer>();
            game.GetAllPlayers(players);
            foreach (ITMPlayer player in players)
            {
                PlayerData.AddData(player);
            }

            game.AddEventItemSwing(Item.NatureStaff, SwingStaff);
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

            PlayerData.ReadState(reader, tmVersion, modVersion);
        }

        private void SwingStaff(Item item, ITMHand hand)
        {
            // We only want this swing event to execute for players, so we
            // return early if the actor is not a player.
            if (hand.Owner is not ITMPlayer player)
            {
                return;
            }

            // This item will use 25 mana. If we don't have 25 mana, it
            // won't do anything. If we do have 25 mana, it'll use 25
            // mana.
            TutorialPlayerData data = PlayerData.GetData(player);
            if (data.Mana >= 25)
            {
                data.Mana -= 25;
                _game.AddNotification("Swing Successful!");
            }
            else
            {
                _game.AddNotification("Not enough mana!");
            }

            // Because our data is a reference type, we don't have to set
            // the data after changing it. We can get the data, change what
            // we want, and it'll change on the player.
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

            PlayerData.AddData(player);
        }

        public void PlayerLeft(ITMPlayer player)
        {
            // Called when a player leaves the game.

            PlayerData.RemoveData(player);
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

            // We want to write our save data here.
            PlayerData.WriteState(writer);

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

            // CCTV will use GamerID.Sys1, so we only draw the mana
            // is the ID isn't Sys1 (in other words, only if we're
            // drawing a real player's HUD)
            if (virtualPlayer.GamerID != GamerID.Sys1)
            {
                DrawMana(player, virtualPlayer, vp);
            }
        }

        private void DrawMana(ITMPlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            int x = CoreGlobals.GraphicsDevice.Viewport.Width - 300;
            int y = 50;
            int width = 246;
            int height = 16;
            float mana = PlayerData.GetData(virtualPlayer).Mana;
            SpriteBatchSafe spriteBatch = CoreGlobals.SpriteBatch;

            spriteBatch.Begin();
            DrawBar(spriteBatch, x, y, width, height, mana, 0, 100, Color.White, Color.Black * 0.5f, new Color(71, 129, 235) * 0.8f);
            spriteBatch.End();
        }

        private void DrawBar(SpriteBatchSafe spriteBatch, int x, int y, int width, int height, float current, float min, float max, Color outline, Color back, Color fill)
        {
            float progress = (current - min) / (max - min);

            spriteBatch.DrawFilledBox(new Rectangle(x - 2, y - 2, width + 4, height + 4), 2, outline, back);
            spriteBatch.DrawFilledBox(new Rectangle(x, y, (int)(width * progress), height), 0, fill, fill);
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

            PlayerData.GetData(player).Update();
        }
    }
}
