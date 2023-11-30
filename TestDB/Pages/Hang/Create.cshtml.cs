using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static TestDB.Pages.Hang.HangModel;

namespace TestDB.Pages.Hang
{
    public class CreateModel : PageModel
    {
        public HangInfo hangInfo = new HangInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select dbo.fNewMaH()";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hangInfo.MaH = reader.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            hangInfo.MaH = Request.Form["MaH"];
            hangInfo.TenHang = Request.Form["TenHang"];
            hangInfo.GiaNhap = Convert.ToDecimal(Request.Form["GiaNhap"]);
            hangInfo.GiaBan = Convert.ToDecimal(Request.Form["GiaBan"]);

            if (hangInfo.TenHang.Length == 0 || hangInfo.GiaNhap == 0 || hangInfo.GiaBan == 0)
            {
                errorMessage = "Thông tin không được để trống";
                return;
            }

            if (hangInfo.GiaNhap > hangInfo.GiaBan ||
                hangInfo.GiaNhap < 0 || hangInfo.GiaBan < 0)
            {
                errorMessage = "Thông tin không hợp lệ";
                return;
            }    

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    String sql1 = "insert into HANG values (@MaH, @TenHang, @GiaBan, @GiaNhap)";
                    
                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@MaH", hangInfo.MaH);
                        command.Parameters.AddWithValue("@TenHang", hangInfo.TenHang);
                        command.Parameters.AddWithValue("@GiaNhap", hangInfo.GiaNhap);
                        command.Parameters.AddWithValue("@GiaBan", hangInfo.GiaBan);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            hangInfo.TenHang = ""; hangInfo.GiaNhap = 0; hangInfo.GiaBan = 0;
            successMessage = "Thêm mới hàng hoá thành công!";

            Response.Redirect("/Hang/Hang");
        }
    }
}
