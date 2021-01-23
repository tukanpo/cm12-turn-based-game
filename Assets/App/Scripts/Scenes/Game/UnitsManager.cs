using System.Collections;
using UnityEngine;

namespace App.Scenes.Game
{
    public class UnitsManager : MonoBehaviour
    {
        [SerializeField] Unit _playerPrefab;

        public Unit Player { get; private set; } 

        public void CreatePlayer(GridCoord coord)
        {
            Player = Unit.Spawn(_playerPrefab, transform, Stage.Instance.GetTile(coord));
        }
    }
}
