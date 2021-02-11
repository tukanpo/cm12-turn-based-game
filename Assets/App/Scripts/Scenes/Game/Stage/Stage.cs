using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Util;
using Cinemachine;
using UnityEngine;

namespace App.Scenes.Game
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] StatusBar _playerHealthBar;

        public PlayerUnit Player { get; private set; }

        public List<Unit> Enemies { get; } = new List<Unit>();

        public List<Unit> StaticObjects { get; } = new List<Unit>();

        Grid _grid;
        Tile _tilePrefab;
        StagePathfinding _pathfinding;

        public IEnumerator Initialize()
        {
            yield return AssetLoader.LoadFloorTilePrefab(prefab => _tilePrefab = prefab);

            InitializeGrid();
            InitializeUnits();
        }

        public IEnumerator CreateEnemiesOnRandomTile()
        {
            for (var i = 0; i < 5; i++)
            {
                var emptyCells = _grid.Where(x => x.Unit == null).ToArray();
                var index = Random.Range(0, emptyCells.Length);
                yield return CreateEnemy(emptyCells[index],
                    EnumUtil.Random<Constants.CardinalDirection>());
            }
        }
        
        public IEnumerator CreatePlayer(GridCell cell, Constants.CardinalDirection direction)
        {
            yield return AssetLoader.LoadPlayerUnitPrefab(
                prefab =>
                {
                    Player = Unit.Spawn(
                        Constants.UnitType.Player,
                        prefab,
                        transform, cell, direction);
                    Player.UnitStatus.MaxHealth = 3;
                    Player.UnitStatus.Health = 3;
                    Player.OnUnitDied += OnUnitDied;
                    Player.SetHealthBar(_playerHealthBar);
                    Player.UpdateStatusView();
                });
        }

        public IEnumerator CreateEnemy(GridCell cell, Constants.CardinalDirection direction)
        {
            yield return AssetLoader.LoadEnemyUnitPrefab(
                prefab =>
                {
                    var unit = Unit.Spawn(
                        Constants.UnitType.Enemy,
                        prefab,
                        transform, cell, direction);
                    unit.UnitStatus.MaxHealth = 1;
                    unit.UnitStatus.Health = 1;
                    unit.OnUnitDied += OnUnitDied;
                    unit.UpdateStatusView();
                    Enemies.Add(unit);
                });
        }

        public IEnumerator CreateWall(GridCell cell)
        {
            yield return AssetLoader.LoadWallPrefab(
                prefab =>
                {
                    var unit = Unit.Spawn(
                        Constants.UnitType.StaticObject,
                        prefab, transform, cell, Constants.CardinalDirection.N);
                    StaticObjects.Add(unit);
                });
        }

        public void SetPlayerCamera(CinemachineVirtualCamera vcam)
        {
            var playerTransform = Player.transform;
            vcam.Follow = playerTransform;
            vcam.LookAt = playerTransform;
        }
        
        public IEnumerator CreateStage(int sizeX, int sizeY)
        {
            // 矩形のグリッドを生成してついでにタイルも生成
            _grid = new Grid(sizeX, sizeY);
            foreach (var cell in _grid)
            {
                cell.CreateTile(_tilePrefab, transform);
                yield return null;
            }

            // 経路探索用ノード配列生成
            _pathfinding.CreatePathfindingGrid(_grid);
        }

        public bool IsCoordOutOfRange(Vector2Int coord)
        {
            return coord.x < 0 || coord.x >= _grid.SizeX || coord.y < 0 || coord.y >= _grid.SizeY;
        }

        public bool IsMovableOrAttackableCell(Vector2Int coord)
        {
            if (IsCoordOutOfRange(coord))
            {
                return false;
            }
            
            var unit = _grid[coord.x, coord.y].Unit;
            return !(!ReferenceEquals(unit, null) && unit.UnitType == Constants.UnitType.StaticObject);
        }
        
        public GridCell GetCell(Vector2Int coord)
        {
            return IsCoordOutOfRange(coord) ? null : _grid[coord.x, coord.y];
        }
        
        public AStarGridPathfinding.Result FindPath(GridCell start, GridCell goal)
        {
            return _pathfinding.FindPath(start, goal);
        }
        
        void InitializeGrid()
        {
            // 掃除する
            if (_grid != null)
            {
                foreach (var cell in _grid)
                {
                    Destroy(cell.Tile.gameObject);
                }
            }
            
            _pathfinding = new StagePathfinding();
        }
        
        void InitializeUnits()
        {
            // 掃除
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
        
        void OnUnitDied(Unit unit)
        {
            // 今の所は敵だけ削除する
            if (unit.UnitType != Constants.UnitType.Enemy)
            {
                return;
            }

            unit.Cell.Unit = null;
            Destroy(unit.gameObject);
            var index = Enemies.IndexOf(unit);
            Enemies[index] = null;
        }
    }
}
