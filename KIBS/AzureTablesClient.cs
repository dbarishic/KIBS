using Microsoft.WindowsAzure.Storage;
using System;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KIBS
{
    public class AzureTablesClient
    {
        private static readonly int EXECUTION_LIMIT = 4;
        private static readonly CloudStorageAccount _storage = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AZURE_TABLES_CONNECTION_STRING"));
        private static readonly CloudTableClient _tableClient = _storage.CreateCloudTableClient();
        private static readonly CloudTable _table = _tableClient.GetTableReference("ExecutionLimiting");

        async public Task<bool> CheckUpdateExecutionLimit(string dateExecuted)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<ExecutionOperationEntity>(dateExecuted, dateExecuted);
            TableResult retrievedResult = await _table.ExecuteAsync(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                if (((ExecutionOperationEntity)retrievedResult.Result).ExecutionCount >= EXECUTION_LIMIT)
                    return true; //execution limit reached for the day

                ExecutionOperationEntity updateEntity = (ExecutionOperationEntity)retrievedResult.Result;
                updateEntity.ExecutionCount++;
                TableOperation updateOperation = TableOperation.Replace(updateEntity);
                await _table.ExecuteAsync(updateOperation);
            }
            else
            {
                ExecutionOperationEntity executionOperation = new ExecutionOperationEntity(dateExecuted);
                TableOperation insertOperation = TableOperation.Insert(executionOperation);
                await _table.ExecuteAsync(insertOperation);        
            }

            return false;
        }
    }
}
