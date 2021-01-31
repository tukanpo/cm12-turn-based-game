using System;
using System.Collections;
using App.Scenes.Game.Structure;
using App.Util;
using UnityEngine;

namespace App.Scenes.Game
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] Tile _tilePrefab;

        public GridCell[,] Cells { get; private set; }
        
        // とりあえずシングルトン
        static Stage _instance;

        AStarGrid _aStarGrid;
        AStarGrid.Node[,] _aStarNodes;

        public static Stage Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    throw new Exception();
                }

                return _instance;
            }
        }

        public void CreateStageAsync(Action onFinish)
        {
            StartCoroutine(CreateStage(onFinish));
        }

        public bool IsCoordOutOfRange(GridCoord coord)
        {
            return coord.X < 0 || coord.X >= Cells.GetLength(0) || coord.Y < 0 || coord.Y >= Cells.GetLength(1);
        }
        
        public GridCell GetCell(GridCoord coord)
        {
            return IsCoordOutOfRange(coord) ? null : Cells[coord.X, coord.Y];
        }

        void Awake() => _instance = this;

        IEnumerator CreateStage(Action onFinish)
        {
            const int sizeX = 9;
            const int sizeY = 9;
            
            // 矩形のグリッドを生成してついでにタイルも生成
            Cells = new GridCell[sizeX, sizeY];
            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    var cell = new GridCell(new GridCoord(x, y));
                    cell.CreateTile(_tilePrefab, transform);
                    Cells[x, y] = cell;
                }

                yield return null;
            }
            
            // 経路探索用ノード配列生成
            _aStarGrid = new AStarGrid();
            _aStarNodes = new AStarGrid.Node[Cells.GetLength(1), Cells.GetLength(0)];
            for (var y = 0; y < _aStarNodes.GetLength(0); y++)
            {
                for (var x = 0; x < _aStarNodes.GetLength(1); x++)
                {
                    _aStarNodes[y, x] = new AStarGrid.Node(x, y, GetCell(new GridCoord(x, y)));
                }
            }

            onFinish();
        }

        public AStarGrid.Result FindPath(GridCoord startCoord, GridCoord goalCoord)
        {
            var start = _aStarNodes[startCoord.Y, startCoord.X];
            var goal = _aStarNodes[goalCoord.Y, goalCoord.X];
            return _aStarGrid.FindPath(_aStarNodes, start, goal);
        }
    }
}
