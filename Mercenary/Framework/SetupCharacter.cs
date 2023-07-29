using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CDGCore;
using CDGEngine;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Events;
using FullModdedFuriesAPI.Framework.ModHelpers.DatabaseHelper;
using FullModdedFuriesAPI.Mods.MercenaryClass;
using FullModdedFuriesAPI.Mods.MercenaryClass.Framework;
using Microsoft.Xna.Framework;

namespace Brawler2D
{
    public class SetupCharacter
    {
        private IModHelper Helper;
        private GameController game;
        private ClassType _classType;
        public Dictionary<string, SkillData> skillData;
        public Dictionary<string, EquipmentData> classEquipmentData;
        private IDatabaseHelper _database;
        private int[,] skillTree;
        private string _className;
        private Color _color;

        public SetupCharacter(GameController game, IModHelper helper)
        {
            this.game = game;
            this.Helper = helper;
            this._database = this.Helper.Database;
        }

        public ClassType SetupData(CustomCharacterData data)
        {
            if (PlayerEV.PlayerData.ContainsKey("Player_" + data.Name))
            {
                return (ClassType) this._database.GetCustomClasses()[data.Name];
            }

            var classType = this.AddPlayerData(data);
            this.AddEquipmentData(classType, data);
            this.AddSkillData(classType, data);
            this.AddSkillTree(data);

            ModEntry.monitor.Log($"Data for {data.Name} setup");
            var enumerator = this.LoadSpritesheet(data.Name).GetEnumerator();
            foreach (var s in (IEnumerable) enumerator)
            {
                ;
            }
            ModEntry.monitor.Log($"{data.Name} spritesheet loaded");
            return classType;
        }

        public ClassType AddPlayerData(CustomCharacterData data)
        {
            this._className = data.Name;
            this._color = data.Color;
            string key = $"Player_{data.Name}";

            return (ClassType) this._database.RegisterClass(
                key, data.ClassData, this._color);
        }

        public IEnumerable<object> LoadSpritesheet(string className)
        {
            var enumerator =
                this.Helper.Content.LoadSpritesheet($"Assets//Player{className}Spritesheet", ContentSource.ModFolder).GetEnumerator();
                // this.Helper.Content.LoadSpritesheet("Art//Players//PlayerMercenarySpritesheet", ContentSource.ModFolder).GetEnumerator();

            LoadingScreen.LoadProgress(enumerator);
            yield return null;
            // while (enumerator.MoveNext())
            // {
                // object current = enumerator.Current;
                // yield return null;
            // }
        }

        public IEnumerable<object> AddPlayerClassToPlayerManager(ClassType customClassType, Func<PlayerObj, PlayerClassObj> createClass)
        {
            var playerManager = GameController.g_game.PlayerManager;

            for (int i = 0; i < playerManager.playerArray_count; ++i)
            {

             // this.AddEquipmentData();

                //
                // this.SolveEquipmentMastery(playerManager, i);

                var playerArray =
                    this.Helper.Reflection.GetField<List<PlayerClassObj>>(playerManager.playerArray[i],
                        "m_playerClassList");

                var playerClass = createClass.Invoke(playerManager.playerArray[i]);
                // playerArray.GetValue().Add(playerClass);
                playerArray.GetValue().Add(playerClass);
                CVar sys_guid = CVarSystem.Find("sys_guid");
                playerClass.GUID = (uint) sys_guid.IntegerValue++;
                foreach (var obj in playerClass.LoadContent())
                    yield return (object) obj;

                playerClass.Active = false;
                var playerObj = playerManager.playerArray[i];
                this.SolveEquipmentEquipped(playerObj);
                this.SolveClassEquipmentAndSkill(playerObj, customClassType);
                this.SetStartingSkillsAndEquipment(playerObj, customClassType);
                this.SolveClassLevel(playerObj);

                playerClass.Initialize();
            }
        }

        public Dictionary<string, EquipmentData> AddEquipmentData(ClassType classType, CustomCharacterData data)
        {
            if (this.classEquipmentData != null) return this.classEquipmentData;

            foreach (var equipmentData in data.EquipmentData)
            {
                equipmentData.Value.classType = classType;
                this._database.RegisterEquipment(equipmentData.Key, equipmentData.Value);
            }

            this.classEquipmentData = data.EquipmentData;
            return this.classEquipmentData;
        }

        public Dictionary<string, SkillData> AddSkillData(ClassType classType, CustomCharacterData data)
        {
            if (this.skillData != null) return this.skillData;
            this.skillData = data.SkillData;
            foreach (var skillDataPair in this.skillData)
                skillDataPair.Value.classType = classType;

            this._database.Register(this.skillData);
            return this.skillData;
        }

