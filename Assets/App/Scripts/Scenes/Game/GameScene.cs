using UnityEngine;

namespace App.Scenes.Game
{
    public class GameScene : MonoBehaviour
    {
        [SerializeField] GameController _gameController;
        
        void Start()
        {
            _gameController.StartGame();
        }
    }
}
