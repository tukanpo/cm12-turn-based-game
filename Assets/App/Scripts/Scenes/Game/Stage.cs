using System;
using System.Collections;
using UnityEngine;

namespace App.Scenes.Game
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] Tile _tilePrefab;

        Tile[,] _tiles;
        
        static Stage _instance;

        public static Stage Instance
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

        public void CreateStageAsync(Action onFinish)
        {
            StartCoroutine(CreateStage(onFinish));
        }

        public bool IsTileExists(GridCoord coord)
        {
            return coord.X < 0 || coord.X >= _tiles.GetLength(0) || coord.Y < 0 || coord.Y >= _tiles.GetLength(1);
        }
        
        public Tile GetTile(GridCoord coord)
        {
            return IsTileExists(coord) ? null : _tiles[coord.X, coord.Y];
        }

        void Awake() => _instance = this;

        IEnumerator CreateStage(Action onFinish)
        {
            const int sizeX = 9;
            const int sizeY = 9;
            
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