        public int[,] AddSkillTree(CustomCharacterData data)
        {
            if (this.skillTree != null) return this.skillTree;
            string[,] skillTreeString = data.SkillTree;
            int[,] skillTreeInts = new int[17, 14];
            for (int x = 0; x < skillTreeString.GetLength(0); x++)
                for (int y = 0; y < skillTreeString.GetLength(1); y++)
                {
                    string s = skillTreeString[x, y];
                    if (!string.IsNullOrEmpty(s))
                        skillTreeInts[x, y] = s == "-" ? 1 : this._database.GetCustomSkillIndex(s);
                }

            this._database.RegisterSkillTree(data.Name, () => skillTreeInts, updateSkillTree =>
            {
                this.skillTree = updateSkillTree;
            });

            return this.skillTree;
        }


        public void SetStartingSkillsAndEquipment(PlayerObj playerObj, ClassType classType)
        {
            string className = this.Helper.Database.GetClassTypeName(classType);

            var equipmentData =
                EquipmentEV.EquipmentData.FindAll(data => data.classType == classType);

            var defaultDataA = equipmentData.First(data => data.equipmentSlotType == EquipmentSlotType.ButtonA);
            var defaultDataB = equipmentData.First(data => data.equipmentSlotType == EquipmentSlotType.ButtonB);
            var defaultDataY = equipmentData.First(data => data.equipmentSlotType == EquipmentSlotType.ButtonY);
            var defaultDataX = equipmentData.First(data => data.equipmentSlotType == EquipmentSlotType.ButtonX);

            int defaultA = EquipmentEV.EquipmentData.IndexOf(defaultDataA);
            int defaultB = EquipmentEV.EquipmentData.IndexOf(defaultDataB);
            int defaultY = EquipmentEV.EquipmentData.IndexOf(defaultDataY);
            int defaultX = EquipmentEV.EquipmentData.IndexOf(defaultDataX);

            playerObj.SetSkillLevel((SkillType) this._database.GetCustomSkillIndex(className+"_Skill_A"), (sbyte) 1);
            playerObj.SetSkillLevel((SkillType) this._database.GetCustomSkillIndex(className+"_Skill_B"), (sbyte) 1);
            playerObj.SetSkillLevel((SkillType) this._database.GetCustomSkillIndex(className+"_Skill_X"), (sbyte) 1);
            playerObj.SetSkillLevel((SkillType) this._database.GetCustomSkillIndex(className+"_Skill_Y"), (sbyte) 1);

           playerObj.SetSkillLevel((SkillType) this._database.GetCustomSkillIndex("HP_"+className), 1);
           playerObj.SetSkillLevel((SkillType) this._database.GetCustomSkillIndex("ATK_"+className), 1);
           playerObj.SetSkillLevel((SkillType) this._database.GetCustomSkillIndex("TEC_"+className), 1);
           playerObj.SetSkillLevel((SkillType) this._database.GetCustomSkillIndex("Passive_"+className), 1);

            playerObj
                .SetEquipmentState((EquipmentType) defaultA, EquipmentState.OwnedButNotMastered, true);
            playerObj
                .SetEquipmentState((EquipmentType) defaultB, EquipmentState.OwnedButNotMastered, true);
            playerObj
                .SetEquipmentState((EquipmentType) defaultY, EquipmentState.OwnedButNotMastered, true);
            playerObj
                .SetEquipmentState((EquipmentType) defaultX, EquipmentState.OwnedButNotMastered, true);
            playerObj.EquipEquipment(classType, (EquipmentType) defaultA);
            playerObj.EquipEquipment(classType, (EquipmentType) defaultB);
            playerObj.EquipEquipment(classType, (EquipmentType) defaultY);
            playerObj.EquipEquipment(classType, (EquipmentType) defaultX);

            // playerObj
            //     .SetEquipmentState((EquipmentType) (this._database.GetCustomEquipmentIndex(defaultA.)), EquipmentState.OwnedButNotMastered, true);
            // playerObj
            //     .SetEquipmentState((EquipmentType) (this._database.GetCustomEquipmentIndex("Equipment_MercenaryDummy1")), EquipmentState.OwnedButNotMastered, true);
            // playerObj
            //     .SetEquipmentState((EquipmentType) (this._database.GetCustomEquipmentIndex("Equipment_MercenaryButtonY1")), EquipmentState.OwnedButNotMastered, true);
            // playerObj
            //     .SetEquipmentState((EquipmentType) (this._database.GetCustomEquipmentIndex("Equipment_MercenaryButtonB1")), EquipmentState.OwnedButNotMastered, true);
            // playerObj.EquipEquipment(classType, (EquipmentType) (this._database.GetCustomEquipmentIndex("Equipment_MercenaryAttack1")));
            // playerObj.EquipEquipment(classType, (EquipmentType) (this._database.GetCustomEquipmentIndex("Equipment_MercenaryDummy1")));
            // playerObj.EquipEquipment(classType, (EquipmentType) (this._database.GetCustomEquipmentIndex("Equipment_MercenaryButtonY1")));
            // playerObj.EquipEquipment(classType, (EquipmentType) (this._database.GetCustomEquipmentIndex("Equipment_MercenaryButtonB1")));
        }

