using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Framework.ModHelpers.LocalBuilderHelper;
using HarmonyLib;
using ImGuiMod;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuickStart;

namespace DevMenu
{
    public class LogicBlockInspector
    {
        public static bool Open;
        public string nameWindow = "LogicBlockInspector";


        public string[] options = new[] {"Enemies", "Projectiles"};

        private LogicBlock block;

        private bool cached;
        private static int selected;
        private ILocateBuilderHelper _locateBuilder;
        private IImGuiMenuApi imGuiMenuApi;
        private int OptionSelected;
        private LogicAction selectedAction;
        private object goCatLogic;

        public LogicBlockInspector()
        {
            // var test = new LogicSet(this.GetType() + ":m_callArtilleryLargeLogic");
            // LogicSet.Begin(test);
            // // LogicSet.Add(new ChangePropertyAction((object) this, "pulseIndicator", (object) true), SequenceType.Serial);
            // LogicSet.Add(new ChangePropertyAction(this, "State", (object) CharacterState.Attacking));
            // LogicSet.Add(new ChangePropertyAction(this, "CurrentSpeed", (object) 0));
            // // LogicSet.Add(new ChangePropertyAction(this.source, "LockFlip", (object) true));
            //
            // LogicSet.Add(new RunFunctionAction((object) this, "Render"));
            // // LogicSet.Add(new RunFunctionAction((object) this, "RunArtilleryStrike", ls1));
            // LogicSet.Add(new DelayAction(0.1f));
            // // LogicSet.Add(new ChangePropertyAction(this, "m_artilleryTargetPos", (object) this.artilleryTarget.Position));
            // // LogicSet.Add(new ChangePropertyAction((object) this, "pulseIndicator", (object) false), SequenceType.Serial);
            // // LogicSet.Add(new DelayAction(this.m_chatterEndDelay), SequenceType.Serial);
            // // LogicSet.Add(new DelayAction(this.m_chatterStartDelay));
            // // LogicSet.Add(new RunFunctionAction((object) this, "FireArtilleryOnTarget", Vector2.One));
            // LogicSet.Add(new ChangePropertyAction(this, "isArtilleryAttacking", (object) false));
            // LogicSet.Add(new ChangePropertyAction(this, "State", (object) CharacterState.Idle));
            // // LogicSet.Add(new ChangePropertyAction(this.source, "LockFlip", (object) false));
            //
            //
            // // LogicSet.Add(new RunLogicSetAction(this.m_idleLogic), SequenceType.Serial);
            // // LogicSet.Add(new CallCooldownAction((PlayerClassObj) this), SequenceType.Serial);
            // LogicSet.End();
            //
            // this.block = new LogicBlock(RNG.seed);
            // this.block.AddLogicSet(test);
        }

        public LogicBlockInspector(LogicBlock block)
        {
            this.block = block;
        }

        public void Register(IImGuiMenuApi configMenu, IManifest manifest)
        {
            configMenu.AddMainBarMenuOption(
                manifest, () => "DevTools", () => this.nameWindow,
                () =>
                {
                    if (!this.cached)
                    {
                        this._locateBuilder = ModEntry.ModHelper.LocateBuilder;
                        this.cached = true;
                    }

                    Open = !Open;
                });

            configMenu.AddWindowItem(
                manifest, this.Render, () => Open);

            configMenu.AddContextMenuOption(
                manifest, () => "Logic Block", () => "Logic Block",
                () =>
                {
                    Open = true;
                    //
                    // foreach (var underMouseEntity in ModEntry.underMouseEntities)
                    //     if (underMouseEntity is EnemyObj || underMouseEntity is PlayerClassObj)
                    //     {
                    //         this.targetFields = this.GetFields();
                    //     }
                },
                () =>
                {
                    foreach (var underMouseEntity in MouseContextHandler.underMouseEntities)
                        if (underMouseEntity is EnemyObj || underMouseEntity is PlayerClassObj)
                            return false;
                    return true;
                });


            this.imGuiMenuApi = configMenu;
            //
            // int[,] upgradeTree = SkillEV.EngineerUpgradeTree;
            // for (int x = 0; x < upgradeTree.GetLength(1); ++x)
            // {
            //     for (int y = 0; y < upgradeTree.GetLength(0); ++y)
            //     {
            //         upgradeTree[y, x] = 2;
            //     }
            // }
            //
            // SkillEV.EngineerUpgradeTree = upgradeTree;
        }

