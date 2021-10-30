namespace Templates
{ 
    public enum RegexCheckStatus //DONE +-
    {
        REGEX_SUCCESS = 1,
        REGEX_FAIL = 2,
        UNKNOWN_COMMAND = 3,
        NORMAL = 4,
        HAS_RANGE = 5
        //and other regex statuses for corresponding commands will have come
    }

    public struct StaticFields
    {
        public static readonly string[] ExistingCommands = {"MENU", "CREATE", "INSERT", "CONTAINS", "SEARCH", "PRINT_TREE", "PC", "CLEAR", "EXIT"};
        public static readonly string CommandMenu = "- CREATE [RANGE] set_name_1 [set_name_2 ...]\n"
                                                    + "- INSERT [RANGE] set_name_1 [set_name_2 ...] \"word_1\" [\"word_2\" ...]\n"
                                                    + "- CONTAINS [RANGE] set_name_1 [set_name_2 ...] \"word_1\" [\"word_2\" ...]\n"
                                                    + "- PRINT_TREE [RANGE] set_name_1 [set_name_2 ...]\n"
                                    //                + "- SEARCH set_name [WHERE query] [ASC | DESC]\n\n"
                                                    + "System operations:\n"
                                                    + "- MENU\n"
                                                    + "- CLEAR\n"
                                    //                + "- HELP 'command'\n"
                                                    + "- EXIT";
    }
}
