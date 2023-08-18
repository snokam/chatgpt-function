using System;
using Microsoft.AspNetCore.Http;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Threading.Tasks;
using OpenAI_API;

namespace openai
{
	public class ChatGpt
    {
        static OpenAIAPI api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        public static async Task<ChatResult> getAnswer (ChatMessage[] chatMessages){
            return await api.Chat.CreateChatCompletionAsync(getChatRequest(chatMessages));
        }

        public static async Task streamAnswer(HttpResponse response, ChatMessage[] chatMessages)
        {
            response.Headers.Add("Content-Type", "text/event-stream; charset=utf-8");
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Keep-Alive", "timeout=250, max=10000");

            ChatRequest chatRequest = getChatRequest(chatMessages);

            await foreach (ChatResult res in api.Chat.StreamChatEnumerableAsync(chatRequest)) {
                if (res?.Choices[0]?.Delta?.Content != null) {
                    await response.WriteAsync($"data: {res.Choices[0].Delta.Content.Replace("\n", "<br/>")}\n\n");
                    await response.Body.FlushAsync();
                }
            }

            await response.WriteAsync($"data: \n\n");
        }

        public static ChatRequest getChatRequest(ChatMessage[] chatMessages) {
            return new ChatRequest() {
                Model = Model.GPT4,
                Temperature = 0.1,
                Messages = chatMessages,
            };
        }
    }
}

