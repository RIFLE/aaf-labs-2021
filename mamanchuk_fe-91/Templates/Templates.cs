namespace Templates
{
    [System.Flags]
    public enum RegexCheckStatus
    {
        UNKNOWN_COMMAND = 0b_0000_0000,
        REGEX_SUCCESS = 0b_1000_0000,
        REGEX_FAIL = 0b_0100_0000,
        NORMAL = 0b_0010_0000,
        HAS_RANGE = 0b_0001_0000,
        SEARCH_ASC = 0b_0000_1000,
        SEARCH_DESC = 0b_0000_0100,
        SEARCH_WHERE_MATCH = 0b_0000_0010,
        SEARCH_WHERE_BETWEEN = 0b_0000_0001,
        MAX_MASK = 0b_1111_1111
    }

    public struct StaticFields
    {
        public static readonly string[] ExistingCommands = { "MENU", "CREATE", "INSERT", "CONTAINS", "SEARCH", "PRINT_TREE", "PC", "CLEAR", "EXIT" };
        public static readonly string CommandMenu = "- CREATE [RANGE] set_name_1 [set_name_2 ...]\n"
                                                    + "- INSERT [RANGE] set_name_1 [set_name_2 ...] \"word_1\" [\"word_2\" ...]\n"
                                                    + "- CONTAINS [RANGE] set_name_1 [set_name_2 ...] \"word_1\" [\"word_2\" ...]\n"
                                                    + "- SEARCH [RANGE] set_name_1 [set_name_2 ...] [WHERE query] [ASC | DESC]\n"
                                                    + "         query:= WHERE BETWEEN \"from\",\"to\"  \\or\\  WHERE MATCH \"word\"\n"
                                                    + "- PRINT_TREE [RANGE] set_name_1 [set_name_2 ...]\n"
                                                    + "- LIST_EXISTING_TREES\n"
                                                    + "System operations:\n"
                                                    + "- MENU\n"
                                                    + "- CLEAR\n"
                                                    //    + "- HELP 'command'\n"
                                                    + "- EXIT";
    }
}
