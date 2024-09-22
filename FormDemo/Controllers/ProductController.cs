using FormDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FormDemo.Controllers
{
    public class ProductController : Controller
    {

        private readonly IConfiguration configuration;
        private readonly EmailService emailService;

        public ProductController(IConfiguration _configuration, EmailService _emailService)
        {
            configuration = _configuration;
            emailService = _emailService;
        }

        #region Product List
        public IActionResult Index()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Product_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(sqlDataReader);


            return View(dataTable);
        }
        #endregion
        #region Product Form

        public IActionResult ProductForm(int? ProductId)
        {
            // Fetching the User Dropdown List
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Users_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                UserDropDownModel userDropDownModel = new UserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;

            // If ProductId is null or 0, return empty ProductModel for adding a new product
            if (ProductId == null || ProductId == 0)
            {
                ViewBag.FormTitle = "Add Product";
                return View(new ProductModel());
            }
            ViewBag.FormTitle = "Edit Product";
            // Fetching the product details for editing
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_SelectByID";
            command.Parameters.AddWithValue("@ProductID", ProductId);
            using SqlDataReader reader = command.ExecuteReader();
            DataTable table = new();
            table.Load(reader);

            ProductModel productModel = new();
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                productModel.ProductId = Convert.ToInt32(row["ProductID"]);
                productModel.ProductName = row["ProductName"].ToString();
                productModel.ProductPrice = Convert.ToDecimal(row["ProductPrice"]);
                productModel.ProductCode = row["ProductCode"].ToString();
                productModel.Description = row["Description"].ToString();
                productModel.UserId = Convert.ToInt32(row["UserID"]);
            }

            return View(productModel);
        }

        #endregion
        #region Save Product
        [HttpPost]
        public IActionResult Save(ProductModel productModel)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductForm", productModel);
            }

            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            // Decide between insert or update based on ProductId
            if (productModel.ProductId <= 0)
            {
                command.CommandText = "PR_Product_Insert";
            }
            else
            {
                command.CommandText = "PR_Product_Update";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = productModel.ProductId;
            }

            // Common Parameters for Insert and Update
            command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = productModel.ProductName;
            command.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = productModel.ProductPrice;
            command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = productModel.ProductCode;
            command.Parameters.Add("@Description", SqlDbType.VarChar).Value = productModel.Description;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = productModel.UserId;

            command.ExecuteNonQuery();
            // Set success message in TempData
            TempData["SuccessMessage"] = "Record inserted successfully!";
            return RedirectToAction("Index");
        }
        #endregion
        #region delete product
        [HttpPost]
        public JsonResult deleteProduct(int ProductID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_Product_Delete";
                    sqlCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
                    sqlCommand.ExecuteNonQuery();
                }

                // Return success as a JSON response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine(ex.ToString());

                // Return error details as a JSON response
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        #endregion

        #region Export Excel
        public IActionResult exportExcel()
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Fetch data from the database using a stored procedure
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "PR_Product_SelectAll";
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(sqlDataReader);

                // Create an Excel package using EPPlus
                using (var package = new ExcelPackage())
                {
                    // Create a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Products");

                    // Load the DataTable into the worksheet, starting from cell A1
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

                    // Auto-fit columns for all cells
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Convert the Excel package to a byte array
                    var fileContents = package.GetAsByteArray();

                    // Return the Excel file as a download
                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }
        #endregion
        #region send mail
        public async Task<IActionResult> SendEmailsToUsers()
        {
            // Fetch data from SQL Server
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            string query = "SELECT * FROM Products"; // Replace with your actual query

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dataTable);
            }

            // Iterate through data and send emails
            foreach (DataRow row in dataTable.Rows)
            {
                string recipientEmail = row["UserId"].ToString(); // Replace with the actual column name
                string subject = $"Product Update: {row["ProductName"]}";
                string body = $"Dear User,<br><br>Here are the details for the product:<br>" +
                              $"Product Name: {row["ProductName"]}<br>" +
                              $"Product Price: {row["ProductPrice"]}<br>" +
                              $"Product Code: {row["ProductCode"]}<br>" +
                              $"Description: {row["Description"]}<br><br>Best Regards,<br>Your Company";

                await emailService.SendEmailAsync(recipientEmail, subject, body);
            }

            return View(); // Or redirect to another action/view as needed
        }

        #endregion



    }
}
