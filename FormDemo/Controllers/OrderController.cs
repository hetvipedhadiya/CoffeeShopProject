using FormDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace FormDemo.Controllers
{
    public class OrderController : Controller
    {
       private IConfiguration configuration;
        public OrderController(IConfiguration _configuration)
        {
           configuration = _configuration;
        }

        #region OrderList
        public IActionResult Index()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Orders_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();   
            DataTable dataTable = new DataTable();
            dataTable.Load(sqlDataReader);
            return View(dataTable);
        }
        #endregion

        #region OrderForm

        public IActionResult OrderForm(int? OrderID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Customers_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<CustomerDropDownModel> customerList = new List<CustomerDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel();
                customerDropDownModel.CustomerID = Convert.ToInt32(data["CustomerID"]);
                customerDropDownModel.CustomerName = data["CustomerName"].ToString();
                customerList.Add(customerDropDownModel);
            }
            ViewBag.CustomerList = customerList;


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
            if (OrderID == null || OrderID == 0)
            {
                ViewBag.FormTitle = "Add Order";
                return View(new OrderModel());
            }
            ViewBag.FoemTitle = "Edit Order";
            // Fetching the product details for editing
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Order_SelectByID";
            command.Parameters.AddWithValue("@OrderID", OrderID);
            using SqlDataReader reader = command.ExecuteReader();
            DataTable table = new();
            table.Load(reader);

            OrderModel orderModel = new();
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                orderModel.OrderID = Convert.ToInt32(row["OrderID"]);
                orderModel.OrderDate = Convert.ToDateTime(row["OrderDate"]);
                orderModel.CustomerID = Convert.ToInt16(row["CustomerID"]);
                orderModel.PaymentMode = row["PaymentMode"].ToString();
                orderModel.TotalAmount = Convert.ToDecimal(row["TotalAmount"]);
                orderModel.ShoppingAddress = row["ShippingAddress"].ToString();
                orderModel.OrderNumber = Convert.ToInt32(row["OrderNumber"]);
                orderModel.UserID = Convert.ToInt32(row["UserID"]);
            }

            return View(orderModel);
        }
        #endregion

        #region Save Order
        public IActionResult SaveOrder(OrderModel orderModel)
        {
            if (!ModelState.IsValid)
            {
                return View("OrderForm", orderModel);
            }

            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            // Decide between insert or update based on ProductId
            if (orderModel.OrderID <= 0)
            {
                command.CommandText = "pr_orders_insert";
            }
            else
            {
                command.CommandText = "PR_Order_Update";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderModel.OrderID;
            }

            // Common Parameters for Insert and Update
            command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = orderModel.OrderDate;
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = orderModel.CustomerID;
            command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = orderModel.PaymentMode;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderModel.TotalAmount;
            command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShoppingAddress;
            command.Parameters.Add("@OrderNumber",SqlDbType.Int).Value = orderModel.OrderNumber;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderModel.UserID;

            command.ExecuteNonQuery();
            // Set success message in TempData
            TempData["SuccessMessage"] = "Record inserted successfully!";
            return RedirectToAction("Index");
        }

        #endregion

        #region delete Order
        [HttpPost]
        public JsonResult deleteOrder(int OrderID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_Order_Delete";
                    sqlCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                    sqlCommand.ExecuteNonQuery();
                }

                // Return success as a JSON response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the error message and return failure response
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        #endregion




    }
}
