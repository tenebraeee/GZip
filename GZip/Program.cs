using GZip.Enums;
using GZip.Models.Common;
using GZip.Operations;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GZip
{
    class Program
    {
        static readonly OperationDeterminator operationDeterminator;
        static readonly ManualResetEvent manualResetEvent;
        static int exitCode = 0;

        static Program()
        {
            manualResetEvent = new ManualResetEvent(false);

            operationDeterminator = new OperationDeterminator();

            operationDeterminator.TryAddOperation(OperationTypes.Compress, new CompressOperation(OnError, manualResetEvent));
            operationDeterminator.TryAddOperation(OperationTypes.Decompress, new DecompressOperation(OnError, manualResetEvent));
        }

        static int Main(string[] args)
        {
            var errorMessage = Validate(args);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                Console.WriteLine(errorMessage);

                var helpText = GenerateHelp();
                Console.WriteLine(helpText);

                return 1;
            }

            var options = GenerateOptionsFrom(args);

            string operationName = args[0].Trim();

            var operation = operationDeterminator.Determinate(operationName);

            operation.Execute(options);

            manualResetEvent.WaitOne();

            return exitCode;
        }

        private static Options GenerateOptionsFrom(string[] args)
        {
            string sourceFilePath = args[1].Trim();
            string targetFilePath = args[2].Trim();

            return new Options
            {
                BufferSize = 1024 * 1024 * 50,

                SourceFilePath = sourceFilePath,
                TargetFilePath = targetFilePath,
            };
        }

        private static string Validate(string[] args)
        {
            if (args == null || args.Count() < 3)
            {
                return "ERROR!!! Specify all args and try again!";
            }

            var operationName = args[0].Trim();
            var isAvailableOperation = operationDeterminator.GetAvailableOperations().Contains(operationName);
            if (!isAvailableOperation)
            {
                return "ERROR!!! Operation not specified!";
            }

            return null;
        }

        private static string GenerateHelp()
        {
            var stringBuilder = new StringBuilder();

            var availableOperations = operationDeterminator.GetAvailableOperations();
            var availableOperationsString = string.Join(", ", availableOperations);

            stringBuilder.AppendLine("-----------------------------------------");
            stringBuilder.AppendLine("Run example: ");
            stringBuilder.AppendLine($"{Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName)} operation source target");
            stringBuilder.AppendLine($"operation - {availableOperationsString}");
            stringBuilder.AppendLine($"source    - relative/absolute path to source file");
            stringBuilder.AppendLine($"target    - relative/absolute path to target file (extension will be skiped)");
            stringBuilder.AppendLine("-----------------------------------------");

            return stringBuilder.ToString();
        }

        private static void OnError(string errorMessage)
        {
            exitCode = 1;
            Console.WriteLine(errorMessage);
        }

    }
}
