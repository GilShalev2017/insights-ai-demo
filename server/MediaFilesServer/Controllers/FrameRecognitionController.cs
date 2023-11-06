using FRServer.FrHub;
using FRServer.Models;
using FRServer.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;
using System.Text.Json;
using Xabe.FFmpeg;

namespace UploadFilesServer.Controllers
{
    [Route("api/[controller]")]
    public class FrameRecognitionController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ProcessManager _processManager;


        public FrameRecognitionController(IHubContext<ChatHub> hubContext, ProcessManager processManager)
        {
            _hubContext = hubContext;
            _processManager = processManager;
        }

        [HttpPost]
        public async Task<IActionResult> DetectFaces([FromBody] FaceDetectRequest request)
        {
            await InvokeFaceRecognitionProcessorAppAync(request.VideoFileName);
            return Ok("face detection started.");
        }

        [HttpPut]
        public async Task<IActionResult> StopDetection([FromBody] FaceDetectRequest request)
        {
            if (string.IsNullOrEmpty(request.VideoFileName))
            {
                return BadRequest("Video File Name cannot be empty.");
            }

            if (_processManager.TryGetProcess(request.VideoFileName, out Process process))
            {
                try
                {
                    // Stop the process
                    process?.Kill();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error stopping process: {ex.Message}");
                }
                finally
                {
                    // Remove it from the dictionary
                    _processManager.TryRemoveProcess(request.VideoFileName);

                    await _hubContext.Clients.All.SendAsync("StatusMessage", $"STOPPED:{request.VideoFileName}");
                }
                
                return Ok($"Stopped process: {request.VideoFileName}");
                
            }
            else
            {
                return NotFound($"Process not found: {request.VideoFileName}");
            }
        }

        [HttpGet]
        public IActionResult StopAllDetectionProcesses()
        {
            var processes = _processManager.GetAllProcesses();

            foreach (var process in processes)
            {
                // Stop the process
                process.Kill();
                process.Dispose();
            }
            _processManager.CleanAllProcesses();

            return Ok($"Stopped all processes");
        }
        public async Task InvokeFaceRecognitionProcessorAppAync(string videoFileName)
        {
            var relativeVideoFolderPath = Path.Combine("StaticFiles", "Videos");

            var absoluteVideoFolerPath = Path.Combine(Directory.GetCurrentDirectory(), relativeVideoFolderPath);

            var fullVideoFilePath = Path.Combine(@$"{absoluteVideoFolerPath}\{videoFileName}");

            var facesFolderPath = Path.Combine("StaticFiles", "Faces");

            var fullFacesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), facesFolderPath);

            string pathToConsoleApp = "C:\\Development\\FaceRecognition\\SentiVeillance_9_0_SDK_2023-06-13\\SentiVeillance_9_0_SDK\\Bin\\Win64_x64\\FRProcessor.exe";

            var arguments = $"4 -f {fullVideoFilePath} {fullFacesFolderPath}";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = pathToConsoleApp,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new();

            _processManager.TryAddProcess(videoFileName, process);

            process.StartInfo = processStartInfo;
            process.Exited += (sender, args) =>
            {
                _hubContext.Clients.All.SendAsync("StatusMessage", $"STOPPED:{videoFileName}");
            };
            process.OutputDataReceived += (sender, args) =>
            {
                // Handle the output of the console application here
                if (!string.IsNullOrEmpty(args.Data))
                {
                    // You can log or process the output as needed
                    if (args.Data.Contains("different people"))
                    {
                        Console.WriteLine(args.Data);
                    }
                    if (args.Data.Contains("FACEDETECTED"))
                    {
                        var nameAndScore = args.Data.Replace("FACEDETECTED:", "").Replace("SCORE:", "");
                        //FACEDETECTED:gil-SCORE:80
                        _hubContext.Clients.All.SendAsync("KnownFaceDetected", nameAndScore);
                    }
                    if (args.Data.Contains("SUMMARY"))
                    {
                        var receivedData = args.Data.Replace("SUMMARY:", "");
                        Dictionary<string, SubjectDetailsWithNameAndScores> receivedDictionary = JsonSerializer.Deserialize<Dictionary<string, SubjectDetailsWithNameAndScores>>(receivedData);
                        _hubContext.Clients.All.SendAsync("DetectionSummary", receivedDictionary);
                    }
                }
            };

            process.Start();

            await _hubContext.Clients.All.SendAsync("StatusMessage", $"STARTED:{videoFileName}");

            process.BeginOutputReadLine();

            // Use Task.Run to execute the process asynchronously
            await Task.Run(() =>
            {
                process.WaitForExit();
            });

            await _hubContext.Clients.All.SendAsync("StatusMessage", $"STOPPED:{videoFileName}");
            
            // The process has finished here
        }

        [HttpGet("{file}")]
        public async Task<IActionResult> GetVideoDetails(string file)
        {
            try
            {
                var folderName = Path.Combine("StaticFiles", "Videos");

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                var videoPath = Path.Combine(pathToSave, file);

                FileInfo fi = new FileInfo(videoPath);

                IMediaInfo info = await FFmpeg.GetMediaInfo(videoPath);

                // Video duration
                var duration = info.Duration;
                var roundedTimeSpan = new TimeSpan(duration.Hours, duration.Minutes, duration.Seconds);
                duration = roundedTimeSpan;
                Console.WriteLine("Video duration (HH:mm:ss): " + duration);

                // Video resolution (width x height)
                var resolution = info.VideoStreams.FirstOrDefault()?.Width + "x" + info.VideoStreams.FirstOrDefault()?.Height;
                Console.WriteLine("Video resolution: " + resolution);

                // Video frame rate
                double frameRate = info.VideoStreams.FirstOrDefault().Framerate;
                Console.WriteLine("Video frame rate: " + frameRate + " fps");

                // Video length (number of frames)
                var length = info.VideoStreams.FirstOrDefault()?.Width;
                Console.WriteLine("Video length (frames): " + length);

                // Video codec
                var videoCodec = info.VideoStreams.FirstOrDefault()?.Codec;
                Console.WriteLine("Video codec: " + videoCodec);

                VideoDetails videoDetails = new VideoDetails(duration, resolution, frameRate!, fi.Length, videoCodec);
                
                return Ok(videoDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
      
    }
    
}
