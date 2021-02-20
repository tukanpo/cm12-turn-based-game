using System.Collections;
using App.Util;
using UnityEngine;

namespace App.Scenes.Game.UnitActions
{
    public class MeleeAttack : IUnitAction
    {
        Unit _target;
        
        public void SetParams(Unit target)
        {
            _target = target;
        }
        
        public IEnumerator Execute(Unit unit)
        {
            yield return AnimationUtil.Rotate(unit.transform, _target.transform, 0.3f);
            yield return AnimationUtil.Attack(unit.transform, _target.Cell.Tile.transform.position, 10f);
            yield return _target.TakeDamage(unit.UnitStatus.Damage);

            if (_target.UnitStatus.Health <= 0)
            {
                unit.OnDied?.Invoke(_target);
            }
            
            yield return new WaitForSeconds(0.3f);
            
            Cleanup();
        }

        void Cleanup()
        {
            _target = null;
        }
    }
}