using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenAI_API.Chat;
using Newtonsoft.Json;

public class DTO{
    public string? answer { get; set; }
}

namespace openai
{
    public static class Function
    {

        [FunctionName("syncQuestion")]
        public static async Task<IActionResult> syncQuestion(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1.0/question")] HttpRequest req,
           ILogger log)
        {
            string input = req.Query["input"];

            var result = await ChatGpt.getAnswer(new ChatMessage[] {
                new ChatMessage(ChatMessageRole.User, input)
            });

            return new OkObjectResult(new DTO {
                answer = result.ToString()
            });
        }

        [FunctionName("asyncQuestion")]
        public static async Task asyncQuestion(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1.0/question/async")] HttpRequest req,
           ILogger log)
        {
            string input = req.Query["input"];
            var response = req.HttpContext.Response;

            await ChatGpt.streamAnswer(response, new ChatMessage[] {
                new ChatMessage(ChatMessageRole.User, input)
            });
        }

        [FunctionName("syncConversation")]
        public static async Task<IActionResult> syncConversation(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1.0/conversation")] HttpRequest req,
            ILogger log)
        {
            string body = await req.ReadAsStringAsync();

            var messages = JsonConvert.DeserializeObject<ChatMessage[]>(body);
            var result = await ChatGpt.getAnswer(messages);

            return new OkObjectResult(new DTO {
                answer = result.ToString()
            });
        }

        [FunctionName("asyncConversation")]
        public static async Task asyncConversation(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1.0/conversation/async")] HttpRequest req,
           ILogger log)
        {
            string body = await req.ReadAsStringAsync();
            var response = req.HttpContext.Response;

            var messages = JsonConvert.DeserializeObject<ChatMessage[]>(body);
            await ChatGpt.streamAnswer(response, messages);
        }

      
    }
    

}

