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

            try
            {
                // Log the received request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                _logger.LogInformation($"Received request body: {requestBody}");

                // Validate and deserialize JSON data
                var data = JsonSerializer.Deserialize<JsonObject>(requestBody);

                if (data == null)
                {
                    _logger.LogError("Invalid JSON format. Unable to deserialize.");
                    return new MultiResponse()
                    {
                        HttpResponse = req.CreateResponse(HttpStatusCode.BadRequest),
                    };
                }

                // Validate required fields
                if (!data.ContainsKey("lineID") || !data.ContainsKey("model") || !data.ContainsKey("status") || !data.ContainsKey("serial"))
                {
                    _logger.LogError("Missing required fields in the input data.");
                    return new MultiResponse()
                    {
                        HttpResponse = req.CreateResponse(HttpStatusCode.BadRequest),
                    };
                }

                // Extract values from JSON
                var lineID = (string)data["lineID"];
                var model = (string)data["model"];
                var status = (string)data["status"];
                var serial = (string)data["serial"];

                // Additional data validation if needed

                // Log the validated data
                _logger.LogInformation($"Validated data: LineID={lineID}, Model={model}, Status={status}, Serial={serial}");

                // Create and return the response
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString($"Received data from {lineID}.");

                return new MultiResponse()
                {
                    Document = new ProductionItem
                    {
                        id = System.Guid.NewGuid().ToString(),
                        lineID = lineID,
                        model = model,
                        status = status,
                        serial = serial
                    },
                    HttpResponse = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return new MultiResponse()
                {
                    HttpResponse = req.CreateResponse(HttpStatusCode.InternalServerError),
                };
            }
        }



    }
}
