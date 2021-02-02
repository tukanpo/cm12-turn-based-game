using System;
using System.Collections.Generic;
using App.Scenes.Game.Structure;
using Cinemachine;
using UnityEngine;

namespace App.Scenes.Game
{
    public class UnitsManager : MonoBehaviour
    {
        [SerializeField] Unit _playerPrefab;
        [SerializeField] Unit _enemyPrefab;
        [SerializeField] Unit _wallPrefab;

        public Unit Player { get; private set; }

        public List<Unit> Enemies { get; } = new List<Unit>();

        public List<Unit> StaticObjects { get; } = new List<Unit>();

        static UnitsManager _instance;
        
        int _unitId;
        
        public static UnitsManager Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    throw new Exception();
                }

                return _instance;
            }
        }
        
        public void Initialize()
        {
            _unitId = 0;
        }

        public void CreatePlayer(GridCoord coord, Constants.CardinalDirection direction)
        {
            Player = Unit.Spawn(
                _unitId++,
                Constants.UnitType.Player,
                _playerPrefab, transform, StageManager.Instance.GetCell(coord), direction);
        }

        public void CreateEnemy(GridCoord coord, Constants.CardinalDirection direction)
        {
            var unit = Unit.Spawn(
                _unitId++,
                Constants.UnitType.Enemy,
                _enemyPrefab, transform, StageManager.Instance.GetCell(coord), direction);
            Enemies.Add(unit);
        }

        public void CreateWall(GridCoord coord)
        {
            var unit = Unit.Spawn(
                _unitId++,
                Constants.UnitType.StaticObject,
                _wallPrefab, transform, StageManager.Instance.GetCell(coord), Constants.CardinalDirection.N);
            StaticObjects.Add(unit);
        }

        public void DestroyUnit(Unit unit)
        {
            int index;
            switch (unit.UnitType)
            {
                case Constants.UnitType.Player:
                    Player = null;
                    break;
                case Constants.UnitType.Enemy:
                    index = Enemies.IndexOf(unit);
                    Enemies[index] = null;
                    break;
                case Constants.UnitType.StaticObject:
                    index = StaticObjects.IndexOf(unit);
                    StaticObjects[index] = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            StageManager.Instance.GetCell(unit.Coord).Unit = null;
            Destroy(unit.gameObject);
        }

        public void SetPlayerCamera(CinemachineVirtualCamera vcam)
        {
            var playerTransform = Player.transform;
            vcam.Follow = playerTransform;
            vcam.LookAt = playerTransform;
        }
        
        void Awake() => _instance = this;

        void OnDestroy() => _instance = null;
    }
}
