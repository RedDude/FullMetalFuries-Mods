using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Brawler2D;
using FullModdedFuriesAPI;
using ImGuiXNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;

namespace ImGuiMod.Framework
{
    public class Api : IImGuiMenuApi
    {
        /*********
        ** Fields
        *********/
        /// <summary>Manages the registered mod config menus.</summary>
        private readonly ModImGuiManager ConfigManager;

        /// <summary>Open the config UI for a specific mod.</summary>
        private readonly Action<IManifest> OpenModMenuImpl;

        private ImGuiXNAState imGuiXnaState;
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="configManager">Manages the registered mod config menus.</param>
        /// <param name="imGuiXnaState"></param>
        /// <param name="openModMenu">Open the config UI for a specific mod.</param>
        internal Api(ModImGuiManager configManager, ref ImGuiXNAState imGuiXnaState, Action<IManifest> openModMenu)
        {
            this.ConfigManager = configManager;
            this.OpenModMenuImpl = openModMenu;
            this.imGuiXnaState = imGuiXnaState;
        }


        /****
        ** Must be called first
        ****/
        /// <inheritdoc />
        public void Register(IManifest mod, bool titleScreenOnly = true)
        {
            this.AssertNotNull(mod, nameof(mod));

            if (this.ConfigManager.Get(mod, assert: false) != null)
                throw new InvalidOperationException($"The '{mod.Name}' mod has already registered a config menu, so it can't do it again.");

            this.ConfigManager.Set(mod,
                new ModConfig()); //mod, reset, save, titleScreenOnly)); //new ModConfig(mod, reset, save, titleScreenOnly));
        }


        /****
        ** Basic options
        ****/

        /// <inheritdoc />
        public void AddMainBarMenuOption(IManifest manifest, Func<string> parent, Func<string> name, Action OnClick, string shortcut = null, Func<bool> enabled = null, Func<bool> hidden = null, bool defaultTitleScreenOnly = false)
        {
            foreach (var imGuiMainMenuItem in MainBarManager.mainMenuOptions)
            if (imGuiMainMenuItem.Name.Invoke() == name.Invoke() && imGuiMainMenuItem.Parent == parent.Invoke())
                return;

            MainBarManager.mainMenuOptions.Add(new ImGuiMenuItem(manifest, parent.Invoke(), name, OnClick, shortcut, enabled, hidden, defaultTitleScreenOnly));
        }

        public void AddContextMenuOption(IManifest manifest, Func<string> parent, Func<string> name, Action onClick, Func<bool> hidden = null,
            Func<bool> enabled = null, string shortcut = null)
        {
            MainBarManager.contextMenuOptions.Add(new ImGuiMenuItem(manifest, parent.Invoke(), name, onClick, shortcut, enabled, hidden));

        }

        public void ShowContextMenu(Vector2 position)
        {
            MainBarManager.OpenContextItems(position);
        }

        public void AddWindowItem(IManifest manifest, Action render, Func<bool> enabled)
        {
            MainBarManager.windows.Add(new ImGuiWindowItem(manifest, render, enabled));
        }

        public bool TryGetCurrentMenu(out IManifest mod)
        {
            // if (Mod.ActiveConfigMenu is not SpecificModConfigMenu menu)
            // menu = null;

            // mod = menu?.Manifest;
            // page = menu?.CurrPage;
            // return menu is not null;
            mod = null;
            return false;
        }

        /// <inheritdoc />
        public void OpenModMenu(IManifest mod)
        {
            this.AssertNotNull(mod, nameof(mod));

            this.OpenModMenuImpl(mod);
        }

        /// <inheritdoc />
        public void Unregister(IManifest mod)
        {
            this.AssertNotNull(mod, nameof(mod));

            this.ConfigManager.Remove(mod);
        }

        Texture2D IImGuiMenuApi.GetSpriteTexture(string spriteName, int frame)
        {
            return this.GetSpriteTexture(spriteName, frame);
        }

        /*********
        ** Private methods
        *********/

        /// <summary>Assert that a required parameter is not null.</summary>
        /// <param name="value">The parameter value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">The parameter value is null.</exception>
        private void AssertNotNull(object value, string paramName)
        {
            if (value is null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>Assert that a required parameter is not null or whitespace.</summary>
        /// <param name="value">The parameter value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">The parameter value is null.</exception>
        private void AssertNotNullOrWhitespace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(paramName);
        }

        public Texture2D GetSpriteTexture(string spriteName)
        {
            return this.GetSpriteTexture(spriteName, 0);
        }


        public Texture2D GetSpriteTexture(string spriteName, int frame)
        {
            var spritesheetObj = SpriteLibrary.GetSpritesheetObj(spriteName);
            var xnaTexture = spritesheetObj.SSTexture;
            var spritesheetObjSpriteData = spritesheetObj.spriteDataArray[frame];
            var newBounds = new Rectangle((int) spritesheetObjSpriteData.SSX, (int) spritesheetObjSpriteData.SSY,
                (int) spritesheetObjSpriteData.SSWidth, (int) spritesheetObjSpriteData.SSHeight);
            var croppedTexture = new Texture2D(GameController.g_game.GraphicsDevice, newBounds.Width, newBounds.Height);

            var data = new Color[newBounds.Width * newBounds.Height];
            xnaTexture.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            croppedTexture.SetData(data);
            return croppedTexture;
        }

        public int RegisterTexture(Texture2D texture2D)
        {
            return this.imGuiXnaState.Register(texture2D);
        }

        public void UnregisterTexture(int textureId)
        {
            this.imGuiXnaState.Unregister(textureId);
        }
    }
}
