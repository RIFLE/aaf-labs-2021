using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using PrefixTrieLibrary;
using Templates;
using RCS = Templates.RegexCheckStatus;

namespace Functions
{
    class Imitators
    {
        public static void PrintMenu()
        {
            Console.WriteLine("Operations available:\n"
                            + "- CREATE 'set_name'\n" 
                            + "- INSERT 'set_name' 'value'\n"
            //                + "- PRINT_TREE 'set_name'\n"
                            + "- CONTAINS 'set_name' 'value'\n"
            //                + "- SEARCH set_name [WHERE query] [ASC | DESC]\n\n"
                            + "System operations:\n"
                            + "- MENU\n"
                            + "- CLEAR\n"
            //                + "- HELP 'command'\n"
                            + "- EXIT");
        }
    }
    class Algorithms
    {
        public static int GetNextIndex(ref string str, int currentIndex) //returning next non-(any)whitespace index
        {
            while(str[currentIndex] == ' ' || str[currentIndex] == '\t')
            {
                currentIndex++;
                if(currentIndex == str.Length) break;
            }
            return currentIndex;
        } 
        public static void GetWordsList(ref string buffer, ref List<string> wordsList)
        {
            string accumulator = "";
            int currentIndex = 0;
            for(;currentIndex < buffer.Length;)
            {
                currentIndex = GetNextIndex(ref buffer, currentIndex);
                if(currentIndex >= buffer.Length) break;
                if(buffer[currentIndex] == '\"')
                {
                    accumulator += buffer[currentIndex];
                    currentIndex++;
                    while(buffer[currentIndex] !='\"')
                    {
                        accumulator += buffer[currentIndex];
                        currentIndex++;
                    }
                    accumulator += buffer[currentIndex];
                    currentIndex++;
                }
                else //buffer[i] != "
                {
                    while(buffer[currentIndex] != ' ' && buffer[currentIndex] != '\"')
                    {
                        accumulator += buffer[currentIndex];
                        currentIndex++;
                        if(currentIndex == buffer.Length) break;
                    }
                }
                if( !(accumulator.Contains('\"') && (accumulator.Length == 2)) )
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
                if(commandStr.Length > (commandStr.IndexOf(';')+1) )
                {
                    commandStr = commandStr.Remove(commandStr.IndexOf(';')+1);
                }
                
                if(((commandStr.Split('\"').Length - 1) % 2) != 0) // :D
                {
                    Console.WriteLine("Error: each char [\"] has to be paired (bad syntax)");
                    continue; 
                }

                foreach(char _char in _charArr)
                {
                    if(commandStr.Contains(_char))
                    {
                        Console.WriteLine("Forbidden character [{0}]", _char);
                        break;
                    }
                    else
                    {
                        if(_char == '\\') _charCheck = true;
                        else continue;
                    }
                }
            }
            while(!_charCheck);
            return;
        } 
        public static RegexCheckStatus RegexCheck(ref string commandStr)
        {
            string cmdPrefix = (commandStr.TrimStart('\t',' ')).ToUpper();
            string prefix = "";
            for(int charId=0;      (charId < cmdPrefix.Length)
                                && (cmdPrefix[charId] != '\t')
                                && (cmdPrefix[charId] != '\"')
                                && (cmdPrefix[charId] != ' ')
                                && (cmdPrefix[charId] != ';');
                                                                charId++)
            {
                prefix += cmdPrefix[charId];
            }

            Regex commandTemplate;

            switch(prefix)
            {
                case "MENU":
                {
                    commandTemplate = new Regex(@"^(\s|\t){0,}MENU(\s|\t){0,};$", RegexOptions.IgnoreCase);
                    if(commandTemplate.IsMatch(cmdPrefix)) return RCS.REGEX_SUCCESS;
                    else break;
                }
                
                case "CREATE":
                {
                    commandTemplate = new Regex(@"^(\s|\t){0,}CREATE(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,};$", RegexOptions.IgnoreCase);
                    if(commandTemplate.IsMatch(cmdPrefix)) return RCS.REGEX_SUCCESS;
                    else break;
                }
                
                case "INSERT":
                {
                    commandTemplate = new Regex(@"^(\s|\t){0,}INSERT(\s|\t){1,}""(\w|\s){1,}""(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,};$", RegexOptions.IgnoreCase);
                    if(commandTemplate.IsMatch(cmdPrefix)) return RCS.REGEX_SUCCESS;
                    else break;
                }

                case "CONTAINS":
                {
                    commandTemplate = new Regex(@"^(\s|\t){0,}CONTAINS(\s|\t){1,}""(\w|\s){1,}""(\s|\t){1,}""(\w|\s){1,}""(\s|\t){0,};$",RegexOptions.IgnoreCase);
                    if(commandTemplate.IsMatch(cmdPrefix)) return RCS.REGEX_SUCCESS;
                    else break;
                }
                
                case "":
                {
                    break;
                }
                
                case "pc":
                {
                    commandTemplate = new Regex(@"^(\s|\t){0,}pc(\s|\t){0,};$", RegexOptions.IgnoreCase);
                    if(commandTemplate.IsMatch(cmdPrefix)) return RCS.REGEX_SUCCESS;
                    else break;
                }
                
                case "CLEAR":
                {
                    commandTemplate = new Regex(@"^(\s|\t){0,}CLEAR(\s|\t){0,};$", RegexOptions.IgnoreCase);
                    if(commandTemplate.IsMatch(cmdPrefix)) return RCS.REGEX_SUCCESS;
                    else break;
                }
                
                case "EXIT":
                {
                    commandTemplate = new Regex(@"^(\s|\t){0,}EXIT(\s|\t){0,};$", RegexOptions.IgnoreCase);
                    if(commandTemplate.IsMatch(cmdPrefix)) return RCS.REGEX_SUCCESS;
                    else break;
                }
            }
            
            if(!Templates.StaticFields.existingCommands.Contains(prefix))
            {
                return RCS.UNKNOWN_COMMAND;
            }
            else return RCS.REGEX_FAIL;
        }
    }
}
