using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZoomersClient.Shared.Models;

namespace ZoomersClient.Shared.Data
{
    public class GameRepository
    {
        public ILogger<GameRepository> _logger { get; set; }

        public AppSettings _settings { get; set; }

        private const string TABLE_NAME = "Games";

        public GameRepository(ILogger<GameRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(_settings.StorageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }

            return storageAccount;
        }

        public async Task<Game> SaveAsync(Game game)
        {
            var storage = CreateStorageAccountFromConnectionString();

            var table = await CreateTableAsync();

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(game);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                Game insertedGame = result.Result as Game;

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return insertedGame;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<CloudTable> CreateTableAsync()
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString();

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            Console.WriteLine("Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(TABLE_NAME);
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}", TABLE_NAME);
            }
            
            return table;
        }
       
        public async Task<Game> GetAsync(Guid id) // : where T implements TableEntity
        {
            try
            {
                var table = await CreateTableAsync();

                TableOperation retrieveOperation = TableOperation.Retrieve<Game>("game", id.ToString());
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                Game game = result.Result as Game;
                
                // if (game != null)
                // {
                //     Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", game.PartitionKey, game.RowKey, game.Name, game.Voice);
                // }

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return game;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var table = await CreateTableAsync();

                var deleteEntity = await GetAsync(id);

                if (deleteEntity == null)
                {
                    throw new ArgumentNullException("deleteEntity");
                }

                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                TableResult result = await table.ExecuteAsync(deleteOperation);

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Delete Operation: " + result.RequestCharge);
                }

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}