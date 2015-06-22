using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Pacman
{
    public class DrugAffector : NonMainThreadRoutineHandler, IJsonSerializable
    {
        #region Static

        private static DrugAffector _i;

        public static DrugAffector I
        {
            get
            {
                return _i;
            }
        }

        public static void Initialize()
        {
            _i = new DrugAffector();
        }

        public static void Initialize(JsonDrugAffect drugAffect)
        {
            _i = new DrugAffector();
            _i.isAcive = drugAffect.IsAcitve;
            if (drugAffect.LifeTime > 0) _i.IncreaseAffectTime(drugAffect.LifeTime);
        }
        #endregion
        #region Definition
        private int lifeTime;
        private int tickRate = GameConst.DEFAULT_SLEEP_INTERVAL;
        private bool isAcive;
        private bool started;

        private List<Pawn> targets;
        private Thread tickThread;
        private SyncBeahavior syncBeahavior;
        #endregion
        #region Constructor
        private DrugAffector()
            : base()
        {
            targets = GameObject.FindObjectsOfType<Pawn>().ToList();
            syncBeahavior = SyncBeahavior.I;
        }
        #endregion
        #region GlobalEvents

        protected override void SubscribeOnGlobalEvents()
        {
            SubscribeOnGlobalEvents(Destroy,OnPause,Destroy);
        }

        #endregion
        #region Functional
        public void Cast()
        {
            IncreaseAffectTime(GameConst.DRUG_DURATION_MS);
        }

        private void IncreaseAffectTime(int affectTime)
        {
            lifeTime += affectTime;
            if (tickThread != null) tickThread.Abort();
            RunThread();
        }

        private void RunThread()
        {
            started = true;
            tickThread = new Thread(Tick);
            tickThread.Start();
        }

        private void Affect(bool affected)
        {
            syncBeahavior.AddCallForSyncronize(() => { targets.ForEach((t) => { t.OnDrugAffected(affected); }); });
        }

        private void Tick()
        {
            do
            {
                if (started)
                {
                    started = false;
                    isAcive = true;
                    Affect(true);
                }

                if (lifeTime <= 0)
                {
                    isAcive = false;
                    Affect(false);
                    tickThread.Abort();
                }

                if (isAcive) Thread.Sleep(tickRate);
                
                lifeTime -= tickRate;
            }
            while (isAcive);
        }
        #endregion
        #region Json

        public IJsonObject GetJsonObject()
        {
            JsonDrugAffect drugAffect = new JsonDrugAffect();
            drugAffect.IsAcitve = isAcive;
            drugAffect.LifeTime = lifeTime;
            return drugAffect;
        }

        public JsonObjectType JsonType { get { return JsonObjectType.DrugAffect; } }

        #endregion
        #region NonMainThreadRoutineHandler
        protected override void OnPause(bool paused)
        {
            if (paused)
            {
                if (tickThread != null) tickThread.Abort();
            }
            else
                RunThread();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _i = null;
                if (tickThread != null) tickThread.Abort();
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
