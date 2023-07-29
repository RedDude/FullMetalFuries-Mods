using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Brawler2D;
using CDGEngine;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Framework
{

    public class CampHandler
    {
        private IModHelper Helper;
        private static string originalShopNpcAnimation = "";

        private GameController game;
        private Assembly Assembly;
        private Type CampBaseType;
        private Type TreasureShopType;
        private object _originalContextObject;
        private string _className;
        private ClassType _classType;
        private bool _iconsCreated;

        // private Type TreasureShopType;

        public CampHandler(GameController game, IModHelper helper, ClassType classType)
        {
            this.game = game;
            this.Helper = helper;
            this.Assembly = typeof(GameController).Assembly;
            this.CampBaseType = this.Assembly.GetType("Brawler2D.Camp_Base");
            this._classType = classType ;
            this._className = helper.Database.GetClassTypeName(classType);
        }

        // static void After_SkillTreeObj_SetPlayer(SkillTreeObj __instance, PlayerObj player)
        // {
        //     // this.Helper.Reflection.GetField<int[,]>(skillTreeObj, "m_selectedUpgradeTree");
        //     // this.Helper.Reflection.GetField<SkillType>(skillTreeObj, "m_startingUpgradeType");
        //     // this.m_selectedUpgradeTree = SkillEV.TankUpgradeTree;
        //     // this.m_startingUpgradeType = SkillType.HP_Tank;
        //
        //     this.UpdateAvatarAndHeader(skillTreeObj, "Mercenary");
        // }
        //
        // static void After_EquipmentShopObj_SetPlayer(PlayerObj player)
        // {
        //     return player.currentClassType != PlayerClassObj_Mercenary.MercenaryClassType;
        // }
        //
        // static void After_TreasureShop_SetPlayer(PlayerObj player)
        // {
        //     return player.currentClassType != PlayerClassObj_Mercenary.MercenaryClassType;
        // }

        public void Handle()
        {
            // TODO: Create a CampHelper in FMODF to access this camp related objects

            var playerManager = this.game.PlayerManager;

            var camp = this.game.ScreenManager.arenaScreen;
            var spriteListField = this.CampBaseType.GetField("m_spriteList",
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var spriteList = (List<BrawlerContainerObj>) spriteListField.GetValue(camp);

            if (spriteList == null || spriteList.Count == 0) // camp loaded
                return;

            var skillsPlayerIcons = this.CampBaseType.GetField("m_skillsPlayerIcons",
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var treasuryPlayerIcons = this.CampBaseType.GetField("m_treasuryPlayerIcons",
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var equipmentPlayerIcons = this.CampBaseType.GetField("m_equipmentPlayerIcons",
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);


            string name = this._className;


            if (!this._iconsCreated)
            {
                var skillsPlayerIconsContainerObj = (BrawlerContainerObj) skillsPlayerIcons.GetValue(camp);
                skillsPlayerIconsContainerObj.AddChild(new BrawlerSpriteObj("CampIcon_" + name));
                skillsPlayerIconsContainerObj.GetChildAt(skillsPlayerIconsContainerObj.NumChildren - 1).Visible = false;

                var treasuryPlayerIconsContainerObj = (BrawlerContainerObj) treasuryPlayerIcons.GetValue(camp);
                treasuryPlayerIconsContainerObj.AddChild(new BrawlerSpriteObj("CampIcon_" + name));
                treasuryPlayerIconsContainerObj.GetChildAt(skillsPlayerIconsContainerObj.NumChildren - 1).Visible = false;

                var equipmentPlayerIconsContainerObj = (BrawlerContainerObj) equipmentPlayerIcons.GetValue(camp);
                equipmentPlayerIconsContainerObj.AddChild(new BrawlerSpriteObj("CampIcon_" + name));
                equipmentPlayerIconsContainerObj.GetChildAt(skillsPlayerIconsContainerObj.NumChildren - 1).Visible = false;
                this._iconsCreated = true;
            }


            // this.m_skillsPlayerIcons = new BrawlerContainerObj("CampPlayerIcons_Container");
            // this.m_treasuryPlayerIcons = new BrawlerContainerObj("CampPlayerIcons_Container");
            // this.m_equipmentPlayerIcons = new BrawlerContainerObj("CampPlayerIcons_Container");
            // for (int index = 0; index < this.m_skillsPlayerIcons.NumChildren; ++index)
            // {
            //     this.m_skillsPlayerIcons.GetChildAt(index).Visible = false;
            //     this.m_treasuryPlayerIcons.GetChildAt(index).Visible = false;
            //     this.m_equipmentPlayerIcons.GetChildAt(index).Visible = false;
            // }
            bool isActiveClass = playerManager.getMainPlayerOrHost.currentClassType == this._classType;
            this.UpdateContexts(isActiveClass);
        }

        private void UpdateContexts(bool isActiveClass)
        {
            var skillTreeContext = this.GetContextObjectField("m_skillTreeContextObj");
            var equipmentContext = this.GetContextObjectField("m_equipmentContextObj");
            var treasuryContextObj = this.GetContextObjectField("m_treasuryContextObj");
            var instrumentsContextObj = this.GetContextObjectField("m_instrumentsContextObj");
            // var campFireContextObj = this.GetContextObjectField("m_campFireContextObj");

            if (skillTreeContext != null)
                this._originalContextObject = skillTreeContext.methodObject1;
            if (skillTreeContext != null)
                skillTreeContext.methodObject1 = isActiveClass ? this : this._originalContextObject;
            if (equipmentContext != null)
                equipmentContext.methodObject1 = isActiveClass ? this : this._originalContextObject;
            if (treasuryContextObj != null)
                treasuryContextObj.methodObject1 = isActiveClass ? this : this._originalContextObject;
            // if (instrumentsContextObj != null)
            //     instrumentsContextObj.methodObject1 = isActiveClass ? this : this._originalContextObject;
            // campFireContextObj.methodObject1 = hasMercenaryInParty ? this : this._originalContextObject;

        }

        private void UpdateAvatarAndHeader(object menu, string name)
        {
            var avatarSprite = this.Helper.Reflection.GetField<SpriteObj>(menu, "m_avatarSprite").GetValue();
            var headerSprite = this.Helper.Reflection.GetField<SpriteObj>(menu, "m_headerSprite").GetValue();

            avatarSprite.ChangeSprite("SkillTree_" + name + "_Avatar");
            headerSprite.ChangeSprite("SkillTree_" + name + "_NamePlate");
        }

        public void RunSkillTree()
        {
            var contextObj = this.GetContextObjectField("m_skillTreeContextObj");
            var contextPlayer = contextObj.contextPlayer;
            this.ResetInstrument(contextPlayer);
            this.DisplaySkillTree(contextPlayer);
        }

        public void RunEquipment()
        {
            var contextObj = this.GetContextObjectField("m_equipmentContextObj");
            var contextPlayer = contextObj.contextPlayer;
            this.ResetInstrument(contextPlayer);
            this.DisplayEquipment(contextPlayer);
        }
        //
        // public void RunCampFire()
        // {
        //     var camp = this.game.ScreenManager.arenaScreen;
        //
        //     var playerManager = this.game.PlayerManager;
        //     int playerArrayCount = playerManager.activePlayerArray_count;
        //     for (int index = 0; index < playerArrayCount; ++index)
        //     {
        //         var activePlayer = playerManager.activePlayerArray[index];
        //         if (activePlayer != null)
        //             this.ResetInstrument(activePlayer);
        //     }
        //
        //     if (!BlitNet.Online)
        //         camp.PauseScreen();
        //
        //     this.CampBaseType.GetField("m_displaySwapCharDialog",
        //         BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance).SetValue(camp, true);
        //
        //     var contextObj = this.GetContextObjectField("m_campFireContextObj");
        //     var contextPlayer = contextObj.contextPlayer;
        //
        //     this.CampBaseType.GetField("m_swapCharPlayerController",
        //         BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance).SetValue(camp, contextPlayer);
        //
        //     var swap = (OptionsDialogObj)this.CampBaseType.GetField("m_swapCharDialogObj",
        //         BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance).GetValue(camp);
        //     swap.Display();
        // }
        //
        // public void RunRadio()
        // {
        //     var contextObj = this.GetContextObjectField("m_radioContextObj");
        //     var contextPlayer = contextObj.contextPlayer;
        //     this.ResetInstrument(contextPlayer);
        //     var radioMenuObj = this.GetContextObjectField("m_radioMenuObj");
        //     this.m_radioMenuObj.ActivateRadio(contextPlayer);
        // }

        public void RunTreasury()
        {
            var contextObj = this.GetContextObjectField("m_treasuryContextObj");
            var contextPlayer = contextObj.contextPlayer;
            this.ResetInstrument(contextPlayer);
            this.DisplayTreasury(contextPlayer);
        }

        public ContextSensitiveObj GetContextObjectField(string contextSensitiveObj)
        {
            var camp = this.game.ScreenManager.arenaScreen;
            var field = this.GetCampBaseField(contextSensitiveObj);
            return (ContextSensitiveObj) field.GetValue(camp);
        }

        private FieldInfo GetCampBaseField(string contextSensitiveObj)
        {
            return this.CampBaseType.GetField(contextSensitiveObj,
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
        }

        public void ResetInstrument(PlayerObj activePlayer)
        {
        }

        public void DisplaySkillTree(PlayerObj activePlayer)
        {
            if(activePlayer.currentClassType != this._classType)
                return;
            var camp = this.game.ScreenManager.arenaScreen;
            var field = this.CampBaseType.GetField("m_skillTreeObjList",
                BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance);
            var skillTreeObjList = (List<SkillTreeObj>) field.GetValue(camp);

            var skillTreeObj = skillTreeObjList[activePlayer.playerIndex];
            activePlayer.currentPlayerClass.ResetStates(false);
            activePlayer.currentPlayerClass.PlayAnimation("StandStart", "StandEnd", true, false);
            skillTreeObj.SetPlayer(activePlayer, activePlayer.currentClassType);

            var selectedUpgradeTree = this.Helper.Reflection.GetField<int[,]>(skillTreeObj, "m_selectedUpgradeTree");
            var startingUpgradeType = this.Helper.Reflection.GetField<SkillType>(skillTreeObj, "m_startingUpgradeType");

            string className = this._className;
            selectedUpgradeTree.SetValue(this.Helper.Database.GetSkillTree(className));
            startingUpgradeType.SetValue((SkillType) this.Helper.Database.GetCustomSkillIndex($"HP_{className}"));

            this.UpdateAvatarAndHeader(skillTreeObj, className);

            skillTreeObj.Active = true;
            activePlayer.inputMap.SetAllLocks(true);
            skillTreeObj.OnEnter();
        }

        public void DisplayEquipment(PlayerObj activePlayer)
        {
            if(activePlayer.currentClassType != this._classType)
                return;
            var camp = this.game.ScreenManager.arenaScreen;
            var field = this.CampBaseType.GetField("m_equipmentObjList",
                BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance);
            var equipmentShopObjList = (List<EquipmentShopObj>) field.GetValue(camp);

            var equipmentObj = equipmentShopObjList[activePlayer.playerIndex];
            activePlayer.currentPlayerClass.ResetStates(false);
            activePlayer.currentPlayerClass.ResetCooldowns();
            if (this.game.PlayerManager.isSinglePlayer)
                activePlayer.GetOtherClass().ResetCooldowns();
            activePlayer.DestroyPlayerTurrets(true);

            equipmentObj.SetPlayer(activePlayer, activePlayer.currentClassType);
            this.UpdateAvatarAndHeader(equipmentObj, this._className);

            equipmentObj.Active = true;
            activePlayer.inputMap.SetAllLocks(true);
            equipmentObj.OnEnter();
        }

        public void DisplayTreasury(PlayerObj activePlayer)
        {
            if(activePlayer.currentClassType != this._classType)
                return;
            var camp = this.game.ScreenManager.arenaScreen;
            var field = this.CampBaseType.GetField("m_treasuryObjList",
                BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance);
            var list = (IList) field.GetValue(camp);

            var treasuryObj = (SkillTreeObj) list[activePlayer.playerIndex];
            activePlayer.currentPlayerClass.ResetStates(false);

            // this.TreasureShopType.GetMethod("SetPlayer", BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance).Invoke(treasuryObj, new object []{});
            treasuryObj.SetPlayer(activePlayer, this._classType);
            this.UpdateAvatarAndHeader(treasuryObj, this._className);

            treasuryObj.Active = true;
            activePlayer.inputMap.SetAllLocks(true);
            treasuryObj.OnEnter();
        }

        public void PlayInstrument()
        {
        }


        //Copy and past from the original, but fixing the class name


    }

}
