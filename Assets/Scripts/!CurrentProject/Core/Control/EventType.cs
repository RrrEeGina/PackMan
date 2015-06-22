
namespace Pacman
{
    public enum StateType : byte
    {
        Nothing = 0,
        ScoreRecieved = 1,
        Died = 2,
        Alive = 3,
        AffectedDrug= 4,
        DisaffectedDrug = 5,
        KillFactor = 6
    }
}
