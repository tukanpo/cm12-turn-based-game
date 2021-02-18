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

        // TODO: 外部からのアクセス不可にする！！！！
        public Dictionary<int, Unit> Enemies { get; } = new Dictionary<int, Unit>();

        public List<Unit> StaticObjects { get; } = new List<Unit>();

        StageGrid _stageGrid;
        Tile _tilePrefab;
        PlayerUnit _playerUnitPrefab;
        EnemyUnit _enemyUnitPrefab;
        Unit _wallPrefab;
        StagePathfinding _pathfinding;

        public IEnumerator Initialize()
        {
            yield return AssetLoader.LoadFloorTilePrefab(prefab => _tilePrefab = prefab);
            yield return AssetLoader.LoadPlayerUnitPrefab(prefab => _playerUnitPrefab = prefab);
            yield return AssetLoader.LoadEnemyUnitPrefab(prefab => _enemyUnitPrefab = prefab);
            yield return AssetLoader.LoadWallPrefab(prefab => _wallPrefab = prefab);

            InitializeGrid();
            InitializeUnits();
        }
        
        public IEnumerator CreateStage(int sizeX, int sizeY)
        {
            // 矩形のグリッドを生成してついでにタイルも生成
            _stageGrid = new StageGrid(sizeX, sizeY);
            foreach (var cell in _stageGrid)
            {
                cell.CreateTile(_tilePrefab, transform);
                yield return null;
            }
            
            // お試しで壁を適当に生成
            SpawnWall(GetCell(new Vector2Int(1, 3)));
            SpawnWall(GetCell(new Vector2Int(2, 3)));
            SpawnWall(GetCell(new Vector2Int(3, 2)));
            SpawnWall(GetCell(new Vector2Int(3, 1)));
            SpawnWall(GetCell(new Vector2Int(3, 0)));
            SpawnWall(GetCell(new Vector2Int(3, 3)));
            SpawnWall(GetCell(new Vector2Int(5, 4)));

            // 経路探索用ノード配列生成
            _pathfinding.CreatePathfindingGrid(_stageGrid);
        }
        
        public void SpawnPlayer(StageCell cell)
        {
            Player = Unit.Spawn(
                Constants.UnitType.Player,
                _playerUnitPrefab,
                transform, cell, Constants.CardinalDirection.S);
            Player.UnitStatus.MaxHealth = 3;
            Player.UnitStatus.Health = 3;
            Player.UnitStatus.Damage = 2;
            Player.OnDied += OnDied;
            Player.SetHealthBar(_playerHealthBar);
            Player.UpdateStatusView();
        }

        public void SpawnEnemy(StageCell cell, Constants.CardinalDirection direction)
        {
            var unit = Unit.Spawn(
                Constants.UnitType.Enemy,
                _enemyUnitPrefab,
                transform, cell, direction);
            unit.UnitStatus.MaxHealth = 2;
            unit.UnitStatus.Health = 2;
            unit.UnitStatus.Damage = 1;
            unit.OnDied += OnDied;
            unit.UpdateStatusView();
            Enemies.Add(unit.Id, unit);
        }

        public IEnumerator SpawnEnemiesOnRandomTile()
        {
            for (var i = 0; i < 3; i++)
            {
                var emptyCells = _stageGrid.Where(x => x.Unit == null).ToArray();
                var index = Random.Range(0, emptyCells.Length);
                SpawnEnemy(emptyCells[index], EnumUtil.Random<Constants.CardinalDirection>());
                yield return null;
            }
        }
        
        public void SpawnWall(StageCell cell)
        {
            var unit = Unit.Spawn(
                Constants.UnitType.StaticObject,
                _wallPrefab, transform, cell, Constants.CardinalDirection.N);
            StaticObjects.Add(unit);
        }

        public void SetPlayerCamera(CinemachineVirtualCamera vcam)
        {
            var playerTransform = Player.transform;
            vcam.Follow = playerTransform;
            vcam.LookAt = playerTransform;
        }

        public bool IsCoordOutOfRange(Vector2Int coord)
        {
            return coord.x < 0 || coord.x >= _stageGrid.SizeX || coord.y < 0 || coord.y >= _stageGrid.SizeY;
        }

        public bool IsMovableOrAttackableCell(Vector2Int coord)
        {
            if (IsCoordOutOfRange(coord))
            {
                return false;
            }
            
            var unit = _stageGrid[coord.x, coord.y].Unit;
            return !(!ReferenceEquals(unit, null) && unit.UnitType == Constants.UnitType.StaticObject);
        }
        
        public StageCell GetCell(Vector2Int coord)
        {
            return IsCoordOutOfRange(coord) ? null : _stageGrid[coord.x, coord.y];
        }
        
        public AStarGridPathfinding.Result FindPath(StageCell start, StageCell goal)
        {
            return _pathfinding.FindPath(start, goal);
        }
        
        void InitializeGrid()
        {
            // 掃除する
            if (_stageGrid != null)
            {
                foreach (var cell in _stageGrid)
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

            foreach (var t in Enemies)
            {
                Destroy(t.Value.gameObject);
            }

            Enemies.Clear();

            foreach (var t in StaticObjects.Where(t => !ReferenceEquals(t, null)))
            {
                Destroy(t.gameObject);
            }

            StaticObjects.Clear();
        }
        
        void OnDied(Unit unit)
        {
            // 今の所は敵だけ削除する
            if (unit.UnitType != Constants.UnitType.Enemy)
            {
                return;
            }

            // これ確実に呼ばれるようにしたい
            unit.Cell.Unit = null;
            Destroy(unit.gameObject);
            Enemies.Remove(unit.Id);
        }
    }
}
