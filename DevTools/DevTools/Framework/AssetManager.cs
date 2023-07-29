using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Brawler2D;
using FullModdedFuriesAPI;
using ImGuiMod;
using ImGuiNET;
using SpriteSystem2;

namespace DevMenu
{
    public class AssetManager
    {
        public static bool Open;
        private string nameWindow = "Asset Manager";
        public int Selected = 0;
        public string SelectedSpritesheet = "";

        public string[] options = new[] {"Spritesheets", "Containers"};
        public void Register(IImGuiMenuApi configMenu, IManifest manifest)
        {
            configMenu.AddMainBarMenuOption(
                manifest, () => "DevTools", () => this.nameWindow,
                () =>
                {
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
                this.PopUp();

                if(this.Selected == 0)
                    this.SpritesheetsPane();
                if (this.Selected == 1)
                    this.ContainersPane();
            }

            ImGui.EndChild();
        }

        private void SpritesheetsPane()
        {
            string[] spritesheetPath = SpriteLibrary.GetLoadedSpritesheetPaths();
            ImGui.SameLine();
            // this.PopUp();
            int selected = 0;
            {
                ImGui.BeginChild("spritesheets pane", new ImVec2(200, 0), true, ImGuiWindowFlags.Default);

                for (int i = 0; i < spritesheetPath.Length; i++)
                {
                    if (ImGui.Selectable(spritesheetPath[i], selected == i))
                    {
                        selected = i;
                    }
                }
                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

        int selectedContainer = 0;
        string selectedContainerKey;

        private void ContainersPane()
        {
            var containerDic = ModEntry.ModHelper.Reflection
                .GetField<Dictionary<string, List<ContainerData>>>(typeof(SpriteLibrary), "m_containerDict").GetValue();
            var keys = containerDic.Keys.ToList();
            keys.Sort();
            this.selectedContainerKey = keys[this.selectedContainer];
            ImGui.SameLine();
            {
                ImGui.BeginChild("container pane", new ImVec2(200, 0), true, ImGuiWindowFlags.Default);

                for (int i = 0; i < keys.Count; i++)
                {
                    if (ImGui.Selectable(keys[i], this.selectedContainer == i))
                    {
                        this.selectedContainer = i;
                        this.selectedContainerKey = keys[i];
                    }
                }

                ImGui.EndChild();
            }
            ImGui.SameLine();
            {
                ImGui.BeginChild("container data pane", new ImVec2(200, 0), true, ImGuiWindowFlags.Default);

                int selectedSprite = 0;
                var selectedContainerData = containerDic[this.selectedContainerKey];
                for (int i = 0; i < selectedContainerData.Count; i++)
                {
                    if (ImGui.Selectable(selectedContainerData[i].SpriteName, selectedSprite == i))
                    {
                        selectedSprite = i;
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
                        this.Selected = i;
                    }

                ImGui.EndPopup();
            }
        }
    }
}
