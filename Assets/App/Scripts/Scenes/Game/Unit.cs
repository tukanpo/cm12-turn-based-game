using System;
using System.Collections;
using UnityEngine;

namespace App.Scenes.Game
{
    public class Unit : MonoBehaviour
    {
        public GridCoord Coord { get; private set; }

        public Constants.CardinalDirection Direction { get; protected set; }

        public static Unit Spawn(Unit prefab, Transform parent, Tile tile)
        {
            var unit = Instantiate(prefab, tile.transform.position, prefab.transform.rotation);
            unit.transform.parent = parent;
            unit.Coord = tile.Coord;
            
            return unit;
        }

        public void SetDirection(Constants.CardinalDirection direction)
        {
            var position = transform.position;
            switch (direction)
            {
                case Constants.CardinalDirection.N:
                    transform.LookAt(new Vector3(position.x, position.y, position.z + 1));
                    break;
                case Constants.CardinalDirection.S:
                    transform.LookAt(new Vector3(position.x, position.y, position.z - 1));
                    break;
                case Constants.CardinalDirection.E:
                    transform.LookAt(new Vector3(position.x - 1, position.y, position.z));
                    break;
                case Constants.CardinalDirection.W:
                    transform.LookAt(new Vector3(position.x + 1, position.y, position.z));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            Direction = direction;
        }
        
        // TODO: ここでコマンドを受け取る

        public IEnumerator Move(Tile tile)
        {
            yield return SmoothMove(tile.transform.position, 3.5f, 0.1f);
            
            Coord = tile.Coord;
        }

        IEnumerator SmoothMove(Vector3 destination, float speed, float waitAfter)
        {
            transform.LookAt(destination);
        
            while (Vector3.Distance(transform.position, destination) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }
        
            yield return new WaitForSeconds(waitAfter);
        }
    }
}
