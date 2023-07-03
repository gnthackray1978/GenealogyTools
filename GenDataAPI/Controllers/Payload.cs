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

        public static long ExtractFile(FilePayload filePayload, out string n)
        {
            long size = filePayload.Files.Sum(f => f.Length);


            var f = filePayload.Files.FirstOrDefault();
            n = filePayload.Tags.Split('|').FirstOrDefault();

            if (f != null && n != null)
            {
                using var stream = new FileStream(Path.Combine(Path.GetTempPath(), n), FileMode.Create);

                f.CopyToAsync(stream);
            }

            return size;
        }
    }
}