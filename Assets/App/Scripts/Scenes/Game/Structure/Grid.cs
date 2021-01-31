namespace App.Scenes.Game.Structure
{
    public class Grid
    {
        GridCell[,] _cells;

        public Grid(int sizeX, int sizeY)
        {
            _cells = new GridCell[sizeY, sizeX];
        }

        public GridCell GetCell(int x, int y)
        {
            return _cells[y, x];
        }
    }
}
