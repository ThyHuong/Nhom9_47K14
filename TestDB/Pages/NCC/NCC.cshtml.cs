using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace TestDB.Pages.NCC
{
    public class NCCModel : PageModel
    {
        public List<NCCInfo> listNCC = new List<NCCInfo>();
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM NhaCungCap where TenNCC <> 'deleted'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NCCInfo NCC = new NCCInfo();
                                NCC.MaNCC = reader.GetString(0);
                                NCC.TenNCC = reader.GetString(1);
                                listNCC.Add(NCC);
                            }

                        }
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public NCCInfo searchInfo = new NCCInfo();
        public void OnPost()
        {
            searchInfo.Search = Request.Form["Search"];
            if (searchInfo.Search.Length == 0)
            {
                Response.Redirect("/NCC/NCC");
            }
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var search = new List<string>() { searchInfo.Search };
                    String sql1 = "select * from NhaCungCap where (TenNCC like '%" + search[0] + "%' or MaNCC like '%" + search[0] + "%') and TenNCC <> 'deleted'";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@Search", searchInfo.Search);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NCCInfo NCC = new NCCInfo();
                                NCC.MaNCC = reader.GetString(0);
                                NCC.TenNCC = reader.GetString(1);
                                listNCC.Add(NCC);
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

    public class NCCInfo
    {
        public string? Search;
        public string? MaNCC;
        public string? TenNCC;
    }
}
