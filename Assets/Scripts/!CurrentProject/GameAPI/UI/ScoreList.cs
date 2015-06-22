using UnityEngine;
using UnityEngine.UI;

namespace Pacman
{

    public class ScoreList : MonoBehaviour
    {

        private void OnEnable()
        {
            GetComponent<Text>().text = GameManager.I.GetScoreList();
        }

    }
}
