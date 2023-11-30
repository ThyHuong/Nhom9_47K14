using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static TestDB.Pages.Hang.HangModel;

namespace TestDB.Pages.Hang
{
    public class EditModel : PageModel
    {
        public HangInfo hangInfo = new HangInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            string MaH = Request.Query["MaH"];

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select * from HANG where MaH=@MaH";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MaH", MaH);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hangInfo.MaH = reader.GetString(0);
                                hangInfo.TenHang = reader.GetString(1);
                                hangInfo.GiaBan = reader.GetDecimal(2);
                                hangInfo.GiaNhap = reader.GetDecimal(3);

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

            if (hangInfo.TenHang.Length == 0 || hangInfo.GiaNhap == 0 ||
                hangInfo.GiaBan == 0 || hangInfo.MaH == "")
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
                    String sql = "update HANG " + "set TenHang=@TenHang, GiaNhap=@GiaNhap, GiaBan=@GiaBan where MaH=@MaH";

                    using (SqlCommand command = new SqlCommand(sql, connection))
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

            Response.Redirect("/Hang/Hang");
        }


    }
}
