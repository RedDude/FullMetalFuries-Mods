using System.Collections.Generic;
using CDGEngine;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Framework.ModHelpers.DatabaseHelper;
using Microsoft.Xna.Framework;

namespace Brawler2D
{
    public class InterfaceHandler
    {
        private IModHelper Helper;
        private GameController game;
        private int[,] skillTree;
        private string _className;
        private Color _color;
        private ClassType _classType;
        private IDatabaseHelper _database;
        private string spriteNameGameStats;

        public InterfaceHandler(GameController game, IModHelper helper, ClassType classType)
        {
            this.game = game;
            this.Helper = helper;
            this._database = this.Helper.Database;
            this._classType = classType;
            this._className = helper.Database.GetClassTypeName(classType);
            this._color = this._database.GetClassesColors()[(int) this._classType];

            this.spriteNameGameStats = $"GameStats_{this._className}";

            // this.AddPlayerData();
            // this.AddEquipmentData();
            // this.AddSkillData();
        }

        public void Handle()
        {
            for (int index = 0; index < this.game.PlayerManager.activePlayerArray_count; ++index)
            {
                var activePlayer = this.game.PlayerManager.activePlayerArray[index];
                if (activePlayer.currentClassType == this._classType)
                    this.UpdateInterface(activePlayer.currentPlayerClass);
            }
        }

        public void UpdateInterface(PlayerClassObj playerClass)
        {
            // if(playerClass.player.gameStatObj.spriteName = this.spriteNameGameStats)
                // return;

            // playerClass.ChangeSprite("PlayerHammer_Character");
            playerClass.TextureColor = this._color;
            this.RefreshSmallHUDHeights(playerClass);
            this.SetPlayerSmallHud(playerClass);
            this.SetGameStatClass(playerClass);
        }

        private void RefreshSmallHUDHeights(PlayerClassObj playerClass)
        {
            this.Helper.Reflection.GetField<float>(playerClass,
                "m_smallHUDHeight").SetValue(40f);
        }

        private void SetGameStatClass(PlayerClassObj playerClass)
        {
            if(playerClass.player.gameStatObj.spriteName != this.spriteNameGameStats)
                playerClass.player.gameStatObj.ChangeSprite(this.spriteNameGameStats);
        }

        private void SetPlayerSmallHud(PlayerClassObj playerClass)
        {

//             if (playerClass.classType != ClassType.Mercenary) return;
            var avatar =
                this.Helper.Reflection.GetField<SpriteObj>(playerClass.smallHUD,
                    "m_avatar").GetValue();
            var specialIcon =
                this.Helper.Reflection.GetField<SpriteObj>(playerClass.smallHUD,
                    "m_specialIcon").GetValue();
            var amountText =
                this.Helper.Reflection.GetField<BrawlerTextObj>(playerClass.smallHUD,
                    "m_amountText").GetValue();

            avatar?.ChangeSprite($"Player{this._className}_SmallHud");

            if (specialIcon != null)
                specialIcon.Visible = false;

            if (amountText != null)
                amountText.Visible = false;
        }


    }
}
