using System;
using System.Collections.Generic;
using System.Linq;
using Brawler2D;
using CDGEngine;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Events;
using ImGuiMod;
using ImGuiNET;
using Microsoft.Xna.Framework;
using QuickStart;

namespace DevMenu
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        public static IModHelper ModHelper;

        private StageSelector _stageSelector = new StageSelector();
        private EnemySpawner _enemySpawner = new EnemySpawner();
        private AssetManager _assetManager = new AssetManager();
        private SkillManager _skillManager = new SkillManager();
        private DataEditor _dataEditor = new DataEditor();
        private Inspector _inspector = new Inspector();
        private LogicBlockInspector _logicBlockinspector = new LogicBlockInspector();
        private static GameController _game;

        private IImGuiMenuApi imGuiMenuApi;
        private MouseContextHandler mouseContextHandler;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.mouseContextHandler = new MouseContextHandler();
            ModHelper = this.Helper;
            _game = GameController.g_game;

            this.Helper.Events.GameLoop.GameLaunched += this.InstallMenu();
        }

        private EventHandler<GameLaunchedEventArgs> InstallMenu()
        {
            return (sender, args) =>
            {
                this.imGuiMenuApi = this.Helper.ModRegistry.GetApi<IImGuiMenuApi>("FMODF.ImGui");
                var configMenu = this.imGuiMenuApi;
                if (configMenu == null) return;

                this.Helper.Events.GameLoop.UpdateTicking += (senderx, argsx) => this.mouseContextHandler.CheckOpenContext(_game,  this.imGuiMenuApi);
                configMenu?.Register(
                    mod: this.ModManifest
                );

                this._stageSelector.Register(configMenu, this.ModManifest);
                this._enemySpawner.Register(configMenu, this.ModManifest);
                this._assetManager.Register(configMenu, this.ModManifest);
                this._skillManager.Register(configMenu, this.ModManifest);
                this._dataEditor.Register(configMenu, this.ModManifest);
                this._inspector.Register(configMenu, this.ModManifest);
                this._logicBlockinspector.Register(configMenu, this.ModManifest);

                configMenu.AddContextMenuOption(
                    this.ModManifest, () => "Destroy All Enemies", () => "Destroy All Enemies",
                    () =>
                    {

                        ArenaScreen arenaScreen = GameController.g_game.ScreenManager.arenaScreen;
                        int globalEnemyArrayCount = arenaScreen.currentAndGlobalEnemyArray_count;
                        for (int index1 = 0; index1 < globalEnemyArrayCount; ++index1)
                        {
                            EnemyObj enemyObj = arenaScreen.currentAndGlobalEnemyArray[index1];
                            if (enemyObj.Active)
                            {
                                GameController.g_game.EnemyManager.KillEnemyDontCount(enemyObj);
                            }
                        }
                    },
                    () =>
                    {
                        return false;
                    });

                configMenu.AddContextMenuOption(
                    this.ModManifest, () => "Spawn", () => "Spawn",
                    () =>
                    {
                        var game = GameController.g_game;
                        float physicalScaleX = game.ScreenManager.virtualScreen.physicalScaleX;
                        float physicalScaleY = game.ScreenManager.virtualScreen.physicalScaleY;
                        var mousePos = game.InputMap.GetMousePos();
                        mousePos.X *= 1f / physicalScaleX;
                        mousePos.Y *= 1f / physicalScaleY;

                        this.SpawnEnemy(EnemyType.Enemy_Minitaur_Basic, mousePos);
                    },
                    () =>
                    {
                        return false;
                    });

                configMenu.AddContextMenuOption(
                    this.ModManifest, () => "Kill", () => "Kill",
                    () =>
                    {
                        foreach (var underMouseEntity in MouseContextHandler.underMouseEntities)
                        {
                            if (underMouseEntity is PlayerClassObj player)
                                player.KillPlayer();
                            if (underMouseEntity is EnemyObj enemyObj)
                            {
                                enemyObj.CurrentHealth = 0;
                                enemyObj.Knockdown(null);
                            }
                        }
                    },
                    () =>
                    {
                        foreach (var underMouseEntity in MouseContextHandler.underMouseEntities)
                        {
                            if (underMouseEntity is PlayerClassObj player && player.CurrentHealth > 0)
                                return false;
                            if (underMouseEntity is EnemyObj enemyObj && enemyObj.CurrentHealth > 0)
                                return false;
                        }
                        return true;
                    });

                configMenu.AddContextMenuOption(
                    this.ModManifest, () => "Revive", () => "Revive",
                    () =>
                    {
                        foreach (var underMouseEntity in MouseContextHandler.underMouseEntities)
                        {
                            if (underMouseEntity is PlayerClassObj player)
                                player.RevivePlayer(false);
                        }
                    },
                    () =>
                    {
                        foreach (var underMouseEntity in MouseContextHandler.underMouseEntities)
                        {
                            if (underMouseEntity is PlayerClassObj playerClassObj)
                                if(playerClassObj.CurrentHealth == 0)
                                    return false;

                            // if (underMouseEntity is EnemyObj)
                                // return false;
                        }

                        return true;
                    });
            };
        }

        public EnemyObj SpawnEnemy(EnemyType enemyType, Vector2 position)
        {
            var enemyManager = GameController.g_game.EnemyManager;
            int enemyObjIndex = enemyManager.PreloadEnemy(enemyType, position);
            var enemyObj = enemyManager.enemyArray[enemyObjIndex];
            enemyObj.spawnAnimType = EnemySpawnAnimType.None;
            GameController.g_game.ScreenManager.arenaScreen.AddToCurrentEnemyList(enemyObj);
            EnemySpawnAnimator.SpawnEnemy(enemyObj, true);

            return enemyObj;
        }
        // public EnemyObj SpawnEnemy(EnemyType enemyType, Vector2 position, EnemySpawnAnimType enemySpawnAnimType = EnemySpawnAnimType.None)
        // {
        //     var enemyManager = GameController.g_game.EnemyManager;
        //     int enemyObjIndex = this.PreloadEnemy(newEnemyObj, position);
        //     var enemyObj = enemyManager.enemyArray[enemyObjIndex];
        //     enemyObj.spawnAnimType = enemySpawnAnimType;
        //     GameController.g_game.ScreenManager.arenaScreen.AddToCurrentEnemyList(enemyObj);
        //     EnemySpawnAnimator.SpawnEnemy(enemyObj, true);
        //
        //     return enemyObj;
        // }


        public EnemyObj SpawnEnemy(EnemyObj newEnemyObj, Vector2 position, EnemySpawnAnimType enemySpawnAnimType = EnemySpawnAnimType.None)
        {
            var enemyManager = GameController.g_game.EnemyManager;
            int enemyObjIndex = this.PreloadEnemy(newEnemyObj, position);
            var enemyObj = enemyManager.enemyArray[enemyObjIndex];
            enemyObj.spawnAnimType = enemySpawnAnimType;
            GameController.g_game.ScreenManager.arenaScreen.AddToCurrentEnemyList(enemyObj);
            EnemySpawnAnimator.SpawnEnemy(enemyObj, true);

            return enemyObj;
        }

        public int PreloadEnemy(EnemyObj enemyObj, Vector2 position)
        {
            var enemyManager = GameController.g_game.EnemyManager;
            foreach (object obj in enemyObj.LoadContent())
                ;
            enemyObj.Initialize();
            enemyObj.isCameraShy = true;
            enemyObj.Active = false;
            enemyObj.Visible = false;
            enemyObj.Collidable = false;
            enemyObj.forceDraw = false;
            enemyObj.Position = position;
            enemyObj.isSpawning = false;
            enemyManager.AddEnemy(enemyObj);
            return enemyManager.enemyArray_count - 1;
        }

        public void ShowExampleAppLayout(bool open) {
            ImGui.SetNextWindowSize(new ImVec2(500, 440), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Level Selector", ref open, ImGuiWindowFlags.MenuBar))
            {
                int selected = 0;
                {
                    ImGui.BeginChild("left pane");//, new ImVec2(150, 0), true);

                    for (int i = 0; i < 10; i++)
                    {
                        if (ImGui.Selectable("Level"+1, selected == i))
                            selected = i;
                    }
                    ImGui.EndChild();
                }
                ImGui.SameLine();

                // Right
                // {
                //     ImGui.BeginGroup();
                //     ImGui.BeginChild("item view", ImVec2(0, -ImGui.GetFrameHeightWithSpacing())); // Leave room for 1 line below us
                //     ImGui.Text("MyObject: %d", selected);
                //     ImGui.Separator();
                //     if (ImGui.BeginTabBar("##Tabs", ImGuiTabBarFlags_None))
                //     {
                //         if (ImGui.BeginTabItem("Description"))
                //         {
                //             ImGui.TextWrapped("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. ");
                //             ImGui.EndTabItem();
                //         }
                //         if (ImGui.BeginTabItem("Details"))
                //         {
                //             ImGui.Text("ID: 0123456789");
                //             ImGui.EndTabItem();
                //         }
                //         ImGui.EndTabBar();
                //     }
                //     ImGui.EndChild();
                //     if (ImGui.Button("Revert")) {}
                //     ImGui.SameLine();
                //     if (ImGui.Button("Save")) {}
                //     ImGui.EndGroup();
                // }
            }
            ImGui.End();
        }
    }
}
