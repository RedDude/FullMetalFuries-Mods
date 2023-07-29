using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CDGEngine;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Mods.MercenaryClass;
using FullModdedFuriesAPI.Mods.MercenaryClass.Framework;
using FullModdedFuriesAPI.Mods.MercenaryClass.Source;
using FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brawler2D
{
    [HarmonyPatch]
    public class ProjectileMileDrawPatcher
    {
        private GameController game;

        public ProjectileMileDrawPatcher(GameController game, IModHelper helper)
        {
            this.game = game;
        }

        public void Patch()
        {
            var harmony = new Harmony("CharacterLoader");

            //Handle Character Selection
            harmony.Patch(
                original: AccessTools.Method(typeof(ProjectileObj), "Draw"),
                postfix: this.GetHarmonyMethod(nameof(this.After_Draw))
            );

        }

        public static void After_Draw(ProjectileObj __instance, Camera2D camera, float elapsedSeconds)
        {
            EnemyControllableManager.DrawProjectileFeedback(__instance, camera, elapsedSeconds);
        }


        protected HarmonyMethod GetHarmonyMethod(string name, int? priority = null)
        {
            var method = new HarmonyMethod(
                AccessTools.Method(this.GetType(), name)
                ?? throw new InvalidOperationException($"Can't find patcher method {name}.")
            );

            if (priority.HasValue)
                method.priority = priority.Value;

            return method;
        }

    }
}
