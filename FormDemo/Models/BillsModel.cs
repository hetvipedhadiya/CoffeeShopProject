using System.ComponentModel.DataAnnotations;

namespace FormDemo.Models
{
    public class BillsModel
    {
 
        public int BillId { get; set; }

        [Required(ErrorMessage = "Bill Number is required")]
        [MaxLength(20, ErrorMessage = "Bill Number can't be longer than 20 characters")]
        public string BillNumber { get; set; }

        [Required(ErrorMessage = "Bill Date is required")]
        public DateOnly BillDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);


        [Required(ErrorMessage = "Order is required")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Total Amount is required")]
        [Range(0, Double.MaxValue, ErrorMessage = "Total Amount must be a positive number")]
        public double TotalAmount { get; set; }

        [Required(ErrorMessage = "Discount is required")]
        [Range(0, Double.MaxValue, ErrorMessage = "Discount must be a positive number")]
        public double Discount { get; set; }

        [Required(ErrorMessage = "Net Amount is required")]
        [Range(0, Double.MaxValue, ErrorMessage = "Net Amount must be a positive number")]
        public double NetAmount { get; set; }

        [Required(ErrorMessage = "User is required")]
        public int UserId { get; set; }
    }
}
