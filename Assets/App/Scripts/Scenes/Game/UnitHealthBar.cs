using UnityEngine;

namespace App.Scenes.Game
{
    public class UnitHealthBar : MonoBehaviour
    {
        [SerializeField] GameObject[] _dots;

        Camera _camera;

        public void UpdateHealth(int health)
        {
            for (var i = 0; i < _dots.Length; i++)
            {
                _dots[i].SetActive(i < health);
            }
        }
        
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
