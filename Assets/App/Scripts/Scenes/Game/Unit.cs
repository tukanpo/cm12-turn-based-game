using System;
using System.Collections;
using App.Scenes.Game.Structure;
using UnityEngine;

namespace App.Scenes.Game
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] GameObject _body;
        
        public int Id { get; private set; }
        
        public Constants.UnitType UnitType { get; private set; }
        
        public UnitStatus UnitStatus { get; private set; }
        
        public GridCell Cell { get; private set; }
        
        public Constants.CardinalDirection Direction { get; protected set; }
        
        public event Action<Unit> OnUnitDied;

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
            unit.Cell = cell;
            unit.SetDirection(direction);
            unit.Id = unitId;
            unit.UnitType = unitType;
            unit.UnitStatus = new UnitStatus();
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
        
        // TODO: ここでコマンドを受け取る。ここで実行されるのはアニメーションのみにする（？）

        public IEnumerator Move(GridCell destinationCell)
        {
            // 移動前のセルから参照を外して移動先セルに参照をセットする
            Cell.Unit = null;
            Cell = null;
            destinationCell.Unit = this;

            // やっつけ
            var speed = UnitType == Constants.UnitType.Player ? 3.5f : 6f;
            
            yield return SmoothMove(destinationCell.Tile.transform.position, speed, 0.1f);

            Cell = destinationCell;
        }

        public IEnumerator Attack(Unit target)
        {
            transform.LookAt(target.transform);
            
            // TODO: ★直接呼びたくない！ 非同期にしたい
            yield return target.Defence();

            if (target.UnitStatus.Health <= 0)
            {
                OnUnitDied?.Invoke(target);
            }
            
            yield return new WaitForSeconds(0.3f);
        }

        public IEnumerator Defence()
        {
            yield return Die();
        }

        public IEnumerator Die()
        {
            yield return Blink(0.3f);
           
            UnitStatus.Health = 0;
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

        IEnumerator Blink(float duration)
        {
            var mat = _body.GetComponent<Renderer>().material;
            var originalColor = mat.color;
            
            var limit = Time.time + duration;
            while (Time.time < limit)
            {
                mat.color = new Color(1f, 1f, 0f);
                yield return new WaitForSeconds(0.1f);
                mat.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
            
            mat.color = originalColor;
        }
    }
}
