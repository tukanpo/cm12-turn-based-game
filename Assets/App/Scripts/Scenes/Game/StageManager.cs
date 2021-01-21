using System;
using System.Collections;
using UnityEngine;

namespace App.Scenes.Game
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] Tile _tilePrefab;

        Tile[,] _tiles;

        public void CreateStageAsync(Action onFinish)
        {
            StartCoroutine(CreateStage(onFinish));
        }

        IEnumerator CreateStage(Action onFinish)
        {
            const int sizeX = 10;
            const int sizeY = 10;
            
            _tiles = new Tile[sizeX, sizeY];

            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    var tile = Tile.Spawn(_tilePrefab, transform, new GridCoord(x, y));
                    _tiles[x, y] = tile;
                }

                yield return null;
            }

            onFinish();
        }
    }
}
