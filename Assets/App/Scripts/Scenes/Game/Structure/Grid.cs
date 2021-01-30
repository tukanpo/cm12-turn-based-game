namespace App.Scenes.Game.Structure
{
    public class Grid
    {
        Cell[,] _cells;

        public Grid(int sizeX, int sizeY)
        {
            _cells = new Cell[sizeY, sizeX];
        }

        public Cell GetCell(int x, int y)
        {
            return _cells[y, x];
        }
    }
}
