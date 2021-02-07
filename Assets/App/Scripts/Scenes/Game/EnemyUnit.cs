using System;
using UnityEngine;

namespace App.Scenes.Game
{
    public class EnemyUnit : Unit
    {
        [SerializeField] UnitHealthBar _healthBar;

        public override void UpdateStatusView()
        {
            if (ReferenceEquals(_healthBar, null))
            {
                throw new Exception();
            }
            
            _healthBar.UpdateHealth(UnitStatus.Health);
        }
    }
}
