using UnityEngine;

namespace App.Scenes.Game
{
    public class StatusBarDot : MonoBehaviour
    {
        [SerializeField] GameObject _on;

        public void SetValue(bool isOn)
        {
            _on.SetActive(isOn);
        }
    }
}
