using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Brawler2D;

namespace ImGuiXNA {
    public class ImGuiXNAState {

        public Game Game;

        public int FontTexture = 0;

        public Effect Effect;

        private RasterizerState RasterizerState = new RasterizerState() {
            CullMode = CullMode.None,
            DepthBias = 0,
            FillMode = FillMode.Solid,
            MultiSampleAntiAlias = false,
            ScissorTestEnable = true,
            SlopeScaleDepthBias = 0
        };

        private Dictionary<int, Texture2D> _IdTextureMap = new Dictionary<int, Texture2D>();
        private Dictionary<Texture2D, int> _TextureIdMap = new Dictionary<Texture2D, int>();
        private int _TextureId = 1;

        private double _Time = 0.0f;
        private int _ScrollWheelValue;

        private static bool _Initialized = false;
        public static bool Initialized => _Initialized;

        private static List<int> _Keys = new List<int>();

        public ImGuiXNAState(Game game) {

            this.Game = game;
        }

        public static void Init() {
            if (_Initialized)
                return;
            _Initialized = true;

            ImGuiIO io = ImGui.GetIO();

            _Keys.Add(io.KeyMap[ImGuiKey.Tab] = (int) Keys.Tab);
            _Keys.Add(io.KeyMap[ImGuiKey.LeftArrow] = (int) Keys.Left);
            _Keys.Add(io.KeyMap[ImGuiKey.RightArrow] = (int) Keys.Right);
            _Keys.Add(io.KeyMap[ImGuiKey.UpArrow] = (int) Keys.Up);
            _Keys.Add(io.KeyMap[ImGuiKey.DownArrow] = (int) Keys.Down);
            _Keys.Add(io.KeyMap[ImGuiKey.PageUp] = (int) Keys.PageUp);
            _Keys.Add(io.KeyMap[ImGuiKey.PageDown] = (int) Keys.PageDown);
            _Keys.Add(io.KeyMap[ImGuiKey.Home] = (int) Keys.Home);
            _Keys.Add(io.KeyMap[ImGuiKey.End] = (int) Keys.End);
            _Keys.Add(io.KeyMap[ImGuiKey.Delete] = (int) Keys.Delete);
            _Keys.Add(io.KeyMap[ImGuiKey.Backspace] = (int) Keys.Back);
            _Keys.Add(io.KeyMap[ImGuiKey.Enter] = (int) Keys.Enter);
            _Keys.Add(io.KeyMap[ImGuiKey.Escape] = (int) Keys.Escape);
            _Keys.Add(io.KeyMap[ImGuiKey.A] = (int) Keys.A);
            _Keys.Add(io.KeyMap[ImGuiKey.C] = (int) Keys.C);
            _Keys.Add(io.KeyMap[ImGuiKey.V] = (int) Keys.V);
            _Keys.Add(io.KeyMap[ImGuiKey.X] = (int) Keys.X);
            _Keys.Add(io.KeyMap[ImGuiKey.Y] = (int) Keys.Y);
            _Keys.Add(io.KeyMap[ImGuiKey.Z] = (int) Keys.Z);


            TextInputEXT.TextInput += OnTextInput;
            io.SetGetClipboardTextFn(GetClipboardTextFn);
            io.SetSetClipboardTextFn(SetClipboardTextFn);


            // If no font added, add default font.
            if (io.FontAtlas.Fonts.Size == 0)
                io.FontAtlas.AddDefaultFont();
        }

        public void InitializeImGui() {
            ImGuiXNAState.Init();
            if (!File.Exists("imgui.ini"))
                File.WriteAllText("imgui.ini", "");
        }

        public static void OnTextInput(char c) => ImGui.AddInputCharacter(c);

        public readonly static GetClipboardTextFn GetClipboardTextFn = (userData) => SDL2.SDL.SDL_GetClipboardText();
        public readonly static SetClipboardTextFn SetClipboardTextFn = (userData, text) => SDL2.SDL.SDL_SetClipboardText(text);


        public void BuildTextureAtlas() {
            ImGuiIO io = ImGui.IO;
            // ImFontTextureData texData = io.FontAtlas.GetTexDataAsAlpha8();
            // Alpha8 has got some blending issues.
            ImFontTextureData texData = io.FontAtlas.GetTexDataAsRGBA32();

            Texture2D tex = new Texture2D(this.Game.GraphicsDevice, texData.Width, texData.Height, false, SurfaceFormat.Color);

            int[] data = new int[texData.Width * texData.Height];
            Marshal.Copy(texData.Pixels, data, 0, data.Length);
            tex.SetData(data);

            this.FontTexture = this.Register(tex);

            io.FontAtlas.SetTexID(this.FontTexture);
            io.FontAtlas.ClearTexData(); // Clears CPU side texture data.
        }

