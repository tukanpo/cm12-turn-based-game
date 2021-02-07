using UnityEngine;

namespace App.Scenes.Game
{
    public class UnitHealthBar : MonoBehaviour
    {
        [SerializeField] GameObject[] _dots;

        public void UpdateHealth(int health)
        {
            for (var i = 0; i < _dots.Length; i++)
            {
                _dots[i].SetActive(i < health);
            }
        }
    }
}
