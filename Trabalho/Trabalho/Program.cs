using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Trabalho
{
    class Program
    {
        public static List<Node> NodeList;
        public static List<int> VisitedIds = new List<int>();
        public static List<Distance> DistanceList;
        public static double Total;
        public static List<int> astarIds = new List<int>();


        static void Main(string[] args)
        {

            NodeList = new List<Node>();
            DistanceList = new List<Distance>();
            MakeList();
            SetConnections();
            SetDistances();

            Console.WriteLine("Escolha uma opção:\n");
            Console.WriteLine("1- BFS");
            Console.WriteLine("2- IDS");
            Console.WriteLine("3- A*");
            Console.Write("\nOpção:");
            int op = int.Parse(Console.ReadLine());
            Console.Write("De :");
            var from = int.Parse(Console.ReadLine());
            Console.Write("Até :");
            var to = int.Parse(Console.ReadLine());
            switch (op)
            {
                case 1:
                    breadthFirstSearch(from, to);
                    break;
                case 2:
                    Console.Write("Limite :");
                    var limit = int.Parse(Console.ReadLine());
                    ids(from, to, limit);
                    break;
                default:
                    astar(from, to);
                    Console.WriteLine("\n"+from);
                    foreach (var item in astarIds)
                    {
                        Console.WriteLine(item);
                    }
                    break;
            }


            Console.ReadKey();
        }

        private static void ids(int p, int target, int limit)
        {
            for (int maxDepth = 1; maxDepth < limit; maxDepth++)
            {
                depthFirstSearch(p, target, maxDepth);
            }
        }

        private static void breadthFirstSearch(int root, int target)
        {

            var q = new Queue<Node>();
            var rootNode = Node.FindById(root, NodeList);
            q.Enqueue(rootNode);

            while (q.Count > 0)
            {
                var node = q.Dequeue();
                if (!visitado(VisitedIds, node.Id))
                {
                    Console.WriteLine(node.Id);
                    VisitedIds.Add(node.Id);
                }

                for (int i = 0, c = node.Childs.Count; i < c; i++)
                {
                    if (node.Childs[i].Id == target)
                    {
                        Console.WriteLine(target);
                        return;
                    }
                    q.Enqueue(node.Childs[i]);
                }
            }
        }

        private static void depthFirstSearch(int node, int target, int maxDepth)
        {
            if (!visitado(VisitedIds, node))
            {
                Console.WriteLine(node);
                VisitedIds.Add(node);
            }

            if (maxDepth <= 0 || node == target)
            {
                return;
            }
            var realNode = Node.FindById(node, NodeList);

            for (int i = 0, c = realNode.Childs.Count; i < c; i++)
            {
                if (realNode.Childs[i].Id == target)
                {
                    Console.WriteLine(target);
                    return;
                }
                depthFirstSearch(realNode.Childs[i].Id, target, maxDepth - 1);

            }
        }

        public static int NoWay(int p)
        {
            bool flag = true;
            int i = VisitedIds.Count - 1;
            while (flag)
            {
                var node = Node.FindById(VisitedIds[i], NodeList);
                foreach (var item in node.Childs)
                {
                    if (!visitado(VisitedIds, item.Id))
                    {
                        return item.Id;
                    }
                }
                astarIds.Remove(node.Id);
                i--;
                if (i == 0)
                    flag = false;
            }
            return 0;
        }

        private static void astar(int p, int target)
        {
            var node = Node.FindById(p, NodeList);
            if (!visitado(VisitedIds, node.Id))
            {
                VisitedIds.Add(node.Id);
                if (node.Id == target)
                    return;
                var targetNode = Node.FindById(target, NodeList);
                var costList = new Dictionary<int, double>();
                double reta = 0;
                foreach (var item in node.Childs)
                {
                    if (!visitado(VisitedIds, item.Id))
                    {
                        if (item.Id != target)
                            reta = CalculateDistance(item.X, item.Y, targetNode.X, targetNode.Y);
                        else
                            reta = 0;
                        if (item.Id == target)
                        {
                            astarIds.Add(item.Id);
                            return;
                        }
                        costList.Add(item.Id, DistanceList.Where(w => w.PointA == node.Id && w.PointB == item.Id).First().Cost + reta + Total);
                    }
                }
                var min = IndexOfMin(costList);
                if (min == -1)
                    min = NoWay(min);
                else
                    Total += costList[min];
                astarIds.Add(min);
                astar(min, target);
            }
        }

        private static int IndexOfMin(Dictionary<int, double> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            double minValue = int.MaxValue;
            int minIndex = -1;
            int index = -1;

            foreach (var num in source)
            {
                index++;

                if (num.Value <= minValue)
                {
                    minValue = num.Value;
                    minIndex = num.Key;
                }
            }


            return minIndex;
        }

        private static Double rad2deg(Double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        private static Double deg2rad(Double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private const Double kEarthRadiusKms = 6376.5;

        private static Double CalculateDistance(Double lat1, Double lon1, Double lat2, Double lon2)
        {

            lat1 = deg2rad(lat1);
            lat2 = deg2rad(lat2);
            lon1 = deg2rad(lon1);
            lon2 = deg2rad(lon2);
            var dist = (6371 * Math.Acos(Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1) + Math.Sin(lat1) * Math.Sin(lat2)));
            return dist;
        }

        private static bool visitado(List<int> visitados, int param)
        {
            foreach (var item in visitados)
            {
                if (item == param)
                    return true;
            }
            return false;
        }

        private static void MakeList()
        {
            StreamReader sr = new StreamReader("DOCS/positions.csv");
            var reader = sr.ReadLine();
            reader = sr.ReadLine();//Titles

            while (reader != null)
            {
                var array = reader.Split(',');
                var node = new Node();
                node.Id = int.Parse(array[0]);
                node.X = int.Parse(array[1]);
                node.Y = int.Parse(array[2]);
                NodeList.Add(node);
                reader = sr.ReadLine();
            }

            sr.Close();
        }

        private static void SetConnections()
        {
            StreamReader sr = new StreamReader("DOCS/connections.csv");
            var reader = sr.ReadLine();
            while (reader != null)
            {
                var array = reader.Split(',');
                var allChilds = new List<Node>();

                for (int i = 1; i < array.Length; i++)
                {
                    NodeList.Find(t => t.Id == int.Parse(array[0])).Childs.Add(Node.FindById(int.Parse(array[i]), NodeList));
                }
                reader = sr.ReadLine();
            }

            sr.Close();
        }

        private static void SetDistances()
        {
            StreamReader sr = new StreamReader("DOCS/distances.csv");
            var reader = sr.ReadLine();
            while (reader != null)
            {
                var array = reader.Split(',');
                var distance = new Distance();
                distance.PointA = int.Parse(array[0]);
                distance.PointB = int.Parse(array[1]);
                distance.Cost = int.Parse(array[2]);
                DistanceList.Add(distance);
                //reverse

                distance.PointA = int.Parse(array[1]);
                distance.PointB = int.Parse(array[0]);
                distance.Cost = int.Parse(array[2]);
                DistanceList.Add(distance);
                reader = sr.ReadLine();
            }

            sr.Close();
        }
    }
}
