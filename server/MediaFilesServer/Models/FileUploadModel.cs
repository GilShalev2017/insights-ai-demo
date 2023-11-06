namespace FRServer.Models
{
    public class FileUploadModel
    {
        public List<IFormFile> Files { get; set; }
        public string AdditionalText { get; set; }
    }
}
