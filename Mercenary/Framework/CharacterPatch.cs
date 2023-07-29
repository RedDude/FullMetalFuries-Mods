using System;
using System.Reflection;
using FMOD_;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Mods.MercenaryClass;
using FullModdedFuriesAPI.Mods.MercenaryClass.Framework;
using HarmonyLib;

namespace Brawler2D
{
    [HarmonyPatch]
    public class CharacterPatch
    {
        private GameController game;
        private Assembly Assembly;
        static private Type CampBaseType;
        private Type TreasureShopType;
        private object _originalContextObject;
        private static IModHelper Helper;
        private static CampHandler campHandler;
        private static Type CharacterSelectMenuType;

        public CharacterPatch(GameController game, IModHelper helper)
        {
            this.game = game;
            Helper = helper;
            CharacterPatch.campHandler = new CampHandler(game, helper, ClassType.None);

            this.Assembly = typeof(GameController).Assembly;
            CampBaseType = this.Assembly.GetType("Brawler2D.Camp_Base");
            CharacterSelectMenuType = this.Assembly.GetType("Brawler2D.CharacterSelectMenuObj");
        }

        public void Patch()
        {
            var harmony = new Harmony("CharacterLoader");
            harmony.Patch(
            original: AccessTools.Method(CampBaseType, "ResetInstrument"),
            prefix: this.GetHarmonyMethod(nameof(this.Before_ResetInstrument))
            );

            harmony.Patch(
                original: AccessTools.Method(CampBaseType, "PlayInstrument"),
                prefix: this.GetHarmonyMethod(nameof(this.Before_PlayInstrument))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(SkillTreeObj), "IsSkillLocked"),
                prefix: this.GetHarmonyMethod(nameof(this.Before_IsSkillLocked))
            );
        }

        static bool Before_PlayInstrument()
        {
            //TODO: Raise a event instead
            var instrumentsContextObj = campHandler.GetContextObjectField("m_instrumentsContextObj");
            var contextPlayer = instrumentsContextObj.contextPlayer;
            // if((int)contextPlayer.currentClassType <= 4)
            //     return true; //run original method
            if((int)contextPlayer.currentClassType <= 4)
                return true; //run original method

            if ((int) contextPlayer.currentClassType <= 5) //TODO: Fix this Mercenary specific patch
            {
                var currentPlayerClass = contextPlayer.currentPlayerClass;
                currentPlayerClass.ResetPreviousAnimationState();
                currentPlayerClass.State = CharacterState.Idle;
                currentPlayerClass.CurrentSpeed = 0.0f;
                if (currentPlayerClass.playingInstrument)
                {
                    Before_ResetInstrument(contextPlayer);
                    return false;
                }

                currentPlayerClass.playingInstrument = true;
                return false;
            }

            return false;
        }

        static bool Before_ResetInstrument(PlayerObj player)
        {
            if((int)player.currentClassType <= 4)
                return true; //run original method

            if ((int) player.currentClassType <= 5)
            {
                player.currentPlayerClass.playingInstrument = false;
                player.currentPlayerClass.PlayAnimation("StandStart", "StandEnd", true, false);
                return false;
            }
            bool hasCustomClassInstrument = false;
            return hasCustomClassInstrument;
        }

        static bool Before_IsSkillLocked(SkillType skillType, PlayerObj player, bool __result)
        {
          var skillData = SkillEV.SkillData[(int) skillType];
          int skillTypeInt = (int) skillType;
          var classType = skillData.classType;

          int classLevel = player.GetClassLevel(classType);
          if (classType == ClassType.None)
          {
              __result = true;
              return false;
          }

          string className = "";
          foreach (var customClassPair in Helper.Database.GetCustomClasses())
          {
              if (customClassPair.Value != (int) classType)
                  continue;

              className = customClassPair.Key;
          }

          if(string.IsNullOrEmpty(className))
            return true;

          int[,] upgradeTree = Helper.Database.GetSkillTree(className);
          var skillType1 = (SkillType) Helper.Database.GetCustomSkillIndex($"HP_{className}");

          // int[,]  upgradeTree = SkillEV.TankUpgradeTree;
          // var skillType1 = SkillType.HP_Tank;

          // All bellow is a copy and paste from the original method
          if (skillType1 == skillType)
          {
              __result = false;
              return false;
          }

          int row = -1;
          int col = -1;

          for (int rowIndex = 0; rowIndex < upgradeTree.GetLength(0); ++rowIndex)
          {
            for (int colIndex = 0; colIndex < upgradeTree.GetLength(1); ++colIndex)
            {
              if (upgradeTree[rowIndex, colIndex] == skillTypeInt)
              {
                row = rowIndex;
                col = colIndex;
                break;
              }
            }
          }

          if (row == -1 || col == -1)
          {
              __result = true;
              return false;
          }

          bool isLocked = false;
          if (col > 1)
          {
            int num2 = upgradeTree[row, col - 2];
            bool isSkillParentLocked = upgradeTree[row, col - 1] == 1;
            if (player.GetSkillLevel((SkillType) num2) > (sbyte) 0 && isSkillParentLocked)
              isLocked = true;
          }
          if (col < 12)
          {
            int num2 = upgradeTree[row, col + 2];
            bool isSkillParentLocked = upgradeTree[row, col + 1] == 1;
            if (player.GetSkillLevel((SkillType) num2) > (sbyte) 0 && isSkillParentLocked)
              isLocked = true;
          }
          if (row > 1)
          {
            int num2 = upgradeTree[row - 2, col];
            bool isSkillParentLocked = upgradeTree[row - 1, col] == 1;
            if (player.GetSkillLevel((SkillType) num2) > (sbyte) 0 && isSkillParentLocked)
              isLocked = true;
          }
          if (row < 15)
          {
            int num2 = upgradeTree[row + 2, col];
            bool isSkillParentLocked = upgradeTree[row + 1, col] == 1;
            if (player.GetSkillLevel((SkillType) num2) > (sbyte) 0 && isSkillParentLocked)
              isLocked = true;
          }
          // bool isLocked = true;
          __result = !isLocked || classLevel < skillData.minNeededToUnlock;
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
