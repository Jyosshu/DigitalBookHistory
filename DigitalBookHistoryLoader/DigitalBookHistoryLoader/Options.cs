using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DigitalBookHistoryLoader
{
    class Options
    {
        [Value(0, Required = true, HelpText = "Hoopla Json history file to read.")]
        public string InputFile { get; set; }

        [Usage(ApplicationAlias = "DigitalBookHistoryLoader")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>()
                {
                    new Example("Reads Hoopla history json, and loads new books into the database", new Options { InputFile = "Hoopla_History_<date stamp>.json" })
                };
            }
        }
    }
}
