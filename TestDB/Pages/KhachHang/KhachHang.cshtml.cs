using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDB.Pages.Hang;
using System.Data.SqlClient;

namespace TestDB.Pages.KhachHang
{
    public class KhachHangModel : PageModel
    {
        public List<KHInfo> listKH = new List<KHInfo>();
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from KHACHHANG where TenKH <> 'deleted'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public KHInfo searchInfo = new KHInfo();
        public void OnPost()
        {
            searchInfo.Search = Request.Form["Search"];
            if (searchInfo.Search.Length == 0)
            {
                Response.Redirect("/KhachHang/KhachHang");
            }
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var search = new List<string>() { searchInfo.Search };
                    String sql1 = "select * from KHACHHANG where (MaKH like '%" + search[0] + "%' or SDT = CONVERT(varchar(10), hashbytes('MD5','" + search[0] + "'),1)) and TenKH <> 'deleted'";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
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
    }

    public class KHInfo
    {
        public string? Search;
        public string? MaKH;
        public string? TenKH;
        public string? SDT;
        public int result;
    }
}