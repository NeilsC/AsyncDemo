using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using MvcApplication1.ImageService;

namespace MvcApplication1.Controllers
{
    public class ImageController : Controller
    {             
        public async Task<ActionResult> Details(int id)
        {
            using (var serviceClient = new ImageServiceClient())
            {
                var base64ImageString = await serviceClient.DownloadImageAsync(id);

                var bytes = Convert.FromBase64String(base64ImageString);
                return File(bytes, "image/jpeg");
            }
        }
    }
}
