using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FMOD_;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Mods.MercenaryClass;
using FullModdedFuriesAPI.Mods.MercenaryClass.Framework;
using HarmonyLib;

namespace Brawler2D
{
    [HarmonyPatch]
    public class ShieldPatch
    {
        private GameController game;
        private static IModHelper Helper;

        public ShieldPatch(GameController game, IModHelper helper)
        {
            this.game = game;
            Helper = helper;
        }

        public void Patch()
        {
            var harmony = new Harmony("ShieldPatch");
            harmony.Patch(
            original: AccessTools.Method(typeof(EnemyObj), "RemoveAllPlayerShields"),
            postfix: this.GetHarmonyMethod(nameof(this.After_RemoveAllPlayerShields))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(EnemyObj), "ApplyStatusEffect"),
                prefix: this.GetHarmonyMethod(nameof(this.Before_ApplyStatusEffect))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(EnemyManager), "ApplyShield"),
                prefix: this.GetHarmonyMethod(nameof(this.Before_ApplyShield))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(EnemyManager), "GetShieldType"),
                prefix: this.GetHarmonyMethod(nameof(this.After_ShieldType))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(EnemyManager), "ReShuffleEnemyShields"),
                prefix: this.GetHarmonyMethod(nameof(this.After_ReShuffleEnemyShields))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StatusEffectEV), "GetStatusEffectSpriteName"),
                prefix: this.GetHarmonyMethod(nameof(this.GetStatusEffectSpriteName))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StatusEffectEV), "GetClassShieldType"),
                prefix: this.GetHarmonyMethod(nameof(this.GetClassShieldType))
            );

        }

        public static string GetStatusEffectSpriteName(StatusEffect effect)
        {
            return "";
        }

        public static StatusEffect GetClassShieldType(ClassType classType)
        {
            return StatusEffect.None;
        }


        public static StatusEffect FixGetShieldType(ClassType classType)
        {
            return StatusEffect.None;
        }

        // https://sharplab.io/#v2:C4LglgNgNAJiDUAfAAgJgIwFgBQyDMABGgQMIEDeOOBNRhy6AbAQMrACGwArgM4CiAMwEBTAMbACACXYA7GBGElewAPYBbABQkI7HjwAqATwAOwoqiOmAlBWq17RAOysO3fkLHAAdADkVM4QBuOxoAXxCCCIi6F05eQRFxAgBxYWAWAAswYQgYS2EtHT1883yrCMpsBwIeAHcwYFEMgg00MujbKuqCUV0zbV0DE2EvPhkAczAA4QAnEA77ZGc2OPdE70zs3IB9McnpmeCu6t6efqKh0y8AMTBxjOBZ+ePupdi3BM8vTZyYbdv7o9Dgsen1SBd8t8ZGBTHMQTQ3isPh5xN8sr9tixobCjt1QWdwYNIfpZABrZ54hHLVzxFEbdE7EkyUm47owYQCdhcCCgeFOd609a+fxBDrhF7i+zRfACtaeFJpH65fIAWRUMEKROGpWG5RelW6ADd2DMesp1AQALykLzSOQKJQ8VSaNq61kOMACDSic1qAgAQmtSMFXz8AT1lMWzh9TvU7uqHTqDSaLVd1g6Bu6p3OWqueymwiefMRNLlqKVf3zB3j9mzhOKwxudweRZe1RLq0+5YZfwBLeBbdodYGDauWJhrcjHeRQormOxsxrQ7BI8uIyZ5OL1M7dLRWz+G6XNHZnO5vMHUdlXe8YdFg8ltHF4qoVRlwhkXD9q/ydkzNFvUARFWhYzIBLx9kCYH2OOsJQbQG5wTQJAADIAIIsCw2wqqhAAacEPjgb4fn6wZlsAv4RABESoTMagqDMmQzMIMCIQQABCEBTCxEQsBAKi1KxbFcDMMisSwkDvqIwisQACmkMyeoYrEACIqOorHSE60k8VwsKaY8rEkDM/i8Pp2kvHwAAeGRclp3EvGwKjGLJ/GzGxalOqx+hiBk7kqJ5EQAKoyLGxjGOwABGCisQAkjIhpTKIYBReZ9jKWAABeGVKdRYAMdwok8T2uwTAWsxicVCFFfu/zNpB1UYjB5VAT4ykAPLXMhMU+Hw+FAA=
        static IEnumerable<CodeInstruction> HandleInput_Patch(IEnumerable<CodeInstruction> instructions)
        {
            bool foundResetMethod = false;
            int startIndex = -1;

            var methodInfo = AccessTools.Method(typeof(ShieldPatch), nameof(FixGetShieldType), new System.Type[] { typeof(ClassType) });

            var codes = new List<CodeInstruction>(instructions);

            // IL_0002: call valuetype StatusEffect C::HandleCustom(valuetype ClassType)
            // IL_0007: stloc.0
            // IL_0008: ldloc.0
            // IL_0009: ldc.i4.0
            // IL_000a: cgt.un
            // IL_000c: stloc.1
            // // sequence point: hidden
            // IL_000d: ldloc.1
            // IL_000e: brfalse.s IL_0014
            //
            // IL_0010: ldloc.0
            // IL_0011: stloc.2
            // IL_0012: br.s IL_004c

            int index = 1;
            // codes.Insert(index, new CodeInstruction(OpCodes.Call, methodInfo));
            codes.Insert(index, new CodeInstruction(OpCodes.Call, "valuetype StatusEffect ShieldPatch::FixGetShieldType(valuetype ClassType)"));
            codes.Insert(++index, new CodeInstruction(OpCodes.Stloc_0));
            codes.Insert(++index, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(++index, new CodeInstruction(OpCodes.Ldc_I4_0));
            codes.Insert(++index, new CodeInstruction(OpCodes.Cgt_Un));
            codes.Insert(++index, new CodeInstruction(OpCodes.Stloc_1));
            codes.Insert(++index, new CodeInstruction(OpCodes.Ldloc_1));
            codes.Insert(++index, new CodeInstruction(OpCodes.Brfalse_S, "IL_0014"));
            codes.Insert(++index, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(++index, new CodeInstruction(OpCodes.Stloc_2));
            codes.Insert(++index, new CodeInstruction(OpCodes.Br_S, "IL_004c"));
            // for (int i = 0; i < codes.Count; i++)
            // {
            //     if (codes[i].opcode != OpCodes.Call)
            //         continue;
            //
            //     string strOperand = codes[i].operand.ToString();
            //     if (strOperand.Contains("ResetPositions"))
            //     {
            //         startIndex = i-1;
            //         foundResetMethod = true;
            //         break;
            //     }
            // }
            //
            // if (foundResetMethod)
            //     for (int i = startIndex; i > 0; i--)
            //     {
            //         // if (codes[i].opcode == OpCodes.Ldc_I4_4)
            //         // codes[i].opcode = OpCodes.Ldc_I4_5;
            //
            //         if (codes[i].opcode == OpCodes.Ldc_I4_4)
            //         {
            //
            //             codes[i].opcode = OpCodes.Call;
            //             codes[i].operand = AccessTools.Method(typeof(CharacterSelectionPatcherUtil),
            //                 "GetCustomCharacterCount"); //GetHarmonyMethod(nameof(GetCustomCharacterCount));
            //             // codes[i].operand =
            //             // "valuetype [Brawler2D]Brawler2D.ClassType Brawler2D.CharacterSelectionPatcher::GetCustomCharacterCount()";
            //         }
            //
            //         if (codes[i].opcode == OpCodes.Pop)
            //             break;
            //     }

            return codes.AsEnumerable();
        }


        public void After_ReShuffleEnemyShields(ClassType classType)
        {
        }

        public StatusEffect After_ShieldType(ClassType classType)
        {
            return StatusEffect.ENDOFLINE;
        }

        static bool Before_ApplyShield(EnemyObj enemy)
        {
            // if (!BlitNet.Lobby.IsMaster || enemy.shieldToApply == StatusEffect.None)
            //     return;

            return false;
        }

        public void Before_ApplyShield(
            ShieldRotationType rotationType,
            float shatterPercent,
            bool resetShatterHP)
        {
            if (rotationType == ShieldRotationType.None)
                return;
            // shieldRotationType = rotationType;
            // m_shieldShatterPercent = shatterPercent;
            // if (resetShatterHP)
            //     m_shieldShatterHP = CurrentHealthPreMultiplayerMods;
            // m_currentRainbowHP = MaxHealthPreMultiplayerMods * shatterPercent;
            // if (rotationType == ShieldRotationType.Switch && m_shieldAlreadyApplied || (rotationType == ShieldRotationType.Rainbow || rotationType == ShieldRotationType.Random) || shieldToApply == StatusEffect.None)
            //     Game.EnemyManager.SetEnemyShieldState(this);
            // Game.EnemyManager.ApplyShield(this);
            // m_shieldAlreadyApplied = true;
        }
        // public void ApplyShield(EnemyObj enemy)
        // {
        //     if (!BlitNet.Lobby.IsMaster || enemy.shieldToApply == StatusEffect.None)
        //         return;
        //     enemy.RemoveStatusEffect(StatusEffect.Shield_Tank, true);
        //     enemy.RemoveStatusEffect(StatusEffect.Shield_Sniper, true);
        //     enemy.RemoveStatusEffect(StatusEffect.Shield_Fighter, true);
        //     enemy.RemoveStatusEffect(StatusEffect.Shield_Engineer, true);
        //     enemy.ApplyStatusEffect((CharacterObj) enemy, enemy.shieldToApply, 9999999f, true);
        // }


        public void  Before_ApplyStatusEffect(
            CharacterObj source,
            StatusEffect effect,
            float duration,
            bool playSound = true)
        {
        }




        static void After_RemoveAllPlayerShields(EnemyObj enemyObj, bool removeInv)
        {
        }

        /// <summary>Get a Harmony patch method on the current patcher instance.</summary>
        /// <param name="name">The method name.</param>
        /// <param name="priority">The patch priority to apply, usually specified using Harmony's <see cref="Priority"/> enum, or <c>null</c> to keep the default value.</param>
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

        public void StartStatusEffectAnimation(
            CharacterObj source,
            StatusEffect effect,
            bool playSound)
        {
        }

        // [HarmonyPatch(typeof(PlayerClassObj), "RefreshSmallHUDHeights")]
        // [HarmonyPrefix]
        // static void Postfix(PlayerClassObj __instance)
        // {
        //     if (__instance.classType == ClassType.Mercenary)
        //         m_smallHUDHeight(__instance) = 20f;
        // }
        //
        // [HarmonyPatch(typeof(PlayerEV), "GetColour")]
        // [HarmonyPostfix]
        // static void Postfix(PlayerEV __instance, ref Color __result, ref ClassType classType)
        // {
        //     if (classType == ClassType.Mercenary)
        //     {
        //         __result = PlayerClassObj_Mercenary.MERCENARY_COLOUR;
        //     }
        // }

    }
}
