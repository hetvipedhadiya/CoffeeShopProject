using System.ComponentModel.DataAnnotations;

namespace FormDemo.Models
{
    public class CustomerModel
    {
        [Required(ErrorMessage = "The CustomerId is not empty ")]
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "The CustomerName is not empty ")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "The HomeAddress is not empty ")]
        public string HomeAddress { get; set; }
        [Required(ErrorMessage = "The Email is not empty ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The MobileNo is not empty ")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "The GSTNo is not empty ")]
        public string GSTNo { get; set; }
        [Required(ErrorMessage = "The CityName is not empty ")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "The PinCode is not empty ")]
        public string PinCode { get; set; }
        [Required(ErrorMessage = "The NetAmount is not empty ")]
        public double NetAmount { get; set; }
        [Required(ErrorMessage = "The User is not empty ")]
        public int UserId { get; set; }

    }
    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }


}
