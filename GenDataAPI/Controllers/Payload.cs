using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace GenDataAPI.Controllers;

public partial class GedController
{
    public class FilePayload
    {
        // You probably have to align the naming of what you `append()`
        // to the `FormData` in JavaScript (e.g. `formData.append("Files", ...)`
        // instead of `formData.append("files", ...)`). 
        public IFormFile[] Files { get; set; }
        public string Tags { get; set; }

        public static string ExtractFile(FilePayload filePayload,string extractionPath, out string n)
        {
            string size = filePayload.Files.Sum(f => f.Length).ToString();


            var f = filePayload.Files.FirstOrDefault();
            n = filePayload.Tags.Split('|').FirstOrDefault();

            if (f != null && n != null)
            {
                using var stream = new FileStream(Path.Combine(extractionPath, n), FileMode.Create);

                f.CopyToAsync(stream);
            }

            return size;
        }
    }
}