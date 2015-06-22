using UnityEngine;

namespace Pacman
{

    public abstract class DynamicObject : GlobalEventsHandler
    {
        #region Definition
        protected Transform t;
        protected SpriteRenderer spriteRender;
        protected Collider2D coll2D;
        protected DynamicType dType;
        protected float size;
        protected int score;
        protected bool paused;
        #endregion
        #region Properties
        public DynamicType DType { get { return dType; } }
        public GameObject GO { get { return go; } }
        public Transform T { get { return t; } }
        public int Score { get { return score; } }
        #endregion
        #region Initialize
        protected override void AwakeInitialize()
        {
            base.AwakeInitialize();
            t = transform;
            spriteRender = GetComponent<SpriteRenderer>();
            coll2D = GetComponent<Collider2D>();
        }

        protected void AwakeInitialize(float size, int score)
        {
            this.size = size;
            this.score = score;
            var spriteSize = GetComponent<SpriteRenderer>().bounds.size;
            t.localScale = new Vector2(size * GameConst.UNITS_PER_CELL / spriteSize.x, size * GameConst.UNITS_PER_CELL / spriteSize.y);
        }
        #endregion
        #region Methods

        public void ReportCollision(DynamicObject initiator)
        {
            OnCollisionReported(initiator);
        }

        protected abstract void OnCollisionReported(DynamicObject initiator);
        #endregion
    }
}
