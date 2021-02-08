using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Scenes.Game.Structure;
using App.Util;
using Cinemachine;
using UnityEngine;

namespace App.Scenes.Game
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] StatusBar _playerHealthBar;
        [SerializeField] StatusBar _playerActionPointBar;

        public PlayerUnit Player { get; private set; }

        public List<Unit> Enemies { get; } = new List<Unit>();

        public List<Unit> StaticObjects { get; } = new List<Unit>();

        GridCell[,] _cells;
        Tile _tilePrefab;
        StagePathfinding _pathfinding;

        public IEnumerator Initialize()
        {
            yield return AssetLoader.LoadFloorTilePrefab(prefab => _tilePrefab = prefab);

            InitializeGrid();
            InitializeUnits();
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
                    Player.UnitStatus.MaxActionPoint = 1;
                    Player.UnitStatus.ActionPoint = 1;
                    Player.OnUnitDied += OnUnitDied;
                    Player.SetHealthBar(_playerHealthBar);
                    Player.SetActionPointBar(_playerActionPointBar);
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
                    unit.UnitStatus.MaxActionPoint = 1;
                    unit.UnitStatus.ActionPoint = 1;
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
            _cells = new GridCell[sizeX, sizeY];
            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    var cell = new GridCell(new GridCoord(x, y));
                    cell.CreateTile(_tilePrefab, transform);
                    _cells[x, y] = cell;
                }

                yield return null;
            }

            // 経路探索用ノード配列生成
            _pathfinding.CreateGrid(_cells);
        }

        public bool IsCoordOutOfRange(GridCoord coord)
        {
            return coord.X < 0 || coord.X >= _cells.GetLength(0) || coord.Y < 0 || coord.Y >= _cells.GetLength(1);
        }

        public bool IsMovableOrAttackableCell(GridCoord coord)
        {
            if (IsCoordOutOfRange(coord))
            {
                return false;
            }
            
            var unit = _cells[coord.X, coord.Y].Unit;
            return !(!ReferenceEquals(unit, null) && unit.UnitType == Constants.UnitType.StaticObject);
        }
        
        public GridCell GetCell(GridCoord coord)
        {
            return IsCoordOutOfRange(coord) ? null : _cells[coord.X, coord.Y];
        }
        
        public AStarGrid.Result FindPath(GridCell start, GridCell goal)
        {
            return _pathfinding.FindPath(start, goal);
        }
        
        void InitializeGrid()
        {
            // 掃除する
            if (_cells != null)
            {
                for (var x = 0; x < _cells.GetLength(0); x++)
                {
                    for (var y = 0; y < _cells.GetLength(1); y++)
                    {
                        Destroy(_cells[x, y].Tile.gameObject);
                    }
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
