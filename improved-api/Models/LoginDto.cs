using System.ComponentModel.DataAnnotations;

namespace ApiGateServiceLayer.Models
{
    /// <summary>
    /// Data transfer object for SAP Service Layer login credentials
    /// </summary>
    public record LoginDto
    {
        /// <summary>
        /// SAP Company Database name
        /// </summary>
        [Required(ErrorMessage = "CompanyDB is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "CompanyDB must be between 1 and 100 characters")]
        public string CompanyDB { get; init; } = string.Empty;

        /// <summary>
        /// SAP User name
        /// </summary>
        [Required(ErrorMessage = "UserName is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "UserName must be between 1 and 100 characters")]
        public string UserName { get; init; } = string.Empty;

        /// <summary>
        /// SAP User password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Password must be between 1 and 100 characters")]
        public string Password { get; init; } = string.Empty;
    }
}