        public int Register(Texture2D tex) {
            int id;
            if (this._TextureIdMap.TryGetValue(tex, out id))
                return id;
            id = this._TextureId;
            this._TextureId++;
            this._TextureIdMap[tex] = id;
            this._IdTextureMap[id] = tex;
            return id;
        }

        public void Unregister(Texture2D tex) {
            int id;
            if (!this._TextureIdMap.TryGetValue(tex, out id))
                return;
            this._TextureIdMap.Remove(tex);
            this._IdTextureMap.Remove(id);
        }

        public void Unregister(int id) {
            Texture2D tex;
            if (!this._IdTextureMap.TryGetValue(id, out tex))
                return;
            this._TextureIdMap.Remove(tex);
            this._IdTextureMap.Remove(id);
        }

        public int? GetId(Texture2D tex) {
            int id;
            if (this._TextureIdMap.TryGetValue(tex, out id))
                return id;
            return null;
        }

        public Texture2D GetTexture(int id) {
            Texture2D tex;
            if (this._IdTextureMap.TryGetValue(id, out tex))
                return tex;
            return null;
        }

        public Action<ImGuiXNAState, Effect> SetupEffect = _SetupEffect;
        private static void _SetupEffect(ImGuiXNAState self, Effect _effect) {
            ImGuiIO io = ImGui.IO;

            const float translate = 0f;


            if (_effect is BasicEffect) {
                BasicEffect effect = (BasicEffect) _effect;
                effect.World = Matrix.Identity;
                effect.View = Matrix.Identity;
                effect.Projection = Matrix.CreateOrthographicOffCenter(translate, io.DisplaySize.x + translate, io.DisplaySize.y + translate, translate, -1f, 1f);
                effect.TextureEnabled = true;
                effect.VertexColorEnabled = true;
                return;
            }

            if (_effect is AlphaTestEffect) {
                AlphaTestEffect effect = (AlphaTestEffect) _effect;
                effect.World = Matrix.Identity;
                effect.View = Matrix.Identity;
                effect.Projection = Matrix.CreateOrthographicOffCenter(translate, io.DisplaySize.x + translate, io.DisplaySize.y + translate, translate, -1f, 1f);
                effect.VertexColorEnabled = true;
                return;
            }

            throw new Exception($"Default ImGuiXNAState.SetupEffect can't deal with {_effect.GetType().FullName}, please provide your own delegate.");
        }

        public Action<ImGuiXNAState, Effect, Texture2D> SetEffectTexture = _SetEffectTexture;
        private static void _SetEffectTexture(ImGuiXNAState self, Effect _effect, Texture2D texture) {
            ImGuiIO io = ImGui.IO;

            if (_effect is BasicEffect) {
                BasicEffect effect = (BasicEffect) _effect;
                effect.Texture = texture;
                return;
            }

            if (_effect is AlphaTestEffect) {
                AlphaTestEffect effect = (AlphaTestEffect) _effect;
                effect.Texture = texture;
                return;
            }

            throw new Exception($"Default ImGuiXNAState.SetEffectTexture can't deal with {_effect.GetType().FullName}, please provide your own delegate.");
        }

        public void NewFrame(float totalSeconds) {
            ImGuiIO io = ImGui.GetIO();

            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            for (int i = 0; i < _Keys.Count; i++)
                io.KeysDown[_Keys[i]] = keyboard.IsKeyDown((Keys) _Keys[i]);

            io.ShiftPressed = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            io.CtrlPressed = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            io.AltPressed = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            io.SuperPressed = keyboard.IsKeyDown(Keys.LeftWindows) || keyboard.IsKeyDown(Keys.RightWindows);

            io.DisplaySize = new ImVec2(this.Game.GraphicsDevice.PresentationParameters.BackBufferWidth, this.Game.GraphicsDevice.PresentationParameters.BackBufferHeight) ;
            io.DisplayFramebufferScale = new ImVec2(1f, 1f);

            io.DeltaTime = totalSeconds;// this._Time > 0D ? (float) (totalSeconds - this._Time) : (1f / 60f);
            this._Time = totalSeconds;

            var m_game = GameController.g_game;
            float physicalScaleX = m_game.ScreenManager.virtualScreen.physicalScaleX;
            float physicalScaleY = m_game.ScreenManager.virtualScreen.physicalScaleY;
            // CDGRect cdgRect = new CDGRect(this.AbsX + this.m_clickBounds.X + this.clickBoundsMod.X, this.AbsY + this.m_clickBounds.Y + this.clickBoundsMod.Y, this.m_clickBounds.Width + this.clickBoundsMod.Width, this.m_clickBounds.Height + this.clickBoundsMod.Height);
            Vector2 mousePos = m_game.InputMap.GetMousePos();
            mousePos.X *= 1f / physicalScaleX;
            mousePos.Y *= 1f / physicalScaleY;
            // if (!cdgRect.Contains(mousePos))
            //     return;
            // this.m_highlighted = true;

            io.MousePosition = new ImVec2(mousePos.X,  mousePos.Y);
            //io.MousePosition = new ImVec2(mouse.X / 2, mouse.Y / 2);

            io.MouseDown[0] = m_game.InputManager.MouseLeftPressed();// ButtonState.Pressed;
            io.MouseDown[1] = m_game.InputManager.MouseRightJustPressed();//mouse.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = m_game.InputManager.MouseMiddleJustPressed();//mouse.MiddleButton == ButtonState.Pressed;

            int scrollDelta = mouse.ScrollWheelValue - this._ScrollWheelValue;
            io.MouseWheel = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
            this._ScrollWheelValue = mouse.ScrollWheelValue;

            this.Game.IsMouseVisible = !io.MouseDrawCursor;

            ImGui.NewFrame();
        }


