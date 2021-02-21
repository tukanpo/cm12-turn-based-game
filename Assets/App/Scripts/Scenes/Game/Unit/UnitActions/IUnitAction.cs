using System.Collections;

namespace App.Scenes.Game.UnitActions
{
    public interface IUnitAction
    {
        IEnumerator Execute(Unit unit);
    }
}
