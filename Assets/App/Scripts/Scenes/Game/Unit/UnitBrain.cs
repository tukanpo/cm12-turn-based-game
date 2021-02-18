using System.Collections;
using UnityEngine;

namespace App.Scenes.Game
{
    [RequireComponent(typeof(Unit))]
    public abstract class UnitBrain : MonoBehaviour
    {
        protected Unit Unit { get; private set; }
        
        void Awake()
        {
            Unit = GetComponent<Unit>();
        }

        public abstract IEnumerator ThinkAndAction(Stage stage);
    }
}
