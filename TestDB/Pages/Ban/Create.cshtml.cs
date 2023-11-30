using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDB.Pages.Hang;
using TestDB.Pages.KhachHang;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace TestDB.Pages.Ban
{
    public class CreateModel : PageModel
    {
        public HDInfo hd = new HDInfo();
        public List<KHInfo> listKH = new List<KHInfo>();
        public KHInfo searchInfo = new KHInfo();
        
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            searchInfo.Search = Request.Query["Search"];
            string MaKh = Request.Query["MaKH"];
            DateTime now = DateTime.Now;
            hd.ThoiGian = now;
            hd.MaKH = MaKh;
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var search = new List<string>() { searchInfo.Search };
                    String sql3 = "select * from KHACHHANG where (MaKH like '%" + search[0] + "%' or SDT = CONVERT(varchar(10), hashbytes('MD5','" + search[0] + "'),1)) and TenKH <> 'deleted'";
                    String sql = "select dbo.fNewMaHD()";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hd.MaHD = reader.GetString(0);
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand(sql3, connection))
                    {
                        command.Parameters.AddWithValue("@Search", searchInfo.Search);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                KHInfo KH = new KHInfo();
                                KH.MaKH = reader.GetString(0);
                                KH.TenKH = reader.GetString(1);
                                KH.SDT = reader.GetString(2);
                                
                                listKH.Add(KH);
                            }
                        }

                    }

                }
            }
            catch (Exception)
            {
            }
        }
        
        
        public void OnPost()
        {
            hd.MaHD = Request.Form["MaHD"];
            hd.MaKH = Request.Form["MaKH"];
            hd.MaNV = Request.Form["MaNV"];
            hd.ThoiGian = DateTime.ParseExact(Request.Form["ThoiGian"], "dd/MM/yyyy",CultureInfo.InvariantCulture);
            hd.GiamGia = Convert.ToInt32(Request.Form["GiamGia"]);

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql2 = "insert into Ban values (@MaHD, @MaKH, @MaNV, @ThoiGian, @GiamGia, 0)";

                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@MaHD", hd.MaHD);
                        command.Parameters.AddWithValue("@MaKH", hd.MaKH);
                        command.Parameters.AddWithValue("@MaNV", hd.MaNV);
                        command.Parameters.AddWithValue("@ThoiGian", hd.ThoiGian);
                        command.Parameters.AddWithValue("@GiamGia", hd.GiamGia);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            Response.Redirect("/Ban/CTHD");


        }
    }
}
