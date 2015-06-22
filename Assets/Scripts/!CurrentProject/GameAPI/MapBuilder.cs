using ProdToFerra2D.Internal;
using ProdToFerra2D.Public;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Pacman
{
    public class MapBuilder : MonoBehaviour
    {
        #region Static
        public static MapBuilder I { get { return i; } }
        private static MapBuilder i;
        #endregion
        #region UNITY_FIELDS
        public GameObject ferraPrototype;
        public GameObject pacmanPrototype;
        public NonPlayerController nonPlayerControllerPrototype;
        public GameObject[] spookPrototype;

        public GameObject coockiePrototype;
        public GameObject drugPrototype;
        public GameObject cherryPrototype;
        #endregion
        #region Definition
        private GameMap manager;

        private Player singlePlayer;

        private List<NonPlayerController> spooks = new List<NonPlayerController>();

        private GameLevel level;
        #endregion
        #region BUILDER_API

        public void BuildGameMap(Player player, GameLevel level)
        {
            this.level = level;
            BuildMap(null);
            CentreCamera();
            BuildContent(null);
            BuildPlayerAvatar(player, null);
            BuildNPC(level);
            BuildAffector(null);
        }

        public void BuildGameMap(Player player, JsonSavedGame savedGame)
        {
            this.level = savedGame.Level;
            BuildMap(savedGame.Map);
            CentreCamera();
            BuildContent(savedGame.Map);
            BuildPlayerAvatar(player, savedGame.Player);
            BuildNPC(level, savedGame.NPCList);
            BuildAffector(savedGame.Map.DrugAffect);
        }

        public void RestartMap()
        {
            BuildNPC(level);
            DrugAffector.Initialize();
        }

        #endregion
        #region Methods
        #region Functional
        private void Awake()
        {
            i = this;
        }

        private void BuildMap(JsonMap? jsonMap)
        {
            MapInterpritator mapIr = new MapInterpritator();
            var mxCell = mapIr.LoadMapFromFile(Environment.CurrentDirectory + GameManager.DEFAULT_MAP_PATH);

            ContentType[,] mxContent;
            if (jsonMap == null)
                mxContent = mapIr.LoadContentFromFile(Environment.CurrentDirectory + GameManager.DEFAULT_CONTETNT_PATH);
            else
                mxContent = mapIr.LoadContentFromStringList(jsonMap.Value.ContentLines);

            manager = new GameMap(mxCell, mxContent);
            BuildMapByFerr2D(mxCell);
        }

        private void CentreCamera()
        {
            Camera.main.transform.position = new Vector3(0.5f * manager.ColCount * GameConst.UNITS_PER_CELL, 0.5f * manager.RowCount * GameConst.UNITS_PER_CELL, GameConst.CAMERA_Z);
            Camera.main.orthographicSize = 0.5f * manager.RowCount * GameConst.UNITS_PER_CELL;
        }

        private void BuildContent(JsonMap? jsonMap)
        {
            foreach (var cell in manager)
            {
                Content content = null;
                switch (cell.Content)
                {
                    case ContentType.Coockie:
                        content = GameObject.Instantiate(coockiePrototype).GetComponent<Content>();
                        break;
                    case ContentType.Drug:
                        content = GameObject.Instantiate(drugPrototype).GetComponent<Content>();
                        break;
                    case ContentType.Cherry:
                        content = GameObject.Instantiate(cherryPrototype).GetComponent<Content>();
                        if (jsonMap.HasValue) 
                            ((Cherry)content).Initialize(jsonMap.Value.Cherry);
                        else
                            ((Cherry)content).Initialize();
                        break;
                }
                if (content)
                {
                    content.SubscribeMapManager(manager.OnContentChanged);
                    content.transform.position = cell.Center;
                }
            }


        }

        private void BuildPlayerAvatar(Player player, JsonPlayer? jsonPlayer)
        {
            singlePlayer = player;
            if (jsonPlayer == null)
                singlePlayer.Initialize();
            else
                singlePlayer.Initialize(jsonPlayer.Value);
        }

        private void BuildNPC(GameLevel level, List<JsonNPC> jsonNPC = null)
        {
            spooks.Clear();
            spooks.Add(Instantiate<NonPlayerController>(nonPlayerControllerPrototype));
            spooks.Add(Instantiate<NonPlayerController>(nonPlayerControllerPrototype));
            spooks.Add(Instantiate<NonPlayerController>(nonPlayerControllerPrototype));
            spooks.Add(Instantiate<NonPlayerController>(nonPlayerControllerPrototype));

            for (int i = 0; i < spooks.Count; i++)
                if (jsonNPC == null)
                    spooks[i].Initialize(i, level);
                else
                    spooks[i].Initialize(i, level, jsonNPC[i]);
        }

        private void BuildAffector(JsonDrugAffect? jsonDrugAffect)
        {
            if (jsonDrugAffect.HasValue)
                DrugAffector.Initialize(jsonDrugAffect.Value);
            else
                DrugAffector.Initialize();
        }
        #endregion
        #region Ferr2DBuilder
        private ProdToFerra2D.CellType Interpret(CellType pmType)
        {
            switch (pmType)
            {
                case CellType.Abyss:
                    return ProdToFerra2D.CellType.Abyss;
                case CellType.Wall:
                    return ProdToFerra2D.CellType.Wall;
                case CellType.Unknown:
                    return ProdToFerra2D.CellType.Unknown;
                default:
                    return ProdToFerra2D.CellType.Path;
            }
        }


        private void BuildMapByFerr2D(CellType[,] cells)
        {
            var mx = new ProdToFerra2D.CellType[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < mx.GetLength(0); i++)
                for (int j = 0; j < mx.GetLength(1); j++)
                    mx[i, j] = Interpret(cells[i, j]);

            CMap map = new CMap(mx);
            map.RunAnalyze();

            FerrTerrainBuilder builder = new FerrTerrainBuilder(map.FinalPolygons.ConvertAll((p) => new Polygon2D(p.mainPoints, p.lines)),
                ferraPrototype.GetComponent<Ferr2D_Path>(), GameConst.UNITS_PER_CELL, GameConst.UNITS_PER_CELL);
            builder.Build();
        }
        #endregion
        #endregion
    }
}