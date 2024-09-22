using FormDemo.Areas.ProductArea.Models;
using FormDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace FormDemo.Controllers
{
    public class CustomerController : Controller
    {
        private IConfiguration configuration;
        public CustomerController(IConfiguration _configuration) {
            configuration = _configuration;
           }
        #region Customer List
        public IActionResult Index()
        {
            
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Customers_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(sqlDataReader);
            return View(dataTable);

        }
        #endregion

        #region CustomerForm
        public IActionResult CustomerForm(int? customerID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command2 = connection1.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_Users_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                UserDropDownModel userDropDownModel = new UserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;
            // If ProductId is null or 0, return empty ProductModel for adding a new product
            if (customerID == null || customerID == 0)
            {
                ViewBag.FormTitle = "Add Customer";

                return View(new CustomerModel());
            }

            ViewBag.FormTitle = "Edit Customer";
            // Fetching the product details for editing
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Cus_SelectByID";
            command.Parameters.AddWithValue("@CustomerID", customerID);
            using SqlDataReader reader = command.ExecuteReader();
            DataTable table = new();
            table.Load(reader);

            CustomerModel custModel = new();
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                custModel.CustomerId = Convert.ToInt32(row["CustomerID"]);
                custModel.CustomerName = row["CustomerName"].ToString();
                custModel.HomeAddress = (row["HomeAddress"]).ToString();
                custModel.Email = row["Email"].ToString();
                custModel.MobileNo = row["MobileNo"].ToString();
                custModel.GSTNo = row["GSTNo"].ToString();
                custModel.CityName = row["CityName"].ToString();
                custModel.PinCode = row["PinCode"].ToString();
                custModel.NetAmount = Convert.ToDouble(row["NetAmount"]);
                custModel.UserId = Convert.ToInt32(row["UserID"]);
            }

            return View(custModel);
        }

        #endregion

        #region Save Customer
        public IActionResult SaveCust(CustomerModel customerModel)
        {
            if (!ModelState.IsValid)
            {
                return View("CustomerForm", customerModel);
            }

            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            // Decide between insert or update based on ProductId
            if (customerModel.CustomerId <= 0)
            {
                command.CommandText = "PR_Cus_Insert";
            }
            else
            {
                command.CommandText = "PR_Cus_Update";
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerModel.CustomerId;
            }

            // Common Parameters for Insert and Update
            command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = customerModel.CustomerName;
            command.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = customerModel.HomeAddress;
            command.Parameters.Add("@Email", SqlDbType.VarChar).Value = customerModel.Email;
            command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = customerModel.MobileNo;
            command.Parameters.Add("@GSTNo", SqlDbType.VarChar).Value = customerModel.GSTNo;
            command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = customerModel.CityName;
            command.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = customerModel.PinCode;
            command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = customerModel.NetAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = customerModel.UserId;

            command.ExecuteNonQuery();
            // Set success message in TempData
            TempData["SuccessMessage"] = "Record inserted successfully!";
            return RedirectToAction("Index");
        }
        #endregion
        #region delete Customer
        [HttpPost]
        public JsonResult deleteCustomer(int CustomerID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_Cus_Delete";
                    sqlCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = CustomerID;
                    sqlCommand.ExecuteNonQuery();
                }

                // Return success response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.ToString());

                // Return error response
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        #endregion
    }
}
