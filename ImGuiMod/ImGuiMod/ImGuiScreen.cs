using System;
using System.Collections.Generic;
using System.IO;
using Brawler2D;
using CDGEngine;
using FullModdedFuriesAPI;
using ImGuiMod.Framework;
using ImGuiNET;
using ImGuiXNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;

namespace ImGuiMod
{
    public class ImGuiScreen : BrawlerScreen
    {
        public static bool Enabled;

        protected ImGuiXNAState ImGuiState;

        private MainBarManager mainBarManager = new MainBarManager();

        private IModHelper helper;

        public ImGuiScreen(IModHelper helper, ImGuiXNAState imGuiXnaState) : base(GameController.g_game)
        {
            this.DrawIfCovered = true;

            this.helper = helper;
            this.pauseType = PauseType.CannotPause;
            Enabled = true;
            this.ImGuiState = imGuiXnaState;
            this.InitializeImGui();
        }

        public override void Draw(float elapsedSeconds)
        {
            if(!Enabled)
                return;

            if(this.ImGuiState == null)
                return;

            this.ImGuiState.NewFrame(elapsedSeconds);
            this.ImGuiLayout();
            this.ImGuiState.Render();
        }

        protected void InitializeImGui() {
            ImGuiXNAState.Init();
            if (!File.Exists("imgui.ini"))
                File.WriteAllText("imgui.ini", "");

            this.ImGuiState.BuildTextureAtlas();

            this.GetSpriteTexture("PlayerEngineer_Sprite");

            this._xnaTexture = this.CreateTexture(GameController.g_game.GraphicsDevice, 300, 150, pixel =>
            {
                int red = (pixel % 300) / 2;
                return new Color(red, 1, 1);
            });
            this._imGuiTexture = this.ImGuiState.Register( this._xnaTexture );
        }

        public Texture2D GetSpriteTexture(string spriteName, int frame = 0)
        {
            var spritesheetObj = SpriteLibrary.GetSpritesheetObj(spriteName);
            var xnaTexture = spritesheetObj.SSTexture;
            var spritesheetObjSpriteData = spritesheetObj.spriteDataArray[0];
            var newBounds = new Rectangle((int) spritesheetObjSpriteData.SSX, (int) spritesheetObjSpriteData.SSY,
                (int) spritesheetObjSpriteData.SSWidth, (int) spritesheetObjSpriteData.SSHeight);
            var croppedTexture = new Texture2D(GameController.g_game.GraphicsDevice, newBounds.Width, newBounds.Height);

            var data = new Color[newBounds.Width * newBounds.Height];
            xnaTexture.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            croppedTexture.SetData(data);
            return croppedTexture;
        }


        // Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
        private static float f = 0.0f;
        private static bool show_test_window = false;
        private static bool show_another_window = false;
        private static ImVec3 clear_color = new ImVec3(114f / 255f, 144f / 255f, 154f / 255f);
        private static byte[] _textBuffer = new byte[100];
        private Texture2D _xnaTexture;
        private int _imGuiTexture;

        protected void ImGuiLayout()
        {
            this.mainBarManager.ShowMainBar();
            this.mainBarManager.ShowModsWindowItems();
            MainBarManager.ShowModsContextItems();
            // 1. Show a simple window
            // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
            {

                ImGui.Text("Texture sample");
                ImGui.Image(this._imGuiTexture, new ImVec2(100, 150), ImVec2.Zero, ImVec2.One, new ImVec4(1,1,1,1), new ImVec4(1,1,1,1)); // Here, the previously loaded texture is used

                ImGui.Text("Hello, world!");
                ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, null, 1f);
                ImGui.ColorEdit3("clear color", ref clear_color, false);
                if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
                if (ImGui.Button("Another Window")) show_another_window = !show_another_window;
                ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));
            }

            // 2. Show another simple window, this time using an explicit Begin/End pair
            if (show_another_window) {
                ImGui.SetNextWindowSize(new ImVec2(0, 0), ImGuiCond.FirstUseEver);
                ImGui.Begin("Another Window", ref show_another_window);
                ImGui.Text("Hello");
                ImGui.End();
            }

            // 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
            if (show_test_window) {
                ImGui.SetNextWindowPos(new ImVec2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowTestWindow(ref show_test_window);
            }
        }


        public Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            //initialize a texture
            var texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for(int pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint( pixel );
            }

            //set the color
            texture.SetData( data );

            return texture;
        }

