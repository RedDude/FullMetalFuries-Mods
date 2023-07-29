using System.Collections.Generic;
using Brawler2D;
using FullModdedFuriesAPI.Framework.ModHelpers.DatabaseHelper;
using FullModdedFuriesAPI.Mods.MercenaryClass.Framework;
using HarmonyLib;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source
{
    public class MercenaryDummySetup
    {
        private GameController game;
        private IModHelper Helper;
        private IDatabaseHelper _database;
        public static readonly string enemyDummyMercenaryName = "Dummy_Mercenary";


        public MercenaryDummySetup(GameController game, IModHelper helper)
        {
            this.game = game;
            this.Helper = helper;
            this._database = this.Helper.Database;

            // this.AddPlayerData();
            // this.AddEquipmentData();
            // this.AddSkillData();
        }

        public void Create(GameController game)
        {
            var enemyData = new EnemyData(
                enemyDummyMercenaryName,
                0.1f,
                1,
                0,
                0,
                125f,
                0,
                1.5f,
                (byte) 2,
                (byte) 2,
                0.32f,
                1f,
                9999,
                9999,
                15f,
                2f,
                3,
                4,
                0.45f,
                100,
                200);

            var mercenaryErinDummySpritesheet = this.Helper.Content
                .LoadSpritesheet($"Assets//Mercenary_Erin_Dummy_Spritesheet", ContentSource.ModFolder);
            foreach (var o in mercenaryErinDummySpritesheet)
            {
                ;
            }
            var mercenaryAlexDummySpritesheet = this.Helper.Content
                .LoadSpritesheet($"Assets//Mercenary_Alex_Dummy_Spritesheet", ContentSource.ModFolder);
            foreach (var o in mercenaryAlexDummySpritesheet)
            {
                ;
            }
            var mercenaryMegDummySpritesheet = this.Helper.Content
                .LoadSpritesheet($"Assets//Mercenary_Meg_Dummy_Spritesheet", ContentSource.ModFolder);
            foreach (var o in mercenaryMegDummySpritesheet)
            {
                ;
            }

            var customEnemyData = new CustomEnemyData()
            {
                data = enemyData,
                createClass = () => new EnemyDummyMercenary(game),
                loadSpriteSheet = () =>
                {
                    return this.Helper.Content
                    .LoadSpritesheet($"Assets//Mercenary_Tris_Dummy_Spritesheet", ContentSource.ModFolder);
                },
                name = enemyDummyMercenaryName
            };

            this._database.RegisterEnemy(enemyDummyMercenaryName, customEnemyData);
        }


        // public Dictionary<string, SkillData> AddSkillData(ClassType classType, CustomCharacterData data)
        // {
        //     if (this.skillData != null) return this.skillData;
        //     this.skillData = data.SkillData;
        //     foreach (var skillDataPair in this.skillData)
        //         skillDataPair.Value.classType = classType;
        //
        //     this._database.Register(this.skillData);
        //     return this.skillData;
        // }
    }
}
