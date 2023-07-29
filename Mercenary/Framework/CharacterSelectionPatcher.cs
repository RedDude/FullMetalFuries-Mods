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
using HarmonyLib;

namespace Brawler2D
{
    [HarmonyPatch]
    public class CharacterSelectionPatcher
    {
        private GameController game;
        private Assembly Assembly;
        private static IModHelper Helper;
        private static Type CharacterSelectMenuType;

        // private static int currentCharacter = -1;

        private static bool IsCustomCharacterSelected = false;
        private static bool IsLastCustomCharacterSelected = false;

        private static int nextCharacter;
        private static int previousCharacter;
        private static FieldInfo classTypeSelectorField;
        private static FieldInfo leftArrowField;
        private static FieldInfo rightArrowField;

        public CharacterSelectionPatcher(GameController game, IModHelper helper)
        {
            this.game = game;
            Helper = helper;
            this.Assembly = typeof(GameController).Assembly;
            CharacterSelectMenuType = this.Assembly.GetType("Brawler2D.CharacterSelectMenuObj");

            classTypeSelectorField = CharacterSelectMenuType.GetField("m_classTypeSelector", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            CharacterSelectionPatcherUtil.Helper = helper;
        }

        public void Patch()
        {
            var harmony = new Harmony("CharacterLoader");

            //Handle Character Selection
            harmony.Patch(
                original: AccessTools.Method(CharacterSelectMenuType, "SwapClass"),
                prefix: this.GetHarmonyMethod(nameof(this.Before_SwapClass))
            );

            harmony.Patch(
                original: AccessTools.Method(CharacterSelectMenuType, "HandleInput"),
                transpiler: this.GetHarmonyMethod(nameof(this.HandleInput_Patch))
            );

            harmony.Patch(
                original: AccessTools.Method(CharacterSelectMenuType, "ForceSwapLeft"),
                transpiler: this.GetHarmonyMethod(nameof(this.HandleInput_Patch))
            );
            // harmony.Patch(
                // original: AccessTools.Method(CharacterSelectMenuType, "SwapClassTransition"),
                // prefix: this.GetHarmonyMethod(nameof(this.Before_SwapClassTransition))
            // );
            // harmony.Patch(
            //     original: AccessTools.Method(CharacterSelectMenuType, "ForceSwapLeft"),
            //     prefix: this.GetHarmonyMethod(nameof(this.Before_ForceSwapLeft))
            // );
        }

        public static ClassType GetCustomCharacterCount()
        {
            return (ClassType) (Helper.Database.GetClassesNames().Count-1);
            // int count = Helper.Database.GetCustomClasses().Count;
            // if (count <= 4)
            //     return count switch
            //     {
            //         0 => OpCodes.Ldc_I4_4,
            //         1 => OpCodes.Ldc_I4_5,
            //         2 => OpCodes.Ldc_I4_6,
            //         3 => OpCodes.Ldc_I4_7,
            //         4 => OpCodes.Ldc_I4_8,
            //         _ => OpCodes.Ldc_I4_4
            //     };
            // ModEntry.monitor.LogOnce("For now, only 4 custom character are supported in this version of FMODF");
            // return OpCodes.Ldc_I4_8;

        }
        static IEnumerable<CodeInstruction> HandleInput_Patch(IEnumerable<CodeInstruction> instructions)
        {
            bool foundResetMethod = false;
            int startIndex = -1;


            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode != OpCodes.Call)
                    continue;

                string strOperand = codes[i].operand.ToString();
                if (strOperand.Contains("ResetPositions"))
                {
                    startIndex = i-1;
                    foundResetMethod = true;
                    break;
                }
            }

            if (foundResetMethod)
                for (int i = startIndex; i > 0; i--)
                {
                    // if (codes[i].opcode == OpCodes.Ldc_I4_4)
                        // codes[i].opcode = OpCodes.Ldc_I4_5;

                        if (codes[i].opcode == OpCodes.Ldc_I4_4)
                        {

                            codes[i].opcode = OpCodes.Call;
                            codes[i].operand = AccessTools.Method(typeof(CharacterSelectionPatcherUtil),
                                "GetCustomCharacterCount"); //GetHarmonyMethod(nameof(GetCustomCharacterCount));
                            // codes[i].operand =
                            // "valuetype [Brawler2D]Brawler2D.ClassType Brawler2D.CharacterSelectionPatcher::GetCustomCharacterCount()";
                        }

                    if (codes[i].opcode == OpCodes.Pop)
                        break;
                }

            return codes.AsEnumerable();
        }

        static bool Before_SwapClass(object __instance, ClassType classType)
        {
            string className = "";
            foreach (var customClassPair in Helper.Database.GetCustomClasses())
            {
                if (customClassPair.Value != (int) classType)
                    continue;

                className = Helper.Database.GetClassTypeName(classType);
            }

            if (string.IsNullOrWhiteSpace(className))
                return true;

            classTypeSelectorField.SetValue(__instance, classType);
            int stars = 5; //TODO: Solve stars, get it from databaseHelper

            var classTextField = CharacterSelectMenuType.GetField("m_classText", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var classText = (BrawlerTextObj)classTextField.GetValue(__instance);

            var starContainerField = CharacterSelectMenuType.GetField("m_starContainer", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var starContainer = (ContainerObj)starContainerField.GetValue(__instance);

            var portraitField = CharacterSelectMenuType.GetField("m_portrait", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var portrait = (ClickableSpriteObj)portraitField.GetValue(__instance);

            GameController.soundManager.PlayEvent("event:/SFX/Front End/Character Select/sfx_fe_char_change_select", (IPositionalObj) null, false, false);
            string localeKey = className.ToLower() + ".class.name";
            classText.Text = Helper.Translation.Get(localeKey).ToString().ToUpper();//className.ToUpper();

            for (int index = 0; index < starContainer.NumChildren; ++index)
            {
                SpriteObj childAt = starContainer.GetChildAt(index) as SpriteObj;
                (starContainer.GetChildAt(index) as SpriteObj)?.ChangeSprite(childAt.spriteName);
                childAt.ChangeSprite(index <= stars - 1 ? "Difficulty_Star_Filled" : "Difficulty_Star_Empty");
            }
            portrait.ChangeSprite("CharSelect_" + className + "_Portrait");
            var updateText = Helper.Reflection.GetMethod(__instance, "UpdateText");
            updateText.Invoke();


            return false;
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


        // [HarmonyPatch(typeof(PlayerClassObj), "RefreshSmallHUDHeights")]
        // [HarmonyPrefix]
        // static void Postfix(PlayerClassObj __instance)
        // {
        //     if (__instance.classType == ClassType.Mercenary)
        //         m_smallHUDHeight(__instance) = 20f;
        // }
        //
        //
        //
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
