using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Blur = UnityStandardAssets.ImageEffects.Blur;

namespace Pacman
{
    public class MenuManager : MonoBehaviour
    {
        #region Static
        private static MenuManager i;
        public static MenuManager I { get { return i; } }
        #endregion
        #region UNITY_FIELDS

        public GameManager gameManager;

        public GameObject startMenu;
        public GameObject singlePlayerMenu;
        public GameObject levelMenu;
        public GameObject resultMenu;
        public GameObject playMenu;
        public GameObject scoreMenu;

        private Dictionary<MenuType, GameObject> menus;

        #endregion
        #region Definition
        private GameObject _currentMenu;
        private Blur blur;
        #endregion
        #region Properties
        private GameObject currentMenu
        {
            get
            {
                return _currentMenu;
            }
            set
            {
                if (_currentMenu) _currentMenu.SetActive(false);

                if (value == playMenu)
                    blur.enabled = false;
                else
                    blur.enabled = true;

                _currentMenu = value;
                _currentMenu.SetActive(true);
            }
        }
        #endregion
        #region Methods

        private void Awake()
        {
            i = this;
            blur = Camera.main.GetComponent<Blur>();
            menus = new Dictionary<MenuType, GameObject>();
            menus[MenuType.StartMenu] = startMenu;
            menus[MenuType.SinglePlayerMenu] = singlePlayerMenu;
            menus[MenuType.LevelMenu] = levelMenu;
            menus[MenuType.ResultMenu] = resultMenu;
            menus[MenuType.PlayMenu] = playMenu;
            menus[MenuType.ScoreMenu] = scoreMenu;

        }

        public void OpenMenu(MenuType menuType)
        {
            currentMenu = menus[menuType];
        }

        #endregion
    }
}
