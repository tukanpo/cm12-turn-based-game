using App.Util;
using UnityEngine;

namespace App.Scenes.Game.Structure
{
    public class GridCell : AStarGrid.INodeContent
    {
        public GridCoord Coord { get; }
        
        public Tile Tile { get; private set; }
        
        // NOTE: とりあえず
        public Unit Unit { get; set; }

        public GridCell(GridCoord coord)
        {
            Coord = coord;
        }

        public void CreateTile(Tile tilePrefab, Transform transform)
        {
            Tile = Tile.Spawn(tilePrefab, transform, Coord);
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
