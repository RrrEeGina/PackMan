using UnityEngine;
using UnityEngine.UI;

namespace Pacman
{
    public abstract class BaseButton : MonoBehaviour
    {
        [SerializeField]
        protected MenuType containerType;
        [SerializeField]
        protected MenuEventType eventType;

        protected GameManager gameManager;
        protected Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(CallClick);
        }

        private void CallClick()
        {
            if (!gameManager) gameManager = GameManager.I;
            OnClick();
        }

        protected abstract void OnClick();
    }
}
