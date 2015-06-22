using UnityEngine;

namespace Pacman
{
    public class Spook : Pawn
    {
        #region Definition
        private Color defaultColor = new Color(1, 1, 1, 1);
        private Color drugedColor = new Color(1, 1, 1, 0.5f);
        private Color deadColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        #endregion
        #region Properties
        protected override float currentSpeed
        {
            get
            {
                if (hasDrugAffect)
                    return baseSpeed * GameConst.SPOOK_SLOW_K;
                else
                    return baseSpeed;
            }
        }
        #endregion
        #region Initialize
        protected override void AwakeInitialize()
        {
            base.AwakeInitialize();
            AwakeInitialize(GameConst.SPOOK_SIZE, GameConst.SPOOK_SCORE);
            baseSpeed = GameConst.BASE_SPOOK_SPEED * GameConst.UNITS_PER_CELL;
            dType = DynamicType.Spook;
        }
        #endregion
        #region GlobalEvents
        protected override void SubscribeOnGlobalEvents()
        {
            SubscribeOnGlobalEvents(Destroy, OnPaused, Destroy);
        }
        #endregion
        #region Methods
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (CurrentCell == homeCell && !isAlive) Alive();
        }

        protected override void ChangeSprite()
        {
            if (!isAlive) spriteRender.color = deadColor;
            else if (hasDrugAffect) spriteRender.color = drugedColor;
            else spriteRender.color = defaultColor;
        }

        protected override void Died()
        {
            base.Died();
            changeSpriteRequired = true;
        }

        protected override void Alive()
        {
            base.Alive();
            changeSpriteRequired = true;
        }
        #endregion
        #region CallBacks
        protected override void OnDirectionChanged()
        {
        }

        public override void OnDrugAffected(bool affected)
        {
            if (hasDrugAffect && affected) return; 

            base.OnDrugAffected(affected);
            changeSpriteRequired = true;
        }

        protected override void OnCollisionReported(DynamicObject initiator)
        {
            if (isAlive && hasDrugAffect)
                Died();
        }
        #endregion
    }
}