//         static void ShowExampleMenuFile()
// {
//     IMGUI_DEMO_MARKER("Examples/Menu");
//     ImGui::MenuItem("(demo menu)", NULL, false, false);
//     if (ImGui::MenuItem("New")) {}
//     if (ImGui::MenuItem("Open", "Ctrl+O")) {}
//     if (ImGui::BeginMenu("Open Recent"))
//     {
//         ImGui::MenuItem("fish_hat.c");
//         ImGui::MenuItem("fish_hat.inl");
//         ImGui::MenuItem("fish_hat.h");
//         if (ImGui::BeginMenu("More.."))
//         {
//             ImGui::MenuItem("Hello");
//             ImGui::MenuItem("Sailor");
//             if (ImGui::BeginMenu("Recurse.."))
//             {
//                 ShowExampleMenuFile();
//                 ImGui::EndMenu();
//             }
//             ImGui::EndMenu();
//         }
//         ImGui::EndMenu();
//     }
//     if (ImGui::MenuItem("Save", "Ctrl+S")) {}
//     if (ImGui::MenuItem("Save As..")) {}
//
//     ImGui::Separator();
//     IMGUI_DEMO_MARKER("Examples/Menu/Options");
//     if (ImGui::BeginMenu("Options"))
//     {
//         static bool enabled = true;
//         ImGui::MenuItem("Enabled", "", &enabled);
//         ImGui::BeginChild("child", ImVec2(0, 60), true);
//         for (int i = 0; i < 10; i++)
//             ImGui::Text("Scrolling Text %d", i);
//         ImGui::EndChild();
//         static float f = 0.5f;
//         static int n = 0;
//         ImGui::SliderFloat("Value", &f, 0.0f, 1.0f);
//         ImGui::InputFloat("Input", &f, 0.1f);
//         ImGui::Combo("Combo", &n, "Yes\0No\0Maybe\0\0");
//         ImGui::EndMenu();
//     }
//
//     IMGUI_DEMO_MARKER("Examples/Menu/Colors");
//     if (ImGui::BeginMenu("Colors"))
//     {
//         float sz = ImGui::GetTextLineHeight();
//         for (int i = 0; i < ImGuiCol_COUNT; i++)
//         {
//             const char* name = ImGui::GetStyleColorName((ImGuiCol)i);
//             ImVec2 p = ImGui::GetCursorScreenPos();
//             ImGui::GetWindowDrawList()->AddRectFilled(p, ImVec2(p.x + sz, p.y + sz), ImGui::GetColorU32((ImGuiCol)i));
//             ImGui::Dummy(ImVec2(sz, sz));
//             ImGui::SameLine();
//             ImGui::MenuItem(name);
//         }
//         ImGui::EndMenu();
//     }
//
//     // Here we demonstrate appending again to the "Options" menu (which we already created above)
//     // Of course in this demo it is a little bit silly that this function calls BeginMenu("Options") twice.
//     // In a real code-base using it would make senses to use this feature from very different code locations.
//     if (ImGui::BeginMenu("Options")) // <-- Append!
//     {
//         IMGUI_DEMO_MARKER("Examples/Menu/Append to an existing menu");
//         static bool b = true;
//         ImGui::Checkbox("SomeOption", &b);
//         ImGui::EndMenu();
//     }
//
//     if (ImGui::BeginMenu("Disabled", false)) // Disabled
//     {
//         IM_ASSERT(0);
//     }
//     if (ImGui::MenuItem("Checked", NULL, true)) {}
//     if (ImGui::MenuItem("Quit", "Alt+F4")) {}
// }

    }
}

