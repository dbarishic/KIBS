using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace KIBS
{
    public class ExecutionOperationEntity : TableEntity
    {
        public ExecutionOperationEntity(string dateExecuted)
        {
            PartitionKey = dateExecuted; // MMddyyyy
            RowKey = dateExecuted;
            ExecutionDate = PartitionKey;
            ExecutionCount = 1;
        }

        public ExecutionOperationEntity() { }

        public string ExecutionDate { get; set; }
        public int ExecutionCount { get; set; }
    }
}
