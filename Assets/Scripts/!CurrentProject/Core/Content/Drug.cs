
namespace Pacman
{
    public class Drug : Content
    {
        protected override void AwakeInitialize()
        {
            base.AwakeInitialize();
            AwakeInitialize(GameConst.DRUG_SIZE, GameConst.DRUG_SCORE);
            type = ContentType.Drug;
        }
    }
}