        private void SolveClassLevel(PlayerObj playerObj)
        {
            var classLevelField =
                this.Helper.Reflection.GetField<int[]>(playerObj,
                    "m_classLevel");
            int[] classLevels = classLevelField.GetValue();
            int[] newClassLevel = new int[classLevels.Length + 1]; // Need a empty fake index due game enum
            // int[] newClassLevel = new int[classLevel.Length + 2]; // Need a empty fake index due game enum
            for (int index = 0; index < classLevels.Length; index++)
                newClassLevel[index] = classLevels[index];

            classLevelField.SetValue(newClassLevel);
        }

        private void SolveEquipmentEquipped(PlayerObj playerObj)
        {
            var equipmentEquipped = this.Helper.Reflection.GetField<List<EquipmentType[]>>(playerObj,
                "m_equipmentEquipped").GetValue();
            // equipmentEquipped.Add(new EquipmentType[0]); // Need a empty fake index due game enum
            equipmentEquipped.Add(new EquipmentType[5]);
        }

        private void SolveClassEquipmentAndSkill(PlayerObj playerObj, ClassType classType)
        {
            // var cachedTotalMasteryLevel = this.Helper.Reflection.GetField<List<int>>(playerObj,
            //     "m_cachedTotalMasteryLevel").GetValue();
            // for (int index = 0; index < 4; ++index)
            //     cachedTotalMasteryLevel.Add(0);

            var skillUnlockList = this.Helper.Reflection.GetField<List<sbyte>>(playerObj,
                "m_skillUnlockList").GetValue();

            var skillData =
                SkillEV.SkillData.FindAll(data => data.classType == classType);
            foreach (var data in skillData)
            {
                skillUnlockList.Add(1);
            }

            var equipmentStateList = this.Helper.Reflection.GetField<List<EquipmentState>>(playerObj,
                "m_equipmentStateList").GetValue();
            var equipmentMasteryList = this.Helper.Reflection.GetField<List<int>>(playerObj,
                "m_equipmentMasteryList").GetValue();
            var startingMasteryList = this.Helper.Reflection.GetField<List<int>>(playerObj,
                "m_startingMasteryList").GetValue();

            var equipmentData =
                EquipmentEV.EquipmentData.FindAll(data => data.classType == classType);
            foreach (var data in equipmentData)
            {
                equipmentStateList.Add(EquipmentState.NotOwned);
                equipmentMasteryList.Add(0);
                startingMasteryList.Add(0);
            }
            ModEntry.monitor.Log($"m_skillUnlockList");
        }

          public void OnSpriteNotFound(string spriteName, SpriteObj spriteObj, ClassType classType)
        {
            //todo: Generic class and replace string with lookup dictonary

            string classTypeName = this._className;
            if (classTypeName == null)
                return;

            if (spriteName.StartsWith("CampIcon_" + classType))
            {
                spriteObj.ChangeSprite("CampIcon_" + classTypeName);
                return;
            }

            if (spriteName.StartsWith("WrapUp_" + classType))
            {
                spriteObj.ChangeSprite("WrapUp_" + classTypeName + "_Portrait");
                return;
            }

            if (spriteName.StartsWith("GetItem_" + classType))
            {
                spriteObj.ChangeSprite("GetItem_" + classTypeName + "Portrait");
                return;
            }
        }

        private void UpdatePlayerMapIcon(ClassType classType, string spriteName)
        {
            if (this.game.ScreenManager.CurrentScreen is not MapScreen mapScreen) return;
            var playerMapIcons = this.Helper.Reflection.GetField<List<BrawlerContainerObj>>(mapScreen,
                "m_playerMapIconList").GetValue();

            for (int index = 0; index < this.game.PlayerManager.activePlayerArray_count; ++index)
            {
                var activePlayer = this.game.PlayerManager.activePlayerArray[index];
                var playerMapIcon = playerMapIcons[index];
                if (activePlayer.firstSwappableClassType == classType &&
                    playerMapIcon.spriteName != spriteName)
                    playerMapIcon.ChangeSprite(spriteName);
                if (this.game.PlayerManager.activePlayerArray_count == 1 &&
                    activePlayer.secondSwappableClassType == classType &&
                    playerMapIcon.spriteName != spriteName)
                    playerMapIcons[index + 1].ChangeSprite(spriteName);
            }
        }


