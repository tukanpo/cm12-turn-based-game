using System;
using System.Collections;
using App.Scenes.Game.Structure;
using UnityEngine;

namespace App.Scenes.Game
{
    public class Unit : MonoBehaviour
    {
        public int Id { get; private set; }
        
        public Constants.UnitType UnitType { get; private set; }
        
        public GridCoord Coord { get; private set; }

        public Constants.CardinalDirection Direction { get; protected set; }

        public static Unit Spawn(
            int unitId,
            Constants.UnitType unitType,
            Unit prefab,
            Transform parent,
            GridCell cell,
            Constants.CardinalDirection direction)
        {
            var unit = Instantiate(prefab, cell.Tile.transform.position, prefab.transform.rotation);
            unit.transform.parent = parent;
            unit.Coord = cell.Tile.Coord;
            unit.SetDirection(direction);
            unit.Id = unitId;
            unit.UnitType = unitType;
            cell.Unit = unit;
            
            return unit;
        }

        public void SetDirection(Constants.CardinalDirection direction)
        {
            var position = transform.position;
            Vector3 lookAtTransform;
            switch (direction)
            {
                case Constants.CardinalDirection.N:
                    lookAtTransform = new Vector3(position.x, position.y, position.z + 1);
                    break;
                case Constants.CardinalDirection.S:
                    lookAtTransform = new Vector3(position.x, position.y, position.z - 1);
                    break;
                case Constants.CardinalDirection.E:
                    lookAtTransform = new Vector3(position.x - 1, position.y, position.z);
                    break;
                case Constants.CardinalDirection.W:
                    lookAtTransform = new Vector3(position.x + 1, position.y, position.z);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            
            transform.LookAt(lookAtTransform);
            Direction = direction;
        }
        
        // TODO: ここでコマンドを受け取る

        public IEnumerator Move(GridCell destinationCell)
        {
            // 移動前のセルから参照を外して移動先セルに参照をセットする
            StageManager.Instance.GetCell(Coord).Unit = null;
            destinationCell.Unit = this;
            
            yield return SmoothMove(destinationCell.Tile.transform.position, 3.5f, 0.1f);

            Coord = destinationCell.Coord;
        }

        public IEnumerator Attack(Unit target)
        {
            transform.LookAt(target.transform);
            
            yield return target.Defence();
            
            yield return new WaitForSeconds(0.3f);
        }

        public IEnumerator Defence()
        {
            yield return Die();
        }

        public IEnumerator Die()
        {
            UnitsManager.Instance.DestroyUnit(this);
            yield break;
        }

        IEnumerator SmoothMove(Vector3 destination, float speed, float waitAfter)
        {
            transform.LookAt(destination);

            while (Vector3.Distance(transform.position, destination) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }
        
            yield return new WaitForSeconds(waitAfter);
        }
    }
}
