using System;
using System.Collections;
using System.Collections.Generic;
using Brawler2D;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Framework.ModHelpers.LocalBuilderHelper;
using ImGuiMod;
using ImGuiNET;

namespace DevMenu
{
    public class DataEditor
    {
        public static bool Open;
        public string nameWindow = "Save Viewer";

        // private List<SaveData> list = new List<SaveData>();
        // private Dictionary<SaveData, string> fakeNames = new Dictionary<SaveData, string>();
        // private Dictionary<SaveData, int> SaveIcons = new Dictionary<SaveData, int>();
        // private Dictionary<SaveData, Texture2D> SaveIconsTexture2Ds = new Dictionary<SaveData, Texture2D>();

        // public string[] options = new[] {"Profile1", "Profile2", "Profile3", "Profile4", "Profile5", "Profile6"};

        private bool cached;
        private static int selected;
        private ILocateBuilderHelper _locateBuilder;
        private IImGuiMenuApi imGuiMenuApi;
        private int OptionSelected;
        private object[] saveDataDict;

        public DataEditor()
        {

        }

        public void Register(IImGuiMenuApi configMenu, IManifest manifest)
        {
            configMenu.AddMainBarMenuOption(
                manifest, () => "DevTools", () => this.nameWindow,
                () =>
                {
                    if (!this.cached)
                    {

                        this.saveDataDict = ModEntry.ModHelper.Reflection
                            .GetField<object[]>(typeof(SaveManager), "m_saveDataDict").GetValue();
                        // this._locateBuilder = ModEntry.ModHelper.LocateBuilder;
                        // this.GetList();
                        this.cached = true;
                    }

                    Open = !Open;
                });

            configMenu.AddWindowItem(
                manifest, this.Render, () => Open);

            this.imGuiMenuApi = configMenu;
        }

        public void Render()
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowSize(new ImVec2(500, 440), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(this.nameWindow, ref Open))
            {
                // this.PopUp();

                // if(this.OptionSelected == 0)
                // this.RenderList();
                // if(this.OptionSelected == 1)
                this.ReadSaveDataDict();
                // this.RenderInfo();
            }

            ImGui.EndChild();
        }

        // private void RenderList()
        // {
        //     {
        //         ImGui.BeginChild("left pane", new ImVec2(400, 0), true, ImGuiWindowFlags.Default); //, new ImVec2(150, 0), true);
        //
        //         // var names = this.list;
        //         for (int i = 0; i < this.saveDataDict.Length; i++)
        //         {
        //             var profile = this.saveDataDict[i];
        //             string fixedName = Save.descripLocID.Replace("LOC_ID_SaveDETAILS_", "").Replace("_", " ");
        //             string SaveCode = this.fakeNames[Save].Length != 0 ? this.fakeNames[Save] : Save.iconName;
        //             // string enemyName =
        //             //     !this.fakeNames[enemy].Contains("LOC_ID_NAME_") && !this.fakeNames[enemy].Contains("NULL") ? $"{enemyFixed} ({this.fakeNames[enemy]})" : enemyFixed;
        //             string SaveName =
        //                 !this.fakeNames[Save].Contains("NULL") ? $"{fixedName} ({SaveCode})" : fixedName;
        //
        //             // if (this.SaveIcons.ContainsKey(Save))
        //             // {
        //             //     if(ImGui.ImageButton(this.SaveIcons[Save], new ImVec2(100, 10), ImVec2.One, ImVec2.One, 4, new ImVec4(0, 0, 0, 0), new ImVec4(250, 250, 250, 0)))
        //             //     {
        //             //         selected = i;
        //             //     }
        //             // }
        //             // else
        //             // {
        //                 if (ImGui.Selectable(SaveName, selected == i))
        //                 {
        //                     selected = i;
        //                 }
        //             // }
        //
        //
        //         }
        //
        //         ImGui.EndChild();
        //     }
        //     ImGui.SameLine();
        // }

