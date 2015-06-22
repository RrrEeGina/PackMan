using UnityEngine;

namespace Pacman
{
    public interface IPositionListener
    {
        void OnPositonChanged(Vector2 currPosition);
    }
}
