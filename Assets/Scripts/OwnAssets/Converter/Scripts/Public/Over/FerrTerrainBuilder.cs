using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProdToFerra2D.Public
{
    public class FerrTerrainBuilder
    {
        private Vector2 entrancePoint;
        private Vector2 exitPoint;
        private List<Polygon2D> polygons;
        private Ferr2D_Path ferrPathPrototype;
        private float cellWidth;
        private float cellHeight;
        private bool randomizeVertexColor;

        private bool isBuilt = false;
        public FerrTerrainBuilder(List<Polygon2D> polygons, Ferr2D_Path ferrPathPrototype, Vector2 entranceCell, Vector2 exitCell, float cellWidth, float cellHeight, bool randomizeVertexColor = false)
        {
            this.polygons = polygons;
            this.ferrPathPrototype = ferrPathPrototype;

            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;

            this.entrancePoint.x = cellWidth * entranceCell.x;
            this.entrancePoint.y = cellHeight * entranceCell.y;

            this.exitPoint.x = cellWidth * exitCell.x;
            this.exitPoint.y = cellHeight * exitCell.y;

            this.randomizeVertexColor = randomizeVertexColor;
        }

        public FerrTerrainBuilder(List<Polygon2D> polygons, Ferr2D_Path ferrPathPrototype, float cellWidth, float cellHeight)
        {
            this.polygons = polygons;
            this.ferrPathPrototype = ferrPathPrototype;

            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
        }

        public void Build()
        {
            if (!isBuilt)
            {
                polygons.ForEach((p) => p.SetCellWidthAndHeight(cellWidth, cellHeight));

                foreach (var p in polygons)
                {
                    var ferra = GameObject.Instantiate(ferrPathPrototype);
                    ferra.transform.position = new Vector3();
                    var path = ferra.GetComponent<Ferr2D_Path>();
                    var mesh = ferra.GetComponent<Ferr2DT_PathTerrain>();

                    if (randomizeVertexColor)
                        mesh.vertexColor = new Color(Random.Range(0.5f, 0.8f), Random.Range(0.5f, 0.8f), Random.Range(0.5f, 0.8f));
                    path.pathVerts.Clear();

                    foreach (var l in p.Lines)
                        path.pathVerts.Add(l.Points[0]);

                    path.UpdateColliders();
                    path.UpdateDependants(true);
                }
                isBuilt = true;
            }
        }

        public Vector2 EntrancePoint { get { return entrancePoint; } }

        public Vector2 ExitPoint { get { return exitPoint; } }
    }
}
