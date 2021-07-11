using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SippioPOC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SippioPOC
{
    public class TenantAccessService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IConfiguration _configuration;
        Dictionary<string, Tenant> tenants = new Dictionary<string, Tenant>();
        public TenantAccessService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Tenant> GetTenantId()
        {

            if (tenants.Count > 0)
            {
                return tenants[_httpContextAccessor.HttpContext.Request.Headers["TenantId"]];
            }
            
             var EndpointUri = _configuration.GetSection("CosmosMasterDB").GetSection("EndpointUri").Value;
             var PrimaryKey = _configuration.GetSection("CosmosMasterDB").GetSection("PrimaryKey").Value;
             var DatabaseId = _configuration.GetSection("CosmosMasterDB").GetSection("DatabaseId").Value;
             var ContainerId = _configuration.GetSection("CosmosMasterDB").GetSection("ContainerId").Value;

             var cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "SippioPOC" });

             var sqlQueryText = "SELECT * FROM c";
             var database = cosmosClient.GetDatabase(DatabaseId);
             var container = database.GetContainer(ContainerId);

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
            
            return tenants[_httpContextAccessor.HttpContext.Request.Headers["TenantId"]];
            
        }

       
    }
}
