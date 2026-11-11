namespace HR.Services
{
    public class AttachmentService
    {
        private HrDbContext _dbContext;
        private IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        // NuGet Package => Library
        public AttachmentService(HrDbContext dbContext, IWebHostEnvironment env, IConfiguration config)
        {
            _dbContext = dbContext;
            _env = env;
            _config = config;
        }
        public string UploadImage(IFormFile image)
        {
            //var folderPath = "C:\\Users\\abdul\\source\\repos\\HR\\Attachments\\EmployeeImages";
            var imagesFolderPath = Path.Combine("Attachments", "EmployeeImages");

            var folderPath = Path.Combine(_env.WebRootPath, imagesFolderPath);
            if (!Directory.Exists(folderPath))
            {
                // Create Folder
                Directory.CreateDirectory(folderPath);
            }

            var fileExtension = Path.GetExtension(image.FileName); // .png .jpg .jpeg ...
            var fileName = Guid.NewGuid() + fileExtension; // Generate unique file name => 1234-5678-90ab-cdefm + .png

            var filePath = Path.Combine(folderPath, fileName);

            var stream = new FileStream(filePath, FileMode.Create); // Create file
            using (stream)
            {
                image.CopyTo(stream); // Copy image to file Steam
            }

            return Path.Combine(imagesFolderPath, fileName);
        }
    }
}
