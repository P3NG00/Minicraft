using Microsoft.Xna.Framework;

namespace MinicraftGame.Utils
{
    public static class Colors
    {
        public static Color Default => new Color(255, 255, 255);
        public static Color Background => new Color(96, 96, 96);
        public static Color BlockHighlight => new Color(128, 128, 128, 128);
        public static Color BlockHighlightAir => new Color(0, 0, 0, 64);
        public static Color Overlay => new Color(0, 0, 0, 128);
        public static Color TextBackground => new Color(0, 0, 0, 128);
        public static Color TextShadow => new Color(0, 0, 0, 64);
        public static Color TextWorldGenSetting => new Color(255, 255, 255);

        // entity
        public static Color Entity_Player => new Color(255, 0, 0);
        public static Color Entity_Projectile => new Color(255, 0, 128);
        public static Color Entity_BouncyProjectile => new Color(0, 128, 255);
        public static Color Entity_NPC => new Color(255, 128, 0);

        // ui
        public static Color UI_Bar => new Color(0, 0, 0);
        public static Color UI_BlockHit => new Color(255, 255, 255);
        public static Color UI_Life => new Color(255, 0, 0);
        public static Color UI_Pause => new Color(255, 255, 255);
        public static Color UI_Title => new Color(32, 192, 255);
        public static Color UI_TextDebug => new Color(255, 255, 255);
        public static Color UI_TextLife => new Color(255, 255, 255);
        public static Color UI_YouDied => new Color(255, 0, 0);
        public static Color SettingHighlight => new Color(144, 144, 144);

        // inventory
        public static Color HotbarBackground => new Color(0, 0, 0);
        public static Color HotbarSelected => new Color(255, 255, 255);
        public static Color HotbarSlotBackground => new Color(64, 64, 64);
        public static Color HotbarSlotText => new Color(255, 255, 255);
        public static Color HotbarSlotHighlight => new Color(64, 64, 64, 32);

        // debug
        public static Color DebugReason_AirCheck => new Color(255, 128, 0, 64);
        public static Color DebugReason_BlockInteract => new Color(255, 0, 0);
        public static Color DebugReason_RandomBlockTick => new Color(255, 0, 255, 64);
        public static Color DebugReason_CollisionCheck => new Color(0, 0, 255, 64);
        public static Color DebugReason_GrassSpreadCheck => new Color(0, 255, 0, 64);
        public static Color DebugReason_WoodCheck => new Color(255, 255, 0, 64);
        public static Color DebugReason_TNTIgnite => new Color(255, 0, 0, 64);
        public static Color Debug_CenterPoint => new Color(0, 0, 255);

        // color themes
        public static ColorTheme ThemeDefault => new ColorTheme(new Color(0, 0, 0), new Color(255, 255, 255), new Color(255, 255, 255), new Color(0, 0, 0));
        public static ColorTheme ThemeExit => new ColorTheme(new Color(64, 0, 0), new Color(255, 0, 0), new Color(224, 0, 0), new Color(0, 0, 0));
        public static ColorTheme ThemeBlue => new ColorTheme(new Color(0, 0, 0), new Color(0, 96, 128), new Color(255, 255, 255), UI_Title);
    }
}
