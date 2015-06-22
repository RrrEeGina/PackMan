
namespace Pacman
{
    public class Top
    {
        public int Index;
        public string Player;
        public int Score;

        public Top(string player, int score)
        {
            this.Player = player != "" ? player : "Unknown";
            this.Score = score;
        }

        public override string ToString()
        {
            return Index + ") " + Player + " = " + Score;
        }

    }
}
