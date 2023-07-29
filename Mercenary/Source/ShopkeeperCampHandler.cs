using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CDGEngine;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Mods.MercenaryClass.Source;
using Microsoft.Xna.Framework;

namespace Brawler2D
{

    public class ShopkeeperCampHandler
    {
        private IModHelper Helper;
        private static string originalShopNpcAnimation = "";

        private GameController game;
        private Assembly Assembly;
        private Type CampBaseType;
        private Type TreasureShopType;
        private object _originalContextObject;
        private ClassType classType;

        // private Type TreasureShopType;

        public ShopkeeperCampHandler(GameController game, IModHelper helper, ClassType classType)
        {
            this.game = game;
            this.Helper = helper;
            this.Assembly = typeof(GameController).Assembly;
            this.CampBaseType = this.Assembly.GetType("Brawler2D.Camp_Base");
            this.classType = classType;
        }

        public void SwapInCamp()
        {
            // TODO: Create a CampHelper in FMODF to access this camp related objects

            var playerManager = this.game.PlayerManager;

            var camp = this.game.ScreenManager.arenaScreen;
            var spriteListField = this.CampBaseType.GetField("m_spriteList",
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var spriteList = (List<BrawlerContainerObj>) spriteListField.GetValue(camp);

            if (spriteList == null || spriteList.Count == 0) // camp loaded
                return;

            this.UpdateSprite(camp, playerManager, spriteList);
        }

        private void UpdateSprite(ArenaScreen camp, PlayerManager playerManager,
            List<BrawlerContainerObj> spriteList)
        {
            var skillsObj = spriteList[0];
            if (skillsObj == null)
                return;
            var skillNPCField = this.CampBaseType.GetField("m_skillNPC",
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var lisaShop = (SpriteObj) skillNPCField.GetValue(camp);

            var previousSkillTreeFrameField = this.CampBaseType.GetField("m_previousSkillTreeFrame",
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            var UpdateSkillTreeShopkeep =
                this.CampBaseType.GetMethod("UpdateSkillTreeShopkeep", BindingFlags.Instance | BindingFlags.Public);


// Camp_Base
            // var lisaShop = this.Helper.Reflection.GetField<SpriteObj>(camp, "m_skillNPC").GetValue();
            // var spriteList = this.Helper.Reflection.GetField<List<BrawlerContainerObj>>(camp, "m_spriteList").GetValue();
            // var previousSkillTreeFrame = this.Helper.Reflection.GetField<int>(camp, "previousSkillTreeFrame");

            var player = playerManager.activePlayerArray[0];

            this.UpdateShopkeeper(camp, player, lisaShop, skillsObj, UpdateSkillTreeShopkeep);
            var shopkeeperParent = skillsObj.GetChildAt(1) as SpriteObj;
            if (shopkeeperParent.CurrentFrame == 6)
                previousSkillTreeFrameField.SetValue(camp, 6);
        }

        public void UpdateShopkeeper(object camp, PlayerObj player,
            SpriteObj shopkeeper, BrawlerContainerObj skillsObj, MethodInfo updateSkillTreeShopkeep)
        {
            bool hasMercenaryInParty = this.HasMercenaryInParty();
            if (!hasMercenaryInParty && !string.IsNullOrEmpty(originalShopNpcAnimation) &&
                shopkeeper.spriteName != originalShopNpcAnimation)
            {
                shopkeeper.ChangeSprite(originalShopNpcAnimation);
                updateSkillTreeShopkeep.Invoke(camp, new object[] {false});
                this.SwapShopkeeperAnimation(player, shopkeeper);
            }

            if (hasMercenaryInParty &&
                (originalShopNpcAnimation == "" || shopkeeper.spriteName == originalShopNpcAnimation))
            {
                originalShopNpcAnimation = shopkeeper.spriteName;
                shopkeeper.ChangeSprite("PlayerMercenary_Sprite");
                shopkeeper.PlayAnimation("DummyStart", "DummyStart", false);
                var shopkeeperParent = skillsObj.GetChildAt(1) as SpriteObj;
                // if (shopkeeperParent.CurrentFrame >= 7)
                //     shopkeeperParent.GoToFrame(6);
                if (shopkeeperParent.CurrentFrame >= shopkeeperParent.TotalFrames)
                    shopkeeperParent.GoToFrame(shopkeeperParent.CurrentFrame-1);
                updateSkillTreeShopkeep.Invoke(camp, new object[] {false});
                //__instance.UpdateSkillTreeShopkeep(false);
                this.SwapShopkeeperAnimation(player, shopkeeper);
            }

            if (shopkeeper.spriteName == "PlayerMercenary_Sprite")
            {
                shopkeeper.PlayAnimation("DummyStart", "DummyStart", false);
                var shopkeeperParent = skillsObj.GetChildAt(1) as SpriteObj;
                if (shopkeeperParent.CurrentFrame >= shopkeeperParent.TotalFrames)
                    shopkeeperParent.GoToFrame(shopkeeperParent.CurrentFrame-1);
            }
        }

        public void SwapShopkeeperAnimation(PlayerObj player, SpriteObj shopkeeper)
        {
            var currentPlayerClass = player.currentPlayerClass;
            var layeredSprite =
                player.game.SpriteManager.GetLayeredSprite("Projectile_CircleExplosion_Sprite");
            layeredSprite.isPixelSprite = true;
            layeredSprite.Scale = new Vector2(1.5f);
            layeredSprite.Opacity = 0.5f;
            layeredSprite.Layer = currentPlayerClass.Layer + 1f / 1000f;
            layeredSprite.lockLayer = true;
            layeredSprite.TextureColor = this.Helper.Database.GetClassesColors()[
                this.Helper.Database.GetClassesNames().IndexOf("Mercenary")];
            layeredSprite.AnimationSpeed = 0.02222222f;
            layeredSprite.PlayAnimation(false, false);
            layeredSprite.target = (IBrawlerGameObj) shopkeeper.Parent;
        }

        public bool HasMercenaryInParty()
        {
            var playerManager = this.game.PlayerManager;
            int playerArrayCount = playerManager.activePlayerArray_count;

            for (int index = 0; index < playerArrayCount; ++index)
            {
                var currentPlayerClass = playerManager.activePlayerArray[index].currentPlayerClass;
                if (currentPlayerClass.classType != this.classType) continue;
                return true;
            }

            return false;
        }
    }

}
