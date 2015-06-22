using System;
using System.Collections;
using UnityEngine;

namespace Pacman
{
    public class Cherry : Content, IJsonSerializable
    {
        #region Definition
        private Color defaultColor;
        private Color hideColor;
        private CherryState state;
        private float secondForNextState;
        private float tick;
        private Action<int> onCherryAteAgain = null;
        #endregion
        #region Initialize
        protected override void AwakeInitialize()
        {
            base.AwakeInitialize();

            AwakeInitialize(GameConst.CHERRY_SIZE, GameConst.CHERRY_SCORE);
            type = ContentType.Cherry;

            defaultColor = spriteRender.color;
            hideColor = defaultColor;
            hideColor.a = 0;

            tick = GameConst.CHERRY_COROUTINE_TICK_RATE;

            onCherryAteAgain = PlayerPanel.I.OnCherryAteAgain;
        }

        public void Initialize()
        {
            Hide();
            StartCoroutine(StateChangedCoroutine());
            onCherryAteAgain(score);
        }

        public void Initialize(JsonCherry jsonCherry)
        {
            secondForNextState = jsonCherry.TimeForNextState;
            state = jsonCherry.State;
            score = jsonCherry.Score;

            if (state == CherryState.Disable) Hide();
            StartCoroutine(StateChangedCoroutine());
            if (state == CherryState.Pulsation)
                StartCoroutine(Pulsation());

            onCherryAteAgain(score);
        }
        #endregion
        #region GlobalEvents
        protected override void SubscribeOnGlobalEvents()
        {
            SubscribeOnGlobalEvents(Destroy, OnPaused, null);
        }
        #endregion
        #region Methods
        #region Functional
        private void Hide()
        {
            state = CherryState.Disable;
            spriteRender.color = hideColor;
            coll2D.enabled = false;
            secondForNextState = GameConst.CHERRY_DISABLE_INTERVAL;
        }

        private void Show()
        {
            state = CherryState.Enable;
            spriteRender.color = defaultColor;
            coll2D.enabled = true;
            secondForNextState = GameConst.CHERRY_ENABLE_INTERVAL;
        }

        private IEnumerator StateChangedCoroutine()
        {
            while (!paused)
            {
                if (secondForNextState <= 0)
                    if (state == CherryState.Disable)
                        Show();
                    else
                        Hide();

                RunPulsation();

                yield return new WaitForSeconds(tick);
                secondForNextState -= tick;
            }
        }

        private void RunPulsation()
        {
            if (secondForNextState < GameConst.CHERRY_PULS_START && state == CherryState.Enable)
            {
                state = CherryState.Pulsation;
                StartCoroutine(Pulsation());
            }
        }

        private IEnumerator Pulsation()
        {
            while (state == CherryState.Pulsation && !paused)
            {
                if (spriteRender.color == defaultColor)
                    spriteRender.color = hideColor;
                else
                    spriteRender.color = defaultColor;
                yield return new WaitForSeconds(0.33f);
            }
        }
        #endregion
        #region Json
        public JsonObjectType JsonType { get { return JsonObjectType.Cherry; } }

        public IJsonObject GetJsonObject()
        {
            var cherry = new JsonCherry();
            cherry.State = state;
            cherry.TimeForNextState = secondForNextState;
            cherry.Score = score;
            return cherry;
        }

        #endregion
        #endregion
        #region CallBack
        public void OnPaused(bool paused)
        {
            this.paused = paused;
            if (!paused)
            {
                StartCoroutine(StateChangedCoroutine());
                if (state == CherryState.Pulsation)
                    StartCoroutine(Pulsation());
            }
        }

        public void OnGameFinished()
        {
            GameObject.Destroy(GO);
        }

        public void OnRoundRestarted()
        {
            GameObject.Destroy(GO);
        }

        protected override void OnCollisionReported(DynamicObject initiator)
        {
            Hide();
            score += GameConst.CHERRY_SCORE_INCREMENT;
            onCherryAteAgain(score);
        }
        #endregion
    }
}
