using App.Util;
using UnityEngine;

namespace App.Scenes.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] StageManager _stageManager;

        StateMachine<GameController> _sm;

        void Awake()
        {
            _sm = new StateMachine<GameController>(this);
            _sm.AddState<StageStartState>();
            _sm.AddState<PlayerTurnState>();
            _sm.AddState<EnemyTurnState>();
        }

        public void StartGame()
        {
            _sm.Transit<StageStartState>();
        }
        
        #region States

        class StageStartState : StateMachine<GameController>.State
        {
            public override void OnEnter()
            {
                Context._stageManager.CreateStageAsync(() =>
                {
                    StateMachine.Transit<PlayerTurnState>();
                });
            }
        }

        class PlayerTurnState : StateMachine<GameController>.State
        {
            public override void OnEnter()
            {
                Debug.Log("PlayerTurnState::OnEnter");
            }

            public override void OnUpdate()
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    
                }
            }
        }

        class EnemyTurnState : StateMachine<GameController>.State
        {
        }
        
        #endregion
    }
}
