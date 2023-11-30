using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDB.Pages.Hang;
using TestDB.Pages.NhapHang;
using System.Data.SqlClient;

namespace TestDB.Pages.Ban
{
    public class CTHDModel : PageModel
    {
        public HDInfo HD = new HDInfo();
        public List<HangInfo> listHang = new List<HangInfo>();
        public HangInfo searchInfo = new HangInfo();
        public cthdInfo CTHD = new cthdInfo();
        public List<cthdInfo> listCTHD = new List<cthdInfo>();
        public List<cthdInfo> list = new List<cthdInfo>();

        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            string MaH = Request.Query["MaH"];
            searchInfo.Search = Request.Query["Search"];
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var search = new List<string>() { searchInfo.Search };
                    String sql1 = "select MaH, TenHang, GiaBan from HANG where (MaH like '%" + search[0] + "%' or TenHang like '%" + search[0] + "%') and TenHang <> 'deleted'";
                    String sql2 = "select * from Ban where MaHD=@MaHD";
                    String sql3 = "select max(MaHD) from Ban";
                    String sql4 = "select MaHD, HANG.MaH, TenHang, Ban_CHITIET.SoLuong, GiaBan, ThanhTien from Ban_CHITIET join HANG on Ban_CHITIET.MaH = HANG.MaH where MaHD=@MaHD";
                    using (SqlCommand command = new SqlCommand(sql3, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CTHD.MaHD = reader.GetString(0);
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand(sql4, connection))
                    {
                        command.Parameters.AddWithValue("@MaHD", CTHD.MaHD);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cthdInfo cthd = new cthdInfo();
                                cthd.MaHD = reader.GetString(0);
                                cthd.MaH = reader.GetString(1);
                                cthd.TenHang = reader.GetString(2);
                                cthd.SoLuong = reader.GetInt32(3);
                                cthd.GiaBan = reader.GetDecimal(4);
                                cthd.ThanhTien = reader.GetDecimal(5);

                                listCTHD.Add(cthd);
                            }
                        }
                    }
                   
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@MaHD", CTHD.MaHD);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                HD.MaHD = reader.GetString(0);
                                HD.MaKH = reader.GetString(1);
                                HD.MaNV = reader.GetString(2);
                                HD.ThoiGian = reader.GetDateTime(3);
                                HD.GiamGia = reader.GetInt32(4);
                                HD.TongTien = reader.GetDecimal(5);
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@Search", searchInfo.Search);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HangInfo Hang = new HangInfo();
                                Hang.MaH = reader.GetString(0);
                                Hang.TenHang = reader.GetString(1);
                                Hang.GiaBan = reader.GetDecimal(2);

                                listHang.Add(Hang);
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select MaH, TenHang, GiaBan from HANG where MaH=@MaH";
                   
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MaH", MaH);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CTHD.MaH = reader.GetString(0);
                                CTHD.TenHang = reader.GetString(1);
                                CTHD.SoLuong = 1;
                                CTHD.GiaBan = reader.GetDecimal(2);
                                CTHD.ThanhTien = CTHD.GiaBan * CTHD.SoLuong;
                                list.Add(CTHD);
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
            CTHD.MaHD = Request.Form["MaHD"];
            CTHD.MaH = Request.Form["MaH"];
            CTHD.TenHang = Request.Form["TenHang"];
            CTHD.GiaBan = Convert.ToDecimal(Request.Form["GiaBan"]);
            CTHD.SoLuong = Convert.ToInt32(Request.Form["SoLuong"]);
            CTHD.ThanhTien = Convert.ToInt32(Request.Form["SoLuong"]) * Convert.ToDecimal(Request.Form["GiaBan"]);

            if (CTHD.SoLuong == 0)
            {
                errorMessage = "Số lượng không hợp lệ";
                return;
            }

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql1 = "insert into Ban_CHITIET values (@MaHD, @MaH, @SoLuong, @ThanhTien)";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@MaHD", CTHD.MaHD);
                        command.Parameters.AddWithValue("@MaH", CTHD.MaH);
                        command.Parameters.AddWithValue("@SoLuong", CTHD.SoLuong);
                        command.Parameters.AddWithValue("@ThanhTien", CTHD.ThanhTien);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception)
            {
            }
            Response.Redirect("/Ban/CTHD");
        }
    }
    public class cthdInfo
    {
        public string? MaHD;
        public string? MaH;
        public string? TenHang;
        public int SoLuong;
        public decimal GiaBan;
        public decimal ThanhTien;

    }
}
