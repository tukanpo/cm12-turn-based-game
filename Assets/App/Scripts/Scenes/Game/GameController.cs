using System.Collections;
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
        [SerializeField] StageManager _stageManager;
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
                Context._unitsManager.Initialize();
                
                Context._stageManager.CreateStageAsync(() =>
                {
                    Context._fullScreenBoard.gameObject.SetActive(true);
                    
                    Context._unitsManager.CreatePlayer(new GridCoord(4, 4), Constants.CardinalDirection.S);
                    Context._unitsManager.SetPlayerCamera(Context._vcam1);

                    Context._unitsManager.CreateEnemy(
                        new GridCoord(2, 2),
                        EnumUtil.Random<Constants.CardinalDirection>());
                    Context._unitsManager.CreateEnemy(
                        new GridCoord(1, 2),
                        EnumUtil.Random<Constants.CardinalDirection>());
                    Context._unitsManager.CreateEnemy(
                        new GridCoord(7, 7),
                        EnumUtil.Random<Constants.CardinalDirection>());
                    
                    Context._unitsManager.CreateWall(new GridCoord(3, 2));
                    Context._unitsManager.CreateWall(new GridCoord(5, 4));
                    Context._unitsManager.CreateWall(new GridCoord(3, 3));
                    
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
                if (!StageManager.Instance.IsMovableOrAttackableCell(targetCoord))
                {
                    yield break;
                }
 
                _inputEnabled = false;
                
                var cell = StageManager.Instance.GetCell(targetCoord);
                if (cell.Unit != null && cell.Unit.UnitType == Constants.UnitType.Enemy)
                {
                    yield return Context._unitsManager.Player.Attack(cell.Unit);
                }
                else
                {
                    yield return Context._unitsManager.Player.Move(cell);
                }

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
                    if (enemy == null)
                    {
                        continue;
                    }
                    
                    yield return MoveEnemy(enemy);
                }

                StateMachine.Transit<PlayerTurnState>();
            }

            IEnumerator MoveEnemy(Unit enemy)
            {
                var result = StageManager.Instance.FindPath(enemy.Coord, Context._unitsManager.Player.Coord);
                if (result == null)
                {
                    Debug.Log("Path not found!");
                    StateMachine.Transit<PlayerTurnState>();
                    yield break;
                }

                var cell = StageManager.Instance.GetCell(new GridCoord(result.FirstStepNode.X, result.FirstStepNode.Y));
                if (cell.Coord == Context._unitsManager.Player.Coord)
                {
                    // TODO: プレイヤー（探索ゴール）なら攻撃
                }
                else
                {
                    yield return enemy.Move(cell);
                }
            }
        }

        #endregion
    }
}
