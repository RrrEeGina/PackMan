using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace Pacman
{
    public class GameManager : MonoBehaviour
    {
        #region Static

        private static GameManager _i;
        public static GameManager I { get { return _i; } }

        public const string DEFAULT_MAP_PATH = "//GameData//DefaultMap.txt";
        public const string DEFAULT_CONTETNT_PATH = "//GameData//DefaultContent.txt";
        public const string SCORE_LIST_PATH = "//GameData//ScoreList.txt";
        public const string SAVED_GAME_PATH = "//GameData//SavedGame.txt";
        #endregion
        #region UNITY_FIELDS
        public MenuManager menuManager;
        public ResultMenu resultMenu;
        public Player playerPrototype;
        public PlayerPanel playerPanel;
        #endregion
        #region Definition
        private bool paused;
        private bool isGamePlaying;

        private GameLevel level;
        private MenuType currentMenu;

        private List<Top> topPlayers;

        private int currentScore;
        private string currentName;

        #region ObjectForSave

        private GameMap map;
        private Player player;
        private List<NonPlayerController> npcList = new List<NonPlayerController>();

        #endregion
        #endregion
        #region GlobalEvents

        private Dictionary<int, Action> DictionaryGameFinished = new Dictionary<int, Action>();
        private Dictionary<int, Action<bool>> DictionaryGamePaused = new Dictionary<int, Action<bool>>();
        private Dictionary<int, Action> DictionaryRoundRestarted = new Dictionary<int, Action>();

        public void SubscribeOnGlobalEvents(int subID, Action onGameFinished, Action<bool> onGamePaused, Action onRoundRestarted)
        {
            if (onGameFinished != null) DictionaryGameFinished[subID] = onGameFinished;
            if (onGamePaused != null) DictionaryGamePaused[subID] = onGamePaused;
            if (onRoundRestarted != null) DictionaryRoundRestarted[subID] = onRoundRestarted;
        }

        public void UnsubscribeFromGlobalEvents(int subID, Action onGameFinished, Action<bool> onGamePaused, Action onRoundRestarted)
        {
            if (onGameFinished != null) DictionaryGameFinished.Remove(subID);
            if (onGamePaused != null) DictionaryGamePaused.Remove(subID);
            if (onRoundRestarted != null) DictionaryRoundRestarted.Remove(subID);
        }

        #endregion
        #region CallGlobalEvents

        private void DestroyGame()
        {
            isGamePlaying = false;
            new List<Action>(DictionaryGameFinished.Values).ForEach((call) => { call(); });
        }


        public void PauseGame()
        {
            paused = !paused;
            new List<Action<bool>>(DictionaryGamePaused.Values).ForEach((call) => { call(paused); });
        }


        public void RestartRound()
        {
            new List<Action>(DictionaryRoundRestarted.Values).ForEach((call) => { call(); });
            MapBuilder.I.RestartMap();
        }

        #endregion
        #region MenuEvents

        public void OnMenuEventRecieved(MenuType menuType, MenuEventType eventType)
        {
            switch (eventType)
            {
                //Start Menu
                case MenuEventType.SinglePlayerMenu:
                    OpenMenu(MenuType.SinglePlayerMenu);
                    break;
                case MenuEventType.ScoreMenu:
                    OpenMenu(MenuType.ScoreMenu);
                    break;

                //Single Player Menu
                case MenuEventType.NewGame:
                    if (isGamePlaying) DestroyGame();
                    OpenMenu(MenuType.LevelMenu);
                    break;

                case MenuEventType.SaveGame:
                    if (isGamePlaying) SaveGame();
                    break;

                case MenuEventType.LoadGame:
                    var savedGame = LoadGame();
                    if (savedGame != null)
                    {
                        if (isGamePlaying) DestroyGame();
                        OpenMenu(MenuType.PlayMenu);
                        StartGame_Loading(savedGame);
                    }
                    break;

                case MenuEventType.ResumeGame:
                    if (isGamePlaying)
                    {
                        PauseGame();
                        OpenMenu(MenuType.PlayMenu);
                    }
                    break;

                //Game Level Menu
                case MenuEventType.EasyLevel:
                    OpenMenu(MenuType.PlayMenu);
                    StartGame_New(GameLevel.Easy);
                    break;
                case MenuEventType.MediumLevel:
                    OpenMenu(MenuType.PlayMenu);
                    StartGame_New(GameLevel.Medium);
                    break;
                case MenuEventType.HardLevel:
                    OpenMenu(MenuType.PlayMenu);
                    StartGame_New(GameLevel.Hard);
                    break;

                //Quit&ESC
                case MenuEventType.Quit:
                case MenuEventType.ESC_Pressed:
                    switch (menuType)
                    {
                        case MenuType.StartMenu:
                            Application.Quit();
                            break;
                        case MenuType.PlayMenu:
                            PauseGame();
                            OpenMenu(MenuType.SinglePlayerMenu);
                            break;
                        case MenuType.LevelMenu:
                            OpenMenu(MenuType.SinglePlayerMenu);
                            break;
                        case MenuType.SinglePlayerMenu:
                            if (isGamePlaying) DestroyGame();
                            OpenMenu(MenuType.StartMenu);
                            break;
                        case MenuType.ScoreMenu:
                            OpenMenu(MenuType.StartMenu);
                            break;
                    }


                    break;
            }
        }

        private void OpenMenu(MenuType menuType)
        {
            currentMenu = menuType;
            menuManager.OpenMenu(menuType);
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) OnMenuEventRecieved(currentMenu, MenuEventType.ESC_Pressed);
        }

        #endregion
        #region Initialize
        private void Awake()
        {
            _i = this;
            AwakeInitialize();
        }
        private void AwakeInitialize()
        {
            ReadScore();
        }

        private void Start()
        {
            OpenMenu(MenuType.StartMenu);
        }

        #endregion
        #region GAME_API
        private void StartGame_New(GameLevel level)
        {
            var player = SetGameStatesAndGetPlayerInstance(level);
            MapBuilder.I.BuildGameMap(player, level);
        }

        private void StartGame_Loading(JsonSavedGame savedGame)
        {
            var player = SetGameStatesAndGetPlayerInstance(savedGame.Level);
            MapBuilder.I.BuildGameMap(player, savedGame);
        }

        private void SaveGame()
        {
            JsonSavedGame savedGame = new JsonSavedGame();
            savedGame.Level = level;
            savedGame.Map = (JsonMap)map.GetJsonObject();
            savedGame.Player = (JsonPlayer)player.GetJsonObject();
            savedGame.NPCList = npcList.ConvertAll((npc) => (JsonNPC)npc.GetJsonObject());
            File.WriteAllText(Environment.CurrentDirectory + SAVED_GAME_PATH, JsonConvert.SerializeObject(savedGame, Formatting.Indented));
        }

        private JsonSavedGame LoadGame()
        {
            if (File.Exists(Environment.CurrentDirectory + SAVED_GAME_PATH))
            {
                var json = File.ReadAllText(Environment.CurrentDirectory + SAVED_GAME_PATH);
                return JsonConvert.DeserializeObject<JsonSavedGame>(json);
            }
            else
                return null;
        }

        private void FinishGame(bool defeat)
        {
            currentScore = player.Score;
            OpenMenu(MenuType.ResultMenu);
            resultMenu.Refresh(defeat, currentScore);
            DestroyGame();
        }

        #endregion
        #region Score
        private Player SetGameStatesAndGetPlayerInstance(GameLevel level)
        {
            this.level = level;
            npcList.Clear();
            paused = false;
            isGamePlaying = true;

            var player = GameObject.Instantiate(playerPrototype);
            player.SubscribeGameManagerMethods(FinishGame, RestartRound);
            playerPanel.BindPlayer(player);

            return player;
        }


        private void WriteScore()
        {
            AddTop(new Top(currentName, currentScore));
            var json = JsonConvert.SerializeObject(topPlayers, Formatting.Indented);
            File.WriteAllText(Environment.CurrentDirectory + SCORE_LIST_PATH, json);
        }

        private void ReadScore()
        {
            if (File.Exists(Environment.CurrentDirectory + SCORE_LIST_PATH))
            {
                var json = File.ReadAllText(Environment.CurrentDirectory + SCORE_LIST_PATH);
                topPlayers = JsonConvert.DeserializeObject<List<Top>>(json);
            }
            else
                File.WriteAllText(Environment.CurrentDirectory + SCORE_LIST_PATH, "");
        }

        private void AddTop(Top top)
        {

            if (topPlayers == null)
                topPlayers = new List<Top>();
            else
            {
                var minScore = topPlayers.Min(p => p.Score);
                if (topPlayers.Count > 10)
                    if (minScore > top.Score)
                        return;
                    else
                        topPlayers.RemoveAll((p) => { return p.Score == minScore; });
            }

            topPlayers.Add(top);
            topPlayers.Sort((p1, p2) => { return p2.Score - p1.Score; });
            int i = 1; topPlayers.ForEach((p) => { p.Index = i; i++; });
        }

        public string GetScoreList()
        {
            var text = "";
            if (topPlayers != null) topPlayers.ForEach((p) => { text += p.ToString() + Environment.NewLine; });
            return text;
        }
        #endregion
        #region CALLBACK
        public void OnObjectForSaveInstantiated(IJsonSerializable jsonObject)
        {
            switch (jsonObject.JsonType)
            {
                case JsonObjectType.Map:
                    map = (GameMap)jsonObject;
                    break;
                case JsonObjectType.Player:
                    player = (Player)jsonObject;
                    break;
                case JsonObjectType.NPC:
                    npcList.Add((NonPlayerController)jsonObject);
                    break;
            }
        }

        public void OnPlayerNameRecieved(string name)
        {
            currentName = name;
            OpenMenu(MenuType.SinglePlayerMenu);
            WriteScore();
        }

        void OnApplicationQuit()
        {
            DestroyGame();
        }

        #endregion
    }
}
