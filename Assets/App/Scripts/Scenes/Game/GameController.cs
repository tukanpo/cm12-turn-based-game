using System.Collections;
using App.Util;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scenes.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] Image _board;
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
                    Context._unitsManager.CreatePlayer(new GridCoord(4, 4));
                    var playerTransform = Context._unitsManager.Player.transform;
                    Context._vcam1.Follow = playerTransform;
                    Context._vcam1.LookAt = playerTransform;
                    
                    Context._board.gameObject.SetActive(false);
                    
                    StateMachine.Transit<PlayerTurnState>();
                });
            }
        }

        class PlayerTurnState : StateMachine<GameController>.State
        {
            bool _inputEnabled = true;
            
            public override void OnUpdate()
            {
                // TODO: プレイヤーユニットにコマンドを送る

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

                _inputEnabled = true;
            }
        }

        class EnemyTurnState : StateMachine<GameController>.State
        {
        }
        
        #endregion
    }
}
