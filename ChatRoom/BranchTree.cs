using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Microsoft.Azure.SignalR.Samples.ChatRoom
{
    public class BranchTree
    {
        public static List<string> Text = new List<string>();
        public static List<Tree> Nodes = new List<Tree>();
        public static bool IsReady = false;

        public static void LoadText(string path, List<string> text)
        {
            string line;

            using (StreamReader sr = new StreamReader(path))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    text.Add(line);
                }

            }
            if (Text != null)
            {
                IsReady = true;
            }
        }

        public static void SortBranchTree(List<string> text, List<Tree> nodes)
        {
            string tempName;
            for (int i = 0; i < text.Count; i++)
            {
                tempName = text[i].Trim();

                nodes.Add(new Tree(tempName));
                nodes[i].Depth = NumberOfOcc(text[i], "\t");
                if (NumberOfOcc(text[i], "\t") == 0)
                {
                    AddNode(nodes[i], null);
                }
                else if ((NumberOfOcc(text[i], "\t")) > (NumberOfOcc(text[i - 1], "\t")))
                {
                    AddNode(nodes[i], nodes[i - 1].id());

                }
                else if ((NumberOfOcc(text[i], "\t")) == (NumberOfOcc(text[i - 1], "\t")))
                {
                    AddNode(nodes[i], nodes[i - 1].Parent.id());


                }
                else if ((NumberOfOcc(text[i], "\t")) < (NumberOfOcc(text[i - 1], "\t")))
                {
                    int d = NumberOfOcc(text[i], "\t");
                    int j = nodes.Count - 2;
                    while (nodes[j].Depth != d)
                    {
                        j--;
                    }
                    AddNode(nodes[i], nodes[j].Parent.id());



                }
            }
        }

        public static void AddNode(Tree tree, string parentID)
        {

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].id() == parentID)
                {
                    tree.Parent = Nodes[i];
                    tree.Depth = Nodes[i].Depth + 1;
                    Nodes[i].Children.Add(tree);
                    break;

                }

            }

        }

        public static void AddNewNode(string name, string parentID)
        {
            Tree tree = new Tree(name.Trim());
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].id() == parentID)
                {
                    tree.Parent = Nodes[i];
                    tree.Depth = Nodes[i].Depth + 1;
                    Nodes[i].Children.Add(tree);
                    Nodes.Insert(i + 1, tree);
                    break;

                }
                else if (parentID == "null")
                {
                    tree.Parent = null;
                    tree.Depth = 0;
                    Nodes.Add(tree);
                    break;

                }

            }

        }

        public static void DeleteNode(string id)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].id() == id)
                {
                    if (Nodes[i].Parent != null)
                    {
                        Nodes[i].Parent.Children.Remove(Nodes[i]);
                    }

                    Nodes[i].Parent = null;
                    foreach (Tree c in Nodes[i].Children)
                    {
                        c.Parent = null;
                        Nodes.Remove(c);
                    }
                    Nodes[i].Children.Clear();

                    Nodes.RemoveAt(i);
                    break;
                }

            }
        }

        public static void MoveNode(string id, string parentID)
        {
            //reference from https://www.dotnetperls.com/list-find
            Nodes.Find(tree => tree.id() == id).Parent.Children.Remove(Nodes.Find(tree => tree.id() == id));
            Nodes.Find(tree => tree.id() == id).Parent = Nodes.Find(tree => tree.id() == parentID);
            SetDepth(id);
            Nodes.Find(tree => tree.id() == parentID).Children.Add(Nodes.Find(tree => tree.id() == id));


        }

        public static Tree FindNode(string id)
        {
            return Nodes.Find(tree => tree.id() == id);
        }

        public static List<Tree> FindNodebyContent(string contentID)
        {
            List<Tree> GotNodes = new List<Tree>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Content().Equals(contentID, StringComparison.InvariantCultureIgnoreCase))
                {
                    GotNodes.Add(Nodes[i]);

                }

            }

            return GotNodes;
        }

        public static List<Tree> FindLeaves()
        {
            List<Tree> GotNodes = new List<Tree>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (!Nodes[i].Children.Any())
                {
                    GotNodes.Add(Nodes[i]);

                }

            }

            return GotNodes;
        }

        public static List<Tree> FindInternalNodes()
        {
            List<Tree> GotNodes = new List<Tree>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Parent != null && Nodes[i].Children.Any())
                {
                    GotNodes.Add(Nodes[i]);

                }

            }

            return GotNodes;
        }



        public static int NumberOfOcc(string text, string pattern)
        {
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        public static void SetDepth(string id)
        {
            Nodes.Find(tree => tree.id() == id).Depth = Nodes.Find(tree => tree.id() == id).Parent.Depth + 1;
            if (Nodes.Find(tree => tree.id() == id).Children != null)
            {
                foreach (Tree t in Nodes.Find(tree => tree.id() == id).Children)
                {
                    SetDepth(t.id());
                }
            }
        }

        public static string ReadNodes(List<Tree> nodes)
        {
            string output = "";
            List<Tree> Roots = new List<Tree>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Depth == 0)
                {
                    Roots.Add(nodes[i]);
                }
            }
            foreach (Tree t in Roots)
            {
                ReadChildren(t, output);
            }
            return output;
        }

        public static void ReadChildren(Tree node, string output)
        {
            string tabs;
            tabs = new string('\t', node.Depth);
            output += (tabs + node.Content() + ", " + node.id() + "\n");
            if (node.Children != null)
            {
                foreach (Tree t in node.Children)
                {
                    ReadChildren(t, output);
                }
            }
        }

        public static void WriteOutlineFile(List<Tree> nodes, string filename)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\workspace\" + filename + ".txt"))
            {
                //basic way
                /*string tabs;
                for (int i = 0; i < nodes.Count; i++)
                {
                    tabs = new string('\t', nodes[i].Depth);
                    sw.WriteLine(tabs + nodes[i].Content());
                }*/

                //hierarichal way
                List<Tree> Roots = new List<Tree>();
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Depth == 0)
                    {
                        Roots.Add(nodes[i]);
                    }
                }
                foreach (Tree t in Roots)
                {
                    WriteChildren(t, sw);
                }

            }
        }

        public static void WriteChildren(Tree node, StreamWriter sw)
        {
            string tabs;
            tabs = new string('\t', node.Depth);
            sw.WriteLine(tabs + node.Content() + ", " + node.id());
            if (node.Children != null)
            {
                foreach (Tree t in node.Children)
                {
                    WriteChildren(t, sw);
                }
            }
        }



        public string ChatInput(string inputString)
        {
            string outputMessage = "";
            string[] inputArray = new string[4];
            char[] c = new char[] { ' ', ',' };
            inputArray = inputString.Split(c, StringSplitOptions.RemoveEmptyEntries);
            if (inputArray.Length <= 1 && !inputArray[0].Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                outputMessage = "Nothing to operate.";
            }

            if (inputArray[0].Equals("add", StringComparison.InvariantCultureIgnoreCase))
            { 
                AddNewNode(inputArray[2], inputArray[1]);
            }
            else if (inputArray[0].Equals("remove", StringComparison.InvariantCultureIgnoreCase))
            {
                DeleteNode(inputArray[1]);
            }
            else if (inputArray[0].Equals("move", StringComparison.InvariantCultureIgnoreCase))
            {
                MoveNode(inputArray[1], inputArray[2]);
            }
            else if (inputArray[0].Equals("get", StringComparison.InvariantCultureIgnoreCase))
            {

                if (inputArray[1].Equals("leaves", StringComparison.InvariantCultureIgnoreCase))
                {
                    List<Tree> tempTreeList = FindLeaves();
                    if (tempTreeList.Any())
                    {
                        outputMessage = "Found: \n";
                        foreach (Tree t in tempTreeList)
                        {
                            outputMessage += (t.Content() + ", " + t.id() + "\n");
                        }
                    }
                    else
                    {
                        outputMessage = "No leaves found.";
                    }
                }
                else if (inputArray[1].Equals("internal", StringComparison.InvariantCultureIgnoreCase))
                {
                    List<Tree> tempTreeList = FindInternalNodes();
                    if (tempTreeList.Any())
                    {
                        outputMessage = "Found: \n";
                        foreach (Tree t in tempTreeList)
                        {
                            outputMessage += (t.Content() + ", " + t.id() + "\n");
                        }
                    }
                    else
                    {
                        outputMessage = "No internals found.";
                    }
                }
                else
                {

                    if (FindNode(inputArray[1]) != null)
                    {
                        Tree tempTree = FindNode(inputArray[1]);
                        outputMessage = ("Found:\n" + tempTree.Content() + ", " + tempTree.id() + "\n");

                    }
                    else if (FindNodebyContent(inputArray[1]) != null)
                    {
                        List<Tree> tempTreeList = FindNodebyContent(inputArray[1]);
                        if (tempTreeList.Any())
                        {
                            outputMessage = ("Found: \n");
                            foreach (Tree t in tempTreeList)
                            {
                                outputMessage += (t.Content() + ", " + t.id() + "\n");
                            }
                        }
                        else
                        {
                            outputMessage = "Not found.";
                        }
                    }
                    else
                    {
                        outputMessage = (inputArray[1] + " not found.");
                    }
                }

            }
            else if (inputArray[0].Equals("read", StringComparison.InvariantCultureIgnoreCase))
            {
                ReadNodes(Nodes);
            }
            else if (inputArray[0].Equals("write", StringComparison.InvariantCultureIgnoreCase))
            {

            }
            else if (inputArray[0].Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {

                //Close tab
            }
            else
            { }



            return outputMessage;
        }
        
      
    }

    public class Tree : INode
    {
        private string Name;
        public Tree Parent;
        public List<Tree> Children = new List<Tree>();
        public int Depth;
        private static Random rand = new Random();
        private string identifier;
        public Tree(string n)
        {
            Name = n;
            identifier = GenerateID();
        }

        public string id()
        {
            return identifier;
        }

        public string Content()
        {
            return Name;
        }

        public bool IsReady()
        {
            return true;
        }

        private string GenerateID()
        {
            //reference from https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] CharArray = new char[8];
            for (int i = 0; i < CharArray.Length; i++)
            {
                CharArray[i] = chars[rand.Next(chars.Length)];
            }
            return new string(CharArray);
        }
    }

    interface INode
    {
        string id();
        string Content();
        bool IsReady();

    }
}