using System;
using FullModdedFuriesAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ImGuiMod
{
    /// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
    public interface IImGuiMenuApi
    {
        /*********
        ** Methods
        *********/
        /****
        ** Must be called first
        ****/
        /// <summary>Register a mod whose config can be edited through the UI.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
        /// <remarks>Each mod can only be registered once, unless it's deleted via <see cref="Unregister"/> before calling this again.</remarks>
        void Register(IManifest mod, bool titleScreenOnly = false);

        /****
        ** Basic options
        ****/
        /// <summary>Add a section title at the current position in the form.</summary>
        void AddMainBarMenuOption(
            IManifest manifest, Func<string> parent, Func<string> name, Action onClick,
            string shortcut = null, Func<bool> enabled = null, Func<bool> hidden = null,
            bool defaultTitleScreenOnly = false);

        void AddContextMenuOption(
            IManifest manifest, Func<string> parent, Func<string> name, Action onClick,
            Func<bool> hidden = null, Func<bool> enabled = null, string shortcut = null);

        void AddWindowItem(IManifest manifest, Action render, Func<bool> enabled);

        void ShowContextMenu(Vector2 startPosition);

        /// <summary>Get the currently-displayed mod ImGui menu, if any.</summary>
        /// <param name="mod">The manifest of the mod whose ImGui menu is being shown, or <c>null</c> if not applicable.</param>
        /// <returns>Returns whether a mod config menu is being shown.</returns>
        bool TryGetCurrentMenu(out IManifest mod);

        /// <summary>Remove a mod from the config UI and delete all its options and pages.</summary>
        /// <param name="mod">The mod's manifest.</param>
        void Unregister(IManifest mod);

        Texture2D GetSpriteTexture(string spriteName);

        Texture2D GetSpriteTexture(string spriteName, int frame);

        int RegisterTexture(Texture2D texture2D);

        void UnregisterTexture(int textureId);
    }
}
