using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace TestDB.Pages.NhapHang
{
    public class InfoModel : PageModel
    {
        public CTNKInfo ctnkInfo = new CTNKInfo();
        public List<CtnkInfo> listCTnk = new List<CtnkInfo>();
        public void OnGet()
        {
            string MaNhap = Request.Query["MaNhap"];
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select MaNhap, ThoiGian, NhaCungCap.MaNCC, TenNCC, TongTien, GiamGia, nhanvien.MaNV, nhanvien.TenNV from NHAP join NhaCungCap on NHAP.MaNCC = NhaCungCap.MaNCC join nhanvien on nhap.MaNV=nhanvien.MaNV where MaNhap=@MaNhap";
                    String sql1 = "select MaNhap, HANG.MaH, TenHang, NHAP_CHITIET.SoLuong, GiaNhap, ThanhTien from NHAP_CHITIET join HANG on NHAP_CHITIET.MaH = HANG.MaH where MaNhap=@MaNhap";
                    String sql2 = "select count(MaNhap), sum(SoLuong) from NHAP_CHITIET where MaNhap=@MaNhap";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MaNhap", MaNhap);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ctnkInfo.MaNhap = reader.GetString(0);
                                ctnkInfo.ThoiGian = reader.GetDateTime(1);
                                ctnkInfo.MaNCC = reader.GetString(2);
                                ctnkInfo.TenNCC = reader.GetString(3);
                                ctnkInfo.TongTien = reader.GetDecimal(4);
                                ctnkInfo.GiamGia = reader.GetInt32(5);
                                ctnkInfo.MaNV = reader.GetString(6);
                                ctnkInfo.TenNV = reader.GetString(7);
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@MaNhap", MaNhap);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CtnkInfo CTnk = new CtnkInfo();
                                CTnk.MaH = reader.GetString(1);
                                CTnk.TenHang = reader.GetString(2);
                                CTnk.SoLuong = reader.GetInt32(3);
                                CTnk.GiaNhap = reader.GetDecimal(4);
                                CTnk.ThanhTien = reader.GetDecimal(5);

                                listCTnk.Add(CTnk);
                            }

                        }

                    }
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@MaNhap", MaNhap);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ctnkInfo.TongHang = reader.GetInt32(0);
                                ctnkInfo.TongSL = reader.GetInt32(1);
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

    public class CTNKInfo
    {
        public string? MaNhap;
        public DateTime ThoiGian;
        public string? MaNV;
        public string? TenNV;
        public string? MaNCC;
        public string? TenNCC;
        public decimal TongTien;
        public int GiamGia;
        public int TongSL;
        public int TongHang;
    }

    public class CtnkInfo
    {
        public string? MaH;
        public string? TenHang;
        public int SoLuong;
        public decimal GiaNhap;
        public decimal ThanhTien;
    }
}