using System.Collections;

namespace App.Scenes.Game
{
    public interface ICommand
    {
        public IEnumerator Execute();
    }
}
