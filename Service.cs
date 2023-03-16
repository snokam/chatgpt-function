using System;
using Microsoft.AspNetCore.Http;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.IO;
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
            response.StatusCode = 200;
            response.ContentType = "text/event-stream";
            ChatRequest chatRequest = getChatRequest(chatMessages);

            await foreach (ChatResult res in api.Chat.StreamChatEnumerableAsync(chatRequest)) {
                if (res?.Choices[0]?.Delta?.Content != null) {
                    await response.WriteAsync(res.Choices[0].Delta.Content);
                }
            }

            await response.Body.FlushAsync();
        }

        public static ChatRequest getChatRequest(ChatMessage[] chatMessages) {
            return new ChatRequest() {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.1,
                MaxTokens = 200,
                Messages = chatMessages,
            };
        }
    }
}

