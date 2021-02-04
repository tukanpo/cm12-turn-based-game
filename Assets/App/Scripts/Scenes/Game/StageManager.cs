using System;
using System.Collections;
using App.Scenes.Game.Structure;
using App.Util;
using UnityEngine;

namespace App.Scenes.Game
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] Tile _tilePrefab;

        GridCell[,] _cells;
        AStarGrid _aStarGrid;
        AStarGrid.Node[,] _aStarNodes;

        public void CreateStageAsync(Action onFinish)
        {
            StartCoroutine(CreateStage(onFinish));
        }

        public bool IsCoordOutOfRange(GridCoord coord)
        {
            return coord.X < 0 || coord.X >= _cells.GetLength(0) || coord.Y < 0 || coord.Y >= _cells.GetLength(1);
        }

        public bool IsMovableOrAttackableCell(GridCoord coord)
        {
            if (IsCoordOutOfRange(coord))
            {
                return false;
            }
            
            var unit = _cells[coord.X, coord.Y].Unit;
            return !(unit != null && unit.UnitType == Constants.UnitType.StaticObject);
        }
        
        public GridCell GetCell(GridCoord coord)
        {
            return IsCoordOutOfRange(coord) ? null : _cells[coord.X, coord.Y];
        }
        
        public AStarGrid.Result FindPath(GridCoord startCoord, GridCoord goalCoord)
        {
            var start = _aStarNodes[startCoord.Y, startCoord.X];
            var goal = _aStarNodes[goalCoord.Y, goalCoord.X];
            return _aStarGrid.FindPath(_aStarNodes, start, goal);
        }

        IEnumerator CreateStage(Action onFinish)
        {
            const int sizeX = 9;
            const int sizeY = 9;
            
            // 矩形のグリッドを生成してついでにタイルも生成
            _cells = new GridCell[sizeX, sizeY];
            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    var cell = new GridCell(new GridCoord(x, y));
                    cell.CreateTile(_tilePrefab, transform);
                    _cells[x, y] = cell;
                }

                yield return null;
            }
            
            // 経路探索用ノード配列生成
            _aStarGrid = new AStarGrid();
            _aStarNodes = new AStarGrid.Node[_cells.GetLength(1), _cells.GetLength(0)];
            for (var y = 0; y < _aStarNodes.GetLength(0); y++)
            {
                for (var x = 0; x < _aStarNodes.GetLength(1); x++)
                {
                    _aStarNodes[y, x] = new AStarGrid.Node(x, y, GetCell(new GridCoord(x, y)));
                }
            }

            onFinish();
        }
    }
}
