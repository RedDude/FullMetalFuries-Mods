using System.Collections.Generic;
using Brawler2D;
using FullModdedFuriesAPI.Mods.MercenaryClass.Source;
using Microsoft.Xna.Framework;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Framework
{
    public class CustomCharacterData
    {
        public string Name;
        public Color Color;
        public bool HasInstrument;
        public PlayerClassData ClassData;
        public Dictionary<string, EquipmentData> EquipmentData;
        public Dictionary<string, SkillData> SkillData;
        public string[,] SkillTree = new string[17, 14];

        public string EquipmentDefaultA;
        public string EquipmentDefaultB;
        public string EquipmentDefaultX;
        public string EquipmentDefaultY;
    }
}
