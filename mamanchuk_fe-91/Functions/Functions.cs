using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using RCStatus = Templates.RegexCheckStatus;

namespace Functions
{
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
            while (str[currentIndex] == ' ' || str[currentIndex] == '\t')
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

                case "CREATE":
                    {
                        commandTemplate = new Regex(@"^CREATE(\s|\t){1,}(\w){1,}(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.NORMAL;
                        commandTemplate = new Regex(@"^CREATE(\s|\t){1,}RANGE((\s|\t){1,}(\w){1,}){1,}(\s|\t){0,};$");
                        if(commandTemplate.IsMatch(cmdPrefix)) return RCStatus.HAS_RANGE;
                        else break;
                    }

                case "INSERT":
                    {
                        commandTemplate = new Regex(@"^INSERT(\s|\t){1,}(\w){1,}(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.NORMAL;
                        commandTemplate = new Regex(@"^INSERT(\s|\t){1,}RANGE(\s|\t){1,}((\w){1,}(\s|\t){1,}){1,}(""(\w|\s){1,}""(\s|\t){0,}){1,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.HAS_RANGE;
                        else break;
                    }

                case "CONTAINS":
                    {
                        commandTemplate = new Regex(@"^CONTAINS(\s|\t){1,}(\w){1,}(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.NORMAL;
                        commandTemplate = new Regex(@"^CONTAINS(\s|\t){1,}RANGE(\s|\t){1,}((\w){1,}(\s|\t){1,}){1,}(""(\w|\s){1,}""(\s|\t){0,}){1,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.HAS_RANGE;
                        else break;
                    }

                case "PRINT_TREE":
                    {
                        commandTemplate = new Regex(@"^(\s|\t){0,}PRINT_TREE(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,};$");
                        if (commandTemplate.IsMatch(cmdPrefix)) return RCStatus.NORMAL;
                        commandTemplate = new Regex(@"^PRINT_TREE(\s|\t){1,}RANGE((\s|\t){1,}(\w){1,}){1,}(\s|\t){0,};$");
                        if(commandTemplate.IsMatch(cmdPrefix)) return RCStatus.HAS_RANGE;
                        else break;
                    }

                case "":
                    {
                        break;
                    }

                case "PC":
                    {
                        commandTemplate = new Regex(@"^(\s|\t){0,}PC(\s|\t){1,}(\s|\t|\w|""){0,};$");
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
