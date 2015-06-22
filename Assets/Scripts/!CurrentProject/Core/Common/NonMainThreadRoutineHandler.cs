using System;
using System.Collections.Generic;

namespace Pacman
{
    public abstract class NonMainThreadRoutineHandler : IDisposable
    {
        #region Static

        private static HashSet<int> allocatedIDs = new HashSet<int>();

        private static int AllocateID()
        {
            bool alreadyAllocated = false;
            int newID;
            do
            {
                newID = UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue);
                alreadyAllocated = allocatedIDs.Contains(newID);
            }
            while (alreadyAllocated);

            return newID;
        }

        #endregion
        #region Definition
        protected bool disposed;
        protected int id;

        private Action onGameFinished;
        private Action<bool> onGamePaused;
        private Action onRoundRestarted;
        #endregion
        #region Constructor
        protected NonMainThreadRoutineHandler()
        {
            id = AllocateID();
            SubscribeOnGlobalEvents();
        }
        #endregion
        #region GlobalEvents

        protected abstract void SubscribeOnGlobalEvents();

        protected void SubscribeOnGlobalEvents(Action onGameFinished, Action<bool> onGamePaused, Action onRoundRestarted)
        {
            this.onGameFinished = onGameFinished;
            this.onGamePaused = onGamePaused;
            this.onRoundRestarted = onRoundRestarted;
            GameManager.I.SubscribeOnGlobalEvents(id, onGameFinished, onGamePaused, onRoundRestarted);
        }

        protected void UnsubscribeFromGlobalEvents()
        {
            GameManager.I.UnsubscribeFromGlobalEvents(id, onGameFinished, onGamePaused, onRoundRestarted);
        }

        #endregion
        #region Methods
        protected void Destroy() { Dispose(true); }

        protected abstract void OnPause(bool paused);

        #endregion
        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
            UnsubscribeFromGlobalEvents();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NonMainThreadRoutineHandler()
        {
            Dispose(false);
        }
        #endregion
    }

}
