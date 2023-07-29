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
    public class SkillManager
    {
        public static bool Open;
        public string nameWindow = "Skill Manager";

        private List<SkillData> list = new List<SkillData>();
        private Dictionary<SkillData, string> fakeNames = new Dictionary<SkillData, string>();
        private Dictionary<SkillData, int> skillIcons = new Dictionary<SkillData, int>();
        private Dictionary<SkillData, Texture2D> skillIconsTexture2Ds = new Dictionary<SkillData, Texture2D>();

        public string[] options = new[] {"All", "Skill Tree"};

        private bool cached;
        private static int selected;
        private ILocateBuilderHelper _locateBuilder;
        private IImGuiMenuApi imGuiMenuApi;
        private int OptionSelected;

        public SkillManager()
        {

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
                        this.GetList();
                        this.cached = true;
                    }

                    Open = !Open;
                });

            configMenu.AddWindowItem(
                manifest, this.Render, () => Open);

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
            var io = ImGui.GetIO();
            ImGui.SetNextWindowSize(new ImVec2(500, 440), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(this.nameWindow, ref Open))
            {
                this.PopUp();

                if(this.OptionSelected == 0)
                    this.RenderList();
                if(this.OptionSelected == 1)
                    this.RenderSkillTree();
                this.RenderInfo();
            }

            ImGui.EndChild();
        }

        private void RenderList()
        {
            {
                ImGui.BeginChild("left pane", new ImVec2(400, 0), true, ImGuiWindowFlags.Default); //, new ImVec2(150, 0), true);

                var list = this.list;
                for (int i = 0; i < list.Count; i++)
                {
                    var skill = list[i];
                    string fixedName = skill.descripLocID.Replace("LOC_ID_SKILLDETAILS_", "").Replace("_", " ");
                    string skillCode = this.fakeNames[skill].Length != 0 ? this.fakeNames[skill] : skill.iconName;
                    // string enemyName =
                    //     !this.fakeNames[enemy].Contains("LOC_ID_NAME_") && !this.fakeNames[enemy].Contains("NULL") ? $"{enemyFixed} ({this.fakeNames[enemy]})" : enemyFixed;
                    string skillName =
                        !this.fakeNames[skill].Contains("NULL") ? $"{fixedName} ({skillCode})" : fixedName;

                    // if (this.skillIcons.ContainsKey(skill))
                    // {
                    //     if(ImGui.ImageButton(this.skillIcons[skill], new ImVec2(100, 10), ImVec2.One, ImVec2.One, 4, new ImVec4(0, 0, 0, 0), new ImVec4(250, 250, 250, 0)))
                    //     {
                    //         selected = i;
                    //     }
                    // }
                    // else
                    // {
                        if (ImGui.Selectable(i + ":" +skillName, selected == i))
                        {
                            selected = i;
                        }
                    // }


                }

                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

        private void RenderSkillTree()
        {
            {
                ImGui.BeginChild("skilltree pane", new ImVec2(400, 0), true,
                    ImGuiWindowFlags.Default); //, new ImVec2(150, 0), true);

                Dictionary<string, Func<int[,]>> upgradeTrees = ModEntry.ModHelper.Database.GetSkillTrees();

                var size = new ImVec2(this.skillIconsTexture2Ds.Values.First().Width * 2 + 1,
                    this.skillIconsTexture2Ds.Values.First().Height * 2 + 1);

                foreach (var keyValuePair in upgradeTrees)
                {
                    int[,] selectedUpgradeTree = keyValuePair.Value.Invoke();
                    if (ImGui.TreeNode(keyValuePair.Key))
                    {
                        // int classLevel = this.m_player.GetClassLevel(this.m_classType);

                        ImGui.Columns(selectedUpgradeTree.GetLength(1), keyValuePair.Key +"SelectedUpgradeTree", false);
                        for (int col = 2; col < selectedUpgradeTree.GetLength(1) - 3; ++col)
                        {
                            bool found = false;
                            for (int row = 7; row < selectedUpgradeTree.GetLength(0) - 3; ++row)
                            {
                                // if (ImGui.GetColumnIndex() == 0)
                                //     ImGui.Separator();

                                var skill = this.list[selectedUpgradeTree[row, col]];
                                if (this.skillIcons.ContainsKey(skill))
                                {
                                    found = true;
                                    if (ImGui.ImageButton(
                                        this.skillIcons[skill],
                                        new ImVec2(size.x - 1, size.y - 1),
                                        ImVec2.Zero,
                                        ImVec2.One,
                                        1,
                                        new ImVec4(0, 0, 0, 0),
                                        new ImVec4(250, 250, 250, 255)))
                                    {
                                        selected = this.list.IndexOf(skill);
                                    }
                                }
                                else
                                {
                                    ImGui.Button("", size);
                                }

                                // ImGui.ImageButton("%c%c%c", 'a' + i, 'a' + i, 'a' + i);
                                // ImGui.Text("Width %.2f", ImGui.GetColumnWidth());
                                // ImGui.Text("Avail %.2f", ImGui.GetContentRegionAvail().x);
                                // ImGui.Text("Offset %.2f", ImGui.GetColumnOffset());
                                // ImGui.Text("Long text that is likely to clip");
                                // ImGui.Button("Button", ImVec2(-FLT_MIN, 0.0f));

                            }

                            ImGui.NextColumn();

                        }

                        ImGui.Columns(1);
                        ImGui.Separator();
                        ImGui.TreePop();
                    }
                }


                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

        private void RenderInfo()
        {
            ImGui.SameLine();
            {
                ImGui.BeginChild("skillInfo", new ImVec2(200, 0), true, ImGuiWindowFlags.Default);
                var item = this.list[selected];
                if (this.skillIcons.ContainsKey(item))
                {
                    ImGui.Image(this.skillIcons[item], new ImVec2(this.skillIconsTexture2Ds[item].Width * 2, this.skillIconsTexture2Ds[item].Height * 2), ImVec2.Zero, ImVec2.One, new ImVec4(1,1,1,1), new ImVec4(0,0,0,0)); // Here, the previously loaded texture is used
                }

                ImGui.Text(item.iconName);

                ImGui.Text(this._locateBuilder.GetResourceString(item.titleLocID, SaveManager.configFile.languageType));
                ImGui.Text(this._locateBuilder.GetResourceString(item.descripLocID, SaveManager.configFile.languageType));
                ImGui.Text(this._locateBuilder.GetResourceString(item.flavourLocID, SaveManager.configFile.languageType));

                ImGui.Text("index: " + selected);
                ImGui.Text("Cost: " + item.cost);
                ImGui.Text("ClassType: " + item.classType);
                ImGui.Text("CostAppreciation: " + item.costAppreciation);
                ImGui.Text("IsMultiplier: " + item.isMultiplier);
                ImGui.Text("MaxLevel: " + item.maxLevel);
                ImGui.Text("ScalingAppreciation: " + item.scalingAppreciation);

                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

        private List<SkillData> GetList()
        {
            this.list.AddRange(SkillEV.SkillData);

            this.skillIconsTexture2Ds = new Dictionary<SkillData, Texture2D>();
            this.skillIcons = new Dictionary<SkillData, int>();

            foreach (var skill in this.list)
            {
                string key = skill.titleLocID;
                string levelFakeName =
                    ModEntry.ModHelper.LocateBuilder.GetResourceString(key, SaveManager.configFile.languageType);
                this.fakeNames[skill] = levelFakeName;

                if (string.IsNullOrEmpty(skill.iconName)) continue;
                var skillTexture = this.imGuiMenuApi.GetSpriteTexture(skill.iconName, 0);
                int textureId = this.imGuiMenuApi.RegisterTexture(skillTexture);

                this.skillIconsTexture2Ds.Add(skill, skillTexture);
                this.skillIcons.Add(skill, textureId);

            }



            return this.list;
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
