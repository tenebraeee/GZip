using GZip.Enums;
using GZip.Models.Common;
using GZip.Operations;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GZip
{
    class Program
    {
        static readonly OperationDeterminator operationDeterminator;
        static Program()
        {
            operationDeterminator = new OperationDeterminator();

            operationDeterminator.TryAddOperation(OperationTypes.Compress, new CompressOperation());
            operationDeterminator.TryAddOperation(OperationTypes.Decompress, new DecompressOperation());
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

            try
            {
                var operation = operationDeterminator.Determinate(operationName);
                operation.Execute(options);
            }
            catch (Exception ex)
            {
                //todo: писать в логи
                return 1;
            }

            return 0;
        }

        private static Options GenerateOptionsFrom(string[] args)
        {
            string sourceFile = args[1].Trim();
            string targetFile = args[2].Trim();

            return new Options
            {
                BufferSize = 1024 * 1024 * 5,

                SourceFilePath = sourceFile,
                TargetFilePath = targetFile,
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

            string sourceFile = args[1].Trim();

            if (!File.Exists(sourceFile))
            {
                return $"ERROR!!! Source file not found. Search at {sourceFile}.";
            }

            return null;
        }

        private static string GenerateHelp()
        {
            var stringBuilder = new StringBuilder();

            var availableOperations = operationDeterminator.GetAvailableOperations();
            var availableOperationsString = string.Join(", ", availableOperations);

            stringBuilder.AppendLine("-----------------------------------------");
            stringBuilder.AppendLine($"operation        - {availableOperationsString}");
            stringBuilder.AppendLine($"source file path - relative/absolute path to source file");
            stringBuilder.AppendLine($"target file path - relative/absolute path to target file");
            stringBuilder.AppendLine("-----------------------------------------");

            return stringBuilder.ToString();
        }

    }
}
