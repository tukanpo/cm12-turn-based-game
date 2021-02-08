using UnityEngine;

namespace App.Scenes.Game
{
    public class StatusBar : MonoBehaviour
    {
        [SerializeField] StatusBarDot[] _dots;

        public void UpdateValue(int current, int max)
        {
            for (var i = 0; i < _dots.Length; i++)
            {
                _dots[i].gameObject.SetActive(i < max);
                _dots[i].SetValue(i < current);
            }
        }
    }
}
