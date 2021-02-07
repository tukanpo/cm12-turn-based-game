using App.Scenes.Game.Structure;
using UnityEngine;

namespace App.Scenes.Game
{
    public class EnemyUnit : Unit
    {
        [SerializeField] UnitHealthBar _unitHealthBar;

        // public static Unit Spawn(
        //     int unitId,
        //     Transform parent,
        //     GridCell cell,
        //     Constants.CardinalDirection direction)
        // {
        //     AssetLoader.LoadEnemyUnitPrefab();
        //     
        //     var unit = Instantiate(_prefab, cell.Tile.transform.position, _prefab.transform.rotation);
        //     unit.transform.parent = parent;
        //     unit.Cell = cell;
        //     unit.SetDirection(direction);
        //     unit.Id = unitId;
        //     unit.UnitType = Constants.UnitType.Enemy;
        //     unit.UnitStatus = new UnitStatus();
        //     cell.Unit = unit;
        //
        //     return unit;
        // }
    }
}
