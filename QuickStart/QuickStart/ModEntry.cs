using System;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Events;

// using HarmonyLib;

namespace QuickStart
{
    /// <summary>The main entry point for the mod.</summary>
    ///

    public class ModEntry : Mod
    {
        private ModConfig Config;

        // static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        // {
        //     bool foundMassUsageMethod = false;
        //     int startIndex = -1;
        //     int endIndex = -1;
        //
        //     var codes = new List<CodeInstruction>(instructions);
        //     for (int i = 0; i < codes.Count; i++)
        //     {
        //         if (codes[i].opcode != OpCodes.Ldstr) continue;
        //
        //         if (codes[i-1].operand.ToString().Contains("debugLevelName"))
        //         {
        //             codes[i].operand = "Camp";
        //             break;
        //         }

            // }
            // if (foundMassUsageMethod)
            // {
            // Log.Error("END " + i);

            // endIndex = i; // include current 'ret'
            // break;
            // }
            //     // Log.Error("START " + (i + 1));
            //
            //     startIndex = i + 1; // exclude current 'ret'
            //
            //     for (int j = startIndex; j < codes.Count; j++)
            //     {
            //         if (codes[j].opcode == OpCodes.Ret)
            //             break;
            //         string strOperand = codes[j].operand as string;
            //         if (strOperand != "TooBigCaravanMassUsage") continue;
            //         foundMassUsageMethod = true;
            //         break;
            //     }
            // }
            // if (startIndex > -1 && endIndex > -1)
            // {
            //     // we cannot remove the first code of our range since some jump actually jumps to
            //     // it, so we replace it with a no-op instead of fixing that jump (easier).
            //     codes[startIndex].opcode = OpCodes.Nop;
            //     codes.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
            // }

            // return codes.AsEnumerable();
        // }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            if(!this.Config.Enabled)
                return;
            // var field = typeof(GameController).GetField("gameLoaded",
                // BindingFlags.Static |
                // BindingFlags.NonPublic);

            // field.SetValue(null, "Camp");
            // Normally the first argument to "SetValue" is the instance
            // of the type but since we are mutating a static field we pass "null"
            // field.SetValue(null, "baz");
            // this.Helper.Events.GameLoop.GameLaunched += (sender, args) =>
            // {
            //     GameController.g_game.load
            //     // GameController.g_game.ScreenManager.LoadScreen("Camp");
            //     // while (!GameController.g_game.gameLoaded)
            //     // {
            //     //
            //     // }
            //     // if(!GameController.g_game.gameLoaded)
            //     //     return;
            //     // if(!firstTime)
            //         // return;
            //
            //     // firstTime = false;
            //     // SaveManager.configFile.debugLevelName = "Camp";
            //     // GameController.g_game.ScreenManager.LoadScreen("Camp");
            // };

            // this.Helper.Events.GameLoop.GameLaunched += (sender, args) =>
            // {
            //     SaveManager.configFile.debugLevelName = "Camp";
            //     // var reflectedField = this.Helper.Reflection.GetField<string>(GameController.g_game, "m_editorLevelName");
            //     // reflectedField.SetValue("Camp");
            // };
            bool firstTime = true;

            this.Helper.Events.GameLoop.UpdateTicked += this.QuickStartInit();

            // this.Helper.Events.GameLoop.UpdateTicked += this.CheckStuff();
            // this.Helper.Events.Screen.ScreenLoading += (sender, args) =>
            // {
            //     this.Monitor.Log("ScreenLoading");
            //     this.Monitor.Log(args.ScreenType.ToString());
            //     this.Monitor.Log(args.levelName);
            // };
            //
            // this.Helper.Events.Screen.ScreenLoaded += (sender, args) =>
            // {
            //     this.Monitor.Log("ScreenLoaded");
            //     this.Monitor.Log(args.ScreenType.ToString());
            //     this.Monitor.Log(args.levelName);
            // };
            // var selectedPlayer2 =  playerManager.ActivatePlayer(2);
            // PremptiveJoinPlayer(1, 2);
            // selectedPlayer2.profileIndex = (sbyte) (3);
            // SaveManager.LoadGame(3, true);
            // SaveManager.LoadControls(selectedPlayer2, selectedPlayer.game.cultureInfo);

