using System.Collections;

namespace App.Scenes.Game
{
    public interface IUnitAction
    {
        IEnumerator Execute(Unit unit);
    }
}
