using Brawler2D;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Framework
{
    public static class CharacterSelectionPatcherUtil
    {
        public static IModHelper Helper;

        public static ClassType GetCustomCharacterCount()
        {
            return (ClassType) (Helper.Database.GetClassesNames().Count - 1);
        }
    }
}
