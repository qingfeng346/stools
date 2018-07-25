using System;
using System.Collections.Generic;

public class CommandLineArgument {
    private List<string> args = new List<string>();
    public CommandLineArgument(string name) {
    }
    public void Add(string arg) {
        args.Add(arg);
    }
    public string[] GetValues() {
        return args.Count > 0 ? args.ToArray() : null;
    }
    public string GetValue() {
        return args.Count > 0 ? args[0] : null;
    }
}
public class CommandLine {
    public static CommandLine Parse(string[] args) {
        return new CommandLine(args);
    }
    private Dictionary<string, CommandLineArgument> arguments = new Dictionary<string, CommandLineArgument>();
    public CommandLine(string[] args) {
        CommandLineArgument argument = null;
        for (int i = 0; i < args.Length; ++i) {
            string arg = args[i];
            if (arg.StartsWith("-")) {
                if (arguments.ContainsKey(arg)) {
                    argument = arguments[arg];
                } else {
                    argument = new CommandLineArgument(arg);
                    arguments[arg] = argument;
                }
            } else if (argument != null) {
                argument.Add(arg);
            }
        }
    }
    public bool HadValue(string key) {
        return arguments.ContainsKey(key);
    }
    public string[] GetValues(string key) {
        return arguments.ContainsKey(key) ? arguments[key].GetValues() : null;
    }
    public string GetValue(string key) {
        return arguments.ContainsKey(key) ? arguments[key].GetValue() : null;
    }
}