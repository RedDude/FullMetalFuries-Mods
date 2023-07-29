using System;
using System.Collections.Generic;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using FullModdedFuriesAPI;
using HarmonyLib;
using ImGuiMod;
using ImGuiNET;
using Microsoft.Xna.Framework;
using QuickStart;

namespace DevMenu
{
    public class EnemySpawner
    {
        public static bool Open;
        public string nameWindow = "Enemy Spawner";

        private List<string> list = new List<string>();
        private Dictionary<string, string> fakeNames = new Dictionary<string, string>();
        private bool cached;

        public void Register(IImGuiMenuApi configMenu, IManifest manifest)
        {
            configMenu.AddMainBarMenuOption(
                manifest, () => "DevTools", () => this.nameWindow,
                () =>
                {
                    if (!this.cached)
                    {
                        this.GetEnemyList();
                        this.cached = true;
                    }

                    Open = !Open;
                });

            configMenu.AddWindowItem(
                manifest, this.Render, () => Open);

            var harmony = new Harmony("Enemy Spawner");
            harmony.Patch(
                original: AccessTools.Method(typeof(Boss_Charon), "InitializeDeathAnimationLogic"),
                prefix: this.GetHarmonyMethod(nameof(this.Before_Charon_InitializeDeathAnimationLogic))
            );

        }
        public void Render()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowSize(new ImVec2(500, 440), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(this.nameWindow, ref Open))
            {
                int selected = 0;
                {
                    ImGui.BeginChild("left pane"); //, new ImVec2(150, 0), true);

                    var names = this.list;
                    for (int i = 0; i < this.list.Count; i++)
                    {
                        string enemy = this.list[i];
                        string enemyFixed = enemy.Replace("Enemy", "").Replace("_", " ");
                        string enemyName =
                            !this.fakeNames[enemy].Contains("LOC_ID_NAME_") && !this.fakeNames[enemy].Contains("NULL") ? $"{enemyFixed} ({this.fakeNames[enemy]})" : enemyFixed;

                        if (ImGui.Selectable(enemyName.Trim(), selected == i))
                        {
                            selected = i;
                            var spawnEnemy = this.SpawnEnemy((EnemyType) i);

                            if (io.ShiftPressed)
                                MakeEnemyControllable(spawnEnemy); //TEMP
                        }
                    }

                    ImGui.EndChild();
                }
                ImGui.SameLine();
            }
            ImGui.End();
        }
        private List<string> GetEnemyList()
        {
            this.list.AddRange(Enum.GetNames(typeof(EnemyType)));
            // this.list.Remove(Enum.GetName(typeof(EnemyType), EnemyType.ENEMY_COUNTABLE_INDEX_1));
            // this.list.Remove(Enum.GetName(typeof(EnemyType), EnemyType.Boss_Sun));
            // this.list.Remove(Enum.GetName(typeof(EnemyType), EnemyType.Enemy_BigLeech));
            // this.list.Remove(Enum.GetName(typeof(EnemyType), EnemyType.Hazard_SpikeTrap_Floor));
            this.list.RemoveAt(this.list.Count - 1);

                // var GetString = ModEntry.ModHelper.Reflection.GetMethod(localBuilderType, "GetString");
                foreach (string name in this.list)
                {
                    string key = name.ToUpper()
                        .Replace("ENEMY_", "LOC_ID_NAME_")
                        .Replace("BASIC", "B_1")
                        .Replace("ADVANCED", "A_1")
                        .Replace("EXPERT", "E_1")
                        .Replace("MINIBOSS", "MB_1");

                    // string levelFakeName = GetString.Invoke<string>(name, null); //
                    string levelFakeName =
                        ModEntry.ModHelper.LocateBuilder.GetResourceString(key, SaveManager.configFile.languageType);
                    this.fakeNames[name] = levelFakeName;
            }
            // this.enemyList = GameController.g_game.ScreenManager.stageNameList;
            return this.list;
        }

        private void SpawnEnemyOnMouse(EnemyType enemyType)
        {
            if(enemyType == EnemyType.None || enemyType == EnemyType.ENEMY_MAX)
                return;

            var game = GameController.g_game;
            float physicalScaleX = game.ScreenManager.virtualScreen.physicalScaleX;
            float physicalScaleY = game.ScreenManager.virtualScreen.physicalScaleY;
            var mousePos = game.InputMap.GetMousePos();
            mousePos.X *= 1f / physicalScaleX;
            mousePos.Y *= 1f / physicalScaleY;
        }

