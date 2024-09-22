using System.ComponentModel.DataAnnotations;

namespace FormDemo.Models
{
    public class OrderModel
    {
        [Required(ErrorMessage = "OrderId is Not Empty")]
        public int OrderID { get; set; }
        [Required(ErrorMessage = "OrderNumber is Not Empty")]
        public int OrderNumber { get; set; }
        [Required(ErrorMessage = "OrderDate is Not Empty")]
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "CustomerId is Not Empty")]
        public int CustomerID{ get; set; }
        [Required(ErrorMessage = "PaymentMode is Not Empty")]
        public string PaymentMode { get; set; }
        [Required(ErrorMessage = "TotalPayment is Not Empty")]
        public decimal TotalAmount { get; set; }
        [Required(ErrorMessage = "ShippingAddress is Not Empty")]
        public string ShoppingAddress { get; set; }
        [Required(ErrorMessage = "User is Not Empty")]
        public int UserID { get; set; }
    }

    public class OrderDropDownModel
    {
        public int OrderID { get; set; }
        public int OrderNumber { get; set; }
    }


}
