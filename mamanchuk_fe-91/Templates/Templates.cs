using System;

namespace Templates
{ 
    public enum RegexCheckStatus //DONE +-
    {
        REGEX_SUCCESS = 1,
        REGEX_FAIL = 2,
        UNKNOWN_COMMAND = 3,
        //and other regex statuses for corresponding commands will have come
    }

    class StaticFields
    {
        public static readonly string[] existingCommands = {"MENU", "CREATE", "INSERT", "CONTAINS", "SEARCH", "PC", "CLEAR", "EXIT"};
    }
}