        private void RenderSaveTree()
        {
            {
                ImGui.BeginChild("Savetree pane", new ImVec2(400, 0), true,
                    ImGuiWindowFlags.Default); //, new ImVec2(150, 0), true);

                for (int i = 0; i < this.saveDataDict.Length; i++)
                {
                    if (!ImGui.TreeNode("Profile" + i + 1)) continue;

                    var profile = this.saveDataDict[i];
                    // foreach (var keyValuePair in profile)
                    {

                        // ImGui.Text("Width %.2f", ImGui.GetColumnWidth());
                        // ImGui.Text("Avail %.2f", ImGui.GetContentRegionAvail().x);
                        // // int classLevel = this.m_player.GetClassLevel(this.m_classType);
                        //
                        // ImGui.Columns(selectedUpgradeTree.GetLength(1), keyValuePair.Key + "SelectedUpgradeTree",
                        //     false);
                        // for (int col = 2; col < selectedUpgradeTree.GetLength(1) - 3; ++col)
                        // {
                        //     bool found = false;
                        //     for (int row = 7; row < selectedUpgradeTree.GetLength(0) - 3; ++row)
                        //     {
                        //         // if (ImGui.GetColumnIndex() == 0)
                        //         //     ImGui.Separator();
                        //
                        //         var Save = this.list[selectedUpgradeTree[row, col]];
                        //         if (this.SaveIcons.ContainsKey(Save))
                        //         {
                        //             found = true;
                        //             if (ImGui.ImageButton(
                        //                 this.SaveIcons[Save],
                        //                 new ImVec2(size.x - 1, size.y - 1),
                        //                 ImVec2.Zero,
                        //                 ImVec2.One,
                        //                 1,
                        //                 new ImVec4(0, 0, 0, 0),
                        //                 new ImVec4(250, 250, 250, 255)))
                        //             {
                        //                 selected = this.list.IndexOf(Save);
                        //             }
                        //         }
                        //         else
                        //         {
                        //             ImGui.Button("", size);
                        //         }
                        //
                        //         // ImGui.ImageButton("%c%c%c", 'a' + i, 'a' + i, 'a' + i);
                        //         // ImGui.Text("Width %.2f", ImGui.GetColumnWidth());
                        //         // ImGui.Text("Avail %.2f", ImGui.GetContentRegionAvail().x);
                        //         // ImGui.Text("Offset %.2f", ImGui.GetColumnOffset());
                        //         // ImGui.Text("Long text that is likely to clip");
                        //         // ImGui.Button("Button", ImVec2(-FLT_MIN, 0.0f));
                        //
                        //     }
                        //
                        //     ImGui.NextColumn();
                        //
                        // }

                        // ImGui.Columns(1);
                        // ImGui.Separator();
                        ImGui.TreePop();
                    }

                }

                ImGui.EndChild();
            }
            ImGui.SameLine();
        }

        private void ReadSaveDataDict()
        {
            ImGui.BeginChild("Savetree pane", new ImVec2(400, 0), true,
                ImGuiWindowFlags.Default); //, new ImVec2(150, 0), true);

            for (int i = 0; i < 6; i++)
            {
                if (!ImGui.TreeNode("Profile" + (i + 1))) continue;

                var profile = this.saveDataDict[i];
                var profileDict = (IDictionary) profile;

                this.DoGenericDictonary(this.saveDataDict[i], i);

                ImGui.TreePop();

            }

            ImGui.EndChild();


            ImGui.SameLine();
        }

        public void DoGenericDictonary(object obj, int profileIndex)
        {
            foreach (Type iType in obj.GetType().GetInterfaces())
            {
                if (iType.IsGenericType && iType.GetGenericTypeDefinition()
                    == typeof(IDictionary<,>))
                {
                    typeof(DataEditor).GetMethod("ShowContents")
                        .MakeGenericMethod(iType.GetGenericArguments())
                        .Invoke(null, new object[] {obj, profileIndex});
                    break;
                }
            }
        }

