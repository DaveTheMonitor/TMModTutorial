﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
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

            TutorialItems.Initialize(mgr.Offsets);
        }

        public void InitializeGame(ITMGame game)
        {
            // Called once after all mods are initialized.
            // Add events to the game here (eg. item swing events)
            // and set a game field to use later.

            _game = game;

            // This method takes an action. Methods can be implicitly cast
            // to delegates (an action is a delegate without a return value)
            // with the same parameters and return value.
            // MySwingEvent is method we're using for the event.
            game.AddEventItemSwing(TutorialItems.MyStaff, MySwingEvent);
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
        }

        public void Update(ITMPlayer player)
        {
            // Called for each player every frame.
            // Implement any player-dependent logic that needs to run every
            // frame here.
        }

        private void MySwingEvent(Item item, ITMHand hand)
        {
            if (hand.Owner is not ITMPlayer player)
            {
                // ActorInReticle is not available on ITMActor.
                // If you want the item to work for NPCs, you'll
                // have to implement the raycast logic yourself,
                // which is out of the scope of this tutorial.
                return;
            }

            ITMActor target = player.ActorInReticle;
            if (target == null)
            {
                // We return if the target is null, meaning
                // the player isn't targeting anything.
                return;
            }

            // We test the distance between the player and the target,
            // and return if it's greater than the specified distance.
            // This prevents us from damaging NPCs from across the map.
            float distance = 10;
            if (Vector3.Distance(player.Position, target.Position) > distance)
            {
                return;
            }

            // If all of those checks passed, we must have a valid target.
            // Now we can deal damage and spawn our particles.

            // To deal damage, we'll use the TakeDamageAndDisplay method,
            // passing the player as the attacker.
            target.TakeDamageAndDisplay(DamageType.Combat, 20, Vector3.Zero, player, TutorialItems.MyStaff, SkillType.Attack);

            // To spawn particles, we'll use the ITMWorld.AddParticle
            // method, and pass the position of the target + (0, 1, 0)

            // This is the data for the particles we want to spawn.
            ParticleData particle = new ParticleData()
            {
                Size = new Vector4(0.15f, 0.15f, 0.15f, 0),
                // Duration is measured in milliseconds, not seconds
                Duration = 1200,
                StartColor = Color.LightGreen,
                EndColor = Color.Transparent,
                VelocityVariance = new Vector3(5, 5, 5)
            };

            // We use a loop here to spawn multiple particles.
            for (int i = 0; i < 10; i++)
            {
                _game.World.AddParticle(target.Position + new Vector3(0, 1, 0), ref particle);
            }
        }
    }
}
