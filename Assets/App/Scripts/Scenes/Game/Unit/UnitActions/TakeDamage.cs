using System.Collections;
using App.Util;
using UnityEngine;

namespace App.Scenes.Game.UnitActions
{
    public class TakeDamage : MonoBehaviour, IUnitAction
    {
        int _damage;
        
        public void SetParams(int damage)
        {
            _damage = damage;
        }
        
        public IEnumerator Execute(Unit unit)
        {
            unit.UnitStatus.Health -= _damage;
            unit.UpdateStatusView();
            yield return AnimationUtil.Blink(0.3f, unit.Body.GetComponent<Renderer>().material);
        }
    }
}