        public bool setupDataDone = false;
        public bool setupDone = false;

        public ClassType Setup(CustomCharacterData customData, Func<PlayerObj, PlayerClassObj> createClass)
        {
            var classType = ClassType.None;
            CampHandler campHandler = null;
            InterfaceHandler interfaceHandler = null;

            // this.Helper.Events.GameLoop.GameLaunched += (sender, args) =>
            // {
            //     if (this.setupDataDone)
            //         return;
            //
            //     // if(!Context.IsWorldReady || this.game.PlayerManager == null)
            //     // return;
            //
            //     classType = this.SetupData(customData);
            //     this._classType = classType;
            //     campHandler = new CampHandler(this.game, this.Helper, classType);
            //     interfaceHandler = new InterfaceHandler(this.game, this.Helper, classType);
            //
            //     this.game = GameController.g_game;
            //     this.setupDataDone = true;
            //     this.Helper.Events.GameLoop.UpdateTicked += this.SetupInit(this.Helper, classType, createClass);
            // };
            this.Helper.Events.GameLoop.UpdateTicked += (sender, args) =>
            {
                if (this.setupDataDone)
                    return;

                // if(!Context.IsGameLaunched || this.game.PlayerManager == null)
                // return;

                // if(!Context.IsWorldReady || this.game.PlayerManager == null)
                    // return;

                if(!GameController.g_game.gameLoaded)
                    return;

                classType = this.SetupData(customData);
                this._classType = classType;
                campHandler = new CampHandler(this.game, this.Helper, classType);
                interfaceHandler = new InterfaceHandler(this.game, this.Helper, classType);

                this.game = GameController.g_game;
                this.setupDataDone = true;

                var enumerator = this.AddPlayerClassToPlayerManager(classType,
                    createClass
                ).GetEnumerator();
                LoadingScreen.LoadProgress(enumerator);
                var saveHandler = new SaveHandler(this.game, this.Helper, this, customData.Name);
                saveHandler.SetupEvents(classType);
                // this.SetupInit(this.Helper, classType, createClass);
                // this.Helper.Events.GameLoop.UpdateTicked += this.SetupInit(this.Helper, classType, createClass);
            };

            this.Helper.Events.GameLoop.UpdateTicking += (sender, args) =>
            {
                if(!GameController.g_game.gameLoaded)
                    return;

                if(this.game.ScreenManager.arenaScreen == null)
                    return;

                if (this.game.ScreenManager.arenaScreen.IsPaused)
                    return;

                if (this.game.PlayerManager.getMainPlayerOrHost == null)
                    return;

                if(this.IsCamp())
                    campHandler?.Handle();

                interfaceHandler?.Handle();
            };

            this.Helper.Events.GameLoop.UpdateTicking += (sender, args) =>
            {
                if (!Context.IsGameLaunched)
                    return;

                if(this.game.ScreenManager?.arenaScreen == null)
                    return;

                this.UpdatePlayerMapIcon((ClassType) classType, $"Player{this._className}_Character");
            };

            this.Helper.Events.Specialized.SpriteNotFound += (sender, args) =>
            {
                this.OnSpriteNotFound(args.spriteName, args.SpriteObj, classType);
            };

            return classType;
        }

        private EventHandler<UpdateTickedEventArgs> SetupInit(IModHelper helper, ClassType classType, Func<PlayerObj, PlayerClassObj> createClass)
        {
            return (sender, args) =>
            {
                if(!Context.IsGameLaunched || this.game.PlayerManager == null)
                    return;

                if (this.game.PlayerManager.playerArray_count <= 3)
                {
                    this.setupDone = false;
                    return;
                }

                if (this.setupDone)
                    return;


                this.setupDone = true;


                var enumerator = this.AddPlayerClassToPlayerManager(classType,
                    createClass
                ).GetEnumerator();
                LoadingScreen.LoadProgress(enumerator);
                // while (enumerator.MoveNext())
                // {
                //     object current = enumerator.Current;
                // }
                //

                // foreach (var o in this.setupCharacter.CreateAndAddPlayerClass()) {};
                // this.Helper.Events.GameLoop.UpdateTicking -= this.SetupCharacter(helper);
            };
        }

        private bool IsCamp()
        {
            return this.game.ScreenManager.arenaScreen.stageName.ToLower().Contains("camp");
        }

    }
}
