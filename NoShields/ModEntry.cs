using Brawler2D;
using HarmonyLib;

namespace FullModdedFuriesAPI.Mods.NoShields
{
    /// <summary>The main entry point for the mod.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            this.Helper.Events.GameLoop.UpdateTicking += (sender, args) =>
            {

                GlobalEV.CREATE_RETAIL_VERSION = false;
                GlobalEV.DEBUG_MODE = true;
                if (!BlitNet.Lobby.IsMaster) return;
                if (!this.Config.Enabled) return;
                var enemyManager = GameController.g_game.EnemyManager;
                if (enemyManager == null) return;
                if (!enemyManager.areEnemiesAlive) return;

                var enemyArray = enemyManager.enemyArray;
                for (int i = 0; i < enemyManager.enemyArray_count; i++)
                {
                    var enemy = enemyArray[i];
                    if(enemy.IsKilled || !enemy.Active)
                        continue;

                    enemy.shieldToApply = StatusEffect.None;
                    enemy.RemoveAllPlayerShields(false);
                }
            };
        }
    }
}
