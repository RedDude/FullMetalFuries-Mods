using System;
using System.Collections.Generic;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Mods.MercenaryClass;

namespace Brawler2D
{
    public class SaveHandler
    {
        private GameController game;
        private IModHelper helper;
        private SetupCharacter setupCharacter;

        private string _profileDataFilePath = "profileData";

        public SaveHandler(GameController game, IModHelper helper, SetupCharacter setupCharacter, string className = "")
        {
            this.game = game;
            this.helper = helper;
            this.setupCharacter = setupCharacter;
            this._profileDataFilePath = this._profileDataFilePath + className;
        }

        public void SetupEvents(ClassType classType)
        {
            this.helper.Events.Persistence.Saved += (sender, args) =>
            {
                int profileIndex = args.ProfileIndex;
                bool writeToDisk = args.WriteToDisk;
                this.OnSave(profileIndex, writeToDisk, classType);
            };

            this.helper.Events.Persistence.SaveLoaded += (sender, args) =>
            {
                int profileIndex = args.ProfileIndex;
                bool saveToPlayer = args.SaveToPlayer;

                this.SaveLoaded(profileIndex, saveToPlayer, classType);
            };

            this.helper.Events.Persistence.ResetStats += (sender, args) =>
            {
                var player = args.Player;
                this.ResetStats(player, classType);
                this.setupCharacter.SetStartingSkillsAndEquipment(player, classType);
            };

            this.helper.Events.Persistence.DeleteProfile += (sender, args) =>
            {
                int profileIndex = args.ProfileIndex;
                this.DeleteProfile(profileIndex);
            };
        }

        public void OnSave(int profileIndex, bool writeToDisk, ClassType classType)
        {
            var player = GameController.g_game.PlayerManager.GetPlayerAtProfileIndex(profileIndex, true);
            if (player == null)
                throw new Exception("Cannot load player at index: " + (object) profileIndex + ".  No player found");

            var customCharacterProfile =
                this.helper.Data.ReadJsonFile<Dictionary<string, object>>(this._profileDataFilePath + profileIndex) ??
                new Dictionary<string, object>();
            var data = customCharacterProfile;

            // helper.Data.WriteJsonFile("saveData", data );
            data["TotalSkillLevel"] = player.GetTotalUpgradePtsUsed(classType);
            for (int playerEquipSlotIndex = 0; playerEquipSlotIndex < 4; ++playerEquipSlotIndex)
                data["playerEquipSlot" + playerEquipSlotIndex] =
                    (int) player.GetEquippedEquipment(classType,
                        (EquipmentSlotType) playerEquipSlotIndex); //TODO: save equip name not index

            foreach (var skillDataPair in this.setupCharacter.skillData)
                data[skillDataPair.Key] =
                    player.GetSkillLevel((SkillType) this.helper.Database.GetCustomSkillIndex(skillDataPair.Key));

            foreach (var equipmentDataPair in this.setupCharacter.classEquipmentData)
            {
                data[equipmentDataPair.Key] =
                    (int) player.GetEquipmentState(
                        (EquipmentType) this.helper.Database.GetCustomEquipmentIndex(equipmentDataPair.Key));
                data[equipmentDataPair.Key + "MasteryLevel"] =
                    player.GetCurrentMasteryPoints(
                        (EquipmentType) this.helper.Database.GetCustomEquipmentIndex(equipmentDataPair.Key));
            }

            if (!writeToDisk)
                return;

            this.helper.Data.WriteJsonFile(this._profileDataFilePath + profileIndex, data);
            ModEntry.monitor.Log(this._profileDataFilePath + profileIndex + " Saved");
        }

        public void ResetStats(PlayerObj player, ClassType classType)
        {
            //should reset all custom? or just the mercenary one?
            player.RemoveAllEquippedEquipment(classType);

            foreach (var skillDataPair in this.setupCharacter.skillData)
                player.SetSkillLevel((SkillType) this.helper.Database.GetCustomSkillIndex(skillDataPair.Key),
                    (sbyte) 0);

            foreach (var equipmentDataPair in this.setupCharacter.classEquipmentData)
                player.SetEquipmentState(
                    (EquipmentType) this.helper.Database.GetCustomEquipmentIndex(equipmentDataPair.Key),
                    EquipmentState.NotOwned, true);
        }

        public void Saved(int profileIndex, bool writeToDisk, ClassType classType)
        {
            this.OnSave(profileIndex, writeToDisk, classType);
        }

        public void SaveLoaded(int profileIndex, bool saveToPlayer, ClassType classType)
        {
            if (!SaveManager.ProfileExists(profileIndex))
                return;
            if (!saveToPlayer)
                return;

            var player = GameController.g_game.PlayerManager.GetPlayerAtProfileIndex(profileIndex, true);
            if (player == null)
                throw new Exception("Cannot load player at index: " + (object) profileIndex + ".  No player found");

            var customCharacterProfile = new Dictionary<string, object>();
            try
            {
                customCharacterProfile =
                    this.helper.Data.ReadJsonFile<Dictionary<string, object>>(this._profileDataFilePath + profileIndex) ??
                    new Dictionary<string, object>();
            }
            catch (Exception e)
            {
                customCharacterProfile = new Dictionary<string, object>();
            }

            var data = customCharacterProfile;

            if (data.Count == 0)
            {
                this.ResetStats(player, classType);
                this.OnSave(profileIndex, true, classType);
                player.UpdateClassLevel(classType);
                return;
            }

            foreach (var skillDataPair in this.setupCharacter.skillData)
                player.SetSkillLevel((SkillType) this.helper.Database.GetCustomSkillIndex(skillDataPair.Key),
                    (sbyte) 0);

            // data[profileIndex]["TotalSkillLevel"] = player.GetTotalUpgradePtsUsed(mercenaryClassType);

            for (int playerEquipSlotIndex = 0; playerEquipSlotIndex < 4; ++playerEquipSlotIndex)
                player.EquipEquipment(classType, (EquipmentType) (long) data["playerEquipSlot" + playerEquipSlotIndex]);

            foreach (var skillDataPair in this.setupCharacter.skillData)
                player.SetSkillLevel((SkillType) this.helper.Database.GetCustomSkillIndex(skillDataPair.Key),
                    Convert.ToSByte(data[skillDataPair.Key]));

            foreach (var equipmentDataPair in this.setupCharacter.classEquipmentData)
            {
                player.SetEquipmentState(
                    (EquipmentType) this.helper.Database.GetCustomEquipmentIndex(equipmentDataPair.Key),
                    (EquipmentState) (long) data[equipmentDataPair.Key]);
                player.SetMasteryPoints(
                    (EquipmentType) this.helper.Database.GetCustomEquipmentIndex(equipmentDataPair.Key),
                    (int) (long) data[equipmentDataPair.Key + "MasteryLevel"], false);
            }

            player.UpdateClassLevel(classType);
            ModEntry.monitor.Log(this._profileDataFilePath + profileIndex + " Loaded");
        }

        public void DeleteProfile(int profileIndex)
        {
            if (!SaveManager.ProfileExists(profileIndex))
                return;
            this.helper.Data.WriteJsonFile(this._profileDataFilePath + profileIndex, (object) null);
        }
    }
}
