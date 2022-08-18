using Microsoft.Xna.Framework;

namespace Minicraft.Utils
{
    public static class Colors
    {
        public static readonly Color Background = new Color(96, 96, 96);
        public static readonly Color Overlay = new Color(0, 0, 0, 128);

        // entity
        public static readonly Color Entity_Player = new Color(255, 0, 0);
        public static readonly Color Entity_NPC = new Color(255, 128, 0);

        // ui
        public static readonly Color UI_Bar = new Color(0, 0, 0);
        public static readonly Color UI_Life = new Color(255, 0, 0);
        public static readonly Color UI_Title = new Color(32, 192, 255);
        public static readonly Color UI_TextDebug = new Color(0, 0, 0);
        public static readonly Color UI_TextBlock = new Color(0, 0, 0);
        public static readonly Color UI_TextLife = new Color(255, 255, 255);

        // debug
        public static readonly Color DebugReason_BlockUpdate = new Color(255, 0, 255, 64);
        public static readonly Color DebugReason_CollisionCheck = new Color(0, 0, 255, 64);
        public static readonly Color DebugReason_AirCheck = new Color(255, 128, 0, 64);

        // main menu buttons
        public static readonly Color MainMenu_Button_World = new Color(0, 0, 0);
        public static readonly Color MainMenu_Text_World = new Color(255, 255, 255);
        public static readonly Color MainMenu_Button_World_Highlight = new Color(255, 255, 255);
        public static readonly Color MainMenu_Text_World_Highlight = new Color(0, 92, 128);
        public static readonly Color MainMenu_Button_Exit = new Color(64, 0, 0);
        public static readonly Color MainMenu_Text_Exit = new Color(255, 0, 0);
        public static readonly Color MainMenu_Button_Exit_Highlight = new Color(225, 0, 0);
        public static readonly Color MainMenu_Text_Exit_Highlight = new Color(0, 0, 0);

        // game buttons
        public static readonly Color Game_Button_Respawn = new Color(0, 0, 0);
        public static readonly Color Game_Text_Respawn = new Color(255, 255, 255);
        public static readonly Color Game_Button_Respawn_Highlight = new Color(255, 255, 255);
        public static readonly Color Game_Text_Respawn_Highlight = new Color(0, 0, 0);
        public static readonly Color Game_Button_MainMenu = new Color(64, 0, 0);
        public static readonly Color Game_Text_MainMenu = new Color(255, 0, 0);
        public static readonly Color Game_Button_MainMenu_Highlight = new Color(225, 0, 0);
        public static readonly Color Game_Text_MainMenu_Highlight = new Color(0, 0, 0);
    }
}
