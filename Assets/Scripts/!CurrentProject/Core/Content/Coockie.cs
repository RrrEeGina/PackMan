namespace Pacman
{
    public class Coockie : Content
    {
        protected override void AwakeInitialize()
        {
            base.AwakeInitialize();
            AwakeInitialize(GameConst.COOCKIE_SIZE, GameConst.COOCKIE_SCORE);
            type = ContentType.Coockie;
        }
    }
}