
namespace Pacman
{

    public class Ferr2DObject : GlobalEventsHandler
    {
        protected override void SubscribeOnGlobalEvents()
        {
            SubscribeOnGlobalEvents(Destroy, null, null);
        }
    }
}
