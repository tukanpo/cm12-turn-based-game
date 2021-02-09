using System.Collections;

namespace App.Scenes.Game
{
    public class UnitMoveCommand : ICommand
    {
        Unit _unit;
        GridCell _cell;
        
        public void SetParam(Unit unit, GridCell cell)
        {
            _unit = unit;
            _cell = cell;
        }
        
        public IEnumerator Execute()
        {
            yield return _unit.Move(_cell);
        }
    }
}
