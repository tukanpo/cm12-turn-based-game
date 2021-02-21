using System.Collections;
using App.Scenes.Game.UnitActions;
using UnityEngine;

namespace App.Scenes.Game
{
    public class EnemyBrain01 : UnitBrain
    {
        readonly Chase _chase = new Chase();
        readonly MeleeAttack _meleeAttack = new MeleeAttack();

        // TODO: 実行するのではなく結果を返すようにする？
        // TODO: じゃぁ実行はどこがやる？ Unit？
        public override IEnumerator GetAction(Stage stage)
        {
            var result = stage.FindPath(Unit.Cell, stage.Player.Cell);
            if (result == null)
            {
                yield break;
            }

            var cell = stage.GetCell(new Vector2Int(result.FirstStepNode.X, result.FirstStepNode.Y));
            if (cell.Coord == stage.Player.Cell.Coord)
            {
                _meleeAttack.SetParams(stage.Player);
                yield return _meleeAttack.Execute(Unit);
            }
            else
            {
                _chase.SetParams(cell);
                yield return _chase.Execute(Unit);
            }
        }
    }
}
