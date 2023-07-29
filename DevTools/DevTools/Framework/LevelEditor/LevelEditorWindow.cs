using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Brawler2D;
using FullModdedFuriesAPI;
using ImGuiMod;
using ImGuiNET;

namespace DevMenu
{
    public class LevelEditorWindow
    {
        public static bool Open;
        public static bool levelsCached;
        public Dictionary<string, string> fakeNames = new Dictionary<string, string>();
        private List<string> stageNames;
        private string nameWindow = "Stage Selector";
        public void Register(IImGuiMenuApi configMenu, IManifest manifest)
        {

            configMenu.AddMainBarMenuOption(
                manifest, () => "DevTools", () => this.nameWindow,
                () =>
                {
                    if (!levelsCached)
                    {
                        this.stageNames = this.GetStageNames();
                        this.CreateStageFakeNames(this.stageNames);
                        levelsCached = true;
                    }

                    Open = !Open;
                });

            configMenu.AddWindowItem(
                manifest, this.Render, () => Open);
        }
        public void Render()
        {
            ImGui.SetNextWindowSize(new ImVec2(500, 440), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(this.nameWindow, ref Open))
            {
                int selected = 0;
                {
                    ImGui.BeginChild("left pane"); //, new ImVec2(150, 0), true);

                    var names = this.stageNames;
                    for (int i = 0; i < names.Count; i++)
                    {
                        // typeof(localBuilderType).Assembly;
                        // Assembly.GetType();

                        if (!names[i].Contains("Level_") && !names[i].Contains("PrometheusGate_") &&
                            (names[i] != "Camp" || !names[i].Contains("Camp_")))
                            continue;

                        // FIXME: Good candidate to use ImGuiSelectableFlags_SelectOnNav
                        string stageName = this.fakeNames[names[i]].Contains("NULL") ? names[i] : $"{this.fakeNames[names[i]]} ({names[i]})";
                        if (ImGui.Selectable(stageName,
                            names[i].Contains(GameController.g_game.ScreenManager.currentLevelName)) || selected == i)
                        {
                            selected = i;
                            GameController.g_game.ScreenManager.stageFakeName =
                                names[i].Replace("Level_", "LOC_ID_LEVEL_NAME_");
                            GameController.g_game.ScreenManager.LoadScreen(names[i]);
                        }
                    }
                    ImGui.EndChild();
                }
                ImGui.SameLine();
            }
            ImGui.End();
        }
        private List<string> GetStageNames()
        {
            var list = GameController.g_game.ScreenManager.stageNameList;
            list.Sort();
            return list;
        }

        private void CreateStageFakeNames(List<string> names)
        {
            var locateBuilder = ModEntry.ModHelper.LocateBuilder;
            foreach (string name in names)
            {
                string key = name.Replace("Level_", "LOC_ID_LEVEL_NAME_");
                string levelFakeName =
                    locateBuilder.GetResourceString(key, SaveManager.configFile.languageType);

                this.fakeNames[name] = levelFakeName;
            }
        }
    }
}
