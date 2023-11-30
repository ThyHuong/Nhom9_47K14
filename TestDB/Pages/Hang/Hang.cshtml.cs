using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace TestDB.Pages.Hang
{
    public class HangModel : PageModel
    {
        public List<HangInfo> listHang = new List<HangInfo>();
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM HANG WHERE TenHang <> 'deleted'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HangInfo Hang = new HangInfo();
                                Hang.MaH = reader.GetString(0);
                                Hang.TenHang = reader.GetString(1);
                                Hang.GiaBan = reader.GetDecimal(2);
                                Hang.GiaNhap = reader.GetDecimal(3);

                                listHang.Add(Hang);
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
        public HangInfo searchInfo = new HangInfo();
        public void OnPost()
        {
            searchInfo.Search = Request.Form["Search"];
            if (searchInfo.Search.Length == 0 )
            {
                Response.Redirect("/Hang/Hang");
            }
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var search = new List<string>() {searchInfo.Search};
                    String sql1 = "select * from HANG where (MaH like '%" + search[0] + "%' or TenHang like '%" + search[0] + "%') and TenHang <>'deleted'";

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
                                Hang.GiaNhap = reader.GetDecimal(3);

                                listHang.Add(Hang);
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

    public class HangInfo
    {
        public string? Search;
        public string? MaH;
        public string? TenHang;
        public decimal GiaNhap;
        public decimal GiaBan;
    }
}

