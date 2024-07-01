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

            ReadData(Path.Combine(game.World.WorldPath, "tutorialdata.dat"));

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

            // We launch the player into the air when they're on the ground
            // and press R on keyboard, or DPad Down + A on controller.
            if (player.IsOnGround)
            {
                // Keyboard input
                if (InputManager.IsKeyPressed(player.PlayerIndex, Keys.R))
                {
                    LaunchPlayer(player);
                    return true;
                }
                // Controller input
                else if (InputManager.IsButtonPressed(player.PlayerIndex, Buttons.DPadDown)
                    && InputManager.IsButtonPressed(player.PlayerIndex, Buttons.A))
                {
                    LaunchPlayer(player);
                    return true;
                }
            }

            return false;
        }

        private void LaunchPlayer(ITMPlayer player)
        {
            // Be careful with high velocities - this number is the number
            // of blocks the player moves every frame, not every second!
            // Because Total Miner is locked to 60 FPS, By dividing our
            // target blocks per second by 60, we get the correct value.
            // 20f / 60f gives a velocity of 20 blocks/s.
            player.Velocity = new Vector3(player.Velocity.X, 20f / 60f, player.Velocity.Z);
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

            // This ensures that the NPC will target the attacker, even if
            // they can't see them.
            TargetingSystem.Target((IActorBehaviour)player, (IActorBehaviour)target);

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
