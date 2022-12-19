using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Objects
{
    public abstract class GameObject : IEquatable<GameObject>
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

        public bool Equals(GameObject other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Name.Equals(other.Name) && DrawData.Equals(other.DrawData);
        }

        public sealed override bool Equals(object obj) => Equals(obj as GameObject);

        public sealed override int GetHashCode() => Name.GetHashCode() ^ DrawData.GetHashCode() ^ ID.GetHashCode();

        public static bool operator ==(GameObject a, GameObject b)
        {
            bool aNull = ReferenceEquals(a, null);
            bool bNull = ReferenceEquals(b, null);
            if (aNull && bNull)
                return true;
            if (aNull || bNull)
                return false;
            if (ReferenceEquals(a, b))
                return true;
            return a.Equals(b);
        }

        public static bool operator !=(GameObject a, GameObject b) => !(a == b);
    }
}
