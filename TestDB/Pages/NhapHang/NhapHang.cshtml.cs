using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDB.Pages.Hang;
using System.Data.SqlClient;

namespace TestDB.Pages.NhapHang
{
    public class NhapHangModel : PageModel
    {
        public List<NKInfo> listNK = new List<NKInfo>();
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select MaNhap, TenNCC, ThoiGian, TongTien, MaNV, GiamGia from NHAP join NHACUNGCAP on NHAP.MaNCC = NHACUNGCAP.MaNCC where NHACUNGCAP.TenNCC <> 'deleted' order by MaNhap DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NKInfo NK = new NKInfo();
                                NK.MaNhap = reader.GetString(0);
                                NK.TenNCC = reader.GetString(1);
                                NK.ThoiGian = reader.GetDateTime(2);
                                NK.TongTien = reader.GetDecimal(3);
                                NK.MaNV = reader.GetString(4);
                                NK.GiamGia = reader.GetInt32(5);
                                listNK.Add(NK);
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
        public NKInfo searchInfo = new NKInfo();
        public void OnPost()
        {
            searchInfo.Search = Request.Form["Search"];
            if (searchInfo.Search.Length == 0)
            {
                Response.Redirect("/NhapHang/NhapHang");
            }
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var search = new List<string>() { searchInfo.Search };
                    String sql1 = "select MaNhap, TenNCC, ThoiGian, TongTien, MaNV, GiamGia from NHAP join NHACUNGCAP on NHAP.MaNCC = NHACUNGCAP.MaNCC where (MaNhap like '%" + search[0] + "%' or TenNCC like '%" + search[0] + "%') and TenNCC <> 'deleted' order by MaNhap DESC";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@Search", searchInfo.Search);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NKInfo NK = new NKInfo();
                                NK.MaNhap = reader.GetString(0);
                                NK.TenNCC = reader.GetString(1);
                                NK.ThoiGian = reader.GetDateTime(2);
                                NK.TongTien = reader.GetDecimal(3);
                                NK.MaNV = reader.GetString(4);
                                NK.GiamGia = reader.GetInt32(5);

                                listNK.Add(NK);
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

    public class NKInfo
    {
        public string? Search;
        public string? MaNhap;
        public string? TenNCC;
        public DateTime ThoiGian;
        public string? MaNCC;
        public string? MaNV;
        public int? GiamGia;
        public decimal TongTien;
        public int result;
    }
}
