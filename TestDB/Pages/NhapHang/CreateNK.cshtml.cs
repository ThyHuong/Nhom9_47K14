using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDB.Pages.Hang;
using TestDB.Pages.Ban;
using TestDB.Pages.NCC;
using System.Data.SqlClient;
using System.Globalization;

namespace TestDB.Pages.NhapHang
{
    public class CreateNKModel : PageModel
    {
        public NKInfo nk = new NKInfo();
        public List<NCCInfo> listNCC = new List<NCCInfo>();
        public HangInfo searchInfo = new HangInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
            searchInfo.Search = Request.Query["Search"];
            string MaNcc = Request.Query["MaNCC"];
            DateTime now = DateTime.Now;
            nk.MaNCC = MaNcc;
            nk.ThoiGian = now;
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var search = new List<string>() { searchInfo.Search };
                    String sql = "select dbo.fNewMaNhap()";
                    String sql1 = "select * from NhaCungCap where (TenNCC like '%" + search[0] + "%' or MaNCC like '%" + search[0] + "%') and TenNCC <> 'deleted'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nk.MaNhap = reader.GetString(0);
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
                                NCCInfo ncc = new NCCInfo();
                                ncc.MaNCC = reader.GetString(0);
                                ncc.TenNCC = reader.GetString(1);
                                listNCC.Add(ncc);
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
            nk.MaNhap = Request.Form["MaNhap"];
            nk.MaNCC = Request.Form["MaNCC"];
            nk.MaNV = Request.Form["MaNV"];
            nk.ThoiGian = DateTime.ParseExact(Request.Form["ThoiGian"],"dd/MM/yyyy",CultureInfo.InvariantCulture);
            nk.GiamGia = Convert.ToInt32(Request.Form["GiamGia"]);
            try
            {
                String connectionString = "Data Source=THYHUONG;Initial Catalog=TestDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql2 = "insert into NHAP values (@MaNhap, @MaNCC, @MaNV, @ThoiGian, @GiamGia, 0)";

                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        command.Parameters.AddWithValue("@MaNhap", nk.MaNhap);
                        command.Parameters.AddWithValue("@MaNCC", nk.MaNCC);
                        command.Parameters.AddWithValue("@MaNV", nk.MaNV);
                        command.Parameters.AddWithValue("@ThoiGian", nk.ThoiGian);
                        command.Parameters.AddWithValue("@GiamGia", nk.GiamGia);

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
}
