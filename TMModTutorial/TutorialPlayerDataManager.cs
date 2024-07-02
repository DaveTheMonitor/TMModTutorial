using StudioForge.TotalMiner.API;
using System.Collections.Generic;
using System.IO;

namespace TMModTutorial
{
    public sealed class TutorialPlayerDataManager
    {
        private Dictionary<ulong, TutorialPlayerData> _data;
        private ITMGame _game;
        private List<TutorialPlayerSaveData> _saveData;

        public TutorialPlayerData GetData(ITMPlayer player)
        {
            return _data[player.GamerID.ID];
        }

        internal void AddData(ITMPlayer player)
        {
            ulong id = player.GamerID.ID;

            // We loop through the save data to see if this player
            // has data saved. If they do, we'll use that. If they
            // don't, we'll initialize new data.
            foreach (TutorialPlayerSaveData saveData in _saveData)
            {
                if (saveData.ID != id)
                {
                    continue;
                }

                // This will only be executed if the player has data
                // saved.
                _data.Add(id, TutorialPlayerData.FromSaveData(_game, player, saveData));
                return;
            }

            // This will only be executed if the player doesn't have
            // data saved.
            TutorialPlayerData data = new TutorialPlayerData(_game, player);
            _data.Add(player.GamerID.ID, data);
        }

        internal void RemoveData(ITMPlayer player)
        {
            // This overload of Remove sets the out paramater to the
            // item that was removed.
            _data.Remove(player.GamerID.ID, out TutorialPlayerData data);
            UpdateSaveData(data);
        }

        private void UpdateSaveData(TutorialPlayerData data)
        {
            ulong id = data.Player.GamerID.ID;

            // We loop through the save data to find an existing data
            // for this player. If we find it, we overwrite it. If we
            // don't, we add it.
            for (int i = 0; i < _saveData.Count; i++)
            {
                // If the IDs don't match, this isn't the same player,
                // so we continue the loop.
                if (_saveData[i].ID != id)
                {
                    continue;
                }

                // This will only be executed is the ID for the save
                // data and the player match. We overwrite the data
                // and immediately exit the method here.
                _saveData[i] = data.ToSaveData();
                return;
            }

            // This will only be executed if no data for the player
            // was found. Here we'll add new save data for the player.
            _saveData.Add(data.ToSaveData());
        }

        internal void ReadState(BinaryReader reader, int tmVersion, int modVersion)
        {
            // This method reads save data from a BinaryReader and adds
            // it to _saveData.

            // Player Data Format:
            // Player Data Count : int32
            // for each Player Data:
            //   - ID : uint64
            //   - Mana : single

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ulong id = reader.ReadUInt64();
                float mana = reader.ReadSingle();
                TutorialPlayerSaveData saveData = new TutorialPlayerSaveData()
                {
                    ID = id,
                    Mana = mana
                };
                _saveData.Add(saveData);
            }
        }

        internal void WriteState(BinaryWriter writer)
        {
            // We update our save data before writing it so we don't
            // write outdated data or miss any players.
            foreach (TutorialPlayerData data in _data.Values)
            {
                UpdateSaveData(data);
            }

            // Now we write the data to the BinaryWriter

            // Player Data Format:
            // Player Data Count : int32
            // for each Player Data:
            //   - ID : uint64
            //   - Mana : single
            
            writer.Write(_saveData.Count);
            foreach (TutorialPlayerSaveData saveData in _saveData)
            {
                writer.Write(saveData.ID);
                writer.Write(saveData.Mana);
            }
        }

        public TutorialPlayerDataManager(ITMGame game)
        {
            _data = new Dictionary<ulong, TutorialPlayerData>();
            _game = game;

            // This list will store the save state for players. It is
            // empty initially but will have data added to it when
            // the game saves or players leave the game.
            _saveData = new List<TutorialPlayerSaveData>();
        }
    }
}
