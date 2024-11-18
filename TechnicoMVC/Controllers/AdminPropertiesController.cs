using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers{
    public class AdminPropertiesController : Controller{
        private readonly ILogger<AdminPropertiesController> _logger;
        private readonly string sourcePrefix = "https://localhost:7017/api/"; //for other controller change to Repair / Property etc.
        private HttpClient client = new HttpClient();

        public AdminPropertiesController(ILogger<AdminPropertiesController> logger) => _logger = logger;

        [HttpGet]
        public async Task<ResponseApi<PropertyDTO>?> GetPropertyToRedirectController(int id)
        {
            string url = $"{sourcePrefix}Property/properties/id/{id}";
            var response = await client.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            var targetProperty = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<PropertyDTO>>(responseBody);
            return targetProperty;
        }

        [HttpPost]
        public async Task<ResponseApi<PropertyDTO>?> CreatePropertyToRedirectController(PropertyDTO property){
            string url = $"{sourcePrefix}Property/create_property";
            var response = await client.PostAsJsonAsync(url, property);
            var responseBody = await response.Content.ReadAsStringAsync();
            var targetProperty = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<PropertyDTO>>(responseBody);
            return targetProperty;
        }

        [HttpPost]
        public async Task<ResponseApi<PropertyDTO>?> UpdatePropertyToRedirectController(PropertyDTO property){
            string url = $"{sourcePrefix}Property/update_property";
            var response = await client.PatchAsJsonAsync(url, property);
            var responseBody = await response.Content.ReadAsStringAsync();
            var targetProperty = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<PropertyDTO>>(responseBody);
            return targetProperty;
        }

        public async Task<IActionResult> CreatePropertyCallback(PropertyDTO pendingCreationProperty){
            var createdProperty = await CreatePropertyToRedirectController(pendingCreationProperty);
            return RedirectToAction("OwnersAndProperties", "Admin");
        }

        public async Task<IActionResult> UpdatePropertyCallback(PropertyDTO pendingUpdateProperty){
            var updatedProperty = await UpdatePropertyToRedirectController(pendingUpdateProperty);
            return RedirectToAction("OwnersAndProperties", "Admin");
        }

        public IActionResult AdminCreateProperty(){
            return View(new PropertyDTO());
        }

        public async Task<IActionResult> AdminUpdateProperty(int id){
            if(id == 0) RedirectToAction("OwnersAndProperties", "Admin");
            var result = await GetPropertyToRedirectController(id);
            return View(result?.Value);
        }
    }
}