        public static void ShowContents<TKey, TValue>(
            IDictionary<TKey, TValue> data, int index)
        {
            foreach (var pair in data)
            {
                string entryName = pair.Key.ToString();
                SaveManager.SimpleDataType simpleValueType = ModEntry.ModHelper.Reflection
                    .GetField<SaveManager.SimpleDataType>(pair.Value, "m_simpleDataType").GetValue();

                int profileIndex = index + 1;
                // string entryName = (string) profileDict.Keys.GetEnumerator()
                // string entryName = (string)profileData.Key;
                // string entryName = ModEntry.ModHelper.Reflection.GetField<string>(saveData, "entryName")
                //     .GetValue();
                object obj = (object) null;
                switch (simpleValueType)
                {
                    case SaveManager.SimpleDataType.Bool:
                        bool boolValue = SaveManager.LoadBool(profileIndex, entryName);
                        obj = boolValue;
                        break;
                    case SaveManager.SimpleDataType.Byte:
                        byte byteValue = SaveManager.LoadByte(profileIndex, entryName);
                        obj = byteValue;
                        break;
                    case SaveManager.SimpleDataType.SByte:
                        sbyte sbyteValue = SaveManager.LoadSByte(profileIndex, entryName);
                        obj = sbyteValue;
                        break;
                    case SaveManager.SimpleDataType.Int:
                        int intValue = SaveManager.LoadInt(profileIndex, entryName);
                        obj = intValue;
                        break;
                    case SaveManager.SimpleDataType.Uint:
                        uint uintValue = SaveManager.LoadUint(profileIndex, entryName);
                        obj = uintValue;
                        break;
                    case SaveManager.SimpleDataType.Float:
                        float floatValue = SaveManager.LoadFloat(profileIndex, entryName);
                        obj = floatValue;
                        break;
                    case SaveManager.SimpleDataType.String:
                        string stringValue = SaveManager.LoadString(profileIndex, entryName);
                        obj = stringValue;
                        break;
                }

                ImGui.Text(entryName + ": " + obj);
            }
        }

        public static void DebugContents<TKey, TValue>(
            IDictionary<TKey, TValue> data)
        {
            foreach (var pair in data)
            {
                Console.WriteLine(pair.Key + " = " + pair.Value);
            }

        }

