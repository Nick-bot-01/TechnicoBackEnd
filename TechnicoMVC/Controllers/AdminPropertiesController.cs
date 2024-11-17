using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers{
    public class AdminPropertiesController : Controller{
        private readonly ILogger<AdminPropertiesController> _logger;
        private readonly string sourcePrefix = "https://localhost:7017/api/"; //for other controller change to Repair / Property etc.
        private HttpClient client = new HttpClient();

        public AdminPropertiesController(ILogger<AdminPropertiesController> logger) => _logger = logger;

        [HttpPost]
        public async Task<ResponseApi<PropertyDTO>?> CreateRepairToRedirectController(PropertyDTO property){
            string url = $"{sourcePrefix}Property/create_property";
            var response = await client.PostAsJsonAsync(url, property);
            var responseBody = await response.Content.ReadAsStringAsync();
            var targetProperty = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<PropertyDTO>>(responseBody);
            return targetProperty;
        }

        public async Task<IActionResult> CreatePropertyCallback(PropertyDTO pendingCreationProperty){
            var createdProperty = await CreateRepairToRedirectController(pendingCreationProperty);
            return RedirectToAction("OwnersAndProperties", "Admin");
        }

        public IActionResult AdminCreateProperty(){
            return View(new PropertyDTO());
        }
    }
}
