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
            _sm.AddState<GameOverState>();
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
                    
                    Context._unitsManager.CreatePlayer(
                        Context._stageManager.GetCell(new GridCoord(4, 4)),
                        Constants.CardinalDirection.S);
                    Context._unitsManager.SetPlayerCamera(Context._vcam1);

                    Context._unitsManager.CreateEnemy(
                        Context._stageManager.GetCell(new GridCoord(2, 2)),
                        EnumUtil.Random<Constants.CardinalDirection>());
                    Context._unitsManager.CreateEnemy(
                        Context._stageManager.GetCell(new GridCoord(1, 2)),
                        EnumUtil.Random<Constants.CardinalDirection>());
                    Context._unitsManager.CreateEnemy(
                        Context._stageManager.GetCell(new GridCoord(7, 7)),
                        EnumUtil.Random<Constants.CardinalDirection>());
                    
                    Context._unitsManager.CreateWall(Context._stageManager.GetCell(new GridCoord(3, 2)));
                    Context._unitsManager.CreateWall(Context._stageManager.GetCell(new GridCoord(5, 4)));
                    Context._unitsManager.CreateWall(Context._stageManager.GetCell(new GridCoord(3, 3)));
                    
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

                
                // TODO: ★ Unit 系は UnitManager 経由して操作するように変更！！！！！

                
                var targetCoord = Context._unitsManager.Player.Cell.Coord.GetAdjacentCoord(direction);
                if (!Context._stageManager.IsMovableOrAttackableCell(targetCoord))
                {
                    yield break;
                }
 
                _inputEnabled = false;
                
                var cell = Context._stageManager.GetCell(targetCoord);
                if (cell.Unit != null && cell.Unit.UnitType == Constants.UnitType.Enemy)
                {
                    Debug.Log($"Player Attack! enemyId:{cell.Unit.Id}");
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
                var result = Context._stageManager.FindPath(enemy.Cell.Coord, Context._unitsManager.Player.Cell.Coord);
                if (result == null)
                {
                    Debug.Log("Path not found!");
                    StateMachine.Transit<PlayerTurnState>();
                    yield break;
                }

                var cell = Context._stageManager.GetCell(new GridCoord(result.FirstStepNode.X, result.FirstStepNode.Y));
                if (cell.Coord == Context._unitsManager.Player.Cell.Coord)
                {
                    Debug.Log($"Enemy Attack! enemyId:{enemy.Id}");
                    yield return enemy.Attack(Context._unitsManager.Player);
                }
                else
                {
                    yield return enemy.Move(cell);
                }
            }

            // TODO: ここにどうやって通知？
            public void OnPlayerDied()
            {
                StateMachine.Transit<GameOverState>();
            }
        }

        class GameOverState : StateMachine<GameController>.State
        {
            public override void OnEnter()
            {
                // UI 表示
                
                // ボタン押下でリスタート
                Context.StartGame();
            }
        }

        #endregion
    }
}
