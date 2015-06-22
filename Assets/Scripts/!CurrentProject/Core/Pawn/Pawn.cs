using System;
using UnityEngine;

namespace Pacman
{
    public abstract class Pawn : DynamicObject, ISerializable
    {
        #region Definition
        protected bool isAlive;

        protected Cell homeCell;
        protected Cell[] currentCells;

        protected Vector2 direction;
        protected Vector2 waitingDirection;
        protected Vector2 lastDirection;

        protected Vector2 currPosition;
        protected Vector2 lastPosition;

        protected float baseSpeed;

        protected IMovementHandler movementHandler;
        protected Action<StateType, int> EventStateChanged = delegate { };
        protected event Action<Vector2> EventPositionChanged = delegate { };

        protected bool initWithDragAffect;
        protected bool changeSpriteRequired;

        protected bool _hasDrugAffect;
        protected int _killFactor;

        #endregion
        #region Properties
        public Cell CurrentCell { get { return currentCells[0]; } }

        public Cell HomeCell { get { return homeCell; } }

        public bool HasDrugAffect { get { return hasDrugAffect; } }

        public bool IsAlive { get { return isAlive; } }

        protected bool hasDrugAffect
        {
            get
            {
                return _hasDrugAffect;
            }
            set
            {
                _hasDrugAffect = value;
                if (_hasDrugAffect)
                    EventStateChanged(StateType.AffectedDrug, 0);
                else
                    EventStateChanged(StateType.DisaffectedDrug, 0);
            }
        }
        protected int killFactor
        {
            get
            {
                return _killFactor;
            }
            set
            {
                _killFactor = value;
                EventStateChanged(StateType.KillFactor, _killFactor);
            }
        }

        protected abstract float currentSpeed { get; }
        #endregion
        #region Initialize
        public virtual void Initialize(IController controller)
        {
            currPosition = t.position;
            lastPosition = currPosition;
            homeCell = GameMap.I.GetCellByPoint(currPosition);
            isAlive = true;
            hasDrugAffect = false;
            ControllerInitialize(controller);
        }

        public virtual void Initialize(IController controller,JsonPawn jsonPawn)
        {
            t.position = jsonPawn.Position;
            currPosition = t.position;
            lastPosition = currPosition;
            direction = jsonPawn.Direction;
            homeCell = GameMap.I.GetCellByPoint(jsonPawn.HomePoint);

            isAlive = jsonPawn.Alive;
            initWithDragAffect = jsonPawn.HasDrugAffect;
            ControllerInitialize(controller);
        }

        private void ControllerInitialize(IController controller)
        {
            EventStateChanged = controller.OnStateChanged;
            if (controller.IsHuman)
                movementHandler = new PlayerMovementHandler(size, (cells) => { currentCells = cells; }, Translate);
            else
                movementHandler = new NonePlayerMovementHandler(size, (cells) => { currentCells = cells; }, Translate);

            EventPositionChanged += movementHandler.OnPositonChanged;
            EventPositionChanged += controller.OnPositonChanged;
            EventPositionChanged(currPosition);
        }
        #endregion
        #region Method
        #region Movement
        protected virtual void FixedUpdate()
        {
            if (!paused)
            {
                movementHandler.MoveRequest(ref direction, Time.fixedDeltaTime);
                if (changeSpriteRequired) ChangeSprite();

                if (initWithDragAffect)
                {
                    OnDrugAffected(true);
                    initWithDragAffect = false;
                }
            }
        }

        private void Translate(Vector2 dv, Vector2 cellAxis, bool jumpRequired)
        {
            if (!jumpRequired)
            {
                dv *= currentSpeed;
                if (cellAxis.x == 0)
                    t.position = new Vector2(t.position.x + dv.x, cellAxis.y);
                else
                    t.position = new Vector2(cellAxis.x, t.position.y + dv.y);
            }
            else
            {
                var gap = (GapCell)currentCells[1];
                t.position = gap.Center + GameConst.JUMP_OFFSET * gap.Orientation * size;
            }

            lastPosition = currPosition;
            currPosition = t.position;

            if (currPosition != lastPosition) EventPositionChanged(currPosition);
            if (direction != lastDirection) OnDirectionChanged();

            lastDirection = direction;
        }

        public void ChangeDirection(int dx, int dy)
        {
            movementHandler.ChangeDirection(ref direction, dx, dy);
        }

        protected void OnRoundRestarted()
        {
            hasDrugAffect = false;
            isAlive = true;
            t.position = homeCell.Center;
            currPosition = t.position;
            direction.x = 0;
            direction.y = 0;
            changeSpriteRequired = true;
            EventPositionChanged(currPosition);
        }
        #endregion
        #region Gameplay

        public virtual void OnDrugAffected(bool affected)
        {
            if (!isAlive) return;

            hasDrugAffect = affected;
        }

        protected virtual void Died()
        {
            hasDrugAffect = false;
            isAlive = false;
            EventStateChanged(StateType.Died, 0);
        }

        protected virtual void Alive()
        {
            isAlive = true;
            EventStateChanged(StateType.Alive, 0);
        }

        protected abstract void ChangeSprite();
        #endregion
        #region Json
        #region Json
        public JsonObjectType JsonType { get { return JsonObjectType.Pawn; } }

        public IJsonObject GetJsonObject()
        {
            var pawn = new JsonPawn();
            pawn.Position = new JsonVector(currPosition);
            pawn.Direction = new JsonVector(direction);
            pawn.HomePoint = new JsonVector(homeCell.Center);
            pawn.Alive = isAlive;
            pawn.HasDrugAffect = hasDrugAffect;
            pawn.KillFactor = killFactor;
            return pawn;
        }
        #endregion
        #endregion
        #endregion
        #region CallBacks

        protected abstract void OnDirectionChanged();

        public void OnPaused(bool paused)
        {
            this.paused = paused;
        }
        #endregion
    }
}