        public EnemyObj SpawnEnemy(EnemyType enemyType)
        {
            ArenaZone currentZone = (GameController.g_game.ScreenManager.GetScreens()[0] as ArenaScreen).currentZone;
            CDGRect cdgRect = currentZone.zoneRect.ToCDGRect();
            NavMeshObj navMesh = currentZone.navMesh;
            if (navMesh != null)
                cdgRect = navMesh.AbsBounds;
            var position = new Vector2(RNG.get(205).RandomFloat(cdgRect.Left, cdgRect.Right), RNG.get(206).RandomFloat(cdgRect.Top, cdgRect.Bottom));
            return this.SpawnEnemy(enemyType, position);
        }

        public EnemyObj SpawnEnemy(EnemyType enemyType, Vector2 position)
        {
            int enemyObjIndex = GameController.g_game.EnemyManager.PreloadEnemy(enemyType, position);
            var enemyObj = GameController.g_game.EnemyManager.enemyArray[enemyObjIndex];
            enemyObj.spawnAnimType = EnemySpawnAnimType.None;

            GameController.g_game.ScreenManager.arenaScreen.AddToCurrentEnemyList(enemyObj);
            EnemySpawnAnimator.SpawnEnemy((EnemyObj) enemyObj, true);

            return enemyObj;
        }

        private static void MakeEnemyControllable(EnemyObj enemyObj)
        {
            var logicBlock = new LogicBlock(RNG.seed);
            var idleLogic = new LogicSet("IDLE");
            LogicSet.Begin(idleLogic);
            // LogicSet.Add(new ChangePropertyAction(enemyObj, "State", (object) CharacterState.Idle));
            // LogicSet.Add(new ChangePropertyAction(enemyObj, "CurrentSpeed", (object) 0));
            // LogicSet.Add(new PlayAnimationAction(enemyObj, "StandStart", "StandEnd", true));
            LogicSet.Add(new ChangeColourAction(enemyObj, Color.Yellow, 20f, -1));
            LogicSet.End();

            var walkToMeleeLogic = new LogicSet("Control_Enemy");
            LogicSet.Begin(walkToMeleeLogic);
            // LogicSet.Add(new ChangePropertyAction((object) enemyObj, "State", (object) CharacterState.Walking),
                // SequenceType.Serial);
            // LogicSet.Add(new PlayAnimationAction((DisplayObj) enemyObj, "RunStart", "RunEnd", true, false, false),
                // SequenceType.Serial);
            // LogicSet.Add((LogicAction) new ChangePropertyAction((object) this, "CurrentSpeed", (object) spawnEnemy.Speed), SequenceType.Serial);
            LogicSet.Add(new ChangeColourAction(enemyObj, Color.Yellow, 20f, -1),
                SequenceType.Serial);
            LogicSet.Add(new MoveUsingInput(enemyObj, 0f, false), SequenceType.Serial);
            LogicSet.Add(new RunLogicSetAction(idleLogic), SequenceType.Serial);
            LogicSet.End();
            logicBlock.AddLogicSet(walkToMeleeLogic, idleLogic);

            var empty = new LogicBlock(RNG.seed);
            empty.AddLogicSet(idleLogic);

            enemyObj.SetWanderRangeLogic(logicBlock, 100, 0);
            enemyObj.SetMeleeRangeLogic(logicBlock, 100, 0);
            enemyObj.SetProjectileRangeLogic(logicBlock, 100, 0);
        }


        public static bool Before_Charon_InitializeDeathAnimationLogic()
        {
            return (GameController.g_game.ScreenManager.arenaScreen.stageName == "Level_W04_TitanBoss_01");
        }

        /// <summary>Get a Harmony patch method on the current patcher instance.</summary>
        /// <param name="name">The method name.</param>
        /// <param name="priority">The patch priority to apply, usually specified using Harmony's <see cref="Priority"/> enum, or <c>null</c> to keep the default value.</param>
        protected HarmonyMethod GetHarmonyMethod(string name, int? priority = null)
        {
            var method = new HarmonyMethod(
                AccessTools.Method(this.GetType(), name)
                ?? throw new InvalidOperationException($"Can't find patcher method {name}.")
            );

            if (priority.HasValue)
                method.priority = priority.Value;

            return method;
        }
    }
}
