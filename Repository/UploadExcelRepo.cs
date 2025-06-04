using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using TenderTracker.Models;

namespace TenderTracker.Repository
{
    public class UploadExcelRepo
    {
        private readonly string ConnectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UploadExcelRepo(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            ConnectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
            _httpContextAccessor = httpContextAccessor;
        }

        public List<State> GetStates()
        {
            List<State> states = new List<State>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[dbo].[stateList]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    states.Add(new State
                    {
                        StateId = Convert.ToInt32(reader["id"]),
                        StateName = reader["name"].ToString()
                    });
                }
            }
            return states;
        }

        public List<City> GetCitiesByState(int stateId)
        {
            List<City> cities = new List<City>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[dbo].[cityList]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@State_id", stateId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cities.Add(new City
                    {
                        CityId = Convert.ToInt32(reader["id"]),
                        CityName = reader["city"].ToString(),
                        StateId = Convert.ToInt32(reader["state_id"])
                    });
                }
            }

            return cities;
        }

        public string GetStateName(int state_id)
        {
            string stateName = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[GetState]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@state_id", state_id);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        stateName = reader["name"].ToString(); // Replace 'StateName' with your actual column name
                    }
                }
            }

            return stateName; // Return the state name
        }

        public string GetCityByID(int city_id)
        {
            string cityName = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[GetCity]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@city_id", city_id);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        cityName = reader["city"].ToString(); // Replace 'CityName' with your actual column name
                    }
                }
            }

            return cityName; // Return the city name
        }

        public DataTable GetCity(TenderModel model)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[GetCity]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "getcity");
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable DeleteCity(int id)
        {

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[GetCity]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@action", "deletecity");
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);
                }
            }
            return dt;
        }

        public int AddCity(TenderModel model)
        {
            int result = 0;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[GetCity]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@City", model.Citymodel.CityName);
                    cmd.Parameters.AddWithValue("@StateId", model.Citymodel.StateId);
                    cmd.Parameters.AddWithValue("@action", "addcity");
                    conn.Open();
                    result = (int)cmd.ExecuteScalar();
                    conn.Close();
                }
            }
            return result; // Return the result of the operation
        }

        public int insertForm_data(TenderModel model)
        {
            var Name = _httpContextAccessor.HttpContext.Session.GetString("Name");
            int result = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[Sp_insertTender_data]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@Empanelment_type", model.rowData.Empanelment_type);
                        cmd.Parameters.AddWithValue("@Tender", model.rowData.Tender);
                        cmd.Parameters.AddWithValue("@Department", model.rowData.Department);
                        cmd.Parameters.AddWithValue("@State", model.rowData.State);
                        cmd.Parameters.AddWithValue("@StateId", model.rowData.StateId);
                        cmd.Parameters.AddWithValue("@City", model.rowData.City);
                        cmd.Parameters.AddWithValue("@CityId", model.rowData.CityId);
                        cmd.Parameters.AddWithValue("@Manpower", model.rowData.Manpower);
                        cmd.Parameters.AddWithValue("@EMD", model.rowData.EMD);
                        cmd.Parameters.AddWithValue("@Tender_fee", model.rowData.Tender_fee); // Hash before passing
                        cmd.Parameters.AddWithValue("@Pre_bid_date", model.rowData.Pre_bid_date);
                        cmd.Parameters.AddWithValue("@Tender_due_date", model.rowData.Tender_due_date);
                        cmd.Parameters.AddWithValue("@Remarks", model.rowData.Remarks);

                        var fileName = Path.GetFileName(model.rowData.tender_file?.FileName);
                        var filenamewithoutextension = Path.GetFileNameWithoutExtension(fileName);
                        var extension = Path.GetExtension(fileName);
                        string vardatetime = DateTime.Now.ToString("ddMMyyyyHHmmssffff");
                        var newfilenamewithoutextension = vardatetime + filenamewithoutextension;
                        var finalfilename = newfilenamewithoutextension + extension;

                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/document", finalfilename);
                        FileInfo file = new FileInfo(Path.Combine(path));
                        var stream = new FileStream(path, FileMode.Create);
                        model.rowData.tender_file.CopyTo(stream);
                        stream.Close();

                        cmd.Parameters.AddWithValue("@tender_file", finalfilename);
                        cmd.Parameters.AddWithValue("@submitted_by", Name);
                        cmd.Parameters.AddWithValue("@action", "insert");

                        //cmd.Parameters.AddWithValue("@tender_file", model.rowData.tender_file);
                        //cmd.Parameters.AddWithValue("@city_id", model.CityId);
                        //cmd.Parameters.AddWithValue("@state_id", model.StateId);

                        conn.Open();
                        // Execute the query and get the result
                        result = (int)cmd.ExecuteScalar();  // Modify to ExecuteScalar for SELECT
                        conn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log the exception
                Debug.WriteLine("SQL Exception: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                Debug.WriteLine("Exception: " + ex.Message);
            }

            //// Send the generated password to the user's email
            //SendPasswordToEmail(rid.email, generatedPassword, rid);

            return result;
        }
        
        public string Pendingforassigning()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[Sp_insertTender_data]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "pendingforassign");
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);   //Fills the DataTable object dt with data retrieved from the database
                }
            }
            return JsonConvert.SerializeObject(dt);
        }
        public string PendingforRemark(int usertype)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[Sp_insertTender_data]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user_id", usertype);
                    cmd.Parameters.AddWithValue("@action", "pendingremark");
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);   //Fills the DataTable object dt with data retrieved from the database
                }
            }
            return JsonConvert.SerializeObject(dt);
        }
        public string ApprovalRecordList()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[Sp_insertTender_data]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "approvallist");
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);   //Fills the DataTable object dt with data retrieved from the database
                }
            }
            return JsonConvert.SerializeObject(dt);
        }

        public int ApproveTender(List<int> selectedTenderIds)
        {
            var Name = _httpContextAccessor.HttpContext.Session.GetString("Name");
            var id = _httpContextAccessor.HttpContext.Session.GetString("id");
            int user_id = Convert.ToInt32(id);
            int result = 0;
            try
            {
                for (int i = 0; i < selectedTenderIds.Count ; i++)
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("[dbo].[Sp_insertTender_data]", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Tender_id", selectedTenderIds[i]);
                            cmd.Parameters.AddWithValue("@user_id", user_id);
                            cmd.Parameters.AddWithValue("@action", "assign");
                            conn.Open();
                            result += (int)cmd.ExecuteScalar();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
            return result;
        }

        public string FinalRecordList()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[Sp_insertTender_data]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "finallist");
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);   //Fills the DataTable object dt with data retrieved from the database
                }
            }
            return JsonConvert.SerializeObject(dt);
        }

        public string GetTenderById(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[Sp_insertTender_data]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Tender_id", id);
                    cmd.Parameters.AddWithValue("@action", "getbyid");
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);   //Fills the DataTable object dt with data retrieved from the database
                }
            }
            return JsonConvert.SerializeObject(dt);

        }

        public int AddRemarks(TenderModel model)
        {
            var id = _httpContextAccessor.HttpContext.Session.GetString("id");
            int user_id = Convert.ToInt32(id);
            int result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[Sp_insertTender_data]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Tender_id", model.rowData.id);
                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        cmd.Parameters.AddWithValue("@Remarks", model.rowData.Remarks);
                        cmd.Parameters.AddWithValue("@action", "addRemarks");
                        conn.Open();
                        result = (int)cmd.ExecuteScalar();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
            return result;
        }
        public int SaveTenderData(List<TenderData> dataList)
        {
            var Name = _httpContextAccessor.HttpContext.Session.GetString("Name");

            int rowsInserted = 0;

            using (var conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[dbo].[Sp_insertTender_data]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    foreach (var data in dataList)
                    {
                        cmd.Parameters.Clear(); // Clear previous parameters

                        cmd.Parameters.AddWithValue("@Empanelment_type", data.Empanelment_type);
                        cmd.Parameters.AddWithValue("@Tender", data.Tender);
                        cmd.Parameters.AddWithValue("@Department", data.Department);
                        cmd.Parameters.AddWithValue("@State", data.State);
                        cmd.Parameters.AddWithValue("@City", data.City);
                        cmd.Parameters.AddWithValue("@Manpower", data.Manpower);
                        cmd.Parameters.AddWithValue("@EMD", data.EMD);
                        cmd.Parameters.AddWithValue("@Tender_fee", data.Tender_fee);
                        cmd.Parameters.AddWithValue("@Pre_bid_date", data.Pre_bid_date);
                        cmd.Parameters.AddWithValue("@Tender_due_date", data.Tender_due_date);
                        //cmd.Parameters.Add("@Pre_bid_date", SqlDbType.DateTime).Value =data.Pre_bid_date == DateTime.MinValue ? DBNull.Value : (object)data.Pre_bid_date;
                        //cmd.Parameters.Add("@Tender_due_date", SqlDbType.DateTime).Value =                   data.Tender_due_date == DateTime.MinValue ? DBNull.Value : (object)data.Tender_due_date;

                        cmd.Parameters.AddWithValue("@submitted_by", Name);

                        cmd.Parameters.AddWithValue("@tender_file", data.filename);
                        cmd.Parameters.AddWithValue("@action", "insertexcel");
                        rowsInserted += cmd.ExecuteNonQuery();
                    }
                }
            }

            return rowsInserted;
        }

    }
}
