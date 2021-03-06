using System.Collections;
using System.Linq;
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
                
                yield return Context._stage.CreateStage(9, 9);

                Context._stage.SpawnPlayer(Context._stage.GetCell(new Vector2Int(4, 4)));
                Context._stage.SetPlayerCamera(Context._vcam1);

                yield return Context._stage.SpawnEnemiesOnRandomTile();
                
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
                if (!_inputEnabled) return;
                
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
                else if (Input.GetKey(KeyCode.Return))
                {
                    // リスタート
                    StateMachine.Transit<StageStartState>();
                }
            }

            IEnumerator MovePlayer(Constants.CardinalDirection direction)
            {
                if (!_inputEnabled) yield break;

                var targetCoord = Context._stage.Player.Cell.GetAdjacentCoord(direction);
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

                StateMachine.Transit<EnemyTurnState>();
            }
        }

        class EnemyTurnState : StateMachine<GameController>.State
        {
            public override void OnEnter()
            {
                if (!Context._stage.Enemies.Any())
                {
                    // 敵が居なくなったら生成
                    Context.StartCoroutine(SpawnEnemies());
                    return;
                }
                
                Context.StartCoroutine(MoveEnemies());
            }

            IEnumerator SpawnEnemies()
            {
                yield return Context._stage.SpawnEnemiesOnRandomTile();
                StateMachine.Transit<PlayerTurnState>();
            }

            IEnumerator MoveEnemies()
            {
                foreach (var kv in Context._stage.Enemies)
                {
                    yield return MoveEnemy(kv.Value);
                    
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

                var cell = Context._stage.GetCell(new Vector2Int(result.FirstStepNode.X, result.FirstStepNode.Y));
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
