using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDB.Pages.Hang;
using System.Data.SqlClient;

namespace TestDB.Pages
{
    public class IndexModel : PageModel
    {
        public TKInfo loginInfo = new TKInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {

        }
        public void OnPost()
        {
            loginInfo.UserName = Request.Form["UserName"];
            loginInfo.Pass = Request.Form["Pass"];

            if (loginInfo.UserName.Length == 0 || loginInfo.Pass.Length == 0)
            {
                errorMessage = "Vui lòng nhập username và password";
                return;
            }

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql1 = "select dbo.fCheckLogin (@UserName, @Pass)";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", loginInfo.UserName);
                        command.Parameters.AddWithValue("@Pass", loginInfo.Pass);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                loginInfo.result = reader.GetInt32(0);
                            }

                            
                        }
                        if (loginInfo.result == 1)
                        {
                            successMessage = "Đăng nhập thành công!";
                            Response.Redirect("/Ban/Create");
                        }
                        else
                        {
                            errorMessage = "Username hoặc password không đúng";
                            return;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

        }
    }
    public class TKInfo
    {
        public string? UserName;
        public string? Pass;
        public int result;
    }
}