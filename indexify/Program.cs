using System;
using System.Collections.Generic;
using indexify.Models;

namespace indexify
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdLineModel = new CommandLineModel();
            var commandLineWithoutBinaryReference = new List<string>();
            commandLineWithoutBinaryReference.AddRange(Environment.GetCommandLineArgs());
            commandLineWithoutBinaryReference.RemoveAt(0);

            var parseCmdLine = cmdLineModel.ConstructCommandLineModel(commandLineWithoutBinaryReference.ToArray());
            if (parseCmdLine == CommandLineModel.ParseCommandLineStatus.DisplayHelp)
            {
                foreach (var line in cmdLineModel.HelpInfo)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                foreach (var line in cmdLineModel.ErrorInfo)
                {
                    Console.WriteLine(line);
                }
                if (parseCmdLine == CommandLineModel.ParseCommandLineStatus.DisplayError)
                {
                    Environment.Exit(1);
                }

                Console.WriteLine("");
                indexify.Execute(cmdLineModel);
                Console.Read();
            }
        }
    }
}
