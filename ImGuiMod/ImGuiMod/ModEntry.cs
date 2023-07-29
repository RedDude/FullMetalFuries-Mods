using System;
using System.Collections.Generic;
using Brawler2D;
using DevMenu;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Events;
using ImGuiMod.Framework;
using ImGuiNET;
using ImGuiXNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteSystem2;

namespace ImGuiMod
{
    /// <summary>The main entry point for the mod.</summary>
    ///
    public class ModEntry : Mod
    {
        private ModConfig Config;

        /// <summary>Manages registered mod config menus.</summary>
        private readonly ModImGuiManager ConfigManager = new();

        /// <summary>The mod API, if initialized.</summary>
        private Api Api;

        private BrawlerScreenManager screenManager;

        protected ImGuiXNAState ImGuiState;

        /// <inheritdoc />
        public override object GetApi()
        {
            return this.Api ??=
                new Api(this.ConfigManager, ref this.ImGuiState, mod => { }); //this.OpenModMenu(mod, page: null, listScrollRow: null));
        }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // this.Helper.Events.Persistence.ConfigLoaded += (sender, args) =>
            // {
            //     SaveManager.configFile.debugLevelName = "Camp";
            //     // GameController.g_game.ScreenManager.LoadScreen("Camp");
            // };

            this.ImGuiState = new ImGuiXNAState(GameController.g_game);
            this.Helper.Events.GameLoop.GameLaunched += (sender, args) =>
            {
                this.screenManager = GameController.g_game.ScreenManager;
                var context = new ContextMenu();
                context.Register((IImGuiMenuApi) this.GetApi(), this.ModManifest);

            };

            this.Helper.Events.GameLoop.UpdateTicked += this.ToogleImGui();

            this.Helper.Events.GameLoop.UpdateTicked += (sender, args) =>
            {
                if (!GameController.g_game.gameLoaded)
                    return;

                if (GameController.g_game.InputManager.ButtonJustPressed(Buttons.LeftTrigger) != 0)
                    GameController.g_game.PlayerManager.hostPlayer.SwapClass(PlayerSpawnAnimType.Instant, (ClassType)5);

                // var io = ImGui.GetIO();
                // if (io.ShiftPressed)
                //     if (GameController.g_game.PlayerManager.hostPlayer.currentClassType == ClassType.Engineer)
                //     {
                //         var texture2D = this.Helper.Content.Load<Texture2D>("Assets/playerEngineerSpritesheet_credits.png");
                //         ((BrawlerSpriteObj)GameController.g_game.PlayerManager.hostPlayer)).Sprite =
                //             texture2D;
                //     }
                // if (this.IsCamp() &&  this.game.PlayerManager.hostPlayer.currentClassType != PlayerClassObj_Mercenary.MercenaryClassType)
                // {
                //
                // }
            };

        }

        private EventHandler<UpdateTickedEventArgs> ToogleImGui()
        {
            return (sender, args) =>
            {
                if (GameController.g_game.InputManager.KeyJustPressed(Keys.Escape))
                    if (this.CloseImGui(this.screenManager)) return;

                if (!GameController.g_game.InputManager.KeyJustPressed(Keys.F11))
                    return;

                if (this.CloseImGui(this.screenManager)) return;

                ImGuiScreen.Enabled = true;
                this.screenManager.AddScreen(new ImGuiScreen(this.Helper, this.ImGuiState), null);
            };
        }

        private bool CloseImGui(BrawlerScreenManager screenManager)
        {
            if (!ImGuiScreen.Enabled) return false;

            foreach (var screen in screenManager.GetScreens())
            {
                if (!(screen is ImGuiScreen)) continue;
                screenManager.RemoveScreen(screen, false);
                return true;
            }

            ImGuiScreen.Enabled = false;
            return true;
        }
    }
}
