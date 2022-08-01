using Microsoft.Xna.Framework;

namespace Game.Data
{
    public static class UI
    {
        private const int UI_SPACER = 5;

        private static readonly Vector2 BarSize = new Vector2(150, 30);

        public static void Draw(Player player, World world)
        {
            // draw health bar
            var drawPos = new Vector2((Display.WindowSize.X / 2f) - (BarSize.X / 2f), Display.WindowSize.Y - BarSize.Y);
            Display.Draw(drawPos, BarSize, Colors.UI_Bar);
            // adjust size to fit within bar
            drawPos += new Vector2(UI_SPACER);
            var healthSize = BarSize - (new Vector2(UI_SPACER) * 2);
            // readjust size to display real health
            healthSize.X *= player.Life / player.MaxLife;
            Display.Draw(drawPos, healthSize, Colors.UI_Life);
            // draw health numbers on top of bar
            var healthString = $"{player.Life:0.#}/{player.MaxLife:0.#}";
            var textSize = Display.Font.MeasureString(healthString);
            drawPos = new Vector2((Display.WindowSize.X / 2f) - (textSize.X / 2f), Display.WindowSize.Y - 22);
            Display.DrawString(drawPos, healthString, Colors.UI_TextLife);
            // draw currently selected block
            drawPos = new Vector2(UI_SPACER, Display.WindowSize.Y - Display.Font.LineSpacing - UI_SPACER);
            Display.DrawString(drawPos, $"current block: {GameInfo.CurrentBlock.Name}", Colors.UI_TextBlock);
            // draw debug
            if (Debug.Enabled)
            {
                drawPos = new Vector2(UI_SPACER);
                foreach (var debugInfo in new[] {
                    $"window_size: {Display.WindowSize.X}x{Display.WindowSize.Y}",
                    $"world_size: {world.Width}x{world.Height}",
                    $"show_grid: {Display.ShowGrid}",
                    $"time: {(GameInfo.Ticks / (float)World.TICKS_PER_SECOND):0.000}",
                    $"ticks: {GameInfo.Ticks} ({World.TICKS_PER_SECOND} ticks/sec)",
                    $"frames_per_second: {GameInfo.AverageFramesPerSecond:0.000}",
                    $"ticks_per_frame: {GameInfo.AverageTicksPerFrame:0.000}",
                    $"x: {player.Position.X:0.000}",
                    $"y: {player.Position.Y:0.000}",
                    $"block_scale: {Display.BlockScale}",
                    $"mouse_x: {GameInfo.LastMouseBlock.X:0.000} ({GameInfo.LastMouseBlockInt.X})",
                    $"mouse_y: {GameInfo.LastMouseBlock.Y:0.000} ({GameInfo.LastMouseBlockInt.Y})",
                    $"player_velocity: {player.Velocity.Length() * player.MoveSpeed:0.000}",
                    $"player_grounded: {player.IsGrounded}"})
                {
                    Display.DrawString(drawPos, debugInfo, Colors.UI_TextDebug);
                    drawPos.Y += UI_SPACER + Display.Font.LineSpacing;
                }
            }
        }
    }
}
