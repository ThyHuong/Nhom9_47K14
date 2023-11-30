using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDB.Pages.Hang;
using System.Data.SqlClient;

namespace TestDB.Pages.KhachHang
{
    public class CreateModel : PageModel
    {
        public KHInfo khInfo = new KHInfo();
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
                    String sql = "select dbo.fNewMaKH()";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                khInfo.MaKH = reader.GetString(0);
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
            khInfo.MaKH = Request.Form["MaKH"];
            khInfo.TenKH = Request.Form["TenKH"];
            khInfo.SDT = Request.Form["SDT"];

            if (khInfo.SDT.Length != 10)
            {
                errorMessage = "Số điện thoại không hợp lệ";
                return;
            }

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql1 = "insert into KHACHHANG values (@MaKH, @TenKH, @SDT)";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@MaKH", khInfo.MaKH);
                        command.Parameters.AddWithValue("@TenKH", khInfo.TenKH);
                        command.Parameters.AddWithValue("@SDT", khInfo.SDT);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            khInfo.TenKH = ""; khInfo.SDT = "";
            successMessage = "Thêm mới Khách hàng thành công!";

            Response.Redirect("/KhachHang/KhachHang");
        }
    }
}