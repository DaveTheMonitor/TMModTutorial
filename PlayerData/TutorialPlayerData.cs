using StudioForge.Engine;
using StudioForge.TotalMiner.API;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TMModTutorial
{
    public sealed class TutorialPlayerData
    {
        public float Mana { get; set; }
        public ITMPlayer Player { get; private set; }
        private ITMGame _game;

        public TutorialPlayerData(ITMGame game, ITMPlayer player)
        {
            Player = player;
            _game = game;

            Mana = 100;
        }

        internal static TutorialPlayerData FromSaveData(ITMGame game, ITMPlayer player, TutorialPlayerSaveData saveData)
        {
            TutorialPlayerData data = new TutorialPlayerData(game, player)
            {
                Mana = saveData.Mana
            };

            return data;
        }

        internal TutorialPlayerSaveData ToSaveData()
        {
            TutorialPlayerSaveData saveData = new TutorialPlayerSaveData()
            {
                ID = Player.GamerID.ID,
                Mana = Mana
            };

            return saveData;
        }

        public void Update()
        {
            // Services.ElapsedTime is Total Miner's version of DeltaTime.
            // Multiplying 5 by this value gives us the number required to
            // regenerate 5 mana per second.
            Mana += 5 * Services.ElapsedTime;

            // Clamp - that is, limit - the number between 0 and 100. If the
            // mana is below 0, it will be set to 0, and it it's above 100,
            // it will be set to 100.
            Mana = Math.Clamp(Mana, 0, 100);
        }
    }
}
