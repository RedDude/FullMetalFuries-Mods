using System;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace FullModdedFuriesAPI.Mods.FriendlyFire
{
    public class FriendlyFireManagerPatch
    {
        private GameController game;
        private IModHelper Helper;
        private readonly FriendlyFireManager _manager;

        public FriendlyFireManagerPatch(GameController game, IModHelper helper, FriendlyFireManager manager)
        {
            this.game = game;
            this.Helper = helper;
            this._manager = manager;
            this.Patch();
        }

        public void Patch()
        {
            var harmony = new Harmony("FriendlyFireManagerPatch");
            harmony.Patch(
                original: AccessTools.Method(typeof(PlayerClassObj), "StunPlayer"),
                postfix: this.GetHarmonyMethod(nameof(this.StunPlayer))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(PlayerClassObj), "HitCharacterMaster"),
                postfix: this.GetHarmonyMethod(nameof(this.HitCharacterMaster))
            );
        }

        public void HitCharacterMaster(
            PlayerClassObj __instance,
            IDamageObj damageObj,
            Vector2 impactPt,
            float dmgOverride = -1f,
            bool applyModsToDmgOverride = false,
            bool forceHit = false,
            bool baseHit = false)
        {
            this._manager.HandleFriendlyFire(__instance, damageObj, dmgOverride, baseHit);
        }
        public virtual void StunPlayer(PlayerClassObj __instance, IDamageObj enemy)
        {
            if (!FriendlyFireManager.pullByPlayer.ContainsKey(__instance)) return;
            __instance.PushTowardsVector(CDGMath.VectorBetweenPts(__instance.AbsPosition, new Vector2(enemy.X, enemy.Y)), -360f, 0.25f);
            FriendlyFireManager.pullByPlayer.Remove(__instance);
            // __instance.StartRumble(0.2f, 0.2f);
            // this.ResetStates(false);
            // if (enemy != null && !pullByPlayer)
            //     __instance.PushTowardsVector(CDGMath.VectorBetweenPts(__instance.AbsPosition, new Vector2(enemy.X, enemy.Y)),
            //         __instance.m_currentActiveLS = __instance.m_stunLS;
            // __instance.m_stunLS.Execute();
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
