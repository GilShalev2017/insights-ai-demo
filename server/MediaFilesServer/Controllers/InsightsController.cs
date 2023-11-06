using ActIntelligenceService.Domain.Models.InsightProviders;
using ActIntelligenceService.InsightProviders;
using FRServer.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Xabe.FFmpeg;

namespace FRServer.Controllers
{
    [Route("api/[controller]")]
    public class InsightsController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> RunInsights([FromBody] InsightsRequest insightsRequest)
        {
            string audioFilePath = @"C:\Development\FR-DEMO\server\MediaFilesServer\StaticFiles\Audios\" + insightsRequest.VideoFileName.Replace(".mp4", ".mp3");// G7.mp3";

            FileInfo audioFile = new FileInfo(audioFilePath);

            Dictionary<string, List<Transcript>> transcriptsDictionary = new Dictionary<string, List<Transcript>>();

            if (audioFile.Length > 26214400)//That is bigger than 25MB
            {
                List<string> splittedFilePaths = await SplitFileIntoSmallerFiles(audioFilePath, audioFile.Name.Replace(".mp3", ""));

                var tasks = new List<Task<TranscriptionsForFile>>();

                foreach (var splitFilePath in splittedFilePaths)
                {
                    tasks.Add(TranscribeAsync(splitFilePath));
                }

                var results = await Task.WhenAll(tasks);

                foreach (var result in results)
                {
                    transcriptsDictionary.Add(result.FileName, result.Transcripts);
                }
            }
            else
            {
                var transcriptionsForFile = await TranscribeAsync(audioFilePath);

                transcriptsDictionary.Add(audioFile.Name, transcriptionsForFile.Transcripts);
            }

            // Concatenate the transcripts in the order of the file names.
            List<Transcript> combinedTranscripts = new List<Transcript>();
            foreach (var fileName in transcriptsDictionary.Keys.OrderBy(key => key))
            {
                combinedTranscripts.AddRange(transcriptsDictionary[fileName]);
            }

            combinedTranscripts = combinedTranscripts.OrderBy(t => TimeSpan.FromSeconds(t.StartInSeconds)).ToList();

            var transcriptsString = ConvertTranscriptsToString(combinedTranscripts);

            InsightUtilities insightUtilities = new InsightUtilities();

            string accessToken = "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";

            TextAnalysisResponse? textAnalysisResponse = await insightUtilities.GetOpenAiAllTextAnalysisData(accessToken, transcriptsString);

            if (textAnalysisResponse != null)
            {
                textAnalysisResponse.Transcripts = combinedTranscripts;
            }

            var serializedResponse = JsonSerializer.Serialize(textAnalysisResponse);

            return Ok(serializedResponse);
        }

        [HttpPost]
        [Route("translate")]
        public async Task<IActionResult> Translate([FromBody] InsightsRequest insightsRequest)
        {
            InsightUtilities insightUtilities = new ();
            string targetLanguageCode = "en";
            switch (insightsRequest.TargetLanguage.ToLower()){
                case "english":
                    targetLanguageCode = "en";
                    break;
                case "french":
                    targetLanguageCode = "fr";
                    break;
                case "arabic":
                    targetLanguageCode = "ar";
                    break;
                case "spanish":
                    targetLanguageCode = "es";
                    break;
                case "hebrew":
                    targetLanguageCode = "he";
                    break;
                case "german":
                    targetLanguageCode = "de";
                    break;

                case "portuguese":
                    targetLanguageCode = "pt";
                    break;
                case "turkish":
                    targetLanguageCode = "tr";
                    break;
                case "polish":
                    targetLanguageCode = "pl";
                    break;
                case "russian":
                    targetLanguageCode = "ru";
                    break;
                case "chinese":
                    targetLanguageCode = "zh";
                    break;
                case "japanese":
                    targetLanguageCode = "ja";
                    break;
            }
            
            string apiKey = "f58881feb4df48dea54cc609778e886d";
            
            string location = "eastus";
           
            var transcripts = await insightUtilities.GetAzureCaptions(apiKey, location, insightsRequest.Transcripts, targetLanguageCode, null);

            TextAnalysisResponse? textAnalysisResponse = new()
            {
                Transcripts = transcripts
            };

            var serializedResponse = JsonSerializer.Serialize(textAnalysisResponse);

            return Ok(serializedResponse);
        }

        private static async Task<TranscriptionsForFile> TranscribeAsync(string audioFilePath)
        {
            InsightUtilities insightUtilities = new();

            string audioFileName = "videoplayback.mp3";

            string apiUrl = "https://api.openai.com/v1/audio/transcriptions";

            string accessToken = "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";

            var transcripts = await insightUtilities.GetOpenAIAudioBasedCaptions(audioFilePath, audioFileName, apiUrl, accessToken);

            TranscriptionsForFile transcriptionsForFile = new ()
            {
                Transcripts = transcripts,
                FileName = audioFilePath
            };
            return transcriptionsForFile;
        }

        private static Task<List<string>> SplitFileIntoSmallerFiles(string audioFilePath, string fileName)
        {
            string ffmpegPath = "ffmpeg"; // Path to the FFmpeg executable

            // Build the FFmpeg command
            string command = $"-i \"{audioFilePath}\" -f segment -segment_time 300 -c:a copy \"C:\\Development\\FR-DEMO\\server\\MediaFilesServer\\StaticFiles\\Audios\\{fileName}_%d.mp3\""; // $"-i \"{inputFilePath}\" -vn -ar 44100 -ac 2 -ab 192k -f mp3 \"{outputFilePath}\"";
            
            ProcessStartInfo psi = new ProcessStartInfo(ffmpegPath, command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process { StartInfo = psi };

            process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
            process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            var files = Directory.GetFiles(@"C:\Development\FR-DEMO\server\MediaFilesServer\StaticFiles\Audios");
            var splittedFiles = files.Where(file => file.Contains(fileName+"_")).ToList();
            return Task.FromResult(splittedFiles);
        }

        private static string ConvertTranscriptsToString(List<Transcript> transcripts)
        {
            string concatenatedText = string.Join(" ", transcripts.Select(t => t.Text));
            return concatenatedText;
        }
    }
}