        public void Render() {
            ImGui.Render();
            if (ImGui.IO.RenderDrawListsFn == IntPtr.Zero) this.RenderDrawData(ImGui.GetDrawData());
        }

        public void RenderDrawData(ImDrawData drawData) {
            GraphicsDevice device = this.Game.GraphicsDevice;

            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers.
            Viewport lastViewport = device.Viewport;
            Rectangle lastScissorBox = device.ScissorRectangle;

            device.BlendFactor = Color.White;
            device.BlendState = BlendState.NonPremultiplied;
            device.RasterizerState = this.RasterizerState;
            device.DepthStencilState = DepthStencilState.DepthRead;

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            ImGuiIO io = ImGui.GetIO();
            ImGui.ScaleClipRects(drawData, io.DisplayFramebufferScale);

            // Setup projection
            device.Viewport = new Viewport(0, 0, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight);

            if (this.Effect == null) this.Effect = new BasicEffect(device);
            Effect effect = this.Effect;
            this.SetupEffect(this, effect);

            // Render command lists
            for (int n = 0; n < drawData.CmdListsCount; n++) {
                ImDrawList cmdList = drawData[n];

                ImVector<ImDrawVertXNA> vtxBuffer;
                unsafe
                {
                    vtxBuffer = new ImVector<ImDrawVertXNA>(cmdList.VtxBuffer.Native);
                }
                ImDrawVertXNA[] vtxArray = new ImDrawVertXNA[vtxBuffer.Size];
                for (int i = 0; i < vtxBuffer.Size; i++)
                    vtxArray[i] = vtxBuffer[i];

                ImVector<short> idxBuffer;
                unsafe
                {
                    idxBuffer = new ImVector<short>(cmdList.IdxBuffer.Native);
                }
                /*
                short[] idxArray = new short[idxBuffer.Size];
                for (int i = 0; i < idxBuffer.Size; i++)
                    idxArray[i] = idxBuffer[i];
                */

                uint offset = 0;
                for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++) {
                    ImDrawCmd pcmd = cmdList.CmdBuffer[cmdi];
                    if (pcmd.UserCallback != IntPtr.Zero) {
                        pcmd.InvokeUserCallback(ref cmdList, ref pcmd);
                    } else {
                        // Instead of uploading the complete idxBuffer again and again, just upload what's required.
                        short[] idxArray = new short[pcmd.ElemCount];
                        for (int i = 0; i < pcmd.ElemCount; i++)
                            idxArray[i] = idxBuffer[(int) offset + i];
                        this.SetEffectTexture(this, effect, this.GetTexture((int) pcmd.TextureId));
                        device.ScissorRectangle = new Rectangle(
                            (int) pcmd.ClipRect.X,
                            (int) pcmd.ClipRect.Y,
                            (int) (pcmd.ClipRect.Z - pcmd.ClipRect.X),
                            (int) (pcmd.ClipRect.W - pcmd.ClipRect.Y)
                        );
                        foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                            pass.Apply();
                            device.DrawUserIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                vtxArray, 0, vtxBuffer.Size,
                                idxArray, 0, (int) pcmd.ElemCount / 3,
                                ImDrawVertXNA._VertexDeclaration
                            );
                        }
                    }
                    offset += pcmd.ElemCount;
                }

            }

            // Restore modified state
            device.Viewport = lastViewport;
            device.ScissorRectangle = lastScissorBox;
        }

    }
}
