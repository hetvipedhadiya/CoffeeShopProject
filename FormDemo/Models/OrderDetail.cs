using System.ComponentModel.DataAnnotations;

namespace FormDemo.Models
{
    public class OrderDetail
    {
        [Required(ErrorMessage = "The OrderDetailId is not empty ")]
        public int OrderDetailID { get; set; }

        [Required(ErrorMessage = "The Customer is not empty ")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "The User is not empty ")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "The Order is not empty ")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "The ordernumber is not empty ")]
        public int OrderNumber { get; set; }

        [Required(ErrorMessage = "The ProductId is not empty ")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "The Quantity is not empty ")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "The Amount is not empty ")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "The Total Amount is not empty ")]
        public double TotalAmount { get; set; }
    }
}
