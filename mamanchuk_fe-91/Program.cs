using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using PrefixTrieLibrary;
using CStatus = PrefixTrieLibrary.Node.CompletionStatus;
using Algs = Functions.Algorithms;
using Imits = Functions.Imitators;
using RCStatus = Templates.RegexCheckStatus;

class Program
{
    static void Terminal(Dictionary<string, Node> TrieSet)
    {
        Console.WriteLine("Welcome to terminal imitator. Enter \"MENU;\" to see available commands.");

        string commandStr = "";
        List<string> commandList = new List<string>();

        while (true)
        {
            commandList.Clear();
            Algs.GetCommand(ref commandStr);

            Templates.RegexCheckStatus checkResult = Algs.RegexCheck(ref commandStr);
            if (checkResult == RCStatus.REGEX_FAIL)
            {
                Console.WriteLine("REGEX_FAIL occurence. Syntax error.");
                continue;
            }
            else if (checkResult == RCStatus.UNKNOWN_COMMAND)
            {
                Console.WriteLine("UNKNOWN_COMMAND occurence.\nEnter MENU to see the list of available commands.");
                continue;
            }

            commandStr = commandStr.Remove(commandStr.IndexOf(';'));
            Algs.GetWordsList(ref commandStr, ref commandList);
            string[] commandInArgs = commandList.ToArray();

            commandInArgs[0] = commandInArgs[0].ToUpper();
            switch (commandInArgs[0])
            {
                case "MENU":
                    {
                        Imits.PrintMenu();
                        break;
                    }

                case "CREATE":
                    {
                        foreach (string word in commandInArgs.Skip(1 + ((checkResult == RCStatus.HAS_RANGE) ? 1 : 0))) //XD
                        {
                            if (!TrieSet.ContainsKey(word))
                            {
                                Node newTrie = new Node();
                                TrieSet.Add(word, newTrie);
                                Console.WriteLine($"Trie \"{word}\" was created successfully.");
                            }
                            else    //Trie already exists
                            {
                                Console.WriteLine($"Trie with a name \"{word}\" already exists.");
                            }
                        }
                        break;
                    }

                case "INSERT":
                    {
                        if (checkResult == RCStatus.HAS_RANGE)
                        {
                            Regex firstWord = new Regex(@"^""(\w){0,}""$");
                            int firstWordIndex = Array.FindIndex(commandInArgs, word => firstWord.IsMatch(word));
                            int wordsCount = commandInArgs.Length - firstWordIndex;
                            int treesCount = commandInArgs.Length - wordsCount - 2;
                            commandInArgs = commandList.Select(word => { word = word.Replace("\"", ""); return word; }).ToArray();
                            for (int currentTreeIndex = 0; currentTreeIndex < treesCount; currentTreeIndex++)
                            {
                                if (TrieSet.ContainsKey(commandInArgs[currentTreeIndex + 2]))
                                {
                                    for (int currentWordIndex = 0; currentWordIndex < wordsCount; currentWordIndex++)
                                    {
                                        Node.CompletionStatus cs = TrieSet[commandInArgs[currentTreeIndex + 2]]
                                                                   .AddNode(commandInArgs[firstWordIndex + currentWordIndex]);

                                        if (cs == CStatus.NULL) throw new ApplicationException(message: "Logical error: bad return case for AddNode().");

                                        Console.WriteLine("Node \"{1}\" {2} \"{0}\"",
                                                          commandInArgs[currentTreeIndex + 2],
                                                          commandInArgs[firstWordIndex + currentWordIndex],
                                                          (cs == CStatus.NEW_ADDED) ? "was added to" : "already exists in");
                                    }
                                    if (currentTreeIndex < treesCount - 1) Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine($"No trie with the name \"{commandInArgs[currentTreeIndex + 2]}\" exists.");
                                }
                            }
                        }
                        else
                        {
                            commandInArgs[2] = commandInArgs[2].Replace("\"", "");
                            if (TrieSet.ContainsKey(commandInArgs[1]))
                            {
                                Node.CompletionStatus cs = TrieSet[commandInArgs[1]].AddNode(commandInArgs[2]); /*return value to indicate status*/

                                if (cs == CStatus.NULL) throw new ApplicationException(message: "Logical error: bad return case for AddNode().");

                                Console.WriteLine("Node \"{1}\" {2} \"{0}\"", commandInArgs[1], commandInArgs[2],
                                                (cs == CStatus.NEW_ADDED) ? "was added to" : "already exists in");
                            }
                            else
                            {
                                Console.WriteLine("No trie with the name \"{0}\" exists.", commandInArgs[1]);
                            }
                        }
                        break;
                    }

                case "CONTAINS":
                    {
                        if (checkResult == RCStatus.HAS_RANGE)
                        {
                            Regex firstWord = new Regex(@"^""(\w){0,}""$");
                            int firstWordIndex = Array.FindIndex(commandInArgs, word => firstWord.IsMatch(word));
                            int wordsCount = commandInArgs.Length - firstWordIndex;
                            int treesCount = commandInArgs.Length - wordsCount - 2;
                            commandInArgs = commandList.Select(word => { word = word.Replace("\"", ""); return word; }).ToArray();
                            bool containmentStatus;
                            for (int currentTreeIndex = 0; currentTreeIndex < treesCount; currentTreeIndex++)
                            {
                                if (TrieSet.ContainsKey(commandInArgs[currentTreeIndex + 2]))
                                {
                                    for (int currentWordIndex = 0; currentWordIndex < wordsCount; currentWordIndex++)
                                    {
                                        containmentStatus = TrieSet[commandInArgs[currentTreeIndex + 2]].Contains(commandInArgs[firstWordIndex + currentWordIndex]);

                                        Console.WriteLine("Containment status: word \"{1}\" {2} in the tree \"{0}\".",
                                                         commandInArgs[currentTreeIndex + 2],
                                                         commandInArgs[firstWordIndex + currentWordIndex],
                                                         containmentStatus ? "EXISTS" : "DOES NOT EXIST");
                                    }
                                    if (currentTreeIndex < treesCount - 1) Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine($"No trie with the name \"{commandInArgs[currentTreeIndex + 2]}\" exists.");
                                }
                            }
                        }
                        else
                        {
                            commandInArgs[2] = commandInArgs[2].Replace("\"", "");
                            if (TrieSet.ContainsKey(commandInArgs[1]))
                            {
                                bool containmentStatus = TrieSet[commandInArgs[1]].Contains(commandInArgs[2]);

                                Console.WriteLine("Containment status: word \"{1}\" {2} in the tree \"{0}\".",
                                                  commandInArgs[1], commandInArgs[2], containmentStatus ? "EXISTS" : "DOES NOT EXIST");
                            }
                            else
                            {
                                Console.WriteLine("No trie with the name \"{0}\" exists.", commandInArgs[1]);
                            }
                        }
                        break;
                    }

                case "PRINT_TREE":
                    {
                        foreach (string treeName in commandInArgs.Skip(1 + ((checkResult == RCStatus.HAS_RANGE) ? 1 : 0))) //XD
                        {
                            if (TrieSet.ContainsKey(treeName))
                            {
                                Console.Write($"Tree for \"{treeName}\":");
                                string treeStr = TrieSet[treeName].GetTreeAsString();
                                Console.Write("{0}", (treeStr != "\n") ? treeStr : "\n*empty*\n");
                            }
                            else
                            {
                                Console.WriteLine($"No trie with the name \"{treeName}\" exists.");
                            }
                        }
                        break;
                    }

                case "PC": //print command, only for development purposes
                    {
                        foreach (string word in commandInArgs)
                            Console.Write(" |" + word + "| ");
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

    static void GetAndPrintTestTree(ref Dictionary<string, Node> TrieSet, string treeName)
    {
        if (!TrieSet.ContainsKey(treeName))
        {
            Node testTree = new Node();
            TrieSet.Add(treeName, testTree);

            string[] testWords = {"SomeText","Some", "SomeTex", "SomeTextAnd", "SomeTextAndOther", "SomeTextOr", "Different", "SomeTextAndOt", "Diff",
            "Lol", "Lolkek", "Lolkekcheburek", "Lolkekcheburs", "Lolkekchebur", "abc", "abcd", "abce", "abf", "ab", "stugl", "stus", "m", "str", "stug",
            "abc", "abcdefUCC", "abcdes", "abcdef", "abcffe", "abvdsf", "aghrg", "abcdggth", "b"};

            foreach (string word in testWords)
                TrieSet[treeName].AddNode(word);     //Building the trie

            Console.Write($"Tree for \"{treeName}\"");
            Console.WriteLine(TrieSet[treeName].GetTreeAsString());    //Printing the tree :)
        }
        return;
    }

    static void Main(string[] args)
    {
        try
        {
            Dictionary<string, Node> TrieSet = new Dictionary<string, Node>();

            //UNCOMMENT NEXT LINE TO CREATE AND PRINT TREE INSTANTENIOUSLY
            //GetAndPrintTestTree(ref TrieSet, "Test");

            Terminal(TrieSet);

            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caught!\n{0}", ex);
        }
    }
}

