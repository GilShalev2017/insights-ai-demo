using ActIntelligenceService.InsightProviders;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using Xabe.FFmpeg;

namespace UploadFilesServer.Controllers
{
    [Route("api/[controller]")]
    public class MediaFilesController : Controller
    {
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult UploadMultiple()
        {
            try
            {
                var files = Request.Form.Files;

                var folderName = Path.Combine("StaticFiles", "Videos");

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (files.Any(f => f.Length == 0))
                {
                    return BadRequest();
                }

                foreach (var file in files)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                    var fullPath = Path.Combine(pathToSave, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var t = Task.Run(() => CreateThumbnail(pathToSave, fileName, fullPath));
                   
                    var audioFileFullPath = fullPath.Replace("Videos","Audios").Replace(".mp4", ".mp3");

                    if (!System.IO.File.Exists(audioFileFullPath))
                    {
                        var t2 = Task.Run(() => ExtractAudioWithFFmpeg(fullPath, audioFileFullPath));
                    }
                   
                }

                return Ok("All the files are successfully uploaded.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{file}")]
        public IActionResult GetVideo(string file)
        {
            try
            {
                var folderName = Path.Combine("StaticFiles", "Videos");

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                var filePath = Path.Combine(pathToSave, file);

                var fileStream = new FileStream(filePath, FileMode.Open);

                return new FileStreamResult(fileStream, "video/mp4");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet, DisableRequestSizeLimit]
        public IActionResult GetThumbnails()
        {
            try
            {
                var folderName = Path.Combine("StaticFiles", "Videos");
                var pathToRead = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var thumbnails = Directory.EnumerateFiles(pathToRead)
                    .Where(IsAnImageFile)
                    .Select(fullPath => Path.GetFileName(fullPath)).ToList();
                return Ok(thumbnails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        private bool IsAnImageFile(string fileName)
        {
            return fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase);
        }
        private void CreateThumbnail(string pathToSave, string fileName, string fullPath)
        {
            try
            {
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                var thumbnailName = fileName.Replace(".mp4", ".jpg");
                if (!thumbnailName.EndsWith(".jpg"))
                {
                    thumbnailName += ".jpg";
                }
                var saveFullPath = Path.Combine(pathToSave, thumbnailName);
                ffMpeg.GetVideoThumbnail(fullPath, saveFullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static void ExtractAudioWithFFmpeg(string inputFilePath, string outputFilePath)
        {
            string ffmpegPath = "ffmpeg"; // Path to the FFmpeg executable

            // Build the FFmpeg command
            string command = $"-i \"{inputFilePath}\" -vn -ar 44100 -ac 2 -ab 192k -f mp3 \"{outputFilePath}\"";

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
        }
    }
}
