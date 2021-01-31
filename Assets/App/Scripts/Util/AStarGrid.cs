using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Util
{
    /// <summary>
    /// A* アルゴリズムによるグリッド経路探索
    /// </summary>
    public class AStarGrid
    {
        public interface INodeContent
        {
            /// <summary>
            /// 移動可能か？
            /// </summary>
            bool IsMovable();

            /// <summary>
            /// 経路探索用追加コスト
            /// </summary>
            int GetAdditionalCost();
        }

        public class Node : IComparable<Node>
        {
            public int X { get; }

            public int Y { get; }
            
            // スタートからこのノードまでの実コスト
            public int Cost { get; set; }
            
            // 距離（このノードからゴールまでの推定コスト）
            public int Distance { get; set; }
            
            // スコア
            public int Score => Cost + Distance;

            public Node Parent { get; set; }

            public bool IsOpened { get; set; }

            public INodeContent NodeContent { get; }
            
            public Node(int x, int y, INodeContent nodeContent)
            {
                X = x;
                Y = y;
                NodeContent = nodeContent;
            }

            public void Reset()
            {
                Cost = 0;
                Distance = 0;
                Parent = null;
                IsOpened = false;
            }
            
            public void SetDistance(Node target)
            {
                // ヒューリスティック関数によりゴールまでの推定コスト（距離）を求める
                // NOTE: 斜め移動を許可する場合は縦横の差が大きい方を採用するように処理を追加する必要あり
                Distance = Math.Abs(target.X - X) + Math.Abs(target.Y - Y);
            }

            /// <summary>
            /// このノードから親ノードを辿って作成した経路のリストを取得する
            /// </summary>
            /// <param name="path">結果格納用の経路リスト</param>
            public void GetPath(List<Node> path)
            {
                path.Add(this);
                Parent?.GetPath(path);
            }

            #region Implementation of IComparable
            
            /// <summary>
            /// ノードのスコア比較用
            /// </summary>
            /// <param name="other">比較対象</param>
            public int CompareTo(Node other)
            {
                // スコアが同じならコストで比較する
                if (Score == other.Score)
                {
                    return Cost >= other.Cost ? 1 : -1;
                }

                return Score > other.Score ? 1 : -1;
            }
            
            #endregion
        }

        public class Result
        {
            public List<Node> Path { get; } = new List<Node>();

            public Node FirstStepNode => Path[1];

            public void SetResult(Node calculatedGoal)
            {
                // 計算済のゴールノードから経路リストを作成する
                Path.Clear();
                calculatedGoal.GetPath(Path);
                Path.Reverse();
            }
        }

        Node[,] _nodes;
        readonly List<Node> _openNodes = new List<Node>();
        
        // NOTE: キャッシュを考えると外部から渡せる方が良いかもしれない
        //       使い回すかどうかは使用者に委ねる？
        readonly Result _result = new Result();

        public Result FindPath(Node[,] nodes, Node start, Node goal)
        {
            _nodes = nodes;
            foreach (var node in _nodes)
            {
                node.Reset();
            }

            start.IsOpened = true;
            _openNodes.Clear();
            _openNodes.Add(start);

            while(_openNodes.Any())
            {
                // 次のノードをオープンリストから取得する
                var next = GetMinScoreNodeFromOpenNodes();
                if (next == goal)
                {
                    // 最短ルート発見
                    _result.SetResult(goal);
                    return _result;
                }

                OpenAroundNodes(next, goal);
                _openNodes.Remove(next);
            }

            // 見つからなかった
            return null;
        }

        void OpenAroundNodes(Node next, Node goal)
        {
            // 親ノードの接続先ノードをオープンにする
            OpenNode(next.X, next.Y + 1, next, goal);
            OpenNode(next.X, next.Y - 1, next, goal);
            OpenNode(next.X + 1, next.Y, next, goal);
            OpenNode(next.X - 1, next.Y, next, goal);
        }

        void OpenNode(int x, int y, Node parent, Node goal)
        {
            // ノード範囲チェック
            if (x < 0 || x > _nodes.GetLength(1) - 1 || y < 0 || y > _nodes.GetLength(0) - 1)
            {
                return;
            }
            
            var target = _nodes[y, x];
            if (target.IsOpened)
            {
                return;
            }

            // 移動可否判定（ゴールはオープンする必要があるので判定から除外）
            if (target != goal && !target.NodeContent.IsMovable())
            {
                target.IsOpened = true;
                return;
            }

            // 次の Node への最低移動コストは常に１とする
            var additionalCost = 1 + target.NodeContent.GetAdditionalCost();
            target.Parent = parent;
            target.Cost = target.Parent.Cost + additionalCost;
            target.SetDistance(goal);
            target.IsOpened = true;
            
            _openNodes.Add(target);
        }

        Node GetMinScoreNodeFromOpenNodes()
        {
            return _openNodes.Min();
        }
    }
}
