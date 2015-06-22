using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pacman
{
    public class NonPlayerController : GlobalEventsHandler, IController, IJsonSerializable
    {
        #region Definition
        private Pawn avatar;
        private PathFinder pathFinder;
        private Pacman target;

        private List<Cell> currPath;
        private Cell destinationCell;
        private Cell currCell;
        private Cell nextCell;

        private int pathIndex;
        public AIState state;

        private float timeForNextTick;
        private float tickRate;
        private float targetingRatio;

        private float nextActionRefreshTime;
        private float timeForNextAction;
        private bool tacticalAction;

        private bool paused;
        #endregion
        #region Initialize
        public void Initialize(int spookPrototypeIndex, GameLevel level)
        {
            BaseInitialize(spookPrototypeIndex, level);
            avatar.Initialize(this);
            state = AIState.FirstWalk;

            NextAction();
        }

        public void Initialize(int spookPrototypeIndex, GameLevel level, JsonNPC jsonNPC)
        {
            BaseInitialize(spookPrototypeIndex, level);
            avatar.Initialize(this, jsonNPC.Avatar);
            state = jsonNPC.State;

            NextAction();
        }

        private void BaseInitialize(int spookPrototypeIndex, GameLevel level)
        {
            GameManager.I.OnObjectForSaveInstantiated(this);
            pathFinder = new PathFinder();
            target = FindObjectOfType<Pacman>();

            targetingRatio = GameConst.AI_TARGETING_RATIO[(int)level];
            nextActionRefreshTime = GameConst.AI_NEXT_ACTION_REFRESH_RATE[(int)level];
            timeForNextAction = nextActionRefreshTime;

            avatar = ((GameObject)GameObject.Instantiate(MapBuilder.I.spookPrototype[spookPrototypeIndex],
                GameMap.I.NPCRespawns[UnityEngine.Random.Range(0, GameMap.I.NPCRespawns.Count)],
                new Quaternion())).GetComponent<Pawn>();
        }

        #endregion
        #region GlobalEvents
        protected override void SubscribeOnGlobalEvents()
        {
            SubscribeOnGlobalEvents(Destroy, OnPaused, Destroy);
        }

        protected override void Destroy()
        {
            base.Destroy();
        }
        #endregion
        #region Methods
        #region Fucntional
        private void SetPath(List<Vector2> receivedPath)
        {
            currPath = receivedPath.ConvertAll((p) => { return GameMap.I.GetCellByPoint(p); });

            //for (int i = 0; i < receivedPath.Count - 1; i++)
            //    Debug.DrawLine(receivedPath[i], receivedPath[i + 1], Color.red, 10);

            receivedPath.Clear();
            pathIndex = 0;
            destinationCell = currPath.Last();
            currCell = currPath[pathIndex];
            nextCell = currPath[pathIndex + 1];
            Manipulate();
        }
        #endregion
        #region AI

        private void FixedUpdate()
        {
            if (!paused && !tacticalAction)
            {

                if (timeForNextAction <= 0)
                {
                    timeForNextAction = nextActionRefreshTime;
                    NextAction();
                }

                timeForNextAction -= Time.fixedDeltaTime;
            }
        }

        private void NextAction()
        {
            switch (state)
            {
                case AIState.FirstWalk:
                    tacticalAction = true;
                    GetRandomPath();
                    break;
                case AIState.Victime:
                    GetRandomPath();
                    break;
                case AIState.Hunter:
                    if (UnityEngine.Random.Range(0f, 1f) <= targetingRatio)
                        GetTacticalPath(target.CurrentCell);
                    else
                        GetRandomPath();
                    break;
                case AIState.Dead:
                    GetTacticalPath(avatar.HomeCell);
                    break;
            }
        }

        private void Manipulate()
        {
            var dr = (nextCell.Center - currCell.Center).normalized;

            if (nextCell.Type == CellType.Gap && currCell.Type == CellType.Gap)
                dr *= -1;

            avatar.ChangeDirection((int)dr.x, (int)dr.y);
        }

        private void GetTacticalPath(Cell tacticalCell)
        {
            tacticalAction = true;
            if (tacticalCell != avatar.CurrentCell)
                SetPath(pathFinder.RunFinding(avatar.CurrentCell, tacticalCell));
            else
                GetRandomPath();
        }

        private void GetRandomPath()
        {
            tacticalAction = false;
            Cell rndCell = null;
            while (rndCell == null)
            {
                rndCell = GameMap.I.GetCellByPoint(pathFinder.GetRandomPoint());
                if (rndCell == avatar.CurrentCell) rndCell = null;
            }

            SetPath(pathFinder.RunFinding(avatar.CurrentCell, rndCell));
        }

        #endregion
        #region JSon
        public IJsonObject GetJsonObject()
        {
            var npc = new JsonNPC();
            npc.State = state;
            npc.Avatar = (JsonPawn)avatar.GetJsonObject();
            return npc;
        }

        public JsonObjectType JsonType { get { return JsonObjectType.NPC; } }

        #endregion
        #region IController
        public bool IsHuman { get { return false; } }
        public void OnStateChanged(StateType stateType, int someValue)
        {
            switch (stateType)
            {
                case StateType.AffectedDrug:
                    state = AIState.Victime;
                    break;
                case StateType.DisaffectedDrug:
                    state = AIState.Hunter;
                    break;
                case StateType.Died:
                    state = AIState.Dead;
                    break;
                case StateType.Alive:
                    state = AIState.FirstWalk;
                    break;
            }
            if (pathIndex + 1 < currPath.Count) destinationCell = currPath[pathIndex + 1];
        }

        public void OnPositonChanged(Vector2 currPosition)
        {
            if (nextCell == null) return;

            if ((currPosition - nextCell.Center).magnitude <= GameConst.ACCURACY)
            {
                if (nextCell == destinationCell)
                {
                    if (state == AIState.FirstWalk) state = AIState.Hunter;
                    NextAction();
                }
                else
                {
                    pathIndex++;
                    currCell = currPath[pathIndex];
                    nextCell = currPath[pathIndex + 1];
                    avatar.T.position = currCell.Center;
                    Manipulate();
                }
            }
        }
        #endregion
        #endregion
        #region CallBacks
        public void OnPaused(bool paused)
        {
            this.paused = paused;
        }
        #endregion
    }
}
