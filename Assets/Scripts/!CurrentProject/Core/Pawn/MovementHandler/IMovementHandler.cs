using UnityEngine;

namespace Pacman
{

    public interface IMovementHandler : IPositionListener
    {
        void MoveRequest(ref Vector2 direction, float dt);
        void ChangeDirection(ref Vector2 direction, int dx, int dy);
    }
}
