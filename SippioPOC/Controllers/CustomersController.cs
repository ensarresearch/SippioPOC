using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using SippioPOC.Model;
using Microsoft.Extensions.Configuration;

namespace SippioPOC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        //private string databaseId = "Partners";
        //private string containerId = "Customers";

        Dictionary<int, int> Pairs = new Dictionary<int, int>();


        // The Cosmos client instance
        //private CosmosClient cosmosClient;

        // The database we will create
       // private Database database;

        // The container we will create.
        //private Container container;
        //private Tenant tenant;




        private readonly ILogger<CustomersController> _logger;
        private readonly IConfiguration _configuration;
      
        TenantAccessService _tenantAccessService;


        public CustomersController(ILogger<CustomersController> logger, IConfiguration iConfig, TenantAccessService tenantAccessService)
        {
            _logger = logger;
            _configuration = iConfig;
            _tenantAccessService = tenantAccessService;
           
            //Tenant t = tenantService.GetTenant();
            //GetTenants();
            //GetCosmosTenants().Wait();
            //tenant = tenants["Partner1"];
          
            
           // this.cosmosClient = new CosmosClient(tenant.EndpointUri, tenant.PrimaryKey, new CosmosClientOptions() { ApplicationName = "SippioPOC" });
            
            
        }

       

       /* private async Task GetCosmosTenants()
        {

            var EndpointUri = _configuration.GetSection("CosmosMasterDB").GetSection("EndpointUri").Value;
            var PrimaryKey = _configuration.GetSection("CosmosMasterDB").GetSection("PrimaryKey").Value;
            var DatabaseId = _configuration.GetSection("CosmosMasterDB").GetSection("DatabaseId").Value;
            var ContainerId = _configuration.GetSection("CosmosMasterDB").GetSection("ContainerId").Value;

            var cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "SippioPOC" });

            var sqlQueryText = "SELECT * FROM c";
            Database database = cosmosClient.GetDatabase(DatabaseId);
            Container container = database.GetContainer(ContainerId);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Tenant> queryResultSetIterator = container.GetItemQueryIterator<Tenant>(queryDefinition);

            List<Tenant> customers = new List<Tenant>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Tenant> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Tenant tenant in currentResultSet)
                {
                   
                    tenants[tenant.TenantID] = tenant;

                }

            }
            
        }
       */
        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            Tenant tenant=await _tenantAccessService.GetTenantId();
            var cosmosClient = new CosmosClient(tenant.EndpointUri, tenant.PrimaryKey, new CosmosClientOptions());

            var sqlQueryText = "SELECT * FROM c WHERE c.partitionKey = '" + tenant.TenantID +"'";
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(tenant.DatabaseId);
            Container container = await database.CreateContainerIfNotExistsAsync(tenant.ContainerId, "/partitionKey");
           
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Customer> queryResultSetIterator = container.GetItemQueryIterator<Customer>(queryDefinition);

            List<Customer> customers = new List<Customer>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Customer> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Customer customer in currentResultSet)
                {
                    customers.Add(customer);
                    
                }
            }
            return customers;
        }
    }
}
