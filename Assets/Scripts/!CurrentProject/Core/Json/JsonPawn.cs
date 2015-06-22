
namespace Pacman
{
    public struct JsonPawn : IJsonObject
    {
        public bool Alive;
        public bool HasDrugAffect;
        public int KillFactor;
        public JsonVector Position;
        public JsonVector Direction;
        public JsonVector HomePoint;
    }
}
