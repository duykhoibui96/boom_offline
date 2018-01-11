using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BoomOffline.Helper
{
    class ANode
    {
        // Change this depending on what the desired size is for each element in the grid
        public ANode Parent;
        public Vector2 Position;
        public float DistanceToTarget;
        public float Cost;
        public float F
        {
            get
            {
                if (DistanceToTarget != -1 && Cost != -1)
                    return DistanceToTarget + Cost;
                else
                    return -1;
            }
        }
        public bool isWall;

        public ANode(Vector2 pos, bool walkable)
        {
            Parent = null;
            Position = pos;
            DistanceToTarget = -1;
            Cost = 1;
            isWall = walkable;
        }
    }

    class Astar
    {
        List<List<ANode>> Grid;
        int GridRows
        {
            get
            {
                return Grid[0].Count;
            }
        }
        int GridCols
        {
            get
            {
                return Grid.Count;
            }
        }

        public Astar(GameOperator go)
        {
            Grid = UpdateGrid(go);
        }

        public List<List<ANode>> UpdateGrid(GameOperator go)
        {
            int i, j;
            List<ANode> listNode;
            List<List<ANode>> grid = new List<List<ANode>>();
            for (i = 0; i < RoomSetting.Instance.MapSize; i++)
            {
                listNode = new List<ANode>();
                for (j = 0; j < RoomSetting.Instance.MapSize; j++)
                {
                    if (!go.IsPossibleMove(i, j))
                        listNode.Add(new ANode(new Vector2(j, i), true));
                    else
                        listNode.Add(new ANode(new Vector2(j, i), false));
                }
                grid.Add(listNode);
            }

            return grid;
        }

        public ANode FindPath(GameOperator go, Vector2 Start, Vector2 End)
        {
            Grid = UpdateGrid(go);
            ANode start = new ANode(new Vector2((int)(Start.X), (int)(Start.Y)), true);
            ANode end = new ANode(new Vector2((int)(End.X), (int)(End.Y)), true);

            ANode res = null;

            List<ANode> Path = new List<ANode>();
            List<ANode> OpenList = new List<ANode>();
            List<ANode> ClosedList = new List<ANode>();
            List<ANode> neighbors;
            ANode current = start;

            // add start node to Open List
            OpenList.Add(start);

            while (OpenList.Count != 0 && !ClosedList.Exists(x => x.Position == end.Position))
            {
                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                neighbors = GetNeighborNodes(current);


                foreach (ANode aN in neighbors)
                {

                    if (!ClosedList.Contains(aN) && !aN.isWall)
                    {
                        if (!OpenList.Contains(aN))
                        {
                            if (aN.Position.X == end.Position.X && aN.Position.Y == end.Position.Y)
                            {
                                res = current;
                                break;
                            }
                            aN.Parent = current;
                            aN.DistanceToTarget = Math.Abs(aN.Position.X - end.Position.X) + Math.Abs(aN.Position.Y - end.Position.Y);
                            aN.Cost = 1 + aN.Parent.Cost;
                            OpenList.Add(aN);
                            OpenList = OpenList.OrderBy(node => node.F).ToList<ANode>();
                        }
                    }
                }
                if (res != null)
                    break;
            }

            // can't find destination
            if (res == null)
                return null;

            while (res != null && res != start)
            {
                Path.Add(res);
                res = res.Parent;
            }

            if (Path.Count > 0)
                return Path[Path.Count - 1];
            return null;
        }

        private List<ANode> GetNeighborNodes(ANode n)
        {
            List<ANode> temp = new List<ANode>();

            int col = (int)n.Position.X;
            int row = (int)n.Position.Y;

            if (row + 1 < GridRows)
            {
                //temp.Add(Grid[col][row + 1]);
                temp.Add(Grid[row + 1][col]);
            }
            if (row - 1 >= 0)
            {
                //temp.Add(Grid[col][row - 1]);
                temp.Add(Grid[row - 1][col]);
            }
            if (col - 1 >= 0)
            {
                //temp.Add(Grid[col - 1][row]);
                temp.Add(Grid[row][col - 1]);
            }
            if (col + 1 < GridCols)
            {
                //temp.Add(Grid[col + 1][row]);
                temp.Add(Grid[row][col + 1]);
            }

            return temp;
        }
    }
}
