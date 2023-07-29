using System.Runtime.CompilerServices;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Framework.ModHelpers.LocalBuilderHelper;
using ImGuiMod;
using ImGuiMod.Framework;
using ImGuiNET;

namespace DevMenu
{
    public class ContextMenu
    {
        public static bool Open;
        public string nameWindow = "Context";

        private bool cached;
        private static int selected;
        private ILocateBuilderHelper _locateBuilder;
        private IImGuiMenuApi imGuiMenuApi;
        private int OptionSelected;
        private object[] saveDataDict;

        public static ImVec2 startPosition = ImVec2.Zero;
        public ContextMenu()
        {

        }

        public void Register(IImGuiMenuApi configMenu, IManifest manifest)
        {
            configMenu.AddWindowItem(
                manifest, this.Render, () => Open);

            this.imGuiMenuApi = configMenu;
        }

        public void Render()
        {
            var io = ImGui.GetIO();
            // ImGui.SetNextWindowSize(new ImVec2(50, 100), ImGuiCond.FirstUseEver);

            // if (startPosition.x != io.MousePosition.x && startPosition.y != io.MousePosition.y)
            {
                // startPosition = io.MousePosition;
                ImGui.SetNextWindowPos(startPosition, ImGuiCond.Always);
            }
            int actionsNum = 0;

            if (ImGui.Begin(this.nameWindow, ref Open, startPosition, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove))
            {
                foreach (var item in MainBarManager.contextMenuOptions)
                    // if (ImGui.BeginMenu(menuOption.))
                    // {
                    // foreach (var item in mainMenuMainItemGroupCache[menuOption])
                {
                    if (item.Hidden != null && item.Hidden.Invoke())
                        continue;

                    string label = item.Name.Invoke();
                    ++actionsNum;
                    if (ImGui.MenuItem(label, item.IsItemEnabled?.Invoke() ?? true))
                        item.OnClick.Invoke();
                }

                ImGui.SetWindowSize((new ImVec2(actionsNum > 0 ? 100 : 0, actionsNum * 25)));
            }

            ImGui.EndChild();

            if(actionsNum == 0)
                Open = false;

            if (io.MouseDown[0])
                Open = false;
        }

    }
}
