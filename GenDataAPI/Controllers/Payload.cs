using Microsoft.AspNetCore.Http;

namespace GenDataAPI.Controllers;

public partial class GedController
{
    public class Payload
    {
        // You probably have to align the naming of what you `append()`
        // to the `FormData` in JavaScript (e.g. `formData.append("Files", ...)`
        // instead of `formData.append("files", ...)`). 
        public IFormFile[] Files { get; set; }
        public string Tags { get; set; }
    }
}