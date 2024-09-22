
using FormDemo.Areas.ProductArea.Models;
using FormDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;

namespace FormDemo.Controllers
{
    public class BillsController : Controller
    {
        private IConfiguration configuration;
        public BillsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #region Bill List
        public IActionResult Index()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Bills_SelectAll";
            /*database mathi data read kare*/
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable dataTable= new DataTable();
            dataTable.Load(sqlDataReader);
            return View(dataTable);
        }
        #endregion
        #region BillForm
        public IActionResult BillForm(int? BillId)
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

            // If ProductId is null or 0, return empty ProductModel for adding a new product
            if (BillId == null || BillId == 0)
            {
                ViewBag.FormTitle = "Add Bill";
                return View(new BillsModel());
            }
            ViewBag.FormTitle = "Edit Bill";
            // Fetching the product details for editing
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Bills_SelectByID";
            command.Parameters.AddWithValue("@BillID", BillId);
            using SqlDataReader reader = command.ExecuteReader();
            DataTable table = new();
            table.Load(reader);

            BillsModel billModel = new();
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                billModel.BillId = Convert.ToInt32(row["BillID"]);
                billModel.BillNumber = row["BillNumber"].ToString();
                billModel.BillDate = DateOnly.FromDateTime(Convert.ToDateTime(row["BillDate"]));
                billModel.OrderID = Convert.ToInt32(row["OrderID"]);
                billModel.TotalAmount = Convert.ToDouble(row["TotalAmount"]);
                billModel.Discount = Convert.ToDouble(row["Discount"]);
                billModel.NetAmount = Convert.ToDouble(row["NetAmount"]);
                billModel.UserId = Convert.ToInt32(row["UserID"]);
            }

            return View(billModel);
        }
        #endregion

        #region SaveBill
        public IActionResult SaveBill(BillsModel billsmodel)
        {
            if (!ModelState.IsValid)
            {
                // Return the same form view with validation errors
                return View("BillForm", billsmodel);
            }

            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            // Insert or Update based on BillId
            if (billsmodel.BillId <= 0)
            {
                command.CommandText = "PR_Bills_Insert";
            }
            else
            {
                command.CommandText = "PR_Bills_Update";
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = billsmodel.BillId;
            }

            // Common Parameters for Insert and Update
            command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billsmodel.BillNumber;
            command.Parameters.Add("@BillDate", SqlDbType.Date).Value = billsmodel.BillDate.ToDateTime(TimeOnly.MinValue);
            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = billsmodel.OrderID;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billsmodel.TotalAmount;
            command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = billsmodel.Discount;
            command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billsmodel.NetAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = billsmodel.UserId;

            command.ExecuteNonQuery();

            // Set success message in TempData
            TempData["SuccessMessage"] = "Record inserted successfully!";

            // Redirect to the Index action to display the message
            return RedirectToAction("Index");
        }
        #endregion

        #region deleteBill
        [HttpPost]
        public JsonResult deleteBill(int BillID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_Bills_Delete";
                    sqlCommand.Parameters.Add("@BillID", SqlDbType.Int).Value = BillID;
                    sqlCommand.ExecuteNonQuery();
                }

                // Return success response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine(ex.ToString());

                // Return error response
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        #endregion


    }
}
