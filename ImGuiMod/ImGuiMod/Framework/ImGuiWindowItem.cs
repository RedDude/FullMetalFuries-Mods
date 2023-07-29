using System;
using System.Collections.Generic;
using FullModdedFuriesAPI;

namespace ImGuiMod.Framework
{
    /// <summary>The ImGuiOption for a specific mod.</summary>
    public class ImGuiWindowItem
    {
        /*********
        ** Fields
        *********/
        /// <summary>The page to which any new fields should be added.</summary>
        // private ModConfigPage ActiveRegisteringPage;

        public Action Render { get; set; }

        public Func<bool> Enabled { get; set; }

        /*********
        ** Accessors
        *********/
        /// <summary>The name of the mod which registered the mod configuration.</summary>
        public string ModName => this.ModManifest.Name;

        /// <summary>The manifest for the mod which registered the mod configuration.</summary>
        public IManifest ModManifest { get; }

        /// <summary>The callbacks to invoke when an option value changes.</summary>
        public List<Action<string, object>> ChangeHandlers { get; } = new();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="manifest">The manifest for the mod which registered the mod configuration.</param>
        public ImGuiWindowItem(
            IManifest manifest, Action render, Func<bool> enabled)
        {
            this.ModManifest = manifest;
            this.Render = render;
            this.Enabled = enabled;
        }
    }
}
