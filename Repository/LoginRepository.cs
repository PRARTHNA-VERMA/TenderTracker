using System.Data;
using TenderTracker.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TenderTracker.Repository
{
    public class LoginRepository
    {
        private readonly string ConnectionString;

        public LoginRepository(IConfiguration configuration)
        {
            ConnectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
        }

        public DataTable MatchUser(LoginModel model)
        {
            DataTable dt = new DataTable();


            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand("[dbo].[sp_userValid]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Password", model.Password);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);

            return dt;
        }
    }
}