            // var harmony = new Harmony(this.ModManifest.UniqueID);
            // harmony.Patch(
            //     original: AccessTools.Method(typeof(SaveManager), "LoadConfig"),// nameof(SaveManager.LoadConfig)),
            //     postfix: new HarmonyMethod(typeof(ModEntry), nameof(DebugScene_Postfix))
            // );
            // var harmony = new Harmony(this.ModManifest.UniqueID);
            // harmony.Patch(
            //     original: AccessTools.Method(typeof(GameController), "LoadContent"),
            //     transpiler: new HarmonyMethod(typeof(ModEntry), nameof(Transpiler))
            // );

            // Task.Delay(100).ContinueWith(t =>
            // {
            //     // GameController.g_game.ScreenManager.LoadScreen("Camp");
            //     var field = typeof(GameController).GetField("m_editorLevelName",
            //         BindingFlags.Static |
            //         BindingFlags.NonPublic);
            //     field.SetValue(GameController.g_game, "Camp");
            // });

            // var soundTimer = this.Helper.Reflection.GetField<string>(typeof(GameController), "");



            //
            // Patches.Initialize(this.Monitor, this.Config);
            //

        }

        private EventHandler<UpdateTickedEventArgs> CheckStuff()
        {
            return (sender, args) =>
            {
                if (!GameController.g_game.gameLoaded)
                    return;

                if (!BlitNet.Lobby.IsMaster || (!GameController.g_game.PlayerManager.allPlayersDown ||
                                                GameController.g_game.PlayerManager.numActivePlayers <= 0))
                    return;
                // this.Helper.Reflection.GetField<bool>(GameController.g_game.ScreenManager.arenaScreen,
                // "awaitingRunMissionFailed").SetValue(true);

                var logicSet = this.Helper.Reflection.GetField<LogicSet>(
                    GameController.g_game.ScreenManager.arenaScreen,
                    "m_missionFailedLS").GetValue();
                if (!logicSet.IsActive) return;

                logicSet.ForceStop();

                var enemyManager = GameController.g_game.EnemyManager;
                if (enemyManager == null) return;
                if (!enemyManager.areEnemiesAlive) return;

                var enemyArray = enemyManager.enemyArray;
                for (int i = 0; i < enemyManager.enemyArray_count; i++)
                {
                    var enemy = enemyArray[i];
                    if (enemy.IsKilled || !enemy.Active)
                        continue;

                    enemy.StopAllActions();
                }


                // this.Helper.Reflection.GetMethod(GameController.g_game.ScreenManager.arenaScreen, "CancelGameOverAnimation").Invoke();
                // Tween.RunFunction(0.75f, (object) this, "RunMissionFailed");
                // this.awaitingRunMissionFailed = true;
            };
        }

        private int ticksToWait = 50;

        private EventHandler<UpdateTickedEventArgs> QuickStartInit()
        {
            return (sender, args) =>
            {
                if(!GameController.g_game.gameLoaded)
                    return;

                if (this.ticksToWait > 0)
                {
                    this.ticksToWait--;
                    return;
                }
                GameController.g_game.ScreenManager.LoadScreen(
                    string.IsNullOrWhiteSpace(this.Config.MapName)
                    ? "Camp" : this.Config.MapName);

                int numOfPlayers = this.Config.Players;
                for (int i = 1; i <= numOfPlayers; i++)
                {
                    var playerManager = GameController.g_game.PlayerManager;
                    var selectedPlayer = playerManager.ActivatePlayer((sbyte) i);
                    selectedPlayer.useKeyboard = i == 1 && this.Config.UseKeyboard; //true;//controllerIndex == (sbyte) 0;
                    selectedPlayer.justJoinedGame = false;
                    selectedPlayer.isHost = i == 1; ///controllerIndex == this.hostControllerIndex;
                    int playerClass = i == 1 ? this.Config.Player1Class
                        : i == 2 ? this.Config.Player2Class
                        : i == 3 ? this.Config.Player3Class
                        : this.Config.Player4Class;
                    selectedPlayer.SwapClass(PlayerSpawnAnimType.Instant, (ClassType) playerClass);

                    int playerProfile = i == 1 ? this.Config.Player1Profile
                        : i == 2 ? this.Config.Player2Profile
                        : i == 3 ? this.Config.Player3Profile
                        : this.Config.Player4Profile;

                    // SaveManager.ResetStats(selectedPlayer);
                    selectedPlayer.profileIndex = (sbyte) playerProfile;
                    SaveManager.LoadGame(i, true);
                    SaveManager.LoadControls(selectedPlayer, selectedPlayer.game.cultureInfo);
                }

                this.Helper.Events.GameLoop.UpdateTicked -= this.QuickStartInit();
            };
        }
    }
}
