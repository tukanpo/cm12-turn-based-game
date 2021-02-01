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

        public void CreatePlayer(GridCoord coord, Constants.CardinalDirection direction)
        {
            Player = Unit.Spawn(_playerPrefab, transform, Stage.Instance.GetCell(coord), direction);
        }

        public void CreateEnemy(GridCoord coord, Constants.CardinalDirection direction)
        {
            var unit = Unit.Spawn(_enemyPrefab, transform, Stage.Instance.GetCell(coord), direction);
            Enemies.Add(unit);
        }

        public void CreateWall(GridCoord coord)
        {
            var unit = Unit.Spawn(_wallPrefab, transform, Stage.Instance.GetCell(coord), Constants.CardinalDirection.N);
            StaticObjects.Add(unit);
        }

        public void SetPlayerCamera(CinemachineVirtualCamera vcam)
        {
            var playerTransform = Player.transform;
            vcam.Follow = playerTransform;
            vcam.LookAt = playerTransform;
        }
    }
}
