using System;
using UnityEngine;

namespace App.Scenes.Game
{
    public readonly struct GridCoord
    {
        public readonly int X;
        public readonly int Y;

        public static bool operator ==(GridCoord a, GridCoord b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GridCoord a, GridCoord b)
        {
            return !(a == b);
        }
        
        public GridCoord(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public bool Equals(GridCoord other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is GridCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
        
        public Vector3 ToVector3()
        {
            return new Vector3(X, 0, Y);
        }

        /// <summary>
        /// 隣接するフロア座標を取得する
        /// </summary>
        /// <param name="direction">方向</param>
        /// <returns>フロア座標</returns>
        public GridCoord GetAdjacentCoord(Constants.CardinalDirection direction)
        {
            var x = X;
            var y = Y;
            
            switch (direction)
            {
                case Constants.CardinalDirection.N:
                    y += 1;
                    break;
                case Constants.CardinalDirection.S:
                    y -= 1;
                    break;
                case Constants.CardinalDirection.E:
                    x -= 1;
                    break;
                case Constants.CardinalDirection.W:
                    x += 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return new GridCoord(x, y);
        }
    }
}
