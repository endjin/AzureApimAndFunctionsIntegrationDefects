namespace FunctionAppB
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Newtonsoft.Json;

    public static class FunctionB
    {
        [FunctionName("FunctionB")]
        public static async Task<HttpResponseMessage> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "operation/b")]HttpRequestMessage req, TraceWriter log)
        {
            string body = await req.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<FunctionBRequest>(body);

            return req.CreateResponse(HttpStatusCode.OK, new FunctionBResponse { Result = request.Id + " " + request.Content });
        }
    }
}