        public static void LoadProfile(int profileIndex, bool saveToPlayer = true)
        {
            if (!SaveManager.ProfileExists(profileIndex))
                return;
            if (!SaveManager.ProfileLoaded(1))
                SaveManager.SaveGame(1, false, false, "CORRUPTED FILE", (PlayerObj) null);

            var dictProfileFields = new List<PersistenceData>()
            {
                new PersistenceData("playerName", SaveManager.SimpleDataType.String),
                new PersistenceData("goldCollected", "gold", SaveManager.SimpleDataType.Int),
                new PersistenceData("mapObjectiveTextIndex", SaveManager.SimpleDataType.Byte),
                new PersistenceData("currentMap", SaveManager.SimpleDataType.String),
                new PersistenceData("currentStage", SaveManager.SimpleDataType.String),
                new PersistenceData("numBaseCampVisits", SaveManager.SimpleDataType.Int),
                new PersistenceData("skillTreeTutorialComplete", "skillTreeTutComplete"),
                new PersistenceData("equipmentTutorialComplete", "equipmentTutComplete"),
                new PersistenceData("treasuryTutorialComplete", "treasuryTutComplete"),
                new PersistenceData("newGameIntroComplete"),
                new PersistenceData("demoCompleted"),
                new PersistenceData("warmongerTutorialComplete"),
                new PersistenceData("masteryTutorialComplete"),
                new PersistenceData("act1Seen"),
                new PersistenceData("act2Seen"),
                new PersistenceData("act3Seen"),
                new PersistenceData("act4Seen"),
                new PersistenceData("act5Seen"),
                new PersistenceData("act6Seen"),
                new PersistenceData("act7Seen"),
                new PersistenceData("act8Seen"),
                new PersistenceData("act9Seen"),
                new PersistenceData("act10Seen"),
                new PersistenceData("newGamePlusCount", SaveManager.SimpleDataType.Byte),
                new PersistenceData("promNoteTutorialComplete"),
                new PersistenceData("END_FALSE"),
                new PersistenceData("CAVE_ENTER"),
                new PersistenceData("END_TRUE"),
                new PersistenceData("END_DECISION"),
                new PersistenceData("END_TWO"),
                new PersistenceData("COMBO_AIR_CRIT"),
                new PersistenceData("warmongerSpeech2Complete"),
                new PersistenceData("easyModeTutorialComplete"),
                new PersistenceData("totalTimePlayed"),
                new PersistenceData("totalGoldCollected"),
                new PersistenceData("discountTutorialComplete"),
                new PersistenceData("skillTreeUnlocked"),
                new PersistenceData("reviveTutorialComplete"),
                new PersistenceData("enableDeafMode"),
                new PersistenceData("dialogueDisplayType", SaveManager.SimpleDataType.Byte),
                new PersistenceData("enableEasyMode"),
                new PersistenceData("enablePromNoteHints"),
                new PersistenceData("allowSameClass"),
                new PersistenceData("enableEasyMode"),
                new PersistenceData("enableEasyMode"),
                new PersistenceData("disableArrowIndicators"),
                new PersistenceData("enableColorBlindMode"),
            };
            // new PersistenceData("numEnemiesCount", SaveManager.SimpleDataType.Int),
            // int num1 = SaveManager.LoadInt(profileIndex, "numEnemiesCount");
            // for (int enemyIndex = 0; enemyIndex < num1; ++enemyIndex)
            //   playerAtProfileIndex.SetNumEnemiesKilled(enemyIndex, SaveManager.LoadInt(profileIndex, "enemyKilledType" + (object) enemyIndex), false);
            // for (int index1 = 1; index1 < 5; ++index1)
            // {
            //   for (int index2 = 0; index2 < 4; ++index2)
            //     playerAtProfileIndex.EquipEquipment((ClassType) index1, (EquipmentType) SaveManager.LoadByte(profileIndex, "playerEquipSlot" + index1.ToString() + index2.ToString()));
            // }
            // for (int index = 2; index < 180; ++index)
            //   playerAtProfileIndex.SetSkillLevel((SkillType) index, SaveManager.LoadSByte(profileIndex, "playerSkillLevel" + (object) index));
            // playerAtProfileIndex.UpdateAllClassLevels();
            // for (int index = 1; index < 65; ++index)
            //   playerAtProfileIndex.SetEquipmentState((EquipmentType) index, (EquipmentState) SaveManager.LoadByte(profileIndex, "playerEquipmentLevel" + (object) index), false);
            // for (int index = 1; index < 65; ++index)
            //   playerAtProfileIndex.SetMasteryPoints((EquipmentType) index, SaveManager.LoadInt(profileIndex, "playerMasteryLevel" + (object) index), false);
            // foreach (string stageName in SaveManager.m_game.ScreenManager.stageNameList)
            // {
            //   if (dictionary.ContainsKey(stageName))
            //     playerAtProfileIndex.SetStageState(stageName, (StageState) dictionary[stageName].valByte, true, false, false);
            //   else
            //     Console.WriteLine("Could not find " + stageName + " in stage data dict.  Could not update stage state for this entry.");
            // }
        }

        //     private void PopUp()
        //     {
        //         //if (ImGui.Button(this.Selected == -1 ? "<None>" : this.options[this.Selected]))
        //         if (ImGui.Button("select"))
        //             ImGui.OpenPopup("my_select_popup");
        //
        //         if (ImGui.BeginPopup("my_select_popup"))
        //         {
        //             for (int i = 0; i < this.options.Length; i++)
        //                 if (ImGui.Selectable(this.options[i]))
        //                 {
        //                     this.OptionSelected = i;
        //                 }
        //
        //             ImGui.EndPopup();
        //         }
        //     }
        //
    }

    public class PersistenceData
    {
        public string profileField;
        public string saveField;
        public SaveManager.SimpleDataType type = SaveManager.SimpleDataType.Bool;

        public PersistenceData(string profileField, SaveManager.SimpleDataType type)
        {
            this.profileField = profileField;
            this.type = type;
        }

        public PersistenceData(string profileField, string saveField, SaveManager.SimpleDataType type)
        {
            this.profileField = profileField;
            this.saveField = saveField;
            this.type = type;
        }

        public PersistenceData(string profileField)
        {
            this.profileField = profileField;
        }

        public PersistenceData(string profileField, string saveField)
        {
            this.profileField = profileField;
            this.saveField = saveField;
        }
    }
}
