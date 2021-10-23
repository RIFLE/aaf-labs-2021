using System;
using System.Linq;
using System.Collections.Generic;

using PrefixTrieLibrary;
using CStatus = PrefixTrieLibrary.Node.CompletionStatus;
using Algs  = Functions.Algorithms;
using Imits = Functions.Imitators;
using RCS = Templates.RegexCheckStatus;

    class Program
    {
        static void Terminal(Dictionary<string,Node> TrieSet)
        {
            Console.Clear();
            Console.WriteLine("Welcome to terminal imitator. Enter \"MENU;\" to see available commands.");
            
            string commandStr = "";
            List<string> commandList = new List<string>();

            while(true)
            {
                commandList.Clear();
                Algs.GetCommand(ref commandStr);

                Templates.RegexCheckStatus checkResult = Algs.RegexCheck(ref commandStr);
                if(checkResult == RCS.REGEX_FAIL)
                {
                    Console.WriteLine("REGEX_FAIL occurence. Syntax error.");
                    continue;
                }
                else if(checkResult == RCS.UNKNOWN_COMMAND)
                {
                    Console.WriteLine("UNKNOWN_COMMAND occurence.\nEnter MENU to see the list of available commands.");
                    continue;
                }

                commandStr = commandStr.Remove(commandStr.IndexOf(';'));
                Algs.GetWordsList(ref commandStr, ref commandList);
                string[] commandInArgs = commandList.ToArray();

                commandInArgs[0] = commandInArgs[0].ToUpper();
                switch(commandInArgs[0])
                {
                    case "MENU":
                    {
                        Imits.PrintMenu();
                        break;
                    }

                    case "CREATE":
                    {
                        commandInArgs[1] = commandInArgs[1].Replace("\"", "");
                        if(!TrieSet.ContainsKey(commandInArgs[1]))
                        {
                            Node newTrie = new Node();
                            TrieSet.Add(commandInArgs[1], newTrie);
                            Console.WriteLine("Trie \"{0}\" was created successfully.", commandInArgs[1]);
                        }
                        else    //Trie already exists
                        {
                            Console.WriteLine("Trie with a name \"{0}\" already exists.", commandInArgs[1]);
                        }
                        break;
                    }

                    case "INSERT":
                    {
                        commandInArgs = commandList.Select(word => {word = word.Replace("\"", ""); return word;}).ToArray();

                        if(TrieSet.ContainsKey(commandInArgs[1]))
                        {
                            Node.CompletionStatus cs = TrieSet[commandInArgs[1]].AddNode(commandInArgs[2]); /*return value to indicate status*/
                            
                            if(cs == CStatus.NULL) throw new ApplicationException(message:"Logical error: bad return case for AddNode().");
                            
                            Console.WriteLine("Node \"{1}\" {2} \"{0}\"", commandInArgs[1], commandInArgs[2],
                                              (cs == CStatus.NEW_ADDED) ? "was added to" : "already exists in" );
                        }
                        else
                        {
                            Console.WriteLine("No trie with the name \"{0}\" exists.", commandInArgs[1]);
                        }
                        break;
                    }

                    case "CONTAINS":
                    {
                        commandInArgs = commandList.Select(word => {word = word.Replace("\"", ""); return word;}).ToArray();

                        if(TrieSet.ContainsKey(commandInArgs[1]))
                        {
                            Console.WriteLine("Containment status: word \"{1}\" {2} in the tree \"{0}\".", commandInArgs[1], commandInArgs[2],
                            TrieSet[commandInArgs[1]].Contains(commandInArgs[2])? "EXISTS" : "DOES NOT EXIST");
                        }
                        else
                        {
                            Console.WriteLine("No trie with the name \"{0}\" exists.", commandInArgs[1]);
                        }
                        break;
                    }

                    case "pc": //print command, only for development purposes
                    {
                        foreach(string word in commandInArgs)
                            Console.Write(word + " ");
                        Console.Write("\n");
                        break;
                    }

                    case "CLEAR":
                    {
                        Console.Clear();
                        break;
                    }
                    
                    case "EXIT":
                    {
                        return;
                    }

                    default:
                    {
                        throw new ApplicationException(message: "Unpredictable behaviour: no switch match error.");
                    }
                }
            }
        }
        
        static void Main(string[] args)
        {
            try
            {  
                Dictionary<string,Node> TrieSet = new Dictionary<string, Node>();

                Terminal(TrieSet);

                //My trie constructor was tested here...

                // Node prefixTree = new Node();
                
                // /*for building a trie [success]*/

                // prefixTree.AddNode("SomeText");
                // prefixTree.AddNode("Some");
                // prefixTree.AddNode("SomeTex");
                // prefixTree.AddNode("SomeTextAnd");
                // prefixTree.AddNode("SomeTextAndOther");
                // prefixTree.AddNode("SomeTextOr");
                // prefixTree.AddNode("Different");
                // prefixTree.AddNode("SomeTextAndOt");
                // prefixTree.AddNode("Diff");
                // prefixTree.AddNode("Lol");
                // prefixTree.AddNode("Lolkek");
                // prefixTree.AddNode("Lolkekcheburek");

                
                /*test trie, separating descend [success]*/

                // prefixTree.AddNode("abc");
                // prefixTree.AddNode("abcd");
                // prefixTree.AddNode("abce");
                // prefixTree.AddNode("abf");
                
                
                /*test trie, no thisNode descent [success]*/

                // prefixTree.AddNode("ab");
                // prefixTree.AddNode("stugl");
                // prefixTree.AddNode("stus");
                // prefixTree.AddNode("m");
                // prefixTree.AddNode("str");
                // prefixTree.AddNode("stug");

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught!\n{0}", ex);
            }
        }
    }

    