using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho
{
    class Node
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<Node> Childs{ get; set; }

        public Node()
        {
            this.Childs = new List<Node>();
        }

        public static Node FindById(int id, List<Node> list)
        {
            return list.Find(o => o.Id == id);
        }

    }
}
