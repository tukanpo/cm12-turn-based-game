using App.Util;

namespace App.Scenes.Game
{
    public class StageGrid : Array2d<StageCell>
    {
        public StageGrid(int sizeX, int sizeY) : base(sizeX, sizeY)
        {
        }
    }
}
