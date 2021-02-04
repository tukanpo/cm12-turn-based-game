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

        int _unitIdCount;
        
        public void Initialize()
        {
            _unitIdCount = 0;

            if (!ReferenceEquals(Player, null))
            {
                Destroy(Player.gameObject);
                Player = null;
            }

            foreach (var t in Enemies)
            {
                Destroy(t.gameObject);
            }

            Enemies.Clear();

            foreach (var t in StaticObjects)
            {
                Destroy(t.gameObject);
            }

            StaticObjects.Clear();
        }

        public void CreatePlayer(GridCell cell, Constants.CardinalDirection direction)
        {
            Player = Unit.Spawn(
                _unitIdCount++,
                Constants.UnitType.Player,
                _playerPrefab, transform, cell, direction);
            Player.UnitStatus.Health = 1;
            Player.UnitStatus.ActionPoint = 1;
            Player.OnUnitDied += OnUnitDied;
        }

        public void CreateEnemy(GridCell cell, Constants.CardinalDirection direction)
        {
            var unit = Unit.Spawn(
                _unitIdCount++,
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
                _unitIdCount++,
                Constants.UnitType.StaticObject,
                _wallPrefab, transform, cell, Constants.CardinalDirection.N);
            StaticObjects.Add(unit);
        }

        void OnUnitDied(Unit unit)
        {
            if (unit.UnitType != Constants.UnitType.Enemy)
            {
                return;
            }

            unit.Cell.Unit = null;
            Destroy(unit.gameObject);
            var index = Enemies.IndexOf(unit);
            Enemies[index] = null;
        }

        public void SetPlayerCamera(CinemachineVirtualCamera vcam)
        {
            var playerTransform = Player.transform;
            vcam.Follow = playerTransform;
            vcam.LookAt = playerTransform;
        }
    }
}
