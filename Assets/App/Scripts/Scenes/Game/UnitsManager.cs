using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Scenes.Game.Structure;
using Cinemachine;
using UnityEngine;

namespace App.Scenes.Game
{
    public class UnitsManager : MonoBehaviour
    {
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

            foreach (var t in Enemies.Where(t => !ReferenceEquals(t, null)))
            {
                Destroy(t.gameObject);
            }

            Enemies.Clear();

            foreach (var t in StaticObjects.Where(t => !ReferenceEquals(t, null)))
            {
                Destroy(t.gameObject);
            }

            StaticObjects.Clear();
        }

        public IEnumerator CreatePlayer(GridCell cell, Constants.CardinalDirection direction)
        {
            yield return AssetLoader.LoadPlayerUnitPrefab(
                prefab =>
                {
                    Player = Unit.Spawn(
                        _unitIdCount++,
                        Constants.UnitType.Player,
                        prefab,
                        transform, cell, direction);
                    Player.UnitStatus.Health = 3;
                    Player.UnitStatus.ActionPoint = 1;
                    Player.OnUnitDied += OnUnitDied;
                });
        }

        public IEnumerator CreateEnemy(GridCell cell, Constants.CardinalDirection direction)
        {
            yield return AssetLoader.LoadEnemyUnitPrefab(
                prefab =>
                {
                    var unit = Unit.Spawn(
                        _unitIdCount++,
                        Constants.UnitType.Enemy,
                        prefab,
                        transform, cell, direction);
                    unit.UnitStatus.Health = 2;
                    unit.UnitStatus.ActionPoint = 1;
                    unit.OnUnitDied += OnUnitDied;
                    Enemies.Add(unit);
                });
        }

        public IEnumerator CreateWall(GridCell cell)
        {
            yield return AssetLoader.LoadWallPrefab(
                prefab =>
                {
                    var unit = Unit.Spawn(
                        _unitIdCount++,
                        Constants.UnitType.StaticObject,
                        prefab, transform, cell, Constants.CardinalDirection.N);
                    StaticObjects.Add(unit);
                });
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
