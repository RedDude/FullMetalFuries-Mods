namespace QuickStart
{
    public class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public int Players { get; set; } = 1;
        public bool UseKeyboard { get; set; } = false;
        public string MapName { get; set; } = "Camp";

        public int Player1Class { get; set; } = 1;
        public int Player2Class { get; set; } = 2;
        public int Player3Class { get; set; } = 3;
        public int Player4Class { get; set; } = 4;

        public int Player1Profile { get; set; } = 1;
        public int Player2Profile { get; set; } = 2;
        public int Player3Profile { get; set; } = 3;
        public int Player4Profile { get; set; } = 4;
    }
}
