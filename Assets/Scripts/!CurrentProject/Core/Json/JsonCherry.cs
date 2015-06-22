
namespace Pacman
{
    public struct JsonCherry : IJsonObject
    {
        public CherryState State;
        public float TimeForNextState;
        public int Score;
    }
}
