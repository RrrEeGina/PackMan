using UnityEngine;

namespace Pacman
{
    public struct JsonVector : IJsonObject
    {
        public static implicit operator Vector2(JsonVector jsonVecotor)
        {
            return new Vector2(jsonVecotor.X, jsonVecotor.Y);
        }

        public static implicit operator Vector3(JsonVector jsonVecotor)
        {
            return new Vector2(jsonVecotor.X, jsonVecotor.Y);
        }

        public float X;
        public float Y;

        public JsonVector(Vector2 vector)
        {
            this.X = vector.x;
            this.Y = vector.y;
        }
    }
}
