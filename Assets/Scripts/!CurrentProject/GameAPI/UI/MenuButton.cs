
namespace Pacman
{
    public class MenuButton : BaseButton
    {
        protected override void OnClick()
        {
            GameManager.I.OnMenuEventRecieved(containerType, eventType);
        }
    }
}
