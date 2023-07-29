#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using ImGuiNET;
using ImGuiXNA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SomeGame {
    internal class patch_GameClass : Game {

        protected ImGuiXNAState ImGuiState;

        public patch_GameClass()
        {
            this.Window.Title = "FNA-Bootstrap";
            this.Window.AllowUserResizing = true;
            // Activated += (_, _) => { _isWindowFocused = true; };

            // Deactivated += (_, _) =>
            // {
                // _isWindowFocused = false;

                // Window.Title = "FNA-Bootstrap";
            // };

            var graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                PreferMultiSampling = true,
                GraphicsProfile = GraphicsProfile.HiDef,
                SynchronizeWithVerticalRetrace = false
            };
            graphics.ApplyChanges();

            this.IsMouseVisible = true;
            this.IsFixedTimeStep = false;
        }

        protected void orig_Initialize() { }
        protected override void Initialize() {
            this.orig_Initialize();

            this.ImGuiState = new ImGuiXNAState(this);

            if (!File.Exists("imgui.ini"))
                File.WriteAllText("imgui.ini", "");

            this.ImGuiState.BuildTextureAtlas();
        }

        protected void orig_Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(new Color(this.clear_color.X, this.clear_color.Y, this.clear_color.Z));
        }

        protected override void Draw(GameTime gameTime)
        {
            this.orig_Draw(gameTime);

            this.ImGuiState.NewFrame((float) gameTime.TotalGameTime.TotalSeconds);
            this.ImGuiLayout();
            this.ImGuiState.Render();
        }

        // Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
        float f = 0.0f;
        bool show_test_window = true;
        bool show_another_window = false;
        ImVec3 clear_color = new ImVec3(114f / 255f, 144f / 255f, 154f / 255f);
        protected virtual void ImGuiLayout() {
            // 1. Show a simple window
            // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
            {
                ImGui.Text("Hello, world!");
                ImGui.SliderFloat("float", ref this.f, 0.0f, 1.0f, null, 1f);
                ImGui.ColorEdit3("clear color", ref this.clear_color, false);
                if (ImGui.Button("Test Window")) this.show_test_window = !this.show_test_window;
                if (ImGui.Button("Another Window")) this.show_another_window = !this.show_another_window;
                ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));
            }

            // 2. Show another simple window, this time using an explicit Begin/End pair
            if (this.show_another_window) {
                ImGui.SetNextWindowSize(new ImVec2(200, 100), ImGuiCond.FirstUseEver);
                ImGui.Begin("Another Window", ref this.show_another_window);
                ImGui.Text("Hello");
                ImGui.End();
            }

            // 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
            if (this.show_test_window) {
                ImGui.SetNextWindowPos(new ImVec2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowTestWindow(ref this.show_test_window);
            }
        }

    }
}
