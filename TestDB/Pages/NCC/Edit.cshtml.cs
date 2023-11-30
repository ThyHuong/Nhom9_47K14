using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace TestDB.Pages.NCC
{
    public class EditModel : PageModel
    {
        public NCCInfo nccInfo = new NCCInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            string MaNCC = Request.Query["MaNCC"];

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select * from NhaCungCap where MaNCC=@MaNCC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MaNCC", MaNCC);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nccInfo.MaNCC = reader.GetString(0);
                                nccInfo.TenNCC = reader.GetString(1);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            nccInfo.MaNCC = Request.Form["MaNCC"];
            nccInfo.TenNCC = Request.Form["TenNCC"];

            if (nccInfo.TenNCC.Length == 0 || nccInfo.MaNCC.Length == 0)
            {
                errorMessage = "Thông tin không được để trống";
                return;
            }

            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "update NhaCungCap " + "set TenNCC=@TenNCC where MaNCC=@MaNCC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MaNCC", nccInfo.MaNCC);
                        command.Parameters.AddWithValue("@TenNCC", nccInfo.TenNCC);

                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/NCC/NCC");
        }

        
    }
}
