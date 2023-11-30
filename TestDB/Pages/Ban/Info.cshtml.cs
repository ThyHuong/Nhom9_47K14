using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace TestDB.Pages.Ban
{
    public class InfoModel : PageModel
    {
        public CTHDInfo cthdInfo = new CTHDInfo();
        public List<CthdInfo> listCThd = new List<CthdInfo>();
        public void OnGet()
        {
            string MaHD = Request.Query["MaHD"];
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select MaHD, ThoiGian, Ban.MaKH, TenKH, TongTien, GiamGia, nhanvien.MaNV, nhanvien.TenNV from KHACHHANG join Ban on KHACHHANG.MaKH = Ban.MaKH join nhanvien on ban.MaNV=Nhanvien.MaNV where MaHD=@MaHD";
                    String sql1 = "select MaHD, Hang.MaH, TenHang, Ban_CHITIET.SoLuong, GiaBan, ThanhTien from HANG join Ban_CHITIET on HANG.MaH = Ban_CHITIET.MaH where MaHD=@MaHD";
                    String sql2 = "select count(MaHD), sum(SoLuong) from Ban_CHITIET where MaHD=@MaHD";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MaHD", MaHD);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cthdInfo.MaHD = reader.GetString(0);
                                cthdInfo.ThoiGian = reader.GetDateTime(1);
                                cthdInfo.MaKH = reader.GetString(2);
                                cthdInfo.TenKH = reader.GetString(3);
                                cthdInfo.TongTien = reader.GetDecimal(4);
                                cthdInfo.GiamGia = reader.GetInt32(5);
                                cthdInfo.MaNV = reader.GetString(6);
                                cthdInfo.TenNV = reader.GetString(7);
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@MaHD", MaHD);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CthdInfo CThd = new CthdInfo();
                                CThd.MaH = reader.GetString(1);
                                CThd.TenHang = reader.GetString(2);
                                CThd.SoLuong = reader.GetInt32(3);
                                CThd.GiaBan = reader.GetDecimal(4);
                                CThd.ThanhTien = reader.GetDecimal(5);

                                listCThd.Add(CThd);
                            }

                        }

                    }
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@MaHD", MaHD);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cthdInfo.TongHang = reader.GetInt32(0);
                                cthdInfo.TongSL = reader.GetInt32(1);

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
    }

    public class CTHDInfo
    {
        public string? MaHD;
        public DateTime ThoiGian;
        public string? MaNV;
        public string? TenNV;
        public string? MaKH;
        public string? TenKH;
        public decimal TongTien;
        public int GiamGia;
        public int TongSL;
        public int TongHang;
    }

    public class CthdInfo
    {
        public string? MaH;
        public string? TenHang;
        public int SoLuong;
        public decimal GiaBan;
        public decimal ThanhTien;
    }
}