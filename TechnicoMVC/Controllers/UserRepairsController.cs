using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers
{
    public class UserRepairsController : Controller
    {
        private readonly ILogger<RepairController> _logger;
        private readonly string sourcePrefix = "https://localhost:7017/api/Repair/"; //for other controller change to Repair / Property etc.
        private HttpClient client = new HttpClient();

        public UserRepairsController(ILogger<RepairController> logger) => _logger = logger;


        public async Task<IActionResult> GetUserRepairs(string VATNum) //https://localhost:7130/UserRepairs/GetUserRepairs
        {
            if (string.IsNullOrWhiteSpace(VATNum))
            {
                _logger.LogWarning("VATNum is null or empty.");
                return View("Error");
            }
            // Define the API endpoint for retrieving repairs
            string url = $"{sourcePrefix}repairs/get_all_by_vat/{VATNum}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserialize the response body to ResponseApi<List<RepairDTO>>
                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<List<RepairDTO>>>(responseBody, options);

                if (apiResponse?.Value != null)
                {
                    return View(apiResponse.Value);
                }
            }
            _logger.LogError("API request failed with status code: {StatusCode} and message: {Message}", response.StatusCode, response.ReasonPhrase);
            return View("Error");
        }












        //// GET: UserRepairs
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Repairs.ToListAsync());
        //}

        //// GET: UserRepairs/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var repair = await _context.Repairs
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (repair == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(repair);
        //}

        //// GET: UserRepairs/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: UserRepairs/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ScheduledDate,RType,Description,Status,Cost,IsActive")] Repair repair)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(repair);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(repair);
        //}

        //// GET: UserRepairs/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var repair = await _context.Repairs.FindAsync(id);
        //    if (repair == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(repair);
        //}

        //// POST: UserRepairs/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,ScheduledDate,RType,Description,Status,Cost,IsActive")] Repair repair)
        //{
        //    if (id != repair.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(repair);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!RepairExists(repair.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(repair);
        //}

        //// GET: UserRepairs/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var repair = await _context.Repairs
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (repair == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(repair);
        //}

        //// POST: UserRepairs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var repair = await _context.Repairs.FindAsync(id);
        //    if (repair != null)
        //    {
        //        _context.Repairs.Remove(repair);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool RepairExists(int id)
        //{
        //    return _context.Repairs.Any(e => e.Id == id);
        //}
    }
}
