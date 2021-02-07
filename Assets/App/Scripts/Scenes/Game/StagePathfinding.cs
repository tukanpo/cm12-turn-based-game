using App.Scenes.Game.Structure;
using App.Util;

namespace App.Scenes.Game
{
    public class StagePathfinding
    {
        AStarGrid _aStarGrid;
        AStarGrid.Node[,] _aStarNodes;

        public void CreateGrid(GridCell[,] cells)
        {
            _aStarGrid = new AStarGrid();
            _aStarNodes = new AStarGrid.Node[cells.GetLength(0), cells.GetLength(1)];
            for (var x = 0; x < _aStarNodes.GetLength(0); x++)
            {
                for (var y = 0; y < _aStarNodes.GetLength(1); y++)
                {
                    _aStarNodes[x, y] = new AStarGrid.Node(x, y, cells[x, y]);
                }
            }
        }

        public AStarGrid.Result FindPath(GridCoord startCoord, GridCoord goalCoord)
        {
            var start = _aStarNodes[startCoord.X, startCoord.Y];
            var goal = _aStarNodes[goalCoord.X, goalCoord.Y];
            return _aStarGrid.FindPath(_aStarNodes, start, goal);
        }
    }
}
