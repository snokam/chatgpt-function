using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenAI_API;

public class DTO{
    public string? answer { get; set; }
}

namespace openai
{
    public static class Function
    {
        static OpenAIAPI api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        [FunctionName("conversation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string input = req.Query["input"];
            var chat = api.Chat.CreateConversation();
            chat.AppendUserInput(input);
            return new OkObjectResult(new DTO{
                answer = await chat.GetResponseFromChatbot()
            });
        }
    }
}

