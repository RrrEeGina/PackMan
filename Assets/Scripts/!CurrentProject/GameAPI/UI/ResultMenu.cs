using UnityEngine;
using UnityEngine.UI;

namespace Pacman
{

    public class ResultMenu : MonoBehaviour
    {
        #region Static
        private static ResultMenu i;
        public static ResultMenu I { get { return i; } }
        #endregion
        #region UNITY_FIELDS
        public GameManager gameManager;
        public Text playerName;
        public Text totalScore;
        public Text resultText;
        #endregion
        #region Methods
        private void Awake()
        {
            i = this;
        }

        public void Refresh(bool defeat, int totalScore)
        {
            resultText.text = "";
            playerName.text = "";
            this.totalScore.text = totalScore.ToString();
            if (defeat)
                resultText.text = "You won!!!";
            else
                resultText.text = "You lose!!!";
        }

        public void END_EDIT(string text)
        {
            gameManager.OnPlayerNameRecieved(playerName.text);
        }
        #endregion
    }
}
