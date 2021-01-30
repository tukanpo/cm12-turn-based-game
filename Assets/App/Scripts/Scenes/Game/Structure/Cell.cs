using UnityEngine;

namespace App.Scenes.Game.Structure
{
    public class Cell
    {
        public GridCoord Coord { get; }
        
        public Tile Tile { get; }

        public Cell(GridCoord coord)
        {
            Coord = coord;
        }

        public void CreateTile(Tile tilePrefab, Transform transform)
        {
            Tile.Spawn(tilePrefab, transform, Coord);
        }
    }
}
