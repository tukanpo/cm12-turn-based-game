using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace App.Scenes.Game
{
    public class UnitsManager : MonoBehaviour
    {
        [SerializeField] Unit _playerPrefab;
        [SerializeField] Unit _enemyPrefab;

        public Unit Player { get; private set; }
        public List<Unit> Enemies { get; private set; } = new List<Unit>();

        public void CreatePlayer(GridCoord coord, Constants.CardinalDirection direction)
        {
            Player = Unit.Spawn(_playerPrefab, transform, Stage.Instance.GetTile(coord), direction);
        }

        public void CreateEnemy(GridCoord coord, Constants.CardinalDirection direction)
        {
            var enemy = Unit.Spawn(_enemyPrefab, transform, Stage.Instance.GetTile(coord), direction);
            Enemies.Add(enemy);
        }

        public void SetPlayerCamera(CinemachineVirtualCamera vcam)
        {
            var playerTransform = Player.transform;
            vcam.Follow = playerTransform;
            vcam.LookAt = playerTransform;
        }
    }
}
