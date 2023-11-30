using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDB.Pages.Hang;
using System.Data.SqlClient;

namespace TestDB.Pages.Ban
{
    public class BanModel : PageModel
    {
        public List<HDInfo> listHD = new List<HDInfo>();
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select MaHD, MaNV, TenKH, ThoiGian, TongTien, GiamGia from Ban join KHACHHANG on Ban.MaKH = KHACHHANG.MaKH where KHACHHANG.TenKH <> 'deleted' order by MaHD DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HDInfo HD = new HDInfo();
                                HD.MaHD = reader.GetString(0);
                                HD.MaNV = reader.GetString(1);
                                HD.TenKH = reader.GetString(2);
                                HD.ThoiGian = reader.GetDateTime(3);
                                HD.TongTien = reader.GetDecimal(4);
                                HD.GiamGia = reader.GetInt32(5);

                                listHD.Add(HD);
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public HDInfo searchInfo = new HDInfo();
        public void OnPost()
        {
            searchInfo.Search = Request.Form["Search"];
            if (searchInfo.Search.Length == 0)
            {
                Response.Redirect("/Ban/Ban");
            }
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var search = new List<string>() { searchInfo.Search };
                    String sql1 = "select MaHD, MaNV, TenKH, ThoiGian, TongTien, GiamGia from Ban join KHACHHANG on Ban.MaKH = KHACHHANG.MaKH where (MaHD like '%" + search[0] + "%' or SDT = CONVERT(varchar(10), hashbytes('MD5','" + search[0] + "'),1)) and KHACHHANG.TenKH <> 'deleted' order by MaHD DESC";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@Search", searchInfo.Search);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HDInfo HD = new HDInfo();
                                HD.MaHD = reader.GetString(0);
                                HD.MaNV = reader.GetString(1);
                                HD.TenKH = reader.GetString(2);
                                HD.ThoiGian = reader.GetDateTime(3);
                                HD.TongTien = reader.GetDecimal(4);
                                HD.GiamGia = reader.GetInt32(5);
                                
                                listHD.Add(HD);
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
        }
    }

    public class HDInfo
    {
        public string? Search;
        public string? MaHD;
        public string? TenKH;
        public DateTime ThoiGian;
        public decimal TongTien;
        public int GiamGia;
        public string? MaKH;
        public string? MaNV;
        public int result;
    }

    
}
