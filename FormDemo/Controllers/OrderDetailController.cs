using FormDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace FormDemo.Controllers
{
    public class OrderDetailController : Controller
    {
        private IConfiguration configuration;
        public OrderDetailController(IConfiguration _configuration) { 
            configuration = _configuration;
        }
        #region OrderDetail List
        public IActionResult Index()
        {
           
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_OrderDetail_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(sqlDataReader);
            return View(dataTable);


        }
        #endregion

        #region Order DetailForm
        public IActionResult Detail(int? OrderDetailID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Order_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                OrderDropDownModel orderDropDownModel = new OrderDropDownModel();
                orderDropDownModel.OrderID = Convert.ToInt32(data["OrderID"]);
                orderDropDownModel.OrderNumber = Convert.ToInt32(data["OrderNumber"]);
                orderList.Add(orderDropDownModel);
            }
            ViewBag.OrderList = orderList;


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

            
            SqlCommand command3 = connection1.CreateCommand();
            command3.CommandType = System.Data.CommandType.StoredProcedure;
            command3.CommandText = "PR_Products_DropDown";
            SqlDataReader reader3 = command3.ExecuteReader();
            DataTable dataTable3 = new DataTable();
            dataTable3.Load(reader3);
            List<ProductDropdownModel> productList = new List<ProductDropdownModel>();
            foreach (DataRow data in dataTable3.Rows)
            {
                ProductDropdownModel productDropDownModel = new ProductDropdownModel();
                productDropDownModel.ProductID = Convert.ToInt32(data["ProductID"]);
                productDropDownModel.ProductName = data["ProductName"].ToString();
                productList.Add(productDropDownModel);
            }
            ViewBag.ProductList = productList;


            #region OrderDetailByID
            if (OrderDetailID == null || OrderDetailID == 0)
            {
                ViewBag.FormTitle = "Add OrderDetail";
                return View(new OrderDetail());
            }
            ViewBag.FormTitle = "Edit Orderdetail";
            SqlConnection connection4 = new SqlConnection(connectionString);
            connection4.Open();
            SqlCommand command4 = connection4.CreateCommand();
            command4.CommandType = CommandType.StoredProcedure;
            command4.CommandText = "PR_OrderDetail_SelectByID";
            command4.Parameters.AddWithValue("@id", OrderDetailID ?? 0);
            SqlDataReader reader4 = command4.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader4);
            OrderDetail orderDetailModel = new OrderDetail();

            foreach (DataRow dataRow in table.Rows)
            {
                orderDetailModel.OrderDetailID = Convert.ToInt32(@dataRow["OrderDetailID"]);
                orderDetailModel.OrderID = Convert.ToInt32(@dataRow["OrderID"]);
                orderDetailModel.ProductID = Convert.ToInt32(@dataRow["ProductID"]);
                orderDetailModel.Quantity = Convert.ToInt32(@dataRow["Quantity"]);
                orderDetailModel.Amount = Convert.ToDouble(@dataRow["Amount"]);
                orderDetailModel.TotalAmount = Convert.ToDouble(@dataRow["TotalAmount"]);
                orderDetailModel.UserId = Convert.ToInt32(@dataRow["UserID"]);
            }

            #endregion

            return View("Detail", orderDetailModel);
        }
        #endregion
        #region save order 
        public IActionResult Save(OrderDetail orderDetail)
        {
            if (orderDetail.OrderID <= 0)
            {
                ModelState.AddModelError("OrderID", "A valid Order is requried.");
            }
            if (orderDetail.UserId <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is requried.");
            }
            if (orderDetail.ProductID <= 0)
            {
                ModelState.AddModelError("ProductID", "A valid Product is requried.");
            }
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (orderDetail.OrderDetailID == 0)
                {
                    command.CommandText = "PR_OrderDetail_Insert";
                }
                else
                {
                    command.CommandText = "PR_OrderDetail_Update";
                    command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = orderDetail.OrderDetailID;
                }
                command.Parameters.Add("@orderID", SqlDbType.Int).Value = orderDetail.OrderID;
                command.Parameters.Add("@productID", SqlDbType.Int).Value = orderDetail.ProductID;
                command.Parameters.Add("@qnt", SqlDbType.Int).Value = orderDetail.Quantity;
                command.Parameters.Add("@amt", SqlDbType.Decimal).Value = orderDetail.Amount;
                command.Parameters.Add("@totalAmt", SqlDbType.Decimal).Value = orderDetail.TotalAmount;
                command.Parameters.Add("@userID", SqlDbType.Int).Value = orderDetail.UserId;
                command.ExecuteNonQuery();
                // Set success message in TempData
                TempData["SuccessMessage"] = "Record inserted successfully!";
                return RedirectToAction("Index");
            }
            return View("Detail", orderDetail);
        }
        #endregion

        #region delete order
        [HttpPost]
        public JsonResult deleteOrderDetail(int OrderDetailID)
        {
            try
            {
                string connectionstring = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionstring);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "PR_OrderDetail_Delete";
                sqlCommand.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderDetailID;
                sqlCommand.ExecuteNonQuery();

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
