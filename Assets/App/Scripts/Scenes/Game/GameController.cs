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
        [SerializeField] Stage _stage;
        [SerializeField] CinemachineVirtualCamera _vcam1;

        [SerializeField] Image _fullScreenBoard;
        [SerializeField] GameObject _gameOverPanel;
        [SerializeField] Button _retryButton;

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
                Context.StartCoroutine(Initialize());
            }

            IEnumerator Initialize()
            {
                yield return Context._stage.Initialize();

                Context._fullScreenBoard.gameObject.SetActive(true);
                Context._gameOverPanel.gameObject.SetActive(false);
                
                yield return Context._stage.CreateStage(11, 9);

                yield return Context._stage.CreatePlayer(
                    Context._stage.GetCell(new GridCoord(4, 4)),
                    Constants.CardinalDirection.S);
                Context._stage.SetPlayerCamera(Context._vcam1);

                yield return Context._stage.CreateEnemy(
                    Context._stage.GetCell(new GridCoord(2, 2)), 
                    EnumUtil.Random<Constants.CardinalDirection>());
                yield return Context._stage.CreateEnemy(
                    Context._stage.GetCell(new GridCoord(1, 2)),
                    EnumUtil.Random<Constants.CardinalDirection>());
                yield return Context._stage.CreateEnemy(
                    Context._stage.GetCell(new GridCoord(7, 7)),
                    EnumUtil.Random<Constants.CardinalDirection>());

                yield return Context._stage.CreateWall(Context._stage.GetCell(new GridCoord(3, 2)));
                yield return Context._stage.CreateWall(Context._stage.GetCell(new GridCoord(5, 4)));
                yield return Context._stage.CreateWall(Context._stage.GetCell(new GridCoord(3, 3)));

                Context._fullScreenBoard.gameObject.SetActive(false);

                StateMachine.Transit<PlayerTurnState>();
            }
        }

        class PlayerTurnState : StateMachine<GameController>.State
        {
            bool _inputEnabled = true;

            public override void OnEnter()
            {
                _inputEnabled = true;

                Context._stage.Player.ResetActionPoint();
            }

            public override void OnUpdate()
            {
                if (Input.GetKey(KeyCode.W))
                {
                    Context.StartCoroutine(MovePlayer(Constants.CardinalDirection.N));
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    Context.StartCoroutine(MovePlayer(Constants.CardinalDirection.S));
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    Context.StartCoroutine(MovePlayer(Constants.CardinalDirection.E));
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    Context.StartCoroutine(MovePlayer(Constants.CardinalDirection.W));
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    StateMachine.Transit<EnemyTurnState>();
                }
            }

            IEnumerator MovePlayer(Constants.CardinalDirection direction)
            {
                if (!_inputEnabled) yield break;

                var targetCoord = Context._stage.Player.Cell.Coord.GetAdjacentCoord(direction);
                if (!Context._stage.IsMovableOrAttackableCell(targetCoord))
                {
                    yield break;
                }
 
                _inputEnabled = false;
                
                var cell = Context._stage.GetCell(targetCoord);
                if (!ReferenceEquals(cell.Unit, null) && cell.Unit.UnitType == Constants.UnitType.Enemy)
                {
                    yield return Context._stage.Player.Attack(cell.Unit);
                }
                else
                {
                    yield return Context._stage.Player.Move(cell);
                }

                if (Context._stage.Player.UnitStatus.ActionPoint == 0)
                {
                    StateMachine.Transit<EnemyTurnState>();
                }
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
                foreach (var enemy in Context._stage.Enemies)
                {
                    if (ReferenceEquals(enemy, null))
                    {
                        continue;
                    }
                    
                    yield return MoveEnemy(enemy);
                    
                    if (Context._stage.Player.UnitStatus.Health <= 0)
                    {
                        StateMachine.Transit<GameOverState>();
                        yield break;
                    }
                }

                StateMachine.Transit<PlayerTurnState>();
            }

            IEnumerator MoveEnemy(Unit enemy)
            {
                var result = Context._stage.FindPath(enemy.Cell, Context._stage.Player.Cell);
                if (result == null)
                {
                    Debug.Log("Path not found!");
                    StateMachine.Transit<PlayerTurnState>();
                    yield break;
                }

                var cell = Context._stage.GetCell(new GridCoord(result.FirstStepNode.X, result.FirstStepNode.Y));
                if (cell.Coord == Context._stage.Player.Cell.Coord)
                {
                    yield return enemy.Attack(Context._stage.Player);
                }
                else
                {
                    yield return enemy.Move(cell);
                }
            }
        }

        class GameOverState : StateMachine<GameController>.State
        {
            public override void OnEnter()
            {
                Context._retryButton.onClick.AddListener(OnClickRetryButton);

                Context.StartCoroutine(GameOverSequence());
            }

            public override void OnExit()
            {
                Context._retryButton.onClick.RemoveListener(OnClickRetryButton);
            }

            IEnumerator GameOverSequence()
            {
                yield return new WaitForSeconds(0.5f);

                Context._fullScreenBoard.gameObject.SetActive(true);
                Context._gameOverPanel.gameObject.SetActive(true);

                yield return new WaitForSeconds(0.2f);
                yield return new WaitUntil(() => Input.GetKey(KeyCode.Space));
                Retry();
            }

            void OnClickRetryButton()
            {
                Retry();
            }

            void Retry()
            {
                StateMachine.Transit<StageStartState>();
            }
        }

        #endregion
    }
}
