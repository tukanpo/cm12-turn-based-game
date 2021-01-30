using System.Collections;
using System.Collections.Generic;
using App.Scenes.Game.Structure;
using App.Util;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scenes.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] Image _fullScreenBoard;
        [SerializeField] Stage _stage;
        [SerializeField] UnitsManager _unitsManager;
        [SerializeField] CinemachineVirtualCamera _vcam1;

        StateMachine<GameController> _sm;

        void Awake()
        {
            _sm = new StateMachine<GameController>(this);
            _sm.AddState<StageStartState>();
            _sm.AddState<PlayerTurnState>();
            _sm.AddState<EnemyTurnState>();
        }

        void Update() => _sm.UpdateState();

        public void StartGame()
        {
            _sm.Transit<StageStartState>();
        }
        
        #region States

        class StageStartState : StateMachine<GameController>.State
        {
            public override void OnEnter()
            {
                Context._stage.CreateStageAsync(() =>
                {
                    Context._unitsManager.CreatePlayer(new GridCoord(4, 4), Constants.CardinalDirection.S);
                    Context._unitsManager.SetPlayerCamera(Context._vcam1);

                    Context._unitsManager.CreateEnemy(
                        new GridCoord(2, 2),
                        EnumUtil.Random<Constants.CardinalDirection>());
                    
                    Context._fullScreenBoard.gameObject.SetActive(false);
                    
                    StateMachine.Transit<PlayerTurnState>();
                });
            }
        }

        class PlayerTurnState : StateMachine<GameController>.State
        {
            bool _inputEnabled = true;

            public override void OnEnter()
            {
                _inputEnabled = true;
            }

            public override void OnUpdate()
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    Context.StartCoroutine(MovePlayer(Constants.CardinalDirection.N));
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    Context.StartCoroutine(MovePlayer(Constants.CardinalDirection.S));
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Context.StartCoroutine(MovePlayer(Constants.CardinalDirection.E));
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    Context.StartCoroutine(MovePlayer(Constants.CardinalDirection.W));
                }
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    StateMachine.Transit<EnemyTurnState>();
                }
            }

            IEnumerator MovePlayer(Constants.CardinalDirection direction)
            {
                if (!_inputEnabled) yield break;

                var targetCoord = Context._unitsManager.Player.Coord.GetAdjacentCoord(direction);
                if (Stage.Instance.IsTileExists(targetCoord))
                {
                    yield break;
                }
 
                _inputEnabled = false;
                
                var tile = Stage.Instance.GetTile(targetCoord);
                yield return Context._unitsManager.Player.Move(tile);

                StateMachine.Transit<EnemyTurnState>();
            }
        }

        class EnemyTurnState : StateMachine<GameController>.State
        {
            public override void OnEnter()
            {
                Context.StartCoroutine(MoveEnemies());
            }

            IEnumerator MoveEnemies()
            {
                foreach (var enemy in Context._unitsManager.Enemies)
                {
                    yield return MoveEnemy(enemy);
                }
            }

            IEnumerator MoveEnemy(Unit enemy)
            {
                
                var nodes = new AStarSearchGrid.Node[Stage.Instance.Tiles.GetLength(1), Stage.Instance.Tiles.GetLength(0)];
                for (var y = 0; y < nodes.GetLength(0); y++)
                {
                    for (var x = 0; x < nodes.GetLength(1); x++)
                    {
                        nodes[y, x] = new AStarSearchGrid.Node(x, y);
                    }
                }

                var start = nodes[enemy.Coord.Y, enemy.Coord.X];
                var goal = nodes[Context._unitsManager.Player.Coord.Y, Context._unitsManager.Player.Coord.X];

                var aStar = new AStarSearchGrid();
                if (!aStar.Search(nodes, start, goal))
                {
                    Debug.Log("経路探索 失敗");
                    StateMachine.Transit<PlayerTurnState>();
                    yield break;
                }

                var path = new List<AStarSearchGrid.Node>();
                goal.GetPath(path);

                foreach (var n in path)
                {
                    Debug.Log($"{n.X}, {n.Y}");
                }
                
                StateMachine.Transit<PlayerTurnState>();
            }
        }
        
        #endregion
    }
}
