using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace IBASProductionApp
{
    public class IBASPostTrigger 
    {
        private readonly ILogger _logger;

        public class MultiResponse
        {
            [CosmosDBOutput("IBasSupportDB", "bikeline",
            Connection = "CosmosDbConnectionString", CreateIfNotExists = false)]
            public ProductionItem? Document { get; set; }
            public HttpResponseData? HttpResponse { get; set; }
        }

        public IBASPostTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<IBASPostTrigger>();
        }

        //[Function("IBASPostTrigger")]
        //public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        //{
        //    _logger.LogInformation($"C# HTTP trigger function processed a request.");

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    var data = JsonSerializer.Deserialize<JsonObject>(requestBody)!;
        //    var lineID = (string)data["lineID"]!;
        //    string responseMsg = string.IsNullOrEmpty(lineID) ? "Unexpected input" : $"Received data from {lineID}";
        //    var response = req.CreateResponse(HttpStatusCode.OK);
        //    response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        //    response.WriteString($"{responseMsg}");
        //    return response;

        //}

        [Function("IBASPostTrigger")]
        public async Task<MultiResponse> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {

            _logger.LogInformation($"C# HTTP trigger function processed a request.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<JsonObject>(requestBody)!;
            var lineID = (string)data["lineID"]!;
            string message = $"Received data from {lineID}.";
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString($"{message}");
            return new MultiResponse()
            {
                Document = new ProductionItem
                {
                    id = System.Guid.NewGuid().ToString(),
                    lineID = (string)data["lineID"]!,
                    model = (string)data["model"]!,
                    status = (string)data["status"]!,
                    serial = (string)data["serial"]!
                },
                HttpResponse = response
            };


        }


    }
}