        public void Render()
        {
            if (this.block == null && this.goCatLogic == null)
            {
                var enemyManager = GameController.g_game.EnemyManager;
                if (enemyManager == null) return;
                var enemyArray = enemyManager.enemyArray;

                for (int i = 0; i < enemyManager.enemyArray_count; i++)
                {
                    var enemy = enemyArray[i];
                    string enemyName = enemy.Name;
                    if (enemyName.ToLower().Contains("cat"))
                    {
                        var logicBlock = ModEntry.ModHelper.Reflection.GetField<LogicBlock>(enemy, "m_wanderRangeLB").GetValue();
                        this.goCatLogic = logicBlock;
                        this.block = (LogicBlock) this.goCatLogic;
                    }
                }
            }

            var io = ImGui.GetIO();
            ImGui.SetNextWindowSize(new ImVec2(500, 440), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(this.nameWindow, ref Open))
            {
                this.RenderList();
                this.RenderActionInfo();
            }

            ImGui.EndChild();
        }

        private void RenderList()
        {
            {
                ImGui.BeginChild("left pane", new ImVec2(400, 0), true, ImGuiWindowFlags.Default); //, new ImVec2(150, 0), true);

                if (this.block == null)
                {
                    ImGui.EndChild();
                    ImGui.SameLine();
                    return;
                }

                for (int i = 0; i < this.block.LogicSetList.Count; i++)
                {
                    var logicSet = this.block.LogicSetList[i];
                    bool treeNode = ImGui.TreeNode(logicSet.Name);
                    if (treeNode)
                    {
                        this.DrawActionsTree(logicSet);
                        selected = i;
                    }
                }

                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

        private void DrawActionsTree(LogicSet set)
        {
            this.AddActionToTreeAndGotNext(set, set.firstNode);
        }

        private void AddActionToTreeAndGotNext(LogicSet set, LogicAction logicAction)
        {
            if (ImGui.Selectable((set.ActiveLogicAction == logicAction ? "* " : "") + logicAction.Name + "("+logicAction.GetHashCode()+")",  this.selectedAction == logicAction))
            {
                this.selectedAction = logicAction;
            }
            if(set.lastNode != logicAction)
                this.AddActionToTreeAndGotNext(set, logicAction.NextLogicAction);
        }

        private void PopUp()
        {
            //if (ImGui.Button(this.Selected == -1 ? "<None>" : this.options[this.Selected]))
            if (ImGui.Button("select"))
                ImGui.OpenPopup("my_select_popup");

            if (ImGui.BeginPopup("my_select_popup"))
            {
                for (int i = 0; i < this.options.Length; i++)
                    if (ImGui.Selectable(this.options[i]))
                    {
                        this.OptionSelected = i;
                    }

                ImGui.EndPopup();
            }
        }

        private void RenderActionInfo()
        {
            ImGui.SameLine();
            {
                ImGui.BeginChild("ActionInfo", new ImVec2(500, 0), true, ImGuiWindowFlags.Default );
                if (this.selectedAction == null)
                {
                    ImGui.EndChild();
                    ImGui.SameLine();
                    return;
                }

                // if (this.skillIcons.ContainsKey(item))
                // {
                //     ImGui.Image(this.skillIcons[item], new ImVec2(this.skillIconsTexture2Ds[item].Width * 2, this.skillIconsTexture2Ds[item].Height * 2), ImVec2.Zero, ImVec2.One, new ImVec4(1,1,1,1), new ImVec4(0,0,0,0)); // Here, the previously loaded texture is used
                // }

                ImGui.Text(this.selectedAction.Name);

                var props = this.selectedAction.GetType().GetProperties();
                var propsPrivate = this.selectedAction.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                var allProps = propsPrivate.AddRangeToArray(props);
                foreach (var prop in allProps)
                {
                    try
                    {
                        // if (prop.Name == "CollisionID")
                        // {
                        //     prop.SetValue(projectile, 3);
                        // }

                        // if (prop.PropertyType == typeof(string))
                        //     ImGui.InputText(prop.Name, (string)prop.GetValue(projectile, null).ToString());
                        // else if (prop.PropertyType == typeof(bool))
                        //     ImGui.Checkbox(prop.Name, (bool)prop.GetValue(projectile, null));
                        // else if (prop.PropertyType == typeof(int))
                        //     ImGui.DragInt(prop.Name, (int)prop.GetValue(projectile, null), );
                        // else
                            ImGui.Text(prop.Name + ": " + prop.GetValue(this.selectedAction, null));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    var fields = this.selectedAction.GetType().GetFields();
                    var fieldsPrivate = this.selectedAction.GetType()
                        .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                    var allFields = fieldsPrivate.AddRangeToArray(fields);
                    foreach (var field in allFields)
                    {
                        try
                        {
                            if(field.GetType() == typeof(LogicBlock))
                            {
                                if (ImGui.Button(field.Name + ": " + field.GetValue(this.selectedAction)))
                                    new LogicBlockInspector((LogicBlock) field.GetValue(this.selectedAction));
                                continue;
                            }
                            ImGui.Text(field.Name + ": " + field.GetValue(this.selectedAction));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }

                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

    }

}
