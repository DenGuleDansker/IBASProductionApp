using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace IBASProductionApp
{
    public class IBASGetTrigger
    {
        private readonly ILogger _logger;

        public class MultiResponse
        {
            [CosmosDBOutput("IBasSupportDB", "bikeline",
            Connection = "CosmosDbConnectionString", CreateIfNotExists = false)]
            public ProductionItem? Document { get; set; }
            public HttpResponseData? HttpResponse { get; set; }
        }

        public IBASGetTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<IBASGetTrigger>();
        }

        [Function("IBASGetTrigger")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
           _logger.LogInformation("C# HTTP trigger function processed a request.");

            string? name = req.Query["name"];

            string responseMsg = string.IsNullOrEmpty(name) ? "Unexpected input" : $"Hej, {name}.";

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString($"{responseMsg}");
            
            return response;
        }
    }
}
