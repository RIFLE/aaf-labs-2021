using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using RCStatus = Templates.RegexCheckStatus;

namespace Functions
{

    public static class StringArrayExtensions
    {
        public static int IndexOfWord(this string[] source, string toCheck, StringComparison comp)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Equals(toCheck, comp)) return i;
            }
            return -1;
        }

        public static int GetMaxStringLength(this string[] source)
        {
            int maxLen = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (maxLen < source[i].Length) maxLen = source[i].Length;
            }
            return maxLen;
        }
    }

    class Imitators
    {
        public static void PrintMenu()
        {
            Console.WriteLine("Operations available:\n" + Templates.StaticFields.CommandMenu);
        }
    }

    class Algorithms
    {

        public static int GetNextIndex(ref string str, int currentIndex) //returning next non-(any)whitespace index
        {
            while (str[currentIndex] == ' '
                   || str[currentIndex] == '\t'
                   || str[currentIndex] == ',')
            {
                currentIndex++;
                if (currentIndex == str.Length) break;
            }
            return currentIndex;
        }

        public static void GetWordsList(ref string buffer, ref List<string> wordsList)
        {
            string accumulator = "";
            for (int currentIndex = 0; currentIndex < buffer.Length;)
            {
                currentIndex = GetNextIndex(ref buffer, currentIndex);
                if (currentIndex >= buffer.Length) break;
                if (buffer[currentIndex] == '\"')
                {
                    accumulator += buffer[currentIndex];
                    currentIndex++;
                    while (buffer[currentIndex] != '\"')
                    {
                        accumulator += buffer[currentIndex];
                        currentIndex++;
                    }
                    accumulator += buffer[currentIndex];
                    currentIndex++;
                }
                else //buffer[i] != "
                {
                    while (buffer[currentIndex] != ' ' && buffer[currentIndex] != '\"')
                    {
                        accumulator += buffer[currentIndex];
                        currentIndex++;
                        if (currentIndex == buffer.Length) break;
                    }
                }
                if (!(accumulator.Contains('\"') && (accumulator.Length == 2)))
                    wordsList.Add(new string(accumulator.ToArray()));
                accumulator = "";
            }
            return;
        }

        public static void GetCommand(ref string commandStr)
        {
            string _charArr = "[]{}|/+^%$#@!()\\"; //prohibited characters to use in command
            bool _charCheck = false;

            do
            {
                Console.Write(">> ");

                commandStr = Console.ReadLine();
                while (!commandStr.Contains(";"))
                {
                    commandStr += Console.ReadLine();
                }
                if (commandStr.Length > (commandStr.IndexOf(';') + 1))
                {
                    commandStr = commandStr.Remove(commandStr.IndexOf(';') + 1);
                }

                if (((commandStr.Split('\"').Length - 1) % 2) != 0) // :D
                {
                    Console.WriteLine("Syntax error: each char [\"] has to be paired.");
                    continue;
                }

                foreach (char _char in _charArr)
                {
                    if (commandStr.Contains(_char))
                    {
                        Console.WriteLine("Syntax error: forbidden character [{0}].", _char);
                        break;
                    }
                    else
                    {
                        if (_char == '\\') _charCheck = true;
                        else continue;
                    }
                }
            }
            while (!_charCheck);
            return;
        }

        public static Templates.RegexCheckStatus RegexCheck(ref string commandStr)
        {
            string cmdPrefix = (commandStr.TrimStart('\t', ' ')).ToUpper();
            string prefix = "";
            for (int charId = 0; (charId < cmdPrefix.Length)
                                && (cmdPrefix[charId] != '\t')
                                && (cmdPrefix[charId] != '\"')
                                && (cmdPrefix[charId] != ' ')
                                && (cmdPrefix[charId] != ';');
                                                                charId++)
            {
                prefix += cmdPrefix[charId];
            }

            Regex commandTemplate;

            switch (prefix)
            {
                case "MENU":
                    {
                        commandTemplate = new Regex(@"^MENU(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.REGEX_SUCCESS;
                        else break;
                    }

                case "LIST_EXISTING_TREES":
                    {
                        commandTemplate = new Regex(@"^LIST_EXISTING_TREES(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.REGEX_SUCCESS;
                        else break;
                    }

                case "CREATE":
                    {
                        commandTemplate = new Regex(@"^CREATE(\s|\t){1,}(\w){1,}(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.NORMAL | RCStatus.REGEX_SUCCESS;
                        commandTemplate = new Regex(@"^CREATE(\s|\t){1,}RANGE((\s|\t){1,}(\w){1,}){1,}(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.HAS_RANGE | RCStatus.REGEX_SUCCESS;
                        else break;
                    }

                case "INSERT":
                    {
                        commandTemplate = new Regex(@"^INSERT(\s|\t){1,}(\w){1,}(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.NORMAL | RCStatus.REGEX_SUCCESS;
                        commandTemplate = new Regex(@"^INSERT(\s|\t){1,}RANGE(\s|\t){1,}((\w){1,}(\s|\t){1,}){1,}(""(\w|\s){1,}""(\s|\t){0,}){1,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.HAS_RANGE | RCStatus.REGEX_SUCCESS;
                        else break;
                    }

                case "CONTAINS":
                    {
                        commandTemplate = new Regex(@"^CONTAINS(\s|\t){1,}(\w){1,}(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.NORMAL | RCStatus.REGEX_SUCCESS;
                        commandTemplate = new Regex(@"^CONTAINS(\s|\t){1,}RANGE(\s|\t){1,}((\w){1,}(\s|\t){1,}){1,}(""(\w|\s){1,}""(\s|\t){0,}){1,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.HAS_RANGE | RCStatus.REGEX_SUCCESS;
                        else break;
                    }

                case "PRINT_TREE":
                    {
                        commandTemplate = new Regex(@"^PRINT_TREE(\s|\t){1,}(\w){1,}(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.NORMAL | RCStatus.REGEX_SUCCESS;
                        commandTemplate = new Regex(@"^PRINT_TREE(\s|\t){1,}RANGE((\s|\t){1,}(\w){1,}){1,}(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.HAS_RANGE | RCStatus.REGEX_SUCCESS;
                        else break;
                    }

                case "SEARCH":
                    {
                        commandTemplate = new Regex(@"^SEARCH(\s|\t){1,}(\w){1,}((\s|\t){0,}|(\s|\t){1,}(ASC|DESC))(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.NORMAL | RCStatus.REGEX_SUCCESS;
                        commandTemplate = new Regex(@"^SEARCH(\s|\t){1,}(\w){1,}(\s|\t){1,}WHERE(\s|\t){1,}BETWEEN(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,},(\s|\t){0,}""(\w|\s){1,}""((\s|\t){0,}|((\s|\t){1,}(ASC|DESC)(\s|\t){0,}));$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.SEARCH_WHERE_BETWEEN | RCStatus.REGEX_SUCCESS;
                        commandTemplate = new Regex(@"^SEARCH(\s|\t){1,}(\w){1,}(\s|\t){1,}WHERE(\s|\t){1,}MATCH(\s|\t){1,}""(\w|\s|\?){1,}\*?""((\s|\t){0,}|((\s|\t){1,}(ASC|DESC)(\s|\t){0,}));$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.SEARCH_WHERE_MATCH | RCStatus.REGEX_SUCCESS;

                        commandTemplate = new Regex(@"^SEARCH(\s|\t){1,}RANGE(\s|\t){1,}(\w){1,}((\s|\t){1,}(\w){1,}){0,}((\s|\t){0,}|((\s|\t){1,}(ASC|DESC)(\s|\t){0,}));$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.HAS_RANGE | RCStatus.REGEX_SUCCESS;
                        commandTemplate = new Regex(@"^SEARCH(\s|\t){1,}RANGE(\s|\t){1,}((\w){1,}(\s|\t){1,}){1,}WHERE(\s|\t){1,}BETWEEN(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,},(\s|\t){0,}""(\w|\s){1,}""((\s|\t){0,}|((\s|\t){1,}(ASC|DESC)(\s|\t){0,}));$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.SEARCH_WHERE_BETWEEN | RCStatus.HAS_RANGE | RCStatus.REGEX_SUCCESS;
                        commandTemplate = new Regex(@"^SEARCH(\s|\t){1,}RANGE(\s|\t){1,}((\w){1,}(\s|\t){1,}){1,}WHERE(\s|\t){1,}MATCH(\s|\t){1,}""(\w|\s|\?){1,}\*?""((\s|\t){0,}|((\s|\t){1,}(ASC|DESC)(\s|\t){0,}));$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.SEARCH_WHERE_MATCH | RCStatus.HAS_RANGE | RCStatus.REGEX_SUCCESS;

                        break;
                    }

                case "PC":
                    {
                        commandTemplate = new Regex(@"^(\s|\t){0,}PC(\s|\t){1,}(\s|\t|\w|""|,){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.REGEX_SUCCESS;
                        else break;
                    }

                case "CLEAR":
                    {
                        commandTemplate = new Regex(@"^(\s|\t){0,}CLEAR(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.REGEX_SUCCESS;
                        else break;
                    }

                case "EXIT":
                    {
                        commandTemplate = new Regex(@"^(\s|\t){0,}EXIT(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.REGEX_SUCCESS;
                        else break;
                    }
            }

            if (!Templates.StaticFields.ExistingCommands.Contains(prefix))
            {
                return RCStatus.UNKNOWN_COMMAND;
            }
            else return RCStatus.REGEX_FAIL;
        }
    }
}
