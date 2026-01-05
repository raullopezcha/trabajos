using ApiGateServiceLayer.Models;
using ApiGateServiceLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;

namespace ApiGateServiceLayer.Controllers
{
    /// <summary>
    /// API Gateway controller for SAP Business One Service Layer operations
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [EnableRateLimiting("fixed")]
    public class ServiceLayerGatewayController : ControllerBase
    {
        private readonly IServiceLayerClient _sl;
        private readonly ILogger<ServiceLayerGatewayController> _logger;

        public ServiceLayerGatewayController(
            IServiceLayerClient sl,
            ILogger<ServiceLayerGatewayController> logger)
        {
            _sl = sl;
            _logger = logger;
        }

        /// <summary>
        /// Establishes a session with SAP Service Layer
        /// </summary>
        /// <returns>Session establishment confirmation</returns>
        /// <response code="200">Session established successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login()
        {
            _logger.LogInformation("Establishing Service Layer session");
            await _sl.EnsureSessionAsync();
            return Ok(ApiResponse<object>.SuccessResponse(
                new { message = "Session established successfully" }
            ));
        }

        /// <summary>
        /// Proxies GET requests to SAP Service Layer
        /// </summary>
        /// <param name="path">Service Layer endpoint path</param>
        /// <returns>Service Layer response</returns>
        /// <response code="200">Request successful</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="502">Bad Gateway - Service Layer error</response>
        [Authorize]
        [HttpGet("{**path}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> ProxyGet(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Path parameter is required",
                    StatusCodes.Status400BadRequest
                ));
            }

            _logger.LogDebug("Proxying GET request to Service Layer: {Path}", path);
            var resp = await _sl.GetAsync(path);
            var json = await resp.Content.ReadAsStringAsync();
            
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = (int)resp.StatusCode
            };
        }

        /// <summary>
        /// Proxies POST requests to SAP Service Layer
        /// </summary>
        /// <param name="path">Service Layer endpoint path</param>
        /// <param name="payload">Request payload</param>
        /// <returns>Service Layer response</returns>
        /// <response code="200">Request successful</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="502">Bad Gateway - Service Layer error</response>
        [Authorize]
        [HttpPost("{**path}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> ProxyPost(string path, [FromBody] JsonElement payload)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Path parameter is required",
                    StatusCodes.Status400BadRequest
                ));
            }

            _logger.LogDebug("Proxying POST request to Service Layer: {Path}", path);
            var resp = await _sl.PostAsync(path, payload);
            var json = await resp.Content.ReadAsStringAsync();
            
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = (int)resp.StatusCode
            };
        }

        /// <summary>
        /// Creates a sales quotation in SAP Business One
        /// </summary>
        /// <param name="dto">Sales quotation data</param>
        /// <returns>Created quotation details</returns>
        /// <response code="200">Quotation created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="502">Bad Gateway - Service Layer error</response>
        [Authorize]
        [HttpPost("quotations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> CreateQuotation([FromBody] SalesQuotationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Invalid quotation data",
                    StatusCodes.Status400BadRequest,
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));
            }

            _logger.LogInformation("Creating sales quotation for CardCode: {CardCode}", dto.CardCode);
            var resp = await _sl.PostAsync("SalesQuotation", dto);
            var json = await resp.Content.ReadAsStringAsync();
            
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = (int)resp.StatusCode
            };
        }

        /// <summary>
        /// Creates a sales quotation and retrieves business partner information in a single batch operation
        /// </summary>
        /// <param name="dto">Sales quotation data</param>
        /// <returns>Quotation and business partner details</returns>
        /// <response code="200">Batch operation successful</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="502">Bad Gateway - Service Layer error</response>
        [Authorize]
        [HttpPost("batch/create-quotation-and-get-partner")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> BatchQuotationAndPartner([FromBody] SalesQuotationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Invalid quotation data",
                    StatusCodes.Status400BadRequest,
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));
            }

            _logger.LogInformation(
                "Executing batch operation: Create quotation and get partner for CardCode: {CardCode}",
                dto.CardCode
            );

            try
            {
                var createResp = await _sl.PostAsync("SalesQuotation", dto);
                createResp.EnsureSuccessStatusCode();
                
                using var cq = JsonDocument.Parse(await createResp.Content.ReadAsStringAsync());
                
                var bpResp = await _sl.GetAsync($"BusinessPartners('{dto.CardCode}')");
                bpResp.EnsureSuccessStatusCode();
                
                var bpJson = await bpResp.Content.ReadAsStringAsync();
                
                var result = new
                {
                    Quotation = cq.RootElement,
                    BusinessPartner = JsonDocument.Parse(bpJson).RootElement
                };

                return Ok(ApiResponse<object>.SuccessResponse(result));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Batch operation failed for CardCode: {CardCode}", dto.CardCode);
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    ApiResponse<object>.ErrorResponse(
                        "Failed to complete batch operation with Service Layer",
                        StatusCodes.Status502BadGateway
                    )
                );
            }
        }
    }
}
