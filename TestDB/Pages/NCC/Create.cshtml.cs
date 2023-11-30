using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace TestDB.Pages.NCC
{
    public class CreateModel : PageModel
    {
        public NCCInfo nccInfo = new NCCInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "select dbo.fNewMaNCC()";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nccInfo.MaNCC = reader.GetString(0);
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

            if (nccInfo.TenNCC.Length == 0)
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
                    String sql1 = "insert into NhaCungCap values(@MaNCC, @TenNCC)";

                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@MaNCC", nccInfo.MaNCC);
                        command.Parameters.AddWithValue("@TenNCC", nccInfo.TenNCC);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) 
            {
                errorMessage= ex.Message;
                return;
            }

            nccInfo.TenNCC = "";
            successMessage = "Thêm mới nhà cung cấp thành công!";

            Response.Redirect("/NCC/NCC");
        }
    }
}
