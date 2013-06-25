using System.ComponentModel;

namespace WinDriver
{
    public enum InvalidRequest
    {
        [Description("Unknown Command")]
        UnknownCommand,

        [Description("Unimplemented Command")]
        UnimplementedCommand,

        [Description("Variable Resource Not Found")]
        VariableResourceNotFound,

        [Description("Invalid Command Method")]
        InvalidCommandMethod,

        [Description("Missing Command Parameter")]
        MissingCommandParameter, 
    }
}