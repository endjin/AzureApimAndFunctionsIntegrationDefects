namespace FunctionAppA
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Newtonsoft.Json;

    public static class FunctionA
    {
        [FunctionName("FunctionA")]
        public static async Task<HttpResponseMessage> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "operation/a")]HttpRequestMessage req, TraceWriter log)
        {
            string body = await req.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<FunctionARequest>(body);

            return req.CreateResponse(HttpStatusCode.OK, new FunctionAResponse { Result = request.Id + " " + request.Content});
        }
    }
}