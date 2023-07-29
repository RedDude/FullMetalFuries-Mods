using System.Collections.Generic;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Framework.ModHelpers.DatabaseHelper;
using FullModdedFuriesAPI.Mods.MercenaryClass.Framework;
using FullModdedFuriesAPI.Mods.TitanClass.Source;

namespace Brawler2D
{
    public class TitanSetupCharacter
    {
        private IModHelper Helper;
        private GameController game;
        private ClassType _classType;
        public Dictionary<string, SkillData> skillData;
        public Dictionary<string, EquipmentData> classEquipmentData;
        private IDatabaseHelper _database;
        private int[,] skillTree;

        public TitanSetupCharacter(GameController game, IModHelper helper)
        {
            this.game = game;
            this.Helper = helper;
            this._database = this.Helper.Database;
        }

        public CustomCharacterData Create()
        {
            return new()
            {
                Name = "Titan",
                Color = PlayerClassObj_Titan.Titan_COLOUR,
                ClassData = this.CreatePlayerClassData(),
                SkillData = this.CreateSkillData(),
                EquipmentData = this.AddEquipmentData(),
                SkillTree = this.CreateSkillTree()
            };
        }

        public PlayerClassData CreatePlayerClassData()
        {
            return new("Player_Titan", 100, 25, 25,
                230f, 1f, 18f, 2f, new string[]
                {
                    "Hammer_Attack2",
                    "Hammer_Attack1",
                    "Hammer_Attack3",
                    "Hammer_Attack4",
                    "Hammer_Attack5"
                });
        }

        public Dictionary<string, SkillData> CreateSkillData()
        {
            return new Dictionary<string, SkillData>()
            {
                //EquipmentSlotType.ButtonX
                {
                    "HP_Titan", new SkillData(this._classType, 20f, (byte) 14, 0,
                        "Upgrade_Common_Health_Sprite", true, false,
                        "LOC_ID_SKILLTITLE_HP_Tank", "LOC_ID_SKILLDESCRIPTION_HP_Tank",
                        "LOC_ID_SKILLDETAILS_HP_Tank", "LOC_ID_LEVELTEXT_HP_Tank", 50, 45, 55)
                },
                {
                    "ATK_Titan",
                    new SkillData(this._classType, 5f, (byte) 14, 0, "Upgrade_Common_Attack_Sprite", true, false,
                        "LOC_ID_SKILLTITLE_ATK_Tank", "LOC_ID_SKILLDESCRIPTION_ATK_Tank",
                        "LOC_ID_SKILLDETAILS_ATK_Tank", "LOC_ID_LEVELTEXT_ATK_Tank", 50, 45, 55)
                },
                {
                    "TEC_Titan", new SkillData(this._classType, 5f, (byte) 14, 0,
                        "Upgrade_Common_Tech_Sprite", true, false,
                        "LOC_ID_SKILLTITLE_TEC_Tank", "LOC_ID_SKILLDESCRIPTION_TEC_Tank",
                        "LOC_ID_SKILLDETAILS_TEC_Tank", "LOC_ID_LEVELTEXT_TEC_Tank", 50, 45, 55)
                },
                {
                    "Passive_Titan", new SkillData(this._classType, 6f, (byte) 1, 25,
                        "Upgrade_Common_Health_Sprite", true,
                        false, "LOC_ID_SKILLTITLE_Passive_Tank", "LOC_ID_SKILLDESCRIPTION_Passive_Tank",
                        "LOC_ID_SKILLDETAILS_Passive_Tank", "LOC_ID_LEVELTEXT_Passive_Tank", 5000, 1, 0)
                },
                {
                    "Titan_Skill_A", new SkillData(this._classType, 1f, (byte) 1, 0,
                        "Upgrade_Engineer_Roll_Sprite", true, false,
                        "LOC_ID_SKILLTITLE_Eng_Skill_A", "LOC_ID_SKILLDESCRIPTION_Eng_Skill_A",
                        "LOC_ID_SKILLDETAILS_Eng_Skill_A", "LOC_ID_LEVELTEXT_Eng_Skill_A", -1, -1, -1)
                },
                {
                    "Titan_Skill_B", new SkillData(this._classType, 1f, (byte) 1, 0,
                        "Upgrade_Engineer_Roll_Sprite", true, false,
                        "LOC_ID_SKILLTITLE_Eng_Skill_B", "LOC_ID_SKILLDESCRIPTION_Eng_Skill_B",
                        "LOC_ID_SKILLDETAILS_Eng_Skill_B", "LOC_ID_LEVELTEXT_Eng_Skill_B", -1, -1, -1)
                },
                {
                    "Titan_Skill_X", new SkillData(this._classType, 1f, (byte) 1, 0,
                        "Upgrade_Engineer_Roll_Sprite", true, false,
                        "LOC_ID_SKILLTITLE_Eng_Skill_X", "LOC_ID_SKILLDESCRIPTION_Eng_Skill_X",
                        "LOC_ID_SKILLDETAILS_Eng_Skill_X", "LOC_ID_LEVELTEXT_Eng_Skill_X", -1, -1, -1)
                },
                {
                    "Titan_Skill_Y", new SkillData(this._classType, 1f, (byte) 1, 0,
                        "Upgrade_Engineer_Roll_Sprite", true, false,
                        "LOC_ID_SKILLTITLE_Eng_Skill_Y", "LOC_ID_SKILLDESCRIPTION_Eng_Skill_Y",
                        "LOC_ID_SKILLDETAILS_Eng_Skill_Y", "LOC_ID_LEVELTEXT_Eng_Skill_Y", -1, -1, -1)
                },
            };
        }

