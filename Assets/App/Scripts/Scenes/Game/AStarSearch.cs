using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Scenes.Game
{
    public class AStarSearchGrid
    {
        public class Node : IComparable<Node>
        {
            public int X { get; }

            public int Y { get; }
            
            public bool IsOpened { get; set; }

            // スタートからこのノードまでの実コスト
            public int Cost { get; set; }
            
            // 距離（このノードからゴールまでの推定コスト）
            public int Distance { get; set; }
            
            // スコア
            public int Score => Cost + Distance;

            public Node Parent { get; set; }

            public Node(int x, int y)
            {
                X = x;
                Y = y;
            }
            
            public void SetDistance(Node target)
            {
                // ヒューリスティック関数によりゴールまでの推定コスト（距離）を求める
                // NOTE: 斜め移動を許可する場合は縦横の差が大きい方を採用するように処理を追加する必要あり
                Distance = Math.Abs(target.X - X) + Math.Abs(target.Y - Y);
            }

            public void GetPath(List<Node> path)
            {
                path.Add(this);
                Parent?.GetPath(path);
            }

            public int CompareTo(Node other)
            {
                // スコアが同じならコストで比較する
                if (Score == other.Score)
                {
                    return Cost >= other.Cost ? 1 : -1;
                }

                return Score > other.Score ? 1 : -1;
            }
        }

        Node[,] _nodes;
        List<Node> _openNodes = new List<Node>();
        
        // TODO: リザルトは型にする？
        public bool Search(Node[,] nodes, Node start, Node goal)
        {
            _nodes = nodes;
            
            start.IsOpened = true;
            _openNodes.Add(start);

            while(_openNodes.Any())
            {
                // 次のノードをオープンリストから取得する
                var next = GetNextParentNode();
                if (next == goal)
                {
                    // 最短ルート発見
                    return true;
                }

                // 親ノードの接続先ノードをオープンにする
                OpenNode(next.X, next.Y + 1, next, goal);
                OpenNode(next.X, next.Y - 1, next, goal);
                OpenNode(next.X + 1, next.Y, next, goal);
                OpenNode(next.X - 1, next.Y, next, goal);

                _openNodes.Remove(next);
            }

            // 見つからなかった
            return false;
        }

        void OpenNode(int x, int y, Node parent, Node goal)
        {
            // ノード存在チェック
            if (x < 0 || x > _nodes.GetLength(1) - 1 || y < 0 || y > _nodes.GetLength(0) - 1)
            {
                return;
            }
            
            var target = _nodes[y, x];

            // オープン済なら何もしない
            if (target.IsOpened)
            {
                return;
            }

            // TODO: 移動できるかチェック
            
            // 次の Node への移動コストは常に１とする
            target.Parent = parent;
            target.Cost = target.Parent.Cost + 1;
            target.SetDistance(goal);
            target.IsOpened = true;
            
            _openNodes.Add(target);
        }

        Node GetNextParentNode()
        {
            // オープンリストから最小スコアのノードを取得する
            return _openNodes.Min();
        }
    }
}
