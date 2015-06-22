
namespace Pacman
{
    public interface IController : IPositionListener
    {
        bool IsHuman { get; }

        void OnStateChanged(StateType eventType, int someValue);
    }
}