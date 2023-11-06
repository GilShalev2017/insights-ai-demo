using FRServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace UploadFilesServer.Controllers
{
    [Route("api/[controller]")]
    public class FaceFilesController : Controller
    {

        [HttpPost, DisableRequestSizeLimit]
        //[Consumes("multipart/form-data", "application/json")]
        public IActionResult UploadMultiple()
        {
            try
            {
                // Access the 'additionalText' value from the model
                // string additionalText = model.AdditionalText;

                var files = Request.Form.Files;

                var folderName = Path.Combine("StaticFiles", "Faces");

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

                }

                return Ok("All the files are successfully uploaded.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
