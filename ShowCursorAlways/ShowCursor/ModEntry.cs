using Brawler2D;
using FullModdedFuriesAPI;

namespace ShowCursor
{
    public class ModEntry : Mod
    {
        // private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            // this.Config = this.Helper.ReadConfig<ModConfig>();
            GlobalEV.SUPPORT_MHOOK = true;
        }
    }
}
