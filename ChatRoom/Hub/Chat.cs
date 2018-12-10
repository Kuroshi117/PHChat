// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-2.1
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Azure.SignalR.Samples.ChatRoom
{
    //The Chat class name becomes 
    public class Chat : Hub
    {
        #region Tree
        public static List<string> Text = new List<string>();
        public static List<Tree> Nodes = new List<Tree>();
        public static bool IsReady = false;

        public static void LoadText(string path, List<string> text)
        {
            try
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
            catch
            {
                IsReady = false;
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
            string tabs;
            for (int i = 0; i < nodes.Count; i++)
            {
                tabs = new string('\t', nodes[i].Depth);
                output += (tabs + nodes[i].Content() + ", " + nodes[i].id() + "\n");
            }
            /*List<Tree> Roots = new List<Tree>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Depth == 0)
                {
                    Roots.Add(nodes[i]);
                }
            }
            foreach (Tree t in Roots)
            {
                output+=ReadChildren(t, output);
            }*/

            return output;
        }
        public static string ReadChildren(Tree node, string output)
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
            return output;
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
        public static string ChatInput(string inputString)
        {
            string outputMessage = "";
            string[] inputArray = new string[4];
            char[] c = new char[] { ' ', ',' };
            inputArray = inputString.Split(c, StringSplitOptions.RemoveEmptyEntries);
            if (inputArray.Length <= 1 && !inputArray[0].Equals("exit", StringComparison.InvariantCultureIgnoreCase) && !inputArray[0].Equals("read", StringComparison.InvariantCultureIgnoreCase))
            {
                outputMessage = "Nothing to operate.";
            }

            if (inputArray[0].Equals("add", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    AddNewNode(inputArray[2], inputArray[1]);
                }
                catch (Exception e)
                {
                    outputMessage += "Incorrect input.";
                }
            }
            else if (inputArray[0].Equals("remove", StringComparison.InvariantCultureIgnoreCase))
            {
                DeleteNode(inputArray[1]);
            }
            else if (inputArray[0].Equals("move", StringComparison.InvariantCultureIgnoreCase))
            {
                MoveNode(inputArray[1], inputArray[2]);
                outputMessage = inputArray[1] + " has been moved to " + inputArray[2] + ".";
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
                try
                {
                    outputMessage += ReadNodes(Nodes);
                }
                catch (InvalidCastException e)
                {
                    outputMessage += e.Message;
                }
            }
            else if (inputArray[0].Equals("path", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    LoadText(inputArray[1], Text);
                    if (IsReady == true)
                    {
                        SortBranchTree(Text, Nodes);
                        outputMessage += "Done.";
                    }
                    else { outputMessage += "Invalid file path."; }
                }
                catch (InvalidCastException e)
                {
                    outputMessage += e.Message;
                }
            }

            else if (inputArray[0].Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {

                
            }
            else
            {
                outputMessage = "Please enter valid command.";
            }



            return outputMessage;
        }
        #endregion 

        private static List<string> users = new List<string>();
        public override Task OnConnectedAsync()
        {
            users.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            users.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public void BroadcastMessage(string name, string message)
        {            
            Clients.All.SendAsync("broadcastMessage", name, message);
            //Clients.All.SendAsync("broadcastMessage", name, ChatInput(message));
        }


        public void Echo(string name, string message)
        {
            Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
        }
    }
}
