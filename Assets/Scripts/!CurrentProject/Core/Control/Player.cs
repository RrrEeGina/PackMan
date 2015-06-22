using System;
using UnityEngine;

namespace Pacman
{
    public class Player : GlobalEventsHandler, IController, IJsonSerializable
    {
        #region Definition
        private Pawn avatar;

        private int _score;
        private int _lifes;

        private bool paused;

        public Action<StateType, int> EventStateChanged;

        private Action<bool> GameManager_FinishGame;
        private Action GameManager_RestartRound;
        #endregion
        #region Properties
        public bool IsHuman { get { return true; } }

        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
                EventStateChanged(StateType.ScoreRecieved, _score);
            }
        }
        private int lifes
        {
            get
            {
                return _lifes;
            }
            set
            {
                _lifes = value;
                EventStateChanged(StateType.Died, _lifes);
            }
        }

        #endregion
        #region Initialized
        public void Initialize()
        {
            GameManager.I.OnObjectForSaveInstantiated(this);
            GameMap.I.SubscribeOnContentIsOver(() => { GameManager_FinishGame(false); });
            this.Score = 0;
            this.lifes = GameConst.LIFE_COUNT;

            avatar = ((GameObject)Instantiate(MapBuilder.I.pacmanPrototype, GameMap.I.SinglePlayerRespawn, new Quaternion())).GetComponent<Pawn>();
            avatar.Initialize(this);
        }

        public void Initialize(JsonPlayer json)
        {
            GameManager.I.OnObjectForSaveInstantiated(this);
            GameMap.I.SubscribeOnContentIsOver(() => { GameManager_FinishGame(false); });
            this.Score = json.Score;
            this.lifes = json.Lifes;

            avatar = ((GameObject)Instantiate(MapBuilder.I.pacmanPrototype, GameMap.I.SinglePlayerRespawn, new Quaternion())).GetComponent<Pawn>();
            avatar.Initialize(this, json.Avatar);
        }
        #endregion
        #region GlobalEvents
        protected override void SubscribeOnGlobalEvents()
        {
            SubscribeOnGlobalEvents(Destroy, OnPaused, null);
        }
        #endregion
        #region Methods
        private void FixedUpdate()
        {
            if (paused) return;

            float x = Input.GetAxis(GameConst.AXIS_X);
            float y = Input.GetAxis(GameConst.AXIS_Y);
            int dx = 0;
            int dy = 0;
            if (x * x > GameConst.SQR_INPUT_LIMIT) dx = (int)Mathf.Sign(x);
            if (y * y > GameConst.SQR_INPUT_LIMIT) dy = (int)Mathf.Sign(y);
            avatar.ChangeDirection(dx, dy);
        }

        public void SubscribeGameManagerMethods(Action<bool> FinishGame, Action RestartRound)
        {
            GameManager_FinishGame = FinishGame;
            GameManager_RestartRound = RestartRound;
        }

        #region Json

        public JsonObjectType JsonType { get { return JsonObjectType.Player; } }

        public IJsonObject GetJsonObject()
        {
            JsonPlayer player = new JsonPlayer();
            player.Lifes = lifes;
            player.Score = Score;
            player.Avatar = (JsonPawn)avatar.GetJsonObject();

            return player;
        }

        #endregion
        #region IController
        public void OnStateChanged(StateType stateType, int someValue)
        {
            switch (stateType)
            {
                case StateType.ScoreRecieved:
                    Score += someValue;
                    break;
                case StateType.Died:
                    lifes--;
                    if (lifes != 0)
                        GameManager_RestartRound();
                    else
                        GameManager_FinishGame(true);
                    break;
                case StateType.KillFactor:
                    EventStateChanged(stateType, someValue);
                    break;
            }
        }

        public void OnPositonChanged(Vector2 currPosition) { }
        #endregion
        #endregion
        #region CallBack
        public void OnPaused(bool paused)
        {
            this.paused = paused;
        }
        #endregion
    }
}
