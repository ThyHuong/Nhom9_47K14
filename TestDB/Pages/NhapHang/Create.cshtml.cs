using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDB.Pages.Hang;
using System.Collections.Generic;
using System;
using System.Data.SqlClient;

namespace TestDB.Pages.NhapHang
{
    public class CreateModel : PageModel
    {
        public NKInfo NK = new NKInfo();

        public ctnkInfo CTNK = new ctnkInfo();
        public List<ctnkInfo> list = new List<ctnkInfo>();
        public List<ctnkInfo> listCTNK = new List<ctnkInfo>();
        public List<HangInfo> listHang = new List<HangInfo>();
        public HangInfo searchInfo = new HangInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            searchInfo.Search = Request.Query["Search"];
            string MaH = Request.Query["MaH"];
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var search = new List<string>() { searchInfo.Search };
                    String sql = "select max(MaNhap) from NHAP";
                    String sql1 = "select MaH, TenHang, GiaNhap from HANG where (MaH like '%" + search[0] + "%' or TenHang like '%" + search[0] + "%') and TenHang <> 'deleted'";
                    String sql2 = "select MaNhap, MaNCC, ThoiGian, TongTien, GiamGia, MaNV from NHAP where MaNhap=@MaNhap";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CTNK.MaNhap = reader.GetString(0);
                                
                            }
                        }
                    }
                    String sql3 = "select MaNhap, HANG.MaH, TenHang, NHAP_CHITIET.SoLuong, GiaNhap, ThanhTien from NHAP_CHITIET join HANG on NHAP_CHITIET.MaH = HANG.MaH where MaNhap=@MaNhap";
                    using (SqlCommand command = new SqlCommand(sql3, connection))
                    {
                        command.Parameters.AddWithValue("@MaNhap", CTNK.MaNhap);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ctnkInfo ctnk = new ctnkInfo();
                                ctnk.MaNhap = reader.GetString(0);
                                ctnk.MaH = reader.GetString(1);
                                ctnk.TenHang = reader.GetString(2);
                                ctnk.SoLuong = reader.GetInt32(3);
                                ctnk.GiaNhap = reader.GetDecimal(4);
                                ctnk.ThanhTien = reader.GetDecimal(5);

                                listCTNK.Add(ctnk);
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@MaNhap", CTNK.MaNhap);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                NK.MaNhap = reader.GetString(0);
                                NK.MaNCC = reader.GetString(1);
                                NK.ThoiGian = reader.GetDateTime(2);
                                NK.TongTien = reader.GetDecimal(3);
                                NK.GiamGia = reader.GetInt32(4);
                                NK.MaNV = reader.GetString(5);
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
                                Hang.GiaNhap = reader.GetDecimal(2);

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
                    String sql2 = "select MaH, TenHang, GiaNhap from HANG where MaH=@MaH";
                    
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@MaH", MaH);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CTNK.MaH = reader.GetString(0);
                                CTNK.TenHang = reader.GetString(1);
                                CTNK.SoLuong = 1;
                                CTNK.GiaNhap = reader.GetDecimal(2);
                                CTNK.ThanhTien = CTNK.GiaNhap * CTNK.SoLuong;
                                list.Add(CTNK);
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
            CTNK.MaNhap = Request.Form["MaNhap"];
            CTNK.MaH = Request.Form["MaH"];
            CTNK.TenHang = Request.Form["TenHang"];
            CTNK.GiaNhap = Convert.ToDecimal(Request.Form["GiaNhap"]);
            CTNK.SoLuong = Convert.ToInt32(Request.Form["SoLuong"]);
            CTNK.ThanhTien = Convert.ToInt32(Request.Form["SoLuong"]) * Convert.ToDecimal(Request.Form["GiaNhap"]);

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql1 = "insert into NHAP_CHITIET values (@MaNhap, @MaH, @SoLuong, @ThanhTien)";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@MaNhap", CTNK.MaNhap);
                        command.Parameters.AddWithValue("@MaH", CTNK.MaH);
                        command.Parameters.AddWithValue("@SoLuong", CTNK.SoLuong);
                        command.Parameters.AddWithValue("@ThanhTien", CTNK.ThanhTien);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            Response.Redirect("/NhapHang/Create");
        }
    }
    public class ctnkInfo
    {
        public string? MaNhap;
        public string? MaH;
        public string? TenHang;
        public int SoLuong;
        public decimal GiaNhap;
        public decimal ThanhTien;

    }
}
