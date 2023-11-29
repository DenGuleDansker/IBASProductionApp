using System;
using Microsoft.Azure.Functions.Worker.Http;

namespace IBASProductionApp
{
    public class MultiResponse
    {
        [CosmosDBOutput("IBasSupportDB", "bikeline",
        Connection = "CosmosDbConnectionString", CreateIfNotExists = false)]
        public ProductionItem? Document { get; set; }
        public HttpResponseData? HttpResponse { get; set; }
    }
}

