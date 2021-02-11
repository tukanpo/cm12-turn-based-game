using UnityEngine;

namespace App.Scenes.Game
{
    public class UnitStatusBar : StatusBar
    {
        Camera _camera;
        
        void Start()
        {
            _camera = Camera.main;
        }
        
        void LateUpdate()
        {
            transform.rotation = _camera.transform.rotation;
        }
    }
}
