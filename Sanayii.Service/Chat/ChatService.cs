using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sanayii.Core.DTOs.ChatDTOs;
using Sanayii.Core.Interfaces;

namespace Sanayii.Service.Chat
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<ChatService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            //_apiKey = config["OpenAI:ApiKey"] ?? throw new ArgumentNullException(nameof(config));
            _apiKey = "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".Trim();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ChatResponseDTO> SendMessageAsync(ChatRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Preparing OpenAI request for message: {Message}", request.Message);

                var payload = new
                {
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content =  @"أنت مساعد ذكي داخل تطبيق 'صنايعي'، وهو تطبيق متخصص في تقديم خدمات منزلية متنوعة مثل السباكة، الكهرباء، النجارة، الدهانات، التكييف، وغيرها. 
                    دورك هو مساعدة العميل على:
                    1. فهم طبيعة المشكلة اللي بيواجهها.
                    2. توجيهه لاختيار الخدمة المناسبة من التطبيق.
                    3. شرح خطوات طلب الخدمة داخل التطبيق، خطوة بخطوة.
                    استخدم لغة بسيطة وودودة تناسب المستخدمين في مصر." },

                  // أمثلة تعليمية Few-shot
                new { role = "user", content = "الفيشة بتاعة المروحة مش شغالة" },
                new { role = "assistant", content =
                    "يبدو أن المشكلة كهربائية. أنصحك بطلب خدمة 'كهربائي' من تطبيق صنايعي.\n" +
                    "خطوات الطلب:\n1. افتح التطبيق.\n2. من القائمة الرئيسية، اختار 'الخدمات'.\n3. بعدين اختار 'كهربائي'.\n4. حدد اليوم والوقت اللي يناسبك.\n5. اضغط 'تأكيد الطلب'.\n" +
                    "لو محتاج مساعدة في أي خطوة، قولي." },

                new { role = "user", content = "الحوض في المطبخ بيخر مية" },
                new { role = "assistant", content =
                    "ده غالباً بسبب تسريب في السباكة. أنصحك بطلب خدمة 'سباك' من خلال تطبيق صنايعي.\n" +
                    "علشان تطلب:\n1. افتح التطبيق.\n2. ادخل على 'الخدمات'.\n3. اختار 'سباك'.\n4. حدد الوقت والمكان.\n5. اضغط 'تأكيد الطلب'." },

                        new { role = "user", content = request.Message }
                    },
                    temperature = 0.7,
                    max_tokens = 1000

                };

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKey);

                var jsonPayload = JsonSerializer.Serialize(payload);
                _logger.LogDebug("Sending payload to OpenAI: {Payload}", jsonPayload);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received response from OpenAI: {Response}", responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("OpenAI API request failed with status {StatusCode}: {Response}",
                        response.StatusCode, responseContent);
                    throw new Exception($"OpenAI API error: {response.StatusCode} - {responseContent}");
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var reply = jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return new ChatResponseDTO { Response = reply };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process chat message");
                throw new Exception("Failed to process your request. Please try again later.", ex);
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
}