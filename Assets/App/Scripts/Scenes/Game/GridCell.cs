using System;
using App.Util;
using UnityEngine;

namespace App.Scenes.Game
{
    public class GridCell : AStarGridPathfinding.INodeContent
    {
        public Vector2Int Coord { get; }
        
        public Tile Tile { get; private set; }
        
        // NOTE: とりあえず
        public Unit Unit { get; set; }

        public GridCell(int x, int y)
        {
            Coord = new Vector2Int(x, y);
        }
        
        public void CreateTile(Tile tilePrefab, Transform transform)
        {
            Tile = Tile.Spawn(tilePrefab, transform, Coord);
        }

        /// <summary>
        /// 隣接するグリッド座標を取得する
        /// </summary>
        /// <param name="direction">方向</param>
        /// <returns>グリッド座標</returns>
        public Vector2Int GetAdjacentCoord(Constants.CardinalDirection direction)
        {
            var x = Coord.x;
            var y = Coord.y;
            
            switch (direction)
            {
                case Constants.CardinalDirection.N:
                    y += 1;
                    break;
                case Constants.CardinalDirection.S:
                    y -= 1;
                    break;
                case Constants.CardinalDirection.E:
                    x -= 1;
                    break;
                case Constants.CardinalDirection.W:
                    x += 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return new Vector2Int(x, y);
        }
        
        #region Implementation of AStarGrid.INodeContent
        
        public bool IsMovable()
        {
            return Unit == null;
        }

        public int GetAdditionalCost()
        {
            return 0;
        }

        #endregion
    }
}
