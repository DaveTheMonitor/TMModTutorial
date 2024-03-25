using StudioForge.TotalMiner.API;

namespace TMModTutorial
{
    public sealed class TutorialPluginProvider : ITMPluginProvider
    {
        // Basic plugin functionality, such as running code on each game update.
        public ITMPlugin GetPlugin() => new TutorialPlugin();

        // Used to add custom arcade machine games.
        public ITMPluginArcade GetPluginArcade() => null;

        // Used to build custom meshes and add new menus for blocks, along with changing some other block properties, such as their hitbox.
        public ITMPluginBlocks GetPluginBlocks() => null;

        // Used to add custom setup menus to blocks and items.
        public ITMPluginGUI GetPluginGUI() => null;

        // Used to add custom networks for multiplayer. This won't be covered here.
        public ITMPluginNet GetPluginNet() => null;

        // Currently unused.
        public ITMPluginBiome GetPluginBiome() => null;

        // Currently unused.
        public ITMPluginConfig GetPluginConfig() => null;
    }
}
