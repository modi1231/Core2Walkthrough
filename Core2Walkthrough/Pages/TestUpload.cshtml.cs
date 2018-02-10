 using System.IO;
 using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Core2Walkthrough.Pages
{
    public class TestUploadModel : PageModel
    {
        [TempData]
        public string Message { get; set; }// no private set b/c we need data back

        //Helps to get server's web path.
        private readonly IHostingEnvironment _environment;

        //Constructor with environment variable passed on load automatically
        public TestUploadModel(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        //Parameter name *MUST* match name in the web portion.
        public async Task<IActionResult> OnPost(IFormFile file)
        {
            //in the wwwroot, create a folder called 'temp'.
            var filePath = Path.Combine(_environment.WebRootPath, "temp");
            filePath = Path.Combine(filePath, file.FileName);

            // useing basic IO file stream create the file in the directory.
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            Message = "Uploaded: " + file.FileName;

            return RedirectToPage("/TestUpload");
        }
    }
}