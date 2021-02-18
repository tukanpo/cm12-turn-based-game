using System.Collections;
using App.Util;
using UnityEngine;

namespace App.Scenes.Game
{
    public class ChaseUnitAction : IUnitAction
    {
        StageCell _destinationCell;
        
        public void SetParams(StageCell destinationCell)
        {
            _destinationCell = destinationCell;
        }
        
        public IEnumerator Execute(Unit unit)
        {
            // 移動前のセルから参照を外して移動先セルに参照をセットする
            unit.Cell.Unit = null;
            unit.Cell = null;
            _destinationCell.Unit = unit;

            unit.StartCoroutine(AnimationUtil.Rotate(unit.transform, _destinationCell.Tile.transform, 0.4f));

            var speed = unit.UnitType == Constants.UnitType.Player ? 3.5f : 6f;
            yield return AnimationUtil.MoveOverSpeed(unit.transform, _destinationCell.Tile.transform.position, speed);
            yield return new WaitForSeconds(0.1f);
            
            unit.Cell = _destinationCell;

            _destinationCell = null;
            
            Cleanup();
        }

        void Cleanup()
        {
            _destinationCell = null;
        }
    }
}
