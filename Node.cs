using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    public class Node
    {
        public int frequency;   //частота знака или корня

        public string symbol { get; private set; }     //символ, если листок
        public bool isParent { get; private set; }

        public (Node left, Node right) children; //дети если корень
        public Node parent { get; private set; }

        public string code = string.Empty;

        // листья выглядят так
        public Node(string c, int f)
        {
            isParent = false;
            symbol = c;
            frequency = f;
        }
        // корни выглядят так
        public Node(Node firstKid, Node secondKid)
        {
            isParent = true;

            frequency = firstKid.frequency + secondKid.frequency;
            symbol = firstKid.symbol + secondKid.symbol;

            children.left = firstKid;
            children.right = secondKid;
        }
        public void SetParent(Node parent)
        {
            this.parent = parent;
        }
    }
}
