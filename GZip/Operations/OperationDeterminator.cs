using GZip.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZip.Operations
{
    public class OperationDeterminator
    {

        IDictionary<OperationTypes, IOperation> operations;

        public OperationDeterminator()
        {
            operations = new Dictionary<OperationTypes, IOperation>();
        }

        public IOperation Determinate(string operationName)
        {
            if (Enum.TryParse(operationName, true, out OperationTypes operationType) &&
                operations.TryGetValue(operationType, out IOperation operation))
            {
                return operation;
            }
            
            return null;
        }

        public bool TryAddOperation(OperationTypes operationType, IOperation operation)
        {
            return operations.TryAdd(operationType, operation);
        }

        public IEnumerable<string> GetAvailableOperations()
        {
            return operations.Keys.Select(k => k.ToString().ToLower());
        }

    }
}
