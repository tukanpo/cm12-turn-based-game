using System;

namespace App.Scenes.Game
{
    public class PlayerUnit : Unit
    {
        PlayerHealthBar _healthBar;

        public void SetHealthBar(PlayerHealthBar healthBar)
        {
            _healthBar = healthBar;
        }

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
