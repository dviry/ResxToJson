using Croc.DevTools.ResxToJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToResx.Cli
{
    class Program
    {
        enum ExitCode
        {
            InputArgumentMissing = -1,
            InvalidOutputArgument = -2,
            CaseArgumentMissing = -3,
            InvalidInputPath = -4,
            OutputFormatArgumentMissing = -5,
            FallbackArgumentMissing = -6
        }

        static void CrashAndBurn(ExitCode code, string crashMessage, params object[] args)
        {
            // Preserve the foreground color
            var c = Console.ForegroundColor;

            // Write out our error message in bright red text
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + crashMessage, args);

            // Restore the foreground color
            Console.ForegroundColor = c;

            // Die!
            Environment.Exit((int)code);
        }

        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length == 1 && (args[0] == "-help" || args[0] == "-?"))
            {
                printHelp();
            }
            var options = getOptions(args);
            //checkOptions(options);

            ConverterLogger logger = JsonToResxConverter.Convert(options);
            foreach (var item in logger.Log)
            {
                ConsoleColor color;
                switch (item.Severity)
                {
                    case Severity.Trace:
                        color = ConsoleColor.DarkGray;
                        break;
                    case Severity.Info:
                        color = ConsoleColor.White;
                        break;
                    case Severity.Warning:
                        color = ConsoleColor.Yellow;
                        break;
                    case Severity.Error:
                        color = ConsoleColor.DarkRed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var backupColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(item.Message);
                Console.ForegroundColor = backupColor;
            }

#if DEBUG
            Console.ReadLine();
#endif
        }

        static JsonToResxConverterOptions getOptions(string[] args)
        {
            var options = new JsonToResxConverterOptions();
            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];
                if (key == "-i" || key == "-input")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.InputArgumentMissing, "Value for option 'input' is missing");
                    }
                    options.Inputs.Add(args[i + 1]);
                    i++;
                    continue;
                }

                if (key == "-dir" || key == "-outputDir")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.InvalidOutputArgument, "Value for option 'outputDir' is missing");
                    }
                    options.OutputFolder = args[i + 1];
                    i++;
                    continue;
                }

                if (key == "-file" || key == "-outputFile")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.InvalidOutputArgument, "Value for option 'outputFile' is missing");
                        Console.WriteLine("ERROR: Value for option 'outputFile' is missing");
                        Environment.Exit(-2);
                    }
                    options.OutputFile = args[i + 1];
                    i++;
                    continue;
                }

                if (key == "-fallback" || key == "-fallbackCulture")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.FallbackArgumentMissing, "Value for option 'fallbackCulture' is missing");
                    }
                    options.FallbackCulture = args[i + 1];
                    i++;
                    continue;
                }

                if (key == "-f" || key == "-force")
                {
                    options.Overwrite = OverwriteModes.Force;
                    continue;
                }
            }
            return options;
        }

        static void printHelp()
        {
            Console.WriteLine(
@"JsonToResx
A json to resx-resources converter for using with 18n plugin.
USAGE:
  -input or -i              - path to directory with *.resx files or to separate file 
                              HINT: there can be several such options specifed at once
  -outputDir or -dir        - path to output directory (where result resx files will be placed)
  -outputFile or -file      - filename to use (e.g. Resources.resx)
  -fallback                 - Which lang identifier to use per default (Default: en)
  -force or -f              - overwrite existing read-only files (by default read-only files will not be overwritten)
");
            Environment.Exit(0);
        }
    }
}
