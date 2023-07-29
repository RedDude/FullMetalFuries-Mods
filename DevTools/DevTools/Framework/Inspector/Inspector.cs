using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using DevMenu.Drawers;
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
    public class Inspector
    {
        public static bool Open;
        public string nameWindow = "Inspector";

        private List<string> list = new List<string>();
        private Dictionary<string, string> fakeNames = new Dictionary<string, string>();

        public string[] options = new[] {"Enemies", "Projectiles"};

        private bool cached;
        private static int selected;
        private ILocateBuilderHelper _locateBuilder;
        private IImGuiMenuApi imGuiMenuApi;
        private int OptionSelected;

        public InspectorPanel InspectorPanel;
        public Inspector()
        {
            this.InspectorPanel = new InspectorPanel();
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
                manifest, () => "Inspect", () => "Inspect",
                this.HandleContextMenuClick,
                () =>
                {
                    foreach (var underMouseEntity in MouseContextHandler.underMouseEntities)
                        if (underMouseEntity is EnemyObj || underMouseEntity is ProjectileObj
                                                         // || underMouseEntity is PlayerClassObj
                        )
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

        private void HandleContextMenuClick()
        {
            foreach (var underMouseEntity in MouseContextHandler.underMouseEntities)
            {
                if (underMouseEntity is EnemyObj enemy)
                {
                    this.OptionSelected = 0;
                    var enemyManager = GameController.g_game.EnemyManager;
                    var enemyArray = enemyManager.enemyArray;
                    for (int i = 0; i < enemyManager.enemyArray_count; i++)
                        if (enemyArray[i] == enemy)
                        {
                            selected = i;
                            break;
                        }
                }

                if (underMouseEntity is ProjectileObj projectile)
                {
                    this.OptionSelected = 1;
                    var projectileManager = GameController.g_game.ProjectileManager;
                    if (projectileManager == null) return;
                    var projectileList = projectileManager.activeObjList.ToList();
                    for (int i = 0; i < projectileList.Count; i++)
                        if (projectileList[i] == projectile)
                        {
                            selected = i;
                            break;
                        }
                }

                // if (underMouseEntity is PlayerClassObj player)
                // {
                //     this.OptionSelected = 2;
                // }
                Open = true;
            }
        }

        private Dictionary<string, string> GetEnemyNameList()
        {
            this.list.AddRange(Enum.GetNames(typeof(EnemyType)));
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
            return this.fakeNames;
        }

        public void Render()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowSize(new ImVec2(500, 440), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(this.nameWindow, ref Open))
            {
                this.PopUp();

                if (this.OptionSelected == 0)
                {
                    this.RenderList();
                    this.RenderEnemyInfo();
                }
                if (this.OptionSelected == 1)
                {
                    this.RenderProjectilesList();
                    this.RenderProjectilesInfo();
                }
            }

            ImGui.EndChild();
        }

        private void RenderList()
        {
            {
                ImGui.BeginChild("left pane", new ImVec2(400, 0), true, ImGuiWindowFlags.Default); //, new ImVec2(150, 0), true);

                var enemyManager = GameController.g_game.EnemyManager;
                if (enemyManager == null) return;
                if (!enemyManager.areEnemiesAlive) return;

                var enemyArray = enemyManager.enemyArray;
                for (int i = 0; i < enemyManager.enemyArray_count; i++)
                {
                    var enemy = enemyArray[i];
                    string enemyName = enemy.Name;
                    // skill.descripLocID.Replace("LOC_ID_SKILLDETAILS_", "").Replace("_", " ");
                    // string enemyName = this.fakeNames[fixedName].Length != 0 ? this.fakeNames[fixedName] : enemy.Name;
                    // string enemyName =
                    //     !this.fakeNames[enemy].Contains("LOC_ID_NAME_") && !this.fakeNames[enemy].Contains("NULL") ? $"{enemyFixed} ({this.fakeNames[enemy]})" : enemyFixed;
                    // string skillName =
                        // !this.fakeNames[skill].Contains("NULL") ? $"{fixedName} ({skillCode})" : fixedName;

                    if(enemy.IsKilled || !enemy.Active)
                        continue;

                    if (ImGui.Selectable(i + ":" +enemyName, selected == i))
                        selected = i;
                }

                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

        private void RenderProjectilesList()
        {
            {
                ImGui.BeginChild("left pane", new ImVec2(400, 0), true, ImGuiWindowFlags.Default); //, new ImVec2(150, 0), true);

                var projectileManager = GameController.g_game.ProjectileManager;
                if (projectileManager == null) return;
                var projectileList = projectileManager.activeObjList.ToList();
                for (int i = 0; i < projectileList.Count; i++)
                {
                    var  projectile = projectileList[i];
                    string fixedName =  projectile.Name ?? projectile.spriteName;
                    // skill.descripLocID.Replace("LOC_ID_SKILLDETAILS_", "").Replace("_", " ");
                    // string  projectileName = this.fakeNames[fixedName].Length != 0 ? this.fakeNames[fixedName] :  projectile.Name;
                    // string  projectileName =
                    //     !this.fakeNames[ projectile].Contains("LOC_ID_NAME_") && !this.fakeNames[ projectile].Contains("NULL") ? $"{ projectileFixed} ({this.fakeNames[ projectile]})" :  projectileFixed;
                    // string skillName =
                    // !this.fakeNames[skill].Contains("NULL") ? $"{fixedName} ({skillCode})" : fixedName;

                    if(!projectile.Active)
                        continue;

                    if (ImGui.Selectable(i + ":" + fixedName, selected == i))
                    {
                        selected = i;
                    }
                }

                var list = this.list;

                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

        private void RenderEnemyInfo()
        {
            ImGui.SameLine();
            {
                ImGui.BeginChild("enemyInfo", new ImVec2(200, 0), true, ImGuiWindowFlags.Default);

                var enemyManager = GameController.g_game.EnemyManager;
                if (enemyManager == null) return;
                var enemyArray = enemyManager.enemyArray;
                EnemyObj enemy;
                try
                {
                    enemy = enemyArray[selected];
                }
                catch (Exception e)
                {
                    selected = -1;
                    ImGui.EndChild();
                    ImGui.SameLine();
                    return;
                }

                string enemyName = enemy.Name;
                    // skill.descripLocID.Replace("LOC_ID_SKILLDETAILS_", "").Replace("_", " ");
                // string enemyName = this.fakeNames[fixedName].Length != 0 ? this.fakeNames[fixedName] : enemy.Name;
                // string enemyName =
                //     !this.fakeNames[enemy].Contains("LOC_ID_NAME_") && !this.fakeNames[enemy].Contains("NULL") ? $"{enemyFixed} ({this.fakeNames[enemy]})" : enemyFixed;
                // string skillName =
                // !this.fakeNames[skill].Contains("NULL") ? $"{fixedName} ({skillCode})" : fixedName;

                if (enemy.IsKilled || !enemy.Active)
                {
                    ImGui.EndChild();
                    ImGui.SameLine();
                    return;
                }

                // if (this.skillIcons.ContainsKey(item))
                // {
                //     ImGui.Image(this.skillIcons[item], new ImVec2(this.skillIconsTexture2Ds[item].Width * 2, this.skillIconsTexture2Ds[item].Height * 2), ImVec2.Zero, ImVec2.One, new ImVec4(1,1,1,1), new ImVec4(0,0,0,0)); // Here, the previously loaded texture is used
                // }
                ImGui.Text(enemyName);
                this.InspectorPanel.SetInspectedObject(enemy);
                this.InspectorPanel.Draw();


                // var props = enemy.GetType().GetProperties();
                // foreach (var prop in props)
                // {
                //     ImGui.Text(prop.Name + ": " + prop.GetValue(enemy, null));
                // }

                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

        private void RenderProjectilesInfo()
        {
            ImGui.SameLine();
            {
                ImGui.BeginChild("ProjectileInfo", new ImVec2(200, 0), true, ImGuiWindowFlags.Default);

                var projectileManager = GameController.g_game.ProjectileManager;
                if (projectileManager == null) return;
                var projectileList = projectileManager.activeObjList.ToList();

                if(projectileList.Count == 0)
                    return;

                var projectile = projectileList[selected];
                if(projectile == null)
                    return;

                string fixedName =  projectile.Name ?? projectile.spriteName;
                    // skill.descripLocID.Replace("LOC_ID_SKILLDETAILS_", "").Replace("_", " ");
                // string enemyName = this.fakeNames[fixedName].Length != 0 ? this.fakeNames[fixedName] : projectile.Name;
                // string enemyName =
                //     !this.fakeNames[enemy].Contains("LOC_ID_NAME_") && !this.fakeNames[enemy].Contains("NULL") ? $"{enemyFixed} ({this.fakeNames[enemy]})" : enemyFixed;
                // string skillName =
                // !this.fakeNames[skill].Contains("NULL") ? $"{fixedName} ({skillCode})" : fixedName;

                if (!projectile.Active)
                {
                    ImGui.EndChild();
                    ImGui.SameLine();
                    return;
                }
                    ;

                // if (this.skillIcons.ContainsKey(item))
                // {
                //     ImGui.Image(this.skillIcons[item], new ImVec2(this.skillIconsTexture2Ds[item].Width * 2, this.skillIconsTexture2Ds[item].Height * 2), ImVec2.Zero, ImVec2.One, new ImVec4(1,1,1,1), new ImVec4(0,0,0,0)); // Here, the previously loaded texture is used
                // }

                ImGui.Text(fixedName);
                this.InspectorPanel.SetInspectedObject(projectile);
                this.InspectorPanel.Draw();

                var props = projectile.GetType().GetProperties();
                foreach (var prop in props)
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
                            ImGui.Text(prop.Name + ": " + prop.GetValue(projectile, null));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }

                ImGui.EndChild();
            }
            ImGui.SameLine();
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

    }
}
