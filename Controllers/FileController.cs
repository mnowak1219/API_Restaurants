namespace API_Restaurants.Controllers
{
    [Route("file")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        [HttpGet]
        [ResponseCache(Duration = 600, VaryByQueryKeys = new[] {"fileName"})]
        public ActionResult GetFile([FromQuery] string fileName)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = $"{rootPath}/wwwroot/Files/Private/{fileName}";

            var isFileExist = System.IO.File.Exists(filePath);
            if (!isFileExist)
            {
                return NotFound();
            }
            var fileContents = System.IO.File.ReadAllBytes(filePath);
            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(filePath, out string contentType);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult UploadFile([FromForm]IFormFile file)
        {
            if(file != null && file.Length>0)
            {
                var fileName = file.FileName;
                var rootPath = Directory.GetCurrentDirectory();
                var fullPath = $"{rootPath}/wwwroot/Files/Public/{fileName}";
                using(var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Ok("File uploaded");
            }
            else
                return StatusCode(405);
        }
    }
}