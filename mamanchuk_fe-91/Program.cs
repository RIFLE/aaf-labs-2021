using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using PrefixTrieLibrary;
using CStatus = PrefixTrieLibrary.Node.CompletionStatus;

using Functions;
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
            if (checkResult.HasFlag(RCStatus.REGEX_FAIL))
            {
                Console.WriteLine("REGEX_FAIL occurence. Syntax error.");
                continue;
            }
            else if ((int)checkResult == (int)RCStatus.UNKNOWN_COMMAND)
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
                        foreach (string word in commandInArgs.Skip(1 + ((checkResult.HasFlag(RCStatus.HAS_RANGE)) ? 1 : 0))) //XD
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
                        if (checkResult.HasFlag(RCStatus.HAS_RANGE))
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
                        if (checkResult.HasFlag(RCStatus.HAS_RANGE))
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

                case "SEARCH":
                    {
                        List<string> wordMatches = new List<string>() { };
                        Node.CompletionStatus cs;
                        if (commandInArgs[commandInArgs.Length - 1].ToUpper() == "ASC") { checkResult |= RCStatus.SEARCH_ASC; }
                        else if (commandInArgs[commandInArgs.Length - 1].ToUpper() == "DESC") { checkResult |= RCStatus.SEARCH_DESC; }

                        int lastTreeIndex = 0; bool hasQuery = false;
                        if (hasQuery = (checkResult.HasFlag(RCStatus.SEARCH_WHERE_MATCH) || checkResult.HasFlag(RCStatus.SEARCH_WHERE_BETWEEN)))
                        {
                            lastTreeIndex = commandInArgs.IndexOfWord("WHERE", StringComparison.OrdinalIgnoreCase) - 1;
                        }

                        if (!hasQuery)
                        {
                            if (checkResult.HasFlag(RCStatus.SEARCH_ASC) || checkResult.HasFlag(RCStatus.SEARCH_DESC))
                            {
                                lastTreeIndex = commandInArgs.Length - 2;
                            }
                            else
                            {
                                lastTreeIndex = commandInArgs.Length - 1;
                            }

                            int caseForRange = ((checkResult.HasFlag(RCStatus.HAS_RANGE)) ? 0 : 1);
                            for (int treeIndex = 2 - caseForRange; treeIndex < lastTreeIndex + 1; treeIndex++)
                            {
                                if (TrieSet.ContainsKey(commandInArgs[treeIndex]))
                                {
                                    wordMatches.Clear();
                                    Console.WriteLine($"Matches for \"{commandInArgs[treeIndex]}\"");
                                    cs = TrieSet[commandInArgs[treeIndex]].GetMatchesList(ref wordMatches, "*");
                                    if (cs == Node.CompletionStatus.EMPTY_TREE)
                                    {
                                        Console.WriteLine("The tree is empty.");
                                        continue;
                                    }

                                    if (checkResult.HasFlag(RCStatus.SEARCH_DESC))
                                    {
                                        wordMatches = wordMatches.OrderByDescending(str => str).ThenByDescending(str => str.Length).ToList();
                                    }
                                    else
                                    {
                                        wordMatches = wordMatches.OrderBy(str => str).ThenBy(str => str.Length).ToList();
                                    }

                                    int counter = 1; int matchesQuantity = wordMatches.Count();
                                    int maxLen = wordMatches.ToArray().GetMaxStringLength();
                                    foreach (string match in wordMatches)
                                    {
                                        Console.Write(match.PadRight(maxLen + 1));
                                        if (counter % 5 == 0 && counter != matchesQuantity) Console.WriteLine();
                                        counter++;
                                    }
                                    Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine($"No trie with the name \"{commandInArgs[treeIndex]}\" exists.");
                                }
                            }

                        }
                        else if (checkResult.HasFlag(RCStatus.SEARCH_WHERE_MATCH))
                        {
                            string matchPattern = commandInArgs[lastTreeIndex + 3].Replace("\"", "");
                            for (int treeIndex = 2 - ((checkResult.HasFlag(RCStatus.HAS_RANGE)) ? 0 : 1); treeIndex < lastTreeIndex + 1; treeIndex++)
                            {
                                if (TrieSet.ContainsKey(commandInArgs[treeIndex]))
                                {
                                    wordMatches.Clear();
                                    Console.WriteLine($"Matches for \"{commandInArgs[treeIndex]}\"");
                                    cs = TrieSet[commandInArgs[treeIndex]].GetMatchesList(ref wordMatches, matchPattern);

                                    if (cs == Node.CompletionStatus.NULL_OR_EMPTY_PARAM)
                                    {
                                        Console.WriteLine("Error: null or empty match parameter.");
                                        break;
                                    }
                                    else if (cs == Node.CompletionStatus.EMPTY_TREE)
                                    {
                                        Console.WriteLine("The tree is empty.");
                                        continue;
                                    }
                                    else if (cs == Node.CompletionStatus.NO_MATCH_FOUND)
                                    {
                                        Console.WriteLine("*no matches found*");
                                        continue;
                                    }

                                    if (checkResult.HasFlag(RCStatus.SEARCH_DESC))
                                    {
                                        wordMatches = wordMatches.OrderByDescending(str => str).ThenByDescending(str => str.Length).ToList();
                                    }
                                    else
                                    {
                                        wordMatches = wordMatches.OrderBy(str => str).ThenBy(str => str.Length).ToList();
                                    }

                                    int counter = 1; int matchesQuantity = wordMatches.Count();
                                    int maxLen = wordMatches.ToArray().GetMaxStringLength();
                                    foreach (string match in wordMatches)//(int id = 0; id < wordMatches.Count(); id++)
                                    {
                                        Console.Write(match.PadRight(maxLen + 1));
                                        if (counter % 5 == 0 && counter != matchesQuantity) Console.WriteLine();
                                        counter++;
                                    }
                                    Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine($"No trie with the name \"{commandInArgs[treeIndex]}\" exists.");
                                }
                            }
                        }
                        else if (checkResult.HasFlag(RCStatus.SEARCH_WHERE_BETWEEN))
                        {
                            string startPattern = commandInArgs[lastTreeIndex + 3].Replace("\"", "");
                            string endPattern = commandInArgs[lastTreeIndex + 4].Replace("\"", "");
                            for (int treeIndex = 2 - ((checkResult.HasFlag(RCStatus.HAS_RANGE)) ? 0 : 1); treeIndex < lastTreeIndex + 1; treeIndex++)
                            {
                                if (TrieSet.ContainsKey(commandInArgs[treeIndex]))
                                {
                                    wordMatches.Clear();
                                    Console.WriteLine($"Matches for \"{commandInArgs[treeIndex]}\"");
                                    cs = TrieSet[commandInArgs[treeIndex]].GetMatchesList(ref wordMatches, startPattern, endPattern);

                                    if (cs == (Node.CompletionStatus.NULL_OR_EMPTY_PARAM | Node.CompletionStatus.INVALID_PARAMS))
                                    {
                                        Console.WriteLine("Error: null, empty or invalid parameter.");
                                        break;
                                    }
                                    else if (cs == Node.CompletionStatus.EMPTY_TREE)
                                    {
                                        Console.WriteLine("The tree is empty.");
                                        continue;
                                    }
                                    else if (cs == Node.CompletionStatus.NO_MATCH_FOUND)
                                    {
                                        Console.WriteLine("*no matches found*");
                                        continue;
                                    }

                                    if (checkResult.HasFlag(RCStatus.SEARCH_DESC))
                                    {
                                        wordMatches = wordMatches.OrderByDescending(str => str).ThenByDescending(str => str.Length).ToList();
                                    }
                                    else
                                    {
                                        wordMatches = wordMatches.OrderBy(str => str).ThenBy(str => str.Length).ToList();
                                    }

                                    int counter = 1; int matchesQuantity = wordMatches.Count();
                                    int maxLen = wordMatches.ToArray().GetMaxStringLength();
                                    foreach (string match in wordMatches)//(int id = 0; id < wordMatches.Count(); id++)
                                    {
                                        Console.Write(match.PadRight(maxLen + 1));
                                        if (counter % 5 == 0 && counter != matchesQuantity) Console.WriteLine();
                                        counter++;
                                    }
                                    Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine($"No trie with the name \"{commandInArgs[treeIndex]}\" exists.");
                                }
                            }
                        }
                        break;
                    }

                case "PRINT_TREE":
                    {
                        foreach (string treeName in commandInArgs.Skip(1 + ((checkResult.HasFlag(RCStatus.HAS_RANGE)) ? 1 : 0))) //XD
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
                
                case "LIST_EXISTING_TREES":
                    {
                        var treeNames = TrieSet.Keys.ToArray();
                        int maxLen = treeNames.GetMaxStringLength();
                        int counter = 1; int matchesQuantity = treeNames.Length;
                        foreach(string treeName in treeNames)
                        {
                            Console.Write($"{treeName.PadRight(maxLen + 1)}");
                            if(counter % 5 == 0 && counter != matchesQuantity) Console.WriteLine();
                            counter++;    
                        }
                        if(counter == 0)
                        {
                            Console.WriteLine("*no trees yet exist*");
                        }
                        else Console.WriteLine();
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

            string[] testWords = {"SomeText", "Some", "m", "SomeTextAnd", "SomeTextAndOther", "SomeTextOr", "Different", "SomeTextAndOt", "Diff",
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

