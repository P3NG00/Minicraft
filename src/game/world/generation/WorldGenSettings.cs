using System;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Worlds.Generation
{
    public sealed class WorldGenSettings : ISceneObject
    {
        // settings
        public readonly WorldGenSettingIntMult ChunkWidth = new(new(0.25f, 1f / 7f), "Chunk Width", 16, 1, World.WIDTH, 1, 2);
        public readonly WorldGenSettingInt ChunkHeightVariationRadius = new(new(0.25f, 2f / 7f), "Chunk Height Variation Radius", 32, 0, World.HEIGHT / 2, 1, 10);
        public readonly WorldGenSettingInt StoneOffset = new(new(0.25f, 3f / 7f), "Stone Offset", 32, 0, World.HEIGHT, 1, 10);
        public readonly WorldGenSettingInt SmoothScanRadius = new(new(0.25f, 4f / 7f), "Smooth Scan Radius", 32, 1, World.WIDTH, 1, 10);
        public readonly WorldGenSettingInt TreeSpacingMin = new(new(0.25f, 5f / 7f), "Tree Spacing Min", 5, 0, World.WIDTH, 1, 10);
        public readonly WorldGenSettingInt TreeHeightMin = new(new(0.25f, 6f / 7f), "Tree Height Min", 8, 0, World.HEIGHT, 1, 10);
        public readonly WorldGenSettingInt TreeHeightMax = new(new(0.75f, 1f / 7f), "Tree Height Max", 24, 0, World.HEIGHT, 1, 10);
        public readonly WorldGenSettingInt BranchLengthMax = new(new(0.75f, 2f / 7f), "Branch Length Max", 5, 0, World.WIDTH, 1, 10);
        public readonly WorldGenSettingDecimal TreeChance = new(new(0.75f, 3f / 7f), "Tree Chance", 0.2m, 0, 1, 0.01m, 0.1m);
        public readonly WorldGenSettingDecimal BranchChance = new(new(0.75f, 4f / 7f), "Branch Chance", 0.15m, 0, 1, 0.01m, 0.1m);
        public readonly WorldGenSettingDecimal CaveNoiseScale = new(new(0.75f, 5f / 7f), "Cave Noise Scale", 0.02m, 0, 1, 0.01m, 0.1m);
        public readonly WorldGenSettingDecimal CaveNoiseCutoff = new(new(0.75f, 6f / 7f), "Cave Noise Cutoff", 64, 0, 256, 1, 10);

        // settings array
        private readonly IWorldGenSetting[] _settings;

        public WorldGenSettings()
        {
            TreeHeightMin.SetOnIncrement(() => TreeHeightMax.Value = Math.Max(TreeHeightMin.Value, TreeHeightMax.Value));
            TreeHeightMax.SetOnDecrement(() => TreeHeightMin.Value = Math.Min(TreeHeightMin.Value, TreeHeightMax.Value));
            _settings = new IWorldGenSetting[]
            {
                ChunkWidth,
                ChunkHeightVariationRadius,
                StoneOffset,
                SmoothScanRadius,
                TreeSpacingMin,
                TreeHeightMin,
                TreeHeightMax,
                BranchLengthMax,
                TreeChance,
                BranchChance,
                CaveNoiseScale,
                CaveNoiseCutoff
            };
        }

        public WorldGenSettings CreateCopy()
        {
            var copy = new WorldGenSettings();
            for (int i = 0; i < copy._settings.Length; i++)
            {
                var setting = copy._settings[i];
                if (setting is WorldGenSettingInt settingInt)
                    settingInt.Value = ((WorldGenSettingInt)_settings[i]).Value;
                else if (setting is WorldGenSettingDecimal settingDecimal)
                    settingDecimal.Value = ((WorldGenSettingDecimal)_settings[i]).Value;
                else
                    throw new Exception("Unknown setting type.");
            }
            return copy;
        }

        public void Update() => _settings.ForEach(s => s.Update());

        public void Draw() => _settings.ForEach(s => s.Draw());
    }
}