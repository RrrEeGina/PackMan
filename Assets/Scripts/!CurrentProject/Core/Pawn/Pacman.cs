using UnityEngine;

namespace Pacman
{
    public class Pacman : Pawn
    {
        #region UNITY_FIELDS
        public Sprite spriteBackward;
        public bool GodMod;
        #endregion
        #region Definition
        private Sprite spriteForward;

        private bool _forward = true;
        #endregion
        #region Propeties

        private bool forward
        {
            get
            {
                return _forward;
            }
            set
            {
                if (value)
                    spriteRender.sprite = spriteForward;
                else
                    spriteRender.sprite = spriteBackward;

                _forward = value;
            }
        }

        protected override float currentSpeed
        {
            get
            {
                return baseSpeed;
            }
        }

        #endregion
        #region Initialize
        protected override void AwakeInitialize()
        {
            base.AwakeInitialize();
            AwakeInitialize(GameConst.PACKMAN_SIZE, GameConst.PACMAN_SCORE);
            baseSpeed = GameConst.BASE_PACMAN_SPEED * GameConst.UNITS_PER_CELL;
            dType = DynamicType.Pacman;
            spriteForward = spriteRender.sprite;
        }

        public override void Initialize(IController controller)
        {
            base.Initialize(controller);
            killFactor = 1;
        }

        public override void Initialize(IController controller, JsonPawn jsonPawn)
        {
            base.Initialize(controller, jsonPawn);
            killFactor = jsonPawn.KillFactor;
            changeSpriteRequired = true;
        }

        #endregion
        #region GlobalEvents
        protected override void SubscribeOnGlobalEvents()
        {
            SubscribeOnGlobalEvents(Destroy, OnPaused, OnRoundRestarted);
        }
        #endregion
        #region Methods
        protected virtual void ContentInteraction(Content content)
        {
            var addScore = content.Score;
            EventStateChanged(StateType.ScoreRecieved, addScore);
            if (content.Type == ContentType.Drug) DrugAffector.I.Cast();
        }

        protected virtual void SpookInteraction(Spook spook)
        {
            if (spook.IsAlive)
                if (!spook.HasDrugAffect)
                {
                    if (!GodMod) Died();
                }
                else
                {
                    EventStateChanged(StateType.ScoreRecieved, spook.Score * killFactor);
                    killFactor++;
                }
        }

        protected virtual void PacmanInteraction(Pacman pacman)
        {
            // sorry :(
        }

        protected override void ChangeSprite()
        {
            changeSpriteRequired = false;

            float angle = 0;
            if ((direction.x == 1 || direction.x == 0) && direction.y == 0)
            {
                forward = true;
                angle = 0;
            }
            else if (direction.x == 0 && direction.y == 1)
                angle = forward ? 90 : -90;
            else if (direction.x == -1 && direction.y == 0)
            {
                forward = false;
                angle = 0;
            }
            else if (direction.x == 0 && direction.y == -1)
                angle = forward ? -90 : 90;

            t.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        protected override void Died()
        {
            base.Died();
            killFactor = 1;
        }
        #endregion
        #region CallBack
        public override void OnDrugAffected(bool affected)
        {
            base.OnDrugAffected(affected);
            if (!affected) killFactor = 1;
        }

        protected override void OnDirectionChanged()
        {
            changeSpriteRequired = true;
        }

        protected override void OnCollisionReported(DynamicObject initiator)
        {

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var dynamicObj = other.GetComponent<DynamicObject>();

            if (dynamicObj)
            {
                switch (dynamicObj.DType)
                {
                    case DynamicType.Content:
                        ContentInteraction((Content)dynamicObj);
                        break;
                    case DynamicType.Spook:
                        SpookInteraction((Spook)dynamicObj);
                        break;
                    case DynamicType.Pacman:
                        PacmanInteraction((Pacman)dynamicObj);
                        break;
                }
                dynamicObj.ReportCollision(this);
            }
        }

        #endregion
    }
}
