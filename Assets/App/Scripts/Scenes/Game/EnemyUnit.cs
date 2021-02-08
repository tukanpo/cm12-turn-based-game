using System;
using UnityEngine;

namespace App.Scenes.Game
{
    public class EnemyUnit : Unit
    {
        [SerializeField] UnitStatusBar _statusBar;

        public override void UpdateStatusView()
        {
            if (ReferenceEquals(_statusBar, null))
            {
                throw new Exception();
            }
            
            _statusBar.UpdateValue(UnitStatus.Health, UnitStatus.MaxHealth);
        }
    }
}
