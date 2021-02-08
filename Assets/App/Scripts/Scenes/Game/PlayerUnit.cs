using System;

namespace App.Scenes.Game
{
    public class PlayerUnit : Unit
    {
        StatusBar _healthBar;
        StatusBar _actionPointBar;

        public void SetHealthBar(StatusBar healthBar)
        {
            _healthBar = healthBar;
        }

        public void SetActionPointBar(StatusBar actionPointBar)
        {
            _actionPointBar = actionPointBar;
        }

        public override void UpdateStatusView()
        {
            if (ReferenceEquals(_healthBar, null) || ReferenceEquals(_actionPointBar, null))
            {
                throw new Exception();
            }
            
            _healthBar.UpdateValue(UnitStatus.Health, UnitStatus.MaxHealth);
            _actionPointBar.UpdateValue(UnitStatus.ActionPoint, UnitStatus.MaxActionPoint);
        }
    }
}
