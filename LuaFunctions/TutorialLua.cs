using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace TMModTutorial
{
    internal sealed class TutorialLua
    {
        private ITMGame _game;
        // The script instance contains the actor currently running the script,
        // along with some other useful properties.
        private ITMScriptInstance _si;

        public TutorialLua(ITMGame game, ITMScriptInstance si)
        {
            _game = game;
            _si = si;
        }

        [LuaFuncRegister]
        public void set_fly_mode(string fly_mode)
        {
            // We want to return if the player can't fly to prevent abusing
            // this function.

            // _si.Player will be null if this script runs on an NPC.
            if (_si.Player == null)
            {
                return;
            }

            if (!_si.Player.HasPermission(Permissions.Fly))
            {
                return;
            }

            if (_game.World.GameMode != GameMode.Creative && !_si.Player.IsItemEquippedAndUsable(Item.AmuletOfFlight))
            {
                return;
            }

            // Convert the string fly mode passed by the script to the
            // FlyMode enum. If the script passes an unsupported fly mode,
            // we'll default to FlyMode.None.
            FlyMode flyMode = fly_mode switch
            {
                "none" => FlyMode.None,
                "slow" => FlyMode.Slow,
                "fast" => FlyMode.Fast,
                "custom" => FlyMode.Custom,
                _ => FlyMode.None
            };

            _si.Player.FlyMode = flyMode;
        }
    }
}
