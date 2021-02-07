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
        [SerializeField] StageManager _stageManager;
        [SerializeField] UnitsManager _unitsManager;
        [SerializeField] CinemachineVirtualCamera _vcam1;

        [SerializeField] Image _fullScreenBoard;
        [SerializeField] GameObject _gameOverPanel;
        [SerializeField] Button _retryButton;
        [SerializeField] UnitHealthBar _playerHealthBar;
        
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
                Context._unitsManager.Initialize();
                Context._stageManager.Initialize();

                Context._fullScreenBoard.gameObject.SetActive(true);
                Context._gameOverPanel.gameObject.SetActive(false);
                
                yield return Context._stageManager.CreateStage();

                yield return Context._unitsManager.CreatePlayer(
                    Context._stageManager.GetCell(new GridCoord(4, 4)),
                    Constants.CardinalDirection.S);
                Context._unitsManager.SetPlayerCamera(Context._vcam1);

                yield return Context._unitsManager.CreateEnemy(
                    Context._stageManager.GetCell(new GridCoord(2, 2)), 
                    EnumUtil.Random<Constants.CardinalDirection>());
                yield return Context._unitsManager.CreateEnemy(
                    Context._stageManager.GetCell(new GridCoord(1, 2)),
                    EnumUtil.Random<Constants.CardinalDirection>());
                yield return Context._unitsManager.CreateEnemy(
                    Context._stageManager.GetCell(new GridCoord(7, 7)),
                    EnumUtil.Random<Constants.CardinalDirection>());

                yield return Context._unitsManager.CreateWall(Context._stageManager.GetCell(new GridCoord(3, 2)));
                yield return Context._unitsManager.CreateWall(Context._stageManager.GetCell(new GridCoord(5, 4)));
                yield return Context._unitsManager.CreateWall(Context._stageManager.GetCell(new GridCoord(3, 3)));

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

                var targetCoord = Context._unitsManager.Player.Cell.Coord.GetAdjacentCoord(direction);
                if (!Context._stageManager.IsMovableOrAttackableCell(targetCoord))
                {
                    yield break;
                }
 
                _inputEnabled = false;
                
                var cell = Context._stageManager.GetCell(targetCoord);
                if (!ReferenceEquals(cell.Unit, null) && cell.Unit.UnitType == Constants.UnitType.Enemy)
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
                    if (ReferenceEquals(enemy, null))
                    {
                        continue;
                    }
                    
                    yield return MoveEnemy(enemy);
                    
                    if (Context._unitsManager.Player.UnitStatus.Health <= 0)
                    {
                        StateMachine.Transit<GameOverState>();
                        yield break;
                    }
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
