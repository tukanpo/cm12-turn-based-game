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

        int _unitId;
        
        public void Initialize()
        {
            _unitId = 0;
        }

        public void CreatePlayer(GridCell cell, Constants.CardinalDirection direction)
        {
            Player = Unit.Spawn(
                _unitId++,
                Constants.UnitType.Player,
                _playerPrefab, transform, cell, direction);
            Player.UnitStatus.Health = 1;
            Player.UnitStatus.ActionPoint = 1;
            Player.OnUnitDied += OnUnitDied;
        }

        public void CreateEnemy(GridCell cell, Constants.CardinalDirection direction)
        {
            var unit = Unit.Spawn(
                _unitId++,
                Constants.UnitType.Enemy,
                _enemyPrefab, transform, cell, direction);
            unit.UnitStatus.Health = 1;
            unit.UnitStatus.ActionPoint = 1;
            unit.OnUnitDied += OnUnitDied;
            Enemies.Add(unit);
        }

        public void CreateWall(GridCell cell)
        {
            var unit = Unit.Spawn(
                _unitId++,
                Constants.UnitType.StaticObject,
                _wallPrefab, transform, cell, Constants.CardinalDirection.N);
            StaticObjects.Add(unit);
        }

        void OnUnitDied(Unit unit)
        {
            unit.Cell.Unit = null;
            Destroy(unit.gameObject);

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
        }

        public void SetPlayerCamera(CinemachineVirtualCamera vcam)
        {
            var playerTransform = Player.transform;
            vcam.Follow = playerTransform;
            vcam.LookAt = playerTransform;
        }
    }
}
