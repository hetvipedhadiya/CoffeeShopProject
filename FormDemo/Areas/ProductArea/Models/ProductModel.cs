using System.ComponentModel.DataAnnotations;

namespace FormDemo.Areas.ProductArea.Models
{
    public class ProductModel
    {
        [Required(ErrorMessage = "ProductId is Not Empty")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "ProductName is Not Empty")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "ProductPrice is Not Empty")]

        public decimal ProductPrice { get; set; }
        [Required(ErrorMessage = "ProductCode is Not Empty")]
        [MinLength(6)]
        public string ProductCode { get; set; }
        [Required(ErrorMessage = "Description is Not Empty")]
        public string Description { get; set; }
        [Required(ErrorMessage = "UserId is Not Empty")]
        public int UserId
        {
            get; set;

        }
    }
}
