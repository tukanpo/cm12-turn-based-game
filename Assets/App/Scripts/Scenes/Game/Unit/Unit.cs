using System;
using System.Collections;
using App.Scenes.Game.UnitActions;
using UnityEngine;

namespace App.Scenes.Game
{
    public class Unit : MonoBehaviour
    {
        static int _latestId;

        [SerializeField] UnitBrain _unitBrain;

        // これ IF じゃないから駄目じゃない？
        [SerializeField] TakeDamage _takeDamage;
        
        // TODO: Blink 用. 要らなくなったら削除 これさっさと消したい
        [SerializeField] GameObject _body;

        public GameObject Body => _body;
        
        
        
        #region ユニットの状態
        
        public int Id { get; private set; }
        
        public Constants.UnitType UnitType { get; protected set; }
        
        public UnitStatus UnitStatus { get; protected set; }
        
        // TODO: なんとかしたい
        public StageCell Cell { get; set; }
        
        public Constants.CardinalDirection Direction { get; protected set; }
        
        #endregion

        
        #region 通知したいイベント

        public Action<Unit> OnDied { get; set; }

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

        public IEnumerator ThinkAndAction(Stage stage)
        {
            if (ReferenceEquals(_unitBrain, null))
            {
                Debug.Log("空っぽ");
                yield break;
            }
            
            yield return _unitBrain.GetAction(stage);
        }

        public IEnumerator TakeDamage(int damage)
        {
            _takeDamage.SetParams(damage);
            yield return _takeDamage.Execute(this);
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
