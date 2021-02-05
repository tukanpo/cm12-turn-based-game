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

            // yield return Turn(destinationCell.Tile.transform, 5f);
            
            var speed = UnitType == Constants.UnitType.Player ? 3.5f : 6f;
            yield return MoveOverSpeed(destinationCell.Tile.transform.position, speed, 0.1f);

            // var seconds = UnitType == Constants.UnitType.Player ? 1f : 0.5f;
            // yield return MoveOverSeconds(destinationCell.Tile.transform.position, seconds);

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

        IEnumerator MoveOverSpeed(Vector3 destination, float speed, float waitAfter)
        {
            // transform.LookAt(destination);

            while (Vector3.Distance(transform.position, destination) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }
        
            yield return new WaitForSeconds(waitAfter);
        }
        
        IEnumerator MoveOverSeconds(Vector3 destination, float seconds)
        {
            float elapsedTime = 0;
            while (elapsedTime < seconds)
            {
                var position = transform.position;
                var time = Vector3.Distance(position, destination) / (seconds - elapsedTime) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(position, destination, time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // IEnumerator Turn(Transform target, float seconds)
        // {
        //     var relativePos = target.position - transform.position;
        //     relativePos.y = 0;
        //
        //     var lookRotation = Quaternion.LookRotation(relativePos);
        //
        //     float elapsedTime = 0;
        //     while (elapsedTime < seconds)
        //     {
        //         transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, elapsedTime / seconds);
        //         elapsedTime += Time.deltaTime;
        //         yield return null;
        //     }
        //
        //     Debug.Log("Turn End");
        // }
        //
        // IEnumerator Turn2(Transform target, float speed)
        // {
        //     var diff = target.position - transform.position;
        //     var angle = Vector3.Angle(target.forward, diff);
        //     diff.y = 0;
        //
        //     var lookRotation = Quaternion.LookRotation(diff);
        //
        //     while (Math.Abs(angle - Vector3.Angle(transform.forward, diff)) > float.Epsilon)
        //     {
        //         transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, speed * Time.deltaTime);
        //         yield return null;
        //     }
        // }

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
