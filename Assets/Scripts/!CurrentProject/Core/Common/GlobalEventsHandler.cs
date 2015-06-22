using System;
using UnityEngine;

namespace Pacman
{
    public abstract class GlobalEventsHandler : MonoBehaviour
    {
        #region Definition
        protected GameObject go;

        private Action onGameFinished;
        private Action<bool> onGamePaused;
        private Action onRoundRestarted;
        #endregion
        #region Initialize
        private void Awake()
        {
            AwakeInitialize();
            SubscribeOnGlobalEvents();
        }

        protected virtual void AwakeInitialize()
        {
            go = gameObject;
        }
        #endregion
        #region GlobalEvents
        protected abstract void SubscribeOnGlobalEvents();

        protected virtual void Destroy()
        {
            UnsubscribeFromGlobalEvents();
            Destroy(go);
        }

        protected void SubscribeOnGlobalEvents(Action onGameFinished, Action<bool> onGamePaused, Action onRoundRestarted)
        {
            this.onGameFinished = onGameFinished;
            this.onGamePaused = onGamePaused;
            this.onRoundRestarted = onRoundRestarted;
            GameManager.I.SubscribeOnGlobalEvents(GetInstanceID(), onGameFinished, onGamePaused, onRoundRestarted);
        }

        protected void UnsubscribeFromGlobalEvents()
        {
            GameManager.I.UnsubscribeFromGlobalEvents(GetInstanceID(), onGameFinished, onGamePaused, onRoundRestarted);
        }
        #endregion
    }
}
