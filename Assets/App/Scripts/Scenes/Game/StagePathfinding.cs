using App.Util;

namespace App.Scenes.Game
{
    /// <summary>
    /// ステージ内経路探索
    /// </summary>
    public class StagePathfinding
    {
        AStarGridPathfinding _pathfinding;
        AStarGridPathfinding.Node[,] _aStarNodes;
        
        public void CreatePathfindingGrid(Grid grid)
        {
            _pathfinding = new AStarGridPathfinding();
            _aStarNodes = new AStarGridPathfinding.Node[grid.SizeX, grid.SizeY];
            for (var x = 0; x < _aStarNodes.GetLength(0); x++)
            {
                for (var y = 0; y < _aStarNodes.GetLength(1); y++)
                {
                    _aStarNodes[x, y] = new AStarGridPathfinding.Node(x, y, grid[x, y]);
                }
            }
        }

        public AStarGridPathfinding.Result FindPath(GridCell startCell, GridCell goalCell)
        {
            var start = _aStarNodes[startCell.Coord.x, startCell.Coord.y];
            var goal = _aStarNodes[goalCell.Coord.x, goalCell.Coord.y];
            return _pathfinding.FindPath(_aStarNodes, start, goal);
        }
    }
}
