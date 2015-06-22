using System;
using UnityEngine;

namespace Pacman
{

    public abstract class Content : DynamicObject
    {
        #region Definition
        protected ContentType type;
        private event Action<Content, bool> EventContentChanged = delegate { };

        public ContentType Type { get { return type; } }
        #endregion
        #region Initialize
        protected override void AwakeInitialize()
        {
            base.AwakeInitialize();
            dType = DynamicType.Content;
        }

        public void SubscribeMapManager(Action<Content, bool> onContentChanged)
        {
            EventContentChanged += onContentChanged;
        }
        #endregion
        #region GlobalEvents
        protected override void SubscribeOnGlobalEvents()
        {
            SubscribeOnGlobalEvents(Destroy, null, null);
        }
        #endregion
        #region Methods
        protected override void OnCollisionReported(DynamicObject initiator)
        {
            ChangeContent(true);
            Destroy();
        }
        protected void ChangeContent(bool isDestroyed)
        {
            EventContentChanged(this, isDestroyed);
        }
        #endregion
    }
}
