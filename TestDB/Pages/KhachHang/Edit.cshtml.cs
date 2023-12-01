using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace TestDB.Pages.KhachHang
{
    public class EditModel : PageModel
    {
        public KHInfo khInfo = new KHInfo();
        public KHInfo oldInfo = new KHInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            string MaKH = Request.Query["MaKH"];

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select * from KHACHHANG where MaKH=@MaKH";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MaKH", MaKH);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                khInfo.MaKH = reader.GetString(0);
                                khInfo.TenKH = reader.GetString(1);
                                khInfo.SDT = reader.GetString(2);
                                
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
            
            if (khInfo.MaKH.Length == 0 || khInfo.SDT.Length == 0 || khInfo.TenKH.Length == 0)
            {
                errorMessage = "Tên và số điện thoại không được để trống";
                return;
            }

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    String sql1 = "select * from KHACHHANG where MaKH=@MaKH";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@MaKH", khInfo.MaKH);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                oldInfo.MaKH = reader.GetString(0);
                                oldInfo.TenKH = reader.GetString(1);
                                oldInfo.SDT = reader.GetString(2);
                                
                            }
                        }
                    }
                    connection.Open();
                    if (khInfo.SDT != oldInfo.SDT) {
                        String sql = "update KHACHHANG " + "set TenKH=@TenKH, SDT=@SDT where MaKH=@MaKH";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@MaKH", khInfo.MaKH);
                            command.Parameters.AddWithValue("@TenKH", khInfo.TenKH);
                            command.Parameters.AddWithValue("@SDT", khInfo.SDT);
                            
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        String sql = "update KHACHHANG " + "set TenKH=@TenKH where MaKH=@MaKH";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@MaKH", khInfo.MaKH);
                            command.Parameters.AddWithValue("@TenKH", khInfo.TenKH);
                            
                            command.ExecuteNonQuery();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/KhachHang/KhachHang");
        }


    }
}
