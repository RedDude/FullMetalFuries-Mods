using Brawler2D;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies
{
    public interface IEnemyControllable
    {
        // public bool CheckCanControl(CharacterObj character);
        // public float InterfaceOffset(CharacterObj character);
        //
        // public float SitOffset(CharacterObj character);
        //
        // public void CastSpellY(CharacterObj character, PlayerObj playerObj);
        public void CastSpellB(CharacterObj character, PlayerObj player);

        public void CastSpellA(CharacterObj character, PlayerObj player);

        public void CastSpellR(CharacterObj character, PlayerObj player);

        public void Attack(CharacterObj character, PlayerObj playerObj);
    }
}
