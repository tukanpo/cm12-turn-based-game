using System;
using System.Collections;
using UnityEngine;

namespace App.Scenes.Game
{
    public class Unit : MonoBehaviour
    {
        static int _latestId;
        
        [SerializeField] UnitAnimation _unitAnimation;

        [SerializeField] UnitBrain _unitBrain;
        
        // TODO: Blink 用. 要らなくなったら削除 これさっさと消したい
        [SerializeField] GameObject _body;

        
        #region ユニットの状態
        
        public int Id { get; private set; }
        
        public Constants.UnitType UnitType { get; protected set; }
        
        public UnitStatus UnitStatus { get; protected set; }
        
        // TODO: なんとかしたい
        public StageCell Cell { get; set; }
        
        public Constants.CardinalDirection Direction { get; protected set; }
        
        #endregion

        
        #region 通知したいイベント

        public event Action<Unit> OnDied;

        #endregion

        
        
        public static T Spawn<T>(
            Constants.UnitType unitType,
            T prefab,
            Transform parent,
            StageCell cell,
            Constants.CardinalDirection direction) where T : Unit
        {
            var unit = Instantiate(prefab, cell.Tile.transform.position, prefab.transform.rotation);
            unit.transform.parent = parent;
            unit.Id = _latestId++;
            unit.Cell = cell;
            unit.SetDirection(direction);
            unit.UnitType = unitType;
            unit.UnitStatus = new UnitStatus();
            cell.Unit = unit;

            return unit;
        }

        // TODO: EnemyUnit に移動
        public IEnumerator ThinkAndAction(Stage stage)
        {
            if (ReferenceEquals(_unitBrain, null))
            {
                Debug.Log("空っぽ");
                yield break;
            }
            
            yield return _unitBrain.ThinkAndAction(stage);
        }
        
        // TODO: Action としてまとめる
        // TODO: パラメータで渡ってきた Action を実行するメソッドを作る
        // TODO: まずは Move を排除！
        public IEnumerator Move(StageCell destinationCell)
        {
            // 移動前のセルから参照を外して移動先セルに参照をセットする
            Cell.Unit = null;
            Cell = null;
            destinationCell.Unit = this;

            StartCoroutine(_unitAnimation.Rotate(destinationCell.Tile.transform, 0.4f));

            var speed = UnitType == Constants.UnitType.Player ? 3.5f : 6f;
            yield return _unitAnimation.MoveOverSpeed(destinationCell.Tile.transform.position, speed);
            yield return new WaitForSeconds(0.1f);
            
            Cell = destinationCell;
        }

        public IEnumerator Attack(Unit target)
        {
            yield return _unitAnimation.Rotate(target.transform, 0.3f);
            yield return _unitAnimation.Attack(target.Cell.Tile.transform.position, 10f);
            yield return target.TakeDamage(UnitStatus.Damage);

            if (target.UnitStatus.Health <= 0)
            {
                OnDied?.Invoke(target);
            }
            
            yield return new WaitForSeconds(0.3f);
        }

        public IEnumerator RangedAttack(Unit target)
        {
            yield return _unitAnimation.Rotate(target.transform, 0.3f);
            
            // TODO: 実装
        }

        public IEnumerator TakeDamage(int damage)
        {
            UnitStatus.Health -= damage;
            UpdateStatusView();
            yield return _unitAnimation.Blink(0.3f, _body.GetComponent<Renderer>().material);
        }

        public virtual void UpdateStatusView() {}
        
        void SetDirection(Constants.CardinalDirection direction)
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
    }
}
