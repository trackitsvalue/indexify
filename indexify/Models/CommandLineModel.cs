using System;
using System.Collections.Generic;
using System.IO;
using tiv.elasticClient.ExtensionFunctions;

namespace indexify.Models
{
    class CommandLineModel
    {
        #region Properties

        public string SourceIndexPattern { get; private set; }

        public string DestinationIndex { get; private set; }

        public bool DestIsSource { get; private set; }

        public bool NoDestinationJustDelete { get; private set; }

        public int SourceMinusNumChars { get; private set; }

        public string SourcePlusString { get; private set; }

        public int ScrollCount { get; private set; }

        public bool DryRun { get; private set; }

        public string Url { get; private set; }

        public bool UseBasicAuth { get; private set; }

        public string UserName { get; private set; }
        public string Password { get; private set; }

        public bool Verbose { get; private set; }

        public bool CopyOnly { get; private set; }

        public FileInfo LogFile { get; private set; }

        private List<string> __helpInfo;
        public List<string> HelpInfo
        {
            get
            {
                if (__helpInfo == null)
                {
                    __helpInfo = new List<string>
                    {
                        @"Will reindex a single or series of source indexes based on the passed index pattern into a destination index.",
                        @"",
                        @"INDEXIFY ""url"" ""/src=index-pattern"" ""/dest=index"" | /dest-is-src-minus=NUMBER | /dest-is-src-plus=STRING | /no-dest-delete-source",
                        @"            [/ba=USERNAME:PASSWORD] [/v] [/olf=ouput-logfile] [/copyonly] [/dryrun] [/scrollcount=NUMBER]",
                        @"",
                        @"  ""url""                           The full url to find the elastic instance at, i.e.:",
                        @"                                  https://servername.domain.com:9200",
                        @"",
                        @"  ""/src=index-pattern""            The source index or pattern to search for, can include *'s for wildcards.",
                        @"",
                        @"  ""/dest=index-name""              The destination index name to write to.",
                        @"",
                        @"                                  OR",
                        @"",
                        @"  /dest-is-src-minus=NUMBER       The destination index name should be the source index name minus some number of",
                        @"                                  characters.",
                        @"",
                        @"                                  OR",
                        @"",
                        @"  /dest-is-src-plus=STRING        The destination index name should be the source index name plus the passed string.",
                        @"",
                        @"                                  OR",
                        @"",
                        @"  /no-dest-delete-source          Just delete the source indexes that match the specified criteria.",
                        @"",
                        @"  /ba=USERNAME:PASSWORD           The basic authentication user and password if needed.",
                        @"",
                        @"  /lf=logfile                     The log file to write results of the operations into, i.e.:",
                        @"                                  C:\reindexify\logs.log",
                        @"                                  It can also contain a datetime format for a string format parameter 0 which can be",
                        @"                                  used to insert a format of DateTime.Now into the file name or directory structure,  i.e.:",
                        @"                                  c:\reindexify\logs-{0:yyyy-MM-dd}.log",
                        @"",
                        @"  /v                              If specified, display verbose logging to the",
                        @"                                  console.",
                        @"",
                        @"  /copyonly                       If specified, the index rewriting will not delete the source index on complete",
                        @"                                  and successful copying of all documents.",
                        @"                                  NOTE: Not relevant when option is /no-dest-delete-source.",
                        @"",
                        @"  /scrollcount=NUMBER             If specified, the index rewriting will NOT use the Reindex API and will instead use the",
                        @"                                  Search API with scrolling and the Bulk API for the copy and also potentially the deletes.",
                        @"                                  This is a better option if the document counts are causing timeout exceptions during reindex",
                        @"                                  operations.",
                        @"                                  NOTE: Not relevant when option is /no-dest-delete-source.",
                        @"",
                        @"  /dryrun                         If specified, this will be a dry run only and no changes will be made.",
                    };
                }

                return (__helpInfo);

            }

        }

