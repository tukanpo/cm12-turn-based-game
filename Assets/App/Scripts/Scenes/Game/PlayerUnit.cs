using System;

namespace App.Scenes.Game
{
    public class PlayerUnit : Unit
    {
        StatusBar _healthBar;

        public void SetHealthBar(StatusBar healthBar)
        {
            _healthBar = healthBar;
        }

        public override void UpdateStatusView()
        {
            if (ReferenceEquals(_healthBar, null))
            {
                throw new Exception();
            }
            
            _healthBar.UpdateValue(UnitStatus.Health, UnitStatus.MaxHealth);
        }
    }
}
