using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace SpectraLibraryParser
{
    public class Edge: Tuple<string,string,double>
    {
        public string Node1;
        public string Node2;
        public double Connection;
        
        public Edge(string node1, string node2, double connection):base(node1, node2, connection)
        {
            this.Node1 = node1;
            this.Node2 = node2;
            this.Connection = connection;
        }

        public override bool Equals(object obj)
        {
            Edge e = obj as Edge;
            if ((object) e == null) return false;
            //Test for undirected edge equality
            return ((Node1 == e.Node1 && Node2 == e.Node2 && Math.Abs(Connection - e.Connection) < 0.0001) ||
                    Node1 == e.Node2 && Node2 == e.Node1 && Math.Abs(Connection - e.Connection) < 0.0001);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}", Node1, Node2, Connection);
        }
    }
}
