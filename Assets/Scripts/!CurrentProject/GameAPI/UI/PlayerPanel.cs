using UnityEngine;
using UnityEngine.UI;

namespace Pacman
{
    public class PlayerPanel : MonoBehaviour
    {
        #region Static
        private static PlayerPanel _i;
        public static PlayerPanel I { get { return _i; } }
        #endregion
        #region UNITY_FIELDS
        public Image life1;
        public Image life2;
        public Image life3;

        public Text scoreText;
        public Text cherryText;
        public Text killFactorText;
        #endregion
        #region Methods
        public void BindPlayer(Player player)
        {
            player.EventStateChanged = OnStateChanged;
        }

        private void Awake()
        {
            _i = this;
        }

        private void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }

        private void UpdateLifes(int lifes)
        {
            switch (lifes)
            {
                case 3:
                    life3.enabled = true;
                    life2.enabled = true;
                    life1.enabled = true;
                    break;
                case 2:
                    life3.enabled = false;
                    life2.enabled = true;
                    life1.enabled = true;
                    break;
                case 1:
                    life3.enabled = false;
                    life2.enabled = false;
                    life1.enabled = true;
                    break;
                case 0:
                    life3.enabled = false;
                    life2.enabled = false;
                    life1.enabled = false;
                    break;
            }
        }

        private void UpdateKillFactor(int factor)
        {
            killFactorText.text = "x" + factor.ToString();
        }

        private void UpdateCherry(int currentCherryScore)
        {
            cherryText.text = "+" + currentCherryScore.ToString();
        }
        #endregion
        #region Callback
        public void OnCherryAteAgain(int currentCherryScore)
        {
            UpdateCherry(currentCherryScore);
        }

        public void OnStateChanged(StateType stateType, int someValue)
        {
            switch (stateType)
            {
                case StateType.ScoreRecieved:
                    UpdateScore(someValue);
                    break;
                case StateType.Died:
                    UpdateLifes(someValue);
                    break;
                case StateType.KillFactor:
                    UpdateKillFactor(someValue);
                    break;
            }
        }

        #endregion
    }
}

