using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PictureSaver
{
    public enum CommandType
    {
        Path,
        Destination
    }

    public class Command
    {
        private int argCount;
        private CommandType type;
        private string value;
        private List<string> args;

        public int ArgCount { get { return argCount; } }
        public CommandType Type { get { return type; } }
        public string Value { get { return value; } }
        public List<string> Args { get { return args; } }
        public int RemainingArgCount { get { return argCount - args.Count; } }

        public Command(string str)
        {
            value = str;
            args = new List<string>();
            switch (str)
            {
                case "--destination":
                case "-d":
                    argCount = 1;
                    type = CommandType.Destination;
                    break;
                default:
                    argCount = 0;
                    type = CommandType.Path;
                    break;
            }
        }

        public void AddArg(string arg)
        {
            args.Add(arg);
        }
    }
}