        private List<string> __errorInfo = new List<string>();
        public List<string> ErrorInfo
        {
            get { return (__errorInfo); }
        }

        #endregion

        #region Methods

        public enum ParseCommandLineStatus
        {
            DisplayHelp,
            DisplayError,
            ExecuteProgram
        }

        public ParseCommandLineStatus ConstructCommandLineModel(string[] args)
        {
            try
            {
                if (args.GetLength(0) == 0 || args[0].CompareNoCase("/?"))
                {
                    return ParseCommandLineStatus.DisplayHelp;
                }

                if (args.GetLength(0) < 3)
                {
                    throw new ArgumentException(@"URL, Source index pattern, and destination index settings are required parameters.");
                }
                Url = args[0];
                for (var inc = 1; inc < args.GetLength(0); inc++)
                {
                    ProcessCommandLineParamter(args[inc]);
                }

                if (SourceIndexPattern.IsNullOrEmpty())
                {
                    throw new ArgumentException(@"Source index pattern is a required parameter.");
                }

                if (DestinationIndex.IsNullOrEmpty() && !DestIsSource && !NoDestinationJustDelete)
                {
                    throw new ArgumentException(@"Destination index settings are required parameter(s).");
                }

                return ParseCommandLineStatus.ExecuteProgram;
            }
            catch (Exception ex)
            {
                ErrorInfo.Add(ex.Message + Environment.NewLine + Environment.NewLine + @"Use /? to see help information about this program.");
                return ParseCommandLineStatus.DisplayError;
            }
        }

        private void ProcessCommandLineParamter(string commandLineEntry)
        {
            if (commandLineEntry.CompareNoCase("/v"))
            {
                Verbose = true;
                return;
            }

            if (commandLineEntry.CompareNoCase("/dryrun"))
            {
                DryRun = true;
                return;
            }

            if (commandLineEntry.CompareNoCase("/copyonly"))
            {
                CopyOnly = true;
                return;
            }
            
            if (commandLineEntry.CompareNoCase("/no-dest-delete-source"))
            {
                NoDestinationJustDelete = true;
                return;
            }

            if (commandLineEntry.ToLower().StartsWith("/ba="))
            {
                UseBasicAuth = true;
                var auth = commandLineEntry.Substring(4).Split(':');
                if(auth.Length != 2) throw new ArgumentException("/ba parameter does not contain a valid basic authentication value.");
                UserName = auth[0];
                Password = auth[1];
                return;
            }

            if (commandLineEntry.ToLower().StartsWith("/lf="))
            {
                LogFile = new FileInfo(string.Format(commandLineEntry.Substring(4), DateTime.Now));
                return;
            }

            if (commandLineEntry.ToLower().StartsWith("/scrollcount="))
            {
                int value;
                if (int.TryParse(commandLineEntry.Substring(13), out value) && value > 0)
                {
                    ScrollCount = value;
                }
                return;
            }

            if (commandLineEntry.ToLower().StartsWith("/src="))
            {
                SourceIndexPattern = commandLineEntry.Substring(5);
                return;
            }

            if (commandLineEntry.ToLower().StartsWith("/dest="))
            {
                DestinationIndex = commandLineEntry.Substring(6);
                return;
            }

            if (commandLineEntry.ToLower().StartsWith("/dest-is-src-minus="))
            {
                DestIsSource = true;
                SourceMinusNumChars = commandLineEntry.Substring(19).ToInt();
                if (SourceMinusNumChars < 0) SourceMinusNumChars = SourceMinusNumChars*-1;
                return;
            }

            if (commandLineEntry.ToLower().StartsWith("/dest-is-src-plus="))
            {
                DestIsSource = true;
                SourcePlusString = commandLineEntry.Substring(18);
                return;
            }

            ErrorInfo.Add(string.Format("Unrecognized command line entry '{0}'", commandLineEntry));
        }
        #endregion
    }
}