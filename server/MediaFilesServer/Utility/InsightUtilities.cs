using ActIntelligenceService.Domain.Models;
using ActIntelligenceService.Domain.Models.InsightProviders;
using System.Text;
using System.Text.Json;

namespace ActIntelligenceService.InsightProviders
{
    class SystemMessages
    {
        public const string GPT_Get_Summary_Message = "You are a highly skilled AI trained in language comprehension and summarization. I would like you to read the following text and summarize it into a concise abstract paragraph. Aim to retain the most important points, providing a coherent and readable summary that could help a person understand the main points of the discussion without needing to read the entire text. Please avoid unnecessary details or tangential points.";
        public const string GPT_Get_KeyPoints_Message = "You are a proficient AI with a specialty in distilling information into key points. Based on the following text, identify and list the main points that were discussed or brought up. These should be the most important ideas, findings, or topics that are crucial to the essence of the discussion. Your goal is to provide a list that someone could read to quickly understand what was talked about.";
        public const string GPT_Get_Action_Items_Message = "You are an AI expert in analyzing conversations and extracting action items. Please review the text and identify any tasks, assignments, or actions that were agreed upon or mentioned as needing to be done. These could be tasks assigned to specific individuals, or general actions that the group has decided to take. Please list these action items clearly and concisely.";
        public const string GPT_Get_Sentiment_Analysis_Message = "As an AI with expertise in language and emotion analysis, your task is to analyze the sentiment of the following text. Please consider the overall tone of the discussion, the emotion conveyed by the language used, and the context in which words and phrases are used. Indicate whether the sentiment is generally positive, negative, or neutral, and provide brief explanations for your analysis where possible.";
    }
    public class Translation
    {
        public string text { get; set; }
        public string to { get; set; }
    }
    public class TranslationItem
    {
        public List<Translation> translations { get; set; }
    }
      
    public class InsightUtilities
    {
        private readonly HttpClient _httpClient;
        public InsightUtilities()
        {
            _httpClient = new HttpClient();
        }
        public async Task<List<Transcript>?> GetOpenAITextBasedCaptions(string txtToTranslate, string targetLanguageCode, string apiUrl, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var translationMessage = $"Translate the following text into {targetLanguageCode}: '{txtToTranslate}'";
            var requestBody = new
            {
                model = "gpt-4", // Specify the model
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = translationMessage }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error: " + response.StatusCode);
                return null;
            }

            string jsonString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            JsonDocument responseJson = JsonDocument.Parse(jsonString);

            // Access the message content
            string assistantMessage = responseJson.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return null;// Convert assistantResponse to List<Transcript> ;
        }
        public async Task<List<Transcript>?> GetOpenAIAudioBasedCaptions(string audioFilePath, string audioFileName, string apiUrl, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            _httpClient.Timeout = TimeSpan.FromMinutes(10);

            using var formData = new MultipartFormDataContent
            {
                { new StreamContent(System.IO.File.OpenRead(audioFilePath)), "file", audioFileName },
                { new StringContent("whisper-1"), "model" },
                { new StringContent("verbose_json"), "response_format" }
            };

            var response = await _httpClient.PostAsync(apiUrl, formData);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            var root = JsonSerializer.Deserialize<Root>(jsonString);
            if (root == null)
            {
                return null;
            }

            return root.segments.Select(segment => new Transcript
            {
                Text = segment.text,
                StartInSeconds = (int)segment.start,
                EndInSeconds = (int)segment.end
            }).ToList();
        }
        public async Task<List<Transcript>?> GetWhisperCaptions(string aiServerUrl, string modelType, string audioFilePath, int timeoutMinutes, bool useTranslate = false)
        {
            _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);

            using var formData = new MultipartFormDataContent();
            var fi = new FileInfo(audioFilePath);
            formData.Add(new StringContent(fi.Name), "AudioFileName");
            formData.Add(new StreamContent(System.IO.File.OpenRead(audioFilePath)), "File", fi.Name);
            formData.Add(new StringContent(useTranslate.ToString()), "UseTranslate");
            formData.Add(new StringContent(modelType.ToString()), "ModelType");