        public Dictionary<string, EquipmentData> AddEquipmentData()
        {
            return new Dictionary<string, EquipmentData>()
            {
                //EquipmentSlotType.ButtonX
                {
                    "Equipment_TitanAttack1", new EquipmentData(this._classType, EquipmentSlotType.ButtonX,
                        "Equipment_FighterAttack1_Sprite",
                        7012,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_BasicAttack", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_BasicAttack",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_BasicAttack", "5,4")
                }, {
                    "Equipment_TitanAttack2",new EquipmentData(this._classType, EquipmentSlotType.ButtonX, "Equipment_FighterAttack2_Sprite",
                        5737,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_VampAttack", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_VampAttack",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_VampAttack", "5,4,2.5")
                }, {
                    "Equipment_TitanAttack3", new EquipmentData(this._classType, EquipmentSlotType.ButtonX, "Equipment_FighterAttack3_Sprite",
                    3187,
                    "LOC_ID_EQUIPMENT_TITLE_FTR_HeavyAttack", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_HeavyAttack",
                    "LOC_ID_EQUIPMENT_DETAILS_FTR_HeavyAttack", "8 ,4")
                }, {
                    "Equipment_TitanAttack4",  new EquipmentData(this._classType, EquipmentSlotType.ButtonX, "Equipment_FighterAttack4_Sprite",
                        6375,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_FlameAttack", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_FlameAttack",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_FlameAttack", "5.5 , 4, 5")
                },
                //EquipmentSlotType.ButtonA
                {
                    "Equipment_TitanDummy1", new EquipmentData(this._classType, EquipmentSlotType.ButtonA, "Equipment_FighterLeap1_Sprite", 714,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_BasicLeap", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_BasicLeap",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_BasicLeap", "2, 3.5, 3, 2")
                }, {
                    "Equipment_TitanDummy2", new EquipmentData(this._classType, EquipmentSlotType.ButtonA, "Equipment_FighterLeap2_Sprite", 595,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_AirLeap", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_AirLeap",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_AirLeap", "3, 1.5, 4.5, 6")
                }, {
                    "Equipment_TitanDummy3", new EquipmentData(this._classType, EquipmentSlotType.ButtonA, "Equipment_FighterLeap3_Sprite", 595,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_PullLeap", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_PullLeap",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_PullLeap", "2, 4.5, 4, 4")
                }, {
                    "Equipment_TitanDummy4",  new EquipmentData(this._classType, EquipmentSlotType.ButtonA, "Equipment_FighterLeap4_Sprite", 952,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_MiniLeap", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_MiniLeap",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_MiniLeap", "0, 6.5, 0, 0")
                },
                //EquipmentSlotType.ButtonY
                {
                    "Equipment_TitanButtonY1", new EquipmentData(this._classType, EquipmentSlotType.ButtonY, "Equipment_FighterCounter1_Sprite",
                        637,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_BasicCounter", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_BasicCounter",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_BasicCounter", "2.5, 3, 3.5")
                }, {
                    "Equipment_TitanButtonY2",  new EquipmentData(this._classType, EquipmentSlotType.ButtonY, "Equipment_FighterCounter2_Sprite",
                        510,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_LargerCounter", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_LargerCounter",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_LargerCounter", "2.5, 5, 3.5")
                }, {
                    "Equipment_TitanButtonY3", new EquipmentData(this._classType, EquipmentSlotType.ButtonY, "Equipment_FighterCounter3_Sprite",
                        637,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_RiskCounter", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_RiskCounter",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_RiskCounter", "2.5, 3, 1.5, 1.5")
                }, {
                    "Equipment_TitanButtonY4", new EquipmentData(this._classType, EquipmentSlotType.ButtonY, "Equipment_FighterCounter4_Sprite",
                        765,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_RangeCounter", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_RangeCounter",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_RangeCounter", "2.5, 3, 10")
                },
                //EquipmentSlotType.ButtonB
                {
                    "Equipment_TitanButtonB1", new EquipmentData(this._classType, EquipmentSlotType.ButtonB, "Equipment_FighterSpin1_Sprite", 2805,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_BasicSpin", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_BasicSpin",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_BasicSpin", "4, 2, 1.5, 3, 3")
                }, {
                    "Equipment_TitanButtonB2",  new EquipmentData(this._classType, EquipmentSlotType.ButtonB, "Equipment_FighterSpin2_Sprite", 6375,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_FastSpin", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_FastSpin",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_FastSpin", "3, -1, 0.5, 9, 6")
                }, {
                    "Equipment_TitanButtonB3",  new EquipmentData(this._classType, EquipmentSlotType.ButtonB, "Equipment_FighterSpin3_Sprite", 7012,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_PullSpin", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_PullSpin",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_PullSpin", "4, 2, 1, 4.5, 1.5")
                }, {
                    "Equipment_TitanButtonB4", new EquipmentData(this._classType, EquipmentSlotType.ButtonB, "Equipment_FighterSpin4_Sprite", 3825,
                        "LOC_ID_EQUIPMENT_TITLE_FTR_ChargeSpin", "LOC_ID_EQUIPMENT_DESCRIPTION_FTR_ChargeSpin",
                        "LOC_ID_EQUIPMENT_DETAILS_FTR_ChargeSpin", "4.5, 2, 1.5, 0.5, 2")
                },
            };
        }

        public string[,] CreateSkillTree()
        {
            string[,] skillTree = new string[17, 14];
            // this.skillTree[7, 3] = (int) SkillType.HP_Tank;
            skillTree[9, 6] = "HP_Titan";
            skillTree[9, 3] = "TEC_Titan";
            skillTree[9, 9] = "ATK_Titan";

            return skillTree;
        }
    }
}
