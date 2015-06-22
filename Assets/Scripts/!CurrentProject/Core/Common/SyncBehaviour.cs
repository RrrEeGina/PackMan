using System;
using System.Collections.Generic;

namespace Pacman
{
    public class SyncBeahavior : Singleton<SyncBeahavior>
    {
        private List<Action> waitingCalls = new List<Action>();

        private void FixedUpdate()
        {
            waitingCalls.RemoveAll((call) => { call(); return true; });
        }

        public void AddCallForSyncronize(Action call)
        {
            waitingCalls.Add(call);
        }
       
    }
}
