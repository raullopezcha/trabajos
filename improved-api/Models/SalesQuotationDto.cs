using System.ComponentModel.DataAnnotations;

namespace ApiGateServiceLayer.Models
{
    /// <summary>
    /// Data transfer object for SAP Sales Quotation
    /// </summary>
    public class SalesQuotationDto
    {
        /// <summary>
        /// Business Partner card code
        /// </summary>
        [Required(ErrorMessage = "CardCode is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "CardCode must be between 1 and 50 characters")]
        public string CardCode { get; set; } = string.Empty;

        /// <summary>
        /// Document date
        /// </summary>
        [Required(ErrorMessage = "DocDate is required")]
        public DateTime DocDate { get; set; }

        /// <summary>
        /// List of document lines (items)
        /// </summary>
        [Required(ErrorMessage = "DocumentLines is required")]
        [MinLength(1, ErrorMessage = "At least one document line is required")]
        public List<SalesQuotationLineDto> DocumentLines { get; set; } = new();
    }

    /// <summary>
    /// Data transfer object for Sales Quotation line item
    /// </summary>
    public class SalesQuotationLineDto
    {
        /// <summary>
        /// Item code
        /// </summary>
        [Required(ErrorMessage = "ItemCode is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "ItemCode must be between 1 and 50 characters")]
        public string ItemCode { get; set; } = string.Empty;

        /// <summary>
        /// Quantity
        /// </summary>
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Unit price
        /// </summary>
        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public double Price { get; set; }
    }
}
