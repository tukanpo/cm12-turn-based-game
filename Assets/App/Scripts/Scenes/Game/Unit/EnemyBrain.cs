using System.Collections;
using UnityEngine;

namespace App.Scenes.Game
{
    public class EnemyBrain : UnitBrain
    {
        readonly ChaseUnitAction _chaseUnitAction = new ChaseUnitAction();
        
        // TODO: UnitAction を返すようにする
        public override IEnumerator ThinkAndAction(Stage stage)
        {
            var result = stage.FindPath(Unit.Cell, stage.Player.Cell);
            if (result == null)
            {
                yield break;
            }

            var cell = stage.GetCell(new Vector2Int(result.FirstStepNode.X, result.FirstStepNode.Y));
            if (cell.Coord == stage.Player.Cell.Coord)
            {
                yield return Unit.Attack(stage.Player);
            }
            else
            {
                // yield return Unit.Move(cell);

                _chaseUnitAction.SetParams(cell);
                yield return _chaseUnitAction.Execute(Unit);
            }
        }
    }
}
