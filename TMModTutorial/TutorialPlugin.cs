using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner.API;
using System;

namespace TMModTutorial
{
    public sealed class TutorialPlugin : ITMPlugin
    {
        private ITMGame _game;

        public void Initialize(ITMPluginManager mgr, ITMMod mod)
        {
            // Called once, when the mod is loaded.
            // Load any assets your mods needs (eg. textures) here.
        }

        public void InitializeGame(ITMGame game)
        {
            // Called once after all mods are initialized.
            // Add events to the game here (eg. item swing events)
            // and set a game field to use later.
            _game = game;
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
            _game.AddNotification("It's working!");
        }

        public void Update(ITMPlayer player)
        {
            // Called for each player every frame.
            // Implement any player-dependent logic that needs to run every
            // frame here.
        }
    }
}
