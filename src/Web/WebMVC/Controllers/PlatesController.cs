using Microsoft.AspNetCore.Mvc;
using RTCodingExercise.Microservices.Models;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WebMVC.DTOs;

namespace RTCodingExercise.Microservices.Controllers
{
    public class PlatesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlatesController> _logger;
        private readonly string _catalogApiUrl;

        public PlatesController(
            HttpClient httpClient,
            ILogger<PlatesController> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _catalogApiUrl = configuration["CatalogApi"] ?? "http://localhost:5000/";
        }

        public async Task<IActionResult> Index(
            int pageNumber = 1,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortOrder = "asc",
            string? nameMatch = null)
        {
            try
            {
                nameMatch = nameMatch?.Trim().Replace(" ", "");

                var queryString = $"api/v1/plates?pageNumber={pageNumber}&pageSize=20";
                if (minPrice.HasValue) queryString += $"&minPrice={minPrice}";
                if (maxPrice.HasValue) queryString += $"&maxPrice={maxPrice}";
                if (!string.IsNullOrEmpty(sortOrder)) queryString += $"&sortOrder={sortOrder}";
                if (!string.IsNullOrEmpty(nameMatch)) queryString += $"&nameMatch={Uri.EscapeDataString(nameMatch)}";

                var requestUrl = $"{_catalogApiUrl}{queryString}";
                _logger.LogInformation("Making request to: {Url}", requestUrl);

                var response = await _httpClient.GetAsync(requestUrl);

                _logger.LogInformation("Response status code: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API request failed. Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, errorContent);

                    ViewBag.ErrorMessage = $"Failed to retrieve plates. Status: {response.StatusCode}";
                    return View(new PlatesViewModel()); // Return empty model
                }

                var result = await response.Content.ReadFromJsonAsync<PlatesViewModel>();
                if (result == null)
                {
                    _logger.LogError("Received null response from Catalog API");
                    ViewBag.ErrorMessage = "No data received from the server";
                    return View(new PlatesViewModel());
                }

                return View(result);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                ViewBag.ErrorMessage = "Failed to connect to the plate service";
                return View(new PlatesViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting plates");
                ViewBag.ErrorMessage = "An unexpected error occurred";
                return View(new PlatesViewModel());
            }
        }

        [HttpPatch]
        [Route("Plates/{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePlateStatusDto model)
        {
            try
            {
                var requestUrl = $"{_catalogApiUrl}api/v1/plates/{id}/status";
                _logger.LogInformation("Making request to: {Url}", requestUrl);

                var response = await _httpClient.PatchAsync(
                    requestUrl,
                    JsonContent.Create(new { status = model.Status }));

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PlateDto>();
                return Json(new { success = true, status = result?.Status ?? 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plate status");
                return Json(new { success = false, message = "Failed to update plate status" });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePlateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreatePlateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model validation failed: {Errors}",
                        string.Join(", ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }

                var baseUrl = _catalogApiUrl.TrimEnd('/');
                var requestUrl = $"{baseUrl}/api/v1/plates";

                _logger.LogInformation("Making create request to: {Url} with data: {@Model}", requestUrl, model);

                var response = await _httpClient.PostAsJsonAsync(requestUrl, model);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response status code: {StatusCode}, Content: {Content}",
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed. Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, responseContent);

                    return BadRequest(responseContent);
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating plate: {Message}", ex.Message);
                return BadRequest(new { error = "An error occurred while creating the plate" });
            }
        }
    }
}