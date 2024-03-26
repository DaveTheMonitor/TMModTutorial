using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner;

namespace TMModTutorial
{
    public static class TutorialItems
    {
        public static Item MyStaff { get; private set; }

        public static void Initialize(EnumTypeOffsets offsets)
        {
            // the first item is offsets.ItemID + 0, so we don't have to add anything.
            MyStaff = (Item)offsets.ItemID;

            // For other items, set them to (Item)offsets.ItemID + index
            // eg. MySword = (Item)offsets.ItemID + 1;
        }
    }
}
