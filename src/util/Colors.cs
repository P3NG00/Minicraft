using Microsoft.Xna.Framework;

namespace Minicraft.Utils
{
    public static class Colors
    {
        public static readonly Color Background = new Color(96, 96, 96);
        public static readonly Color BlockHighlight = new Color(128, 128, 128, 128);
        public static readonly Color BlockHighlightAir = new Color(0, 0, 0, 64);
        public static readonly Color Overlay = new Color(0, 0, 0, 128);
        public static readonly Color TextBackground = new Color(0, 0, 0, 128);
        public static readonly Color TextShadow = new Color(0, 0, 0, 64);

        // entity
        public static readonly Color Entity_Player = new Color(255, 0, 0);
        public static readonly Color Entity_NPC = new Color(255, 128, 0);

        // ui
        public static readonly Color UI_Bar = new Color(0, 0, 0);
        public static readonly Color UI_Life = new Color(255, 0, 0);
        public static readonly Color UI_Pause = new Color(255, 255, 255);
        public static readonly Color UI_Title = new Color(32, 192, 255);
        public static readonly Color UI_TextDebug = new Color(255, 255, 255);
        public static readonly Color UI_TextBlock = new Color(255, 255, 255);
        public static readonly Color UI_TextLife = new Color(255, 255, 255);
        public static readonly Color UI_YouDied = new Color(255, 0, 0);

        // inventory
        public static readonly Color HotbarBackground = new Color(0, 0, 0);
        public static readonly Color HotbarSelected = new Color(255, 255, 255);
        public static readonly Color HotbarSlotBackground = new Color(64, 64, 64);
        public static readonly Color HotbarSlotText = new Color(255, 255, 255);

        // debug
        public static readonly Color DebugReason_BlockUpdate = new Color(255, 0, 255, 64);
        public static readonly Color DebugReason_CollisionCheck = new Color(0, 0, 255, 64);
        public static readonly Color DebugReason_AirCheck = new Color(255, 128, 0, 64);

        // color themes
        public static readonly ColorTheme ThemeDefault = new ColorTheme(new Color(0, 0, 0), new Color(255, 255, 255), new Color(255, 255, 255), new Color(0, 0, 0));
        public static readonly ColorTheme ThemeExit = new ColorTheme(new Color(64, 0, 0), new Color(255, 0, 0), new Color(224, 0, 0), new Color(0, 0, 0));
        public static readonly ColorTheme ThemeBlue = new ColorTheme(new Color(0, 0, 0), new Color(255, 255, 255), new Color(255, 255, 255), new Color(0, 96, 128));
    }
}