            var response = await _httpClient.PostAsync(aiServerUrl, formData);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error: " + response.StatusCode);
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            return JsonSerializer.Deserialize<List<Transcript>>(jsonString);
        }
        public InsightResult WrapCaptionsInAnObject(List<Transcript>? transcriptions, string providerId)
        {
            InsightResult insightResult = new();

            TranscriberInsightResponse transcriptsResult = new()
            {
                Transcripts = transcriptions
            };

            insightResult.BulkData = JsonSerializer.Serialize(transcriptsResult);

            insightResult.AIProviderId = providerId;

            return insightResult;
        }
        public InsightResult WrapCaptionsInAnObject(TextAnalysisResponse textAnalysisResponse, string providerId)
        {
            InsightResult insightResult = new()
            {
                BulkData = JsonSerializer.Serialize(textAnalysisResponse),

                AIProviderId = providerId
            };

            return insightResult;
        }
        public async Task<List<Transcript>?> GetAzureCaptions(string apiKey, string location, List<Transcript>? transcriptions, string targetLanguageCode, string? sourceLanguageCode = null)
        {
            if (transcriptions == null)
            {
                return null;
            }

            var azureUrl = $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={targetLanguageCode}";

            if (!string.IsNullOrEmpty(sourceLanguageCode))
            {
                azureUrl += $"&from={sourceLanguageCode}";
            }

            var translationItems = await AzureTranslateTranscriptionsAsync(apiKey, location, azureUrl, transcriptions);

            if (translationItems != null)
            {
                for (int i = 0; i < transcriptions.Count; i++)
                {
                    transcriptions[i].Text = translationItems[i].translations[0].text;
                }

                return transcriptions;
            }

            return null;
        }
        private async Task<List<TranslationItem>?> AzureTranslateTranscriptionsAsync(string apiKey, string location, string azureUrl, List<Transcript> transcriptions)
        {
            var body = transcriptions.Select(transcription => new { Text = transcription.Text }).ToList();
            var requestBody = JsonSerializer.Serialize(body);

            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(azureUrl));
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", location);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                
                JsonDocument jsonDocument = JsonDocument.Parse(responseBody);

                var result = JsonSerializer.Serialize(jsonDocument.RootElement, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                return JsonSerializer.Deserialize<List<TranslationItem>>(result);
            }

            return null;
        }
        public async Task<Domain.Models.InsightProviders.TextAnalysisResponse?> GetOpenAiTextAnalysisData(string apiKey, /*List<InsightType> insightTypes,*/ string transcription)
        {
            string message = GetMessageForAnalysisType(InsightType.Summary);
           
            var result = await GetOpenAiTextAnalysis(apiKey, message, transcription);

            TextAnalysisResponse textAnalysisResponse = new ()
            {
                Summary = result
            };

            return textAnalysisResponse;
        }

        public async Task<TextAnalysisResponse?> GetOpenAiAllTextAnalysisData(string apiKey, string transcription)
        {
            var textAnalysisResponse = new TextAnalysisResponse();
            var tasks = new List<Task<string?>>();

            List<InsightType> insightTypes = new()
            {
                InsightType.Summary,
                InsightType.KeyPoints,
                InsightType.ActionItems,
                InsightType.Sentiment
            };

            foreach (var textAnalysisType in insightTypes)
            {
                string message = GetMessageForAnalysisType(textAnalysisType);
                if (string.IsNullOrEmpty(message))
                    continue;
                tasks.Add(GetOpenAiTextAnalysis(apiKey, message, transcription));
            }

            await Task.WhenAll(tasks);

            for (int i = 0; i < insightTypes.Count; i++)
            {
                if (insightTypes[i] == InsightType.Summary)
                    textAnalysisResponse.Summary = tasks[i].Result;
                else if (insightTypes[i] == InsightType.KeyPoints)
                    textAnalysisResponse.KeyPoints = tasks[i].Result;
                else if (insightTypes[i] == InsightType.ActionItems)
                    textAnalysisResponse.ActionItems = tasks[i].Result;
                else if (insightTypes[i] == InsightType.Sentiment)
                    textAnalysisResponse.Sentiment = tasks[i].Result;
            }

            return textAnalysisResponse;
        }

        private static string GetMessageForAnalysisType(InsightType insightType)
        {
            if(insightType==InsightType.Summary)
                return SystemMessages.GPT_Get_Summary_Message;
            else if(insightType==InsightType.KeyPoints)
                return SystemMessages.GPT_Get_KeyPoints_Message;
            else if (insightType == InsightType.ActionItems)
                return SystemMessages.GPT_Get_Action_Items_Message;
            else if(insightType == InsightType.Sentiment)
                return SystemMessages.GPT_Get_Sentiment_Analysis_Message;
            else
                return string.Empty;
        }
        private async Task<string?> GetOpenAiTextAnalysis(string apiKey, string systemMessage, string transcriptions)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                string apiUrl = "https://api.openai.com/v1/chat/completions";

                var requestBody = new
                {
                    model = "gpt-4",
                    messages = new[]
                    {
                        new { role = "system", content = systemMessage },
                        new { role = "user", content = transcriptions }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    if (jsonString != null)
                    {
                        JsonDocument responseJson = JsonDocument.Parse(jsonString);

                        if (responseJson.RootElement.TryGetProperty("choices", out var choicesElement) &&
                            choicesElement.GetArrayLength() > 0 &&
                            choicesElement[0].TryGetProperty("message", out var messageElement) &&
                            messageElement.TryGetProperty("content", out var contentElement))
                        {
                            return contentElement.GetString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here if needed
                Console.WriteLine($"Error: {ex.Message}");
            }

            return "";
        }
    }
}
