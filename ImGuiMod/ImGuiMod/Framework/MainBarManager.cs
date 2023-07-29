using System;
using System.Collections.Generic;
using System.Linq;
using DevMenu;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace ImGuiMod.Framework
{
    public class MainBarManager
    {
        /// <summary>Manages registered mod config menus.</summary>
        private readonly ModImGuiManager ConfigManager = new();

        public static List<ImGuiMenuItem> mainMenuOptions = new();

        public static List<ImGuiMenuItem> contextMenuOptions = new();

        public static List<string> mainMenuMainItemsCache = new();
        public static List<ImGuiWindowItem> windows = new();
        public static Dictionary<string, List<ImGuiMenuItem>> mainMenuMainItemGroupCache = new();

        /// <summary>The mod API, if initialized.</summary>
        // private Api Api;

        public void ShowMainBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    // ShowExampleMenuFile();
                    ImGui.EndMenu();
                }

                this.CreateModsMainMenuOptions();
                // if (ImGui.BeginMenu("Edit"))
                // {
                //     if (ImGui.MenuItem("Undo", "CTRL+Z")) {}
                //     if (ImGui.MenuItem("Redo", "CTRL+Y", false, false)) {}  // Disabled item
                //     ImGui.Separator();
                //     if (ImGui.MenuItem("Cut", "CTRL+X")) {}
                //     if (ImGui.MenuItem("Copy", "CTRL+C")) {}
                //     if (ImGui.MenuItem("Paste", "CTRL+V")) {}
                //     ImGui.EndMenu();
                // }
                ImGui.EndMainMenuBar();
            }
        }

        void RefreshCache()
        {
            foreach (var mainMenuOption in mainMenuOptions)
            {
                if (!mainMenuMainItemsCache.Contains(mainMenuOption.Parent))
                {
                    mainMenuMainItemsCache.Add(mainMenuOption.Parent);
                    mainMenuMainItemGroupCache[mainMenuOption.Parent] = new List<ImGuiMenuItem>();
                }

                if (!mainMenuMainItemGroupCache[mainMenuOption.Parent].Contains(mainMenuOption))
                {
                    mainMenuMainItemGroupCache[mainMenuOption.Parent].Add(mainMenuOption);
                }
            }
        }

        void CreateModsMainMenuOptions()
        {
            if (mainMenuMainItemsCache.Count == 0)
            {
                this.RefreshCache();
            }

            foreach (string mainMenuOption in mainMenuMainItemsCache)
                if (ImGui.BeginMenu(mainMenuOption))
                {
                    foreach (var item in mainMenuMainItemGroupCache[mainMenuOption])
                    {
                        string label = item.Name.Invoke();
                        if (item.Hidden != null && item.Hidden.Invoke())
                            continue;

                        if (ImGui.MenuItem(label, item.IsItemEnabled?.Invoke() ?? true))
                            item.OnClick.Invoke();
                    }

                    // if (ImGui.MenuItem("Redo", "CTRL+Y", false, false)) {}  // Disabled item
                    // ImGui.Separator();
                    ImGui.EndMenu();
                }
        }

        void CreateModsWindowItems()
        {
            foreach (var window in windows)
                if (window.Enabled.Invoke())
                    window.Render.Invoke();
        }

        public void ShowModsWindowItems()
        {
            if (windows.Count == 0)
                return;

            this.CreateModsWindowItems();
        }

        static public void OpenContextItems(Vector2 position)
        {
            var io = ImGui.GetIO();
            ContextMenu.startPosition = io.MousePosition;
            ContextMenu.Open = true;
            ShowModsContextItems();
        }

        static public void ShowModsContextItems()
        {
            // if (ImGui::Button("Open"))
            // if (!ImGui.BeginPopupContextItem("contextMenu")) return;
            // foreach (var item in contextMenuOptions)
            //     // if (ImGui.BeginMenu(menuOption.))
            //     // {
            //     // foreach (var item in mainMenuMainItemGroupCache[menuOption])
            // {
            //     string label = item.Name.Invoke();
            //     if (item.Hidden != null && item.Hidden.Invoke())
            //         continue;
            //
            //     if (ImGui.MenuItem(label, item.IsItemEnabled?.Invoke() ?? true))
            //         item.OnClick.Invoke();
            // }
            //
            // // if (ImGui.MenuItem("Redo", "CTRL+Y", false, false)) {}  // Disabled item
            // // ImGui.Separator();
            // // ImGui.EndMenu();
            // // }
            // // }
            //
            // ImGui.EndPopup();
        }
    }
}
