using FormDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace FormDemo.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration configuration;
        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #region UserList
        public IActionResult Index()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection sqlConnection = new(connectionString);
            sqlConnection.Open();
            using SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_User_SelectAll";
            using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable dataTable = new();
            dataTable.Load(sqlDataReader);

            return View(dataTable);
        }
        #endregion
        #region userAddEdit Form
        public IActionResult UserForm(int? UserID)
        {
            if (UserID == null || UserID == 0)
            {
                ViewBag.FormTitle = "Add User";
                return View(new UserModel());
            }
            ViewBag.FormTitle = "Edit User";
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_SelectByID";
            command.Parameters.AddWithValue("@UserID",  UserID);
            using SqlDataReader reader = command.ExecuteReader();
            DataTable table = new();
            table.Load(reader);

            UserModel userModel = new();
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                userModel.UserID = Convert.ToInt32(row["UserID"]);
                userModel.UserName = row["UserName"].ToString();
                userModel.Email = row["Email"].ToString();
                userModel.Password = row["Password"].ToString();
                userModel.MobileNo = row["MobileNo"].ToString();
                userModel.Address = row["Address"].ToString();
                userModel.isActive = Convert.ToBoolean(row["IsActive"]);
            }

            return View(userModel);
        }
        #endregion

        #region addUser
        [HttpPost]
        public IActionResult Save(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View("UserForm", userModel);
            }

            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (userModel.UserID == 0)
            {
                command.CommandText = "PR_User_Insert";
            }
            else
            {
                command.CommandText = "PR_User_Update";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userModel.UserID;
            }

            command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userModel.UserName;
            command.Parameters.Add("@Email", SqlDbType.VarChar).Value = userModel.Email;
            command.Parameters.Add("@Password", SqlDbType.VarChar).Value = userModel.Password;
            command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userModel.MobileNo;
            command.Parameters.Add("@Address", SqlDbType.VarChar).Value = userModel.Address;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = userModel.isActive;

            command.ExecuteNonQuery();
            // Set success message in TempData
            TempData["SuccessMessage"] = "Record inserted successfully!";
            return RedirectToAction("Index");
        }
        #endregion
        #region deleteUser
        [HttpPost]
        public JsonResult DeleteUser(int UserID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "PR_users_Delete";
                        sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                // Return success response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.ToString());

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        #endregion


        #region login
        [HttpGet]
        public IActionResult Login()
        {

        return View(); 
        }
        public IActionResult Login(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                        HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                    }

                    return View("Index", "Product");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return View("Login","User");
        }

        #endregion

        #region logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        #endregion


        #region conver password into mass password
        private string MaskPassword(string password)
        {
            // Ensure that the password is exactly 8 characters long
            if (password.Length == 8)
            {
                // Mask the first 5 characters with '*' and keep the last 3 characters visible
                return "*****" + password.Substring(5);
            }

            // Throw an exception if the password is not exactly 8 characters long
            throw new Exception("Password must be exactly 8 characters long.");
        }
        #endregion



    }
}
