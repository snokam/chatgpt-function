using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenAI_API.Chat;
using System;
using Newtonsoft.Json;
using Snokam.Sanity;
using System.Collections.Generic;
using Slack.Webhooks;
using System.Transactions;

public class DTO{
    public string? answer { get; set; }
}

namespace openai
{
    public static class Function
	{
		static SlackClient slackClient = new SlackClient(Environment.GetEnvironmentVariable("SLACK_WEBHOOK"));
        static List<String> scannedEvents = new List<string>();

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

		[FunctionName("slackEvent")]
		public static async Task<dynamic> slackEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1.0/slack/event")] HttpRequest req,
        ILogger log)
		{
			string body = await req.ReadAsStringAsync();

			SlackEvent slackEvent = JsonConvert.DeserializeObject<SlackEvent>(body);
			dynamic rawSlackEvent = JsonConvert.DeserializeObject<dynamic>(body);

			Console.WriteLine("Analysing message ...");
            Console.WriteLine(JsonConvert.SerializeObject(rawSlackEvent));

			if (slackEvent.@event.files.Count > 0 && !scannedEvents.Contains(slackEvent.@event.event_ts)) {
				scannedEvents.Add(slackEvent.@event.event_ts);

				Console.WriteLine("Creating summary ...");
				var summaryResult = await ChatGpt.getAnswer(new ChatMessage[] {
				    new ChatMessage(ChatMessageRole.User, "Kan du oppsummere denne eposten? " + JsonConvert.SerializeObject(rawSlackEvent))
			    });

				var slackSummaryMessage = new SlackMessage
				{
					Channel = "#oppdrag",
					IconEmoji = Emoji.Computer,
					Username = "snokam",
					Text = summaryResult.Choices[0].Message.Content
				};

				slackSummaryMessage.ThreadId = slackEvent.@event.event_ts;
				slackClient.Post(slackSummaryMessage);

				Console.WriteLine("Writing pitch summary ...");
				List<SanityEmployee> employees = await SanityService.GetEmployeesWithoutActiveCustomerContract();

				var pitchResults = await ChatGpt.getAnswer(new ChatMessage[] {
				    new ChatMessage(ChatMessageRole.System, summaryResult.Choices[0].Message.Content),
				    new ChatMessage(ChatMessageRole.User, "Kan du skrive hvorfor disse konsulentene passer til oppdraget: " + JsonConvert.SerializeObject(JsonConvert.SerializeObject(employees))
	            )});

				var slackPitchMessage = new SlackMessage
				{
					Channel = "#oppdrag",
					IconEmoji = Emoji.Computer,
					Username = "snokam",
					Text = pitchResults.Choices[0].Message.Content
				};

				slackPitchMessage.ThreadId = slackEvent.@event.event_ts;
				slackClient.Post(slackPitchMessage);

				return new OkObjectResult(body);

			}
			return new NoContentResult();


		}
	}
}

