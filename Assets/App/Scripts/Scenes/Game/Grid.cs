using App.Util;

namespace App.Scenes.Game
{
    public class Grid : Array2d<GridCell>
    {
        public Grid(int sizeX, int sizeY) : base(sizeX, sizeY)
        {
        }
    }
}
