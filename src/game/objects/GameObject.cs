using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Objects
{
    public abstract class GameObject
    {
        public readonly int ID;
        public readonly string Name;
        public readonly DrawData DrawData;

        public Texture2D Texture => DrawData.Texture;
        public Color Color => DrawData.Color;

        public GameObject(string name, DrawData drawData, int id)
        {
            ID = id;
            Name = name;
            DrawData = drawData;
        }
    }
}
