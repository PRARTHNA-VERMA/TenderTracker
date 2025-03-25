using Microsoft.AspNetCore.Mvc;
using TenderTracker.Repository;
using TenderTracker.Models;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Reflection;
using TenderTracker.Filter;

namespace TenderTracker.Controllers
{
    [ServiceFilter(typeof(LoginFilter))]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly UploadExcelRepo _adminRepo;

        public DashboardController(ILogger<DashboardController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _adminRepo = new UploadExcelRepo(configuration, httpContextAccessor);
        }

        public IActionResult Dashboard()
        {
            // Retrieve user type from session
            var user_type = HttpContext.Session.GetString("user_type").ToString();
            var Name = HttpContext.Session.GetString("Name").ToString();

            ViewBag.user_type = user_type; // Pass user type to the view

            // Retrieve all records from the repository
            var dt = _adminRepo.FinalRecordList();
            dt = dt.Replace("tender_file", "tender_file_name");  // Replace JSON key
            var tenderDataList = JsonConvert.DeserializeObject<List<TenderModelData>>(dt);

            // Directly map the data to TenderModel
            var result = tenderDataList.Select(item => new TenderModel
            {
                rowData = item // Assigning entire TenderModelData object
            }).ToList();
            return View(result);
        }

        public IActionResult ViewTender(int id)
        {
            // Fetch tender data from the repository
            var tenderJson = _adminRepo.GetTenderById(id);
            tenderJson = tenderJson.Replace("tender_file", "tender_file_name");  // Replace JSON key
            var tenders = JsonConvert.DeserializeObject<List<TenderModelData>>(tenderJson);
            var Name = HttpContext.Session.GetString("Name").ToString();

            ViewBag.name = Name;
            // Initialize TenderModel with rowData to prevent null reference errors
            var model = new TenderModel
            {
                rowData = new TenderModelData()
                {
                    states = _adminRepo.GetStates() // Ensure this method returns a valid list
                }
            };

            // If data exists, populate the model
            if (tenders != null && tenders.Any())
            {
                for (var i = 0; i < tenders.Count; i++)
                {

                    model.rowData = tenders.FirstOrDefault();
                }
                // Ensure rowData is not null
                if (model.rowData == null)
                {
                    model.rowData = new TenderModelData();
                }

                // Assign states to rowData
                model.rowData.states = _adminRepo.GetStates();
            }

            return View(model); // Always return a non-null model
        }
        [HttpPost]
        public IActionResult ViewTender(TenderModel model)
        {
            ModelState.Remove("rowdata.states");
            ModelState.Remove("rowdata.cities");
            ModelState.Remove("rowdata.state");
            ModelState.Remove("rowdata.city");
            ModelState.Remove("state");
            ModelState.Remove("city");
            ModelState.Remove("tender_id");
            ModelState.Remove("IsSelected");
            ModelState.Remove("Tendermodel");
            //if (ModelState.IsValid)
            {
                //model.rowData.State = _adminRepo.GetStateName(model.rowData.StateId);
                //model.rowData.City = _adminRepo.GetCityByID(model.rowData.CityId);
                int result = _adminRepo.AddRemarks(model);
                if (result == 0)
                {
                    TempData["ErrorMessage"] = "Remark for this Tender has already been Added.";
                    return RedirectToAction("Dashboard");
                }
                if (result == 1) // Success
                {
                    var Remark_status = new TenderModel();
                    Remark_status.rowData.remark_status = 1;
                    TempData["SuccessMessage"] = "Successfully Added Remarks";
                    return RedirectToAction("Dashboard");
                }
                if (result == -1) 
                {
                    TempData["ErrorMessage1"] = "Can't add Remarks before Assigning";
                    return RedirectToAction("Dashboard");
                }

            }
            //model.rowData.states = _adminRepo.GetStates(); // Fetch states from the repository
            return View(model);
        }


        //public IActionResult ViewTender(int id)
        //{
        //    var model = new TenderModel
        //    {
        //        //states = _adminRepo.GetStates()
        //        rowData = new TenderModelData()
        //        {
        //            states = _adminRepo.GetStates() // Ensure this method returns a valid list, not null
        //        }
        //    };
        //    var tender = _adminRepo.GetTenderById(id);
        //    var result = JsonConvert.DeserializeObject<List<TenderModel>>(tender);


        //    if (result != null && result.Any())
        //    {
        //        return View(result.First()); // Pass a single TenderModel, not a List<TenderModel>
        //    }
        //    //var resultList = new ExcelViewUpload
        //    //{
        //    //    rowData = new RowData()
        //    //};
        //    //resultList.rowData = result?.FirstOrDefault();
        //    return View();
        //}

        public IActionResult GetCities(int stateId)
        {
            var cities = _adminRepo.GetCitiesByState(stateId);
            return Json(cities);
        }

        [HttpGet]
        public IActionResult addRecordForm()
        {
            //var model = new TenderModel
            //{
            //    states = _adminRepo.GetStates() // Fetch states from the repository
            //};
            var model = new TenderModel
            {
                //states = _adminRepo.GetStates()
                rowData = new TenderModelData()
                {
                    states = _adminRepo.GetStates() // Ensure this method returns a valid list, not null
                }
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult addRecordForm(TenderModel model)
        {
            ModelState.Remove("rowdata.states");
            ModelState.Remove("rowdata.cities");
            ModelState.Remove("rowdata.state");
            ModelState.Remove("rowdata.city");
            ModelState.Remove("state");
            ModelState.Remove("city");
            ModelState.Remove("tender_id");
            ModelState.Remove("IsSelected");
            ModelState.Remove("Tendermodel");
            ModelState.Remove("rowdata.Remarks");
            if (ModelState.IsValid)
            {
                model.rowData.State = _adminRepo.GetStateName(model.rowData.StateId);
                model.rowData.City = _adminRepo.GetCityByID(model.rowData.CityId);

                int result = _adminRepo.insertForm_data(model);

                if (result == 1) // Success
                {
                    TempData["SuccessMessage"] = "Successfully Inserted";
                    return RedirectToAction("Dashboard");
                }
                if (result == -1) 
                {
                    TempData["ErrorMessage"] = "Tender No. already added";
                    return View();
                }
            }
            model.rowData.states = _adminRepo.GetStates(); // Fetch states from the repository

            return View(model);
        }

        public IActionResult ApprovalTenderList()
        {
            // Retrieve user type from session
            var usertype = HttpContext.Session.GetString("user_type").ToString();
            var Name = HttpContext.Session.GetString("Name").ToString();

            ViewBag.userType = usertype; // Pass user type to the view

            // Retrieve all records from the repository
            var dt = _adminRepo.ApprovalRecordList();
            dt = dt.Replace("tender_file", "tender_file_name");  // Replace JSON key
            
            // Filter records based on user type
            //var filteredResult = result.Where(item => usertype != "3" || item.submitted_by == Name).ToList();
            var tenderDataList = JsonConvert.DeserializeObject<List<TenderModelData>>(dt);

            // Directly map the data to TenderModel
            var result = tenderDataList.Select(item => new TenderModel
            {
                rowData = item // Assigning entire TenderModelData object
            }).ToList();

            return View(result);
        }

        [HttpPost]
        public IActionResult ApprovalTenderList([FromBody] List<TenderModel> model)
        {
            int result = 0;
            // Get selected tenders where IsSelected == true
            var selectedTenders = model?.Where(t => t.IsSelected).ToList();
            var selectedTenderIds = selectedTenders.Select(t => t.tender_id).ToList();

            //if (selectedTenders.Any())
            //{
            //foreach (var tender in selectedTenders)
            //{
            //    result = _adminRepo.ApproveTender(tender.rowData.id); // Call your method for each selected tender
            //}
            for (int i = 0; i < selectedTenderIds.Count; i++)
            {
                if (model[i].IsSelected)
                {
                    result = _adminRepo.ApproveTender(selectedTenderIds[i]); // Call your method for each selected tender
                    if (result == 1)
                    {
                        TempData["SuccessMessage"] = "Successfully Assigned";
                        return RedirectToAction("ApprovalTenderList");
                    }
                    if (result == -1)
                    {
                        //TempData["ErrorMessage1"] = "User already has 4 active assignments";
                        //return RedirectToAction("ApprovalTenderList");
                        
                            return Json("-1"); // Return "-1" as JSON

                    }
                }
            }
            //}

            return View();
        }

        /*[HttpPost]
public IActionResult ApproveTenders(List<TenderModel> model)
{
    if (model == null || !model.Any(m => m.IsSelected))
    {
        TempData["ErrorMessage"] = "No tenders selected for approval.";
        return RedirectToAction("Dashboard");
    }

    try
    {
        foreach (var item in model.Where(m => m.IsSelected))
        {
            // Call repository function to update the status
            _adminRepo.UpdateTenderStatus(item.Id, "Approved", HttpContext.Session.GetString("Name"));
        }

        TempData["SuccessMessage"] = "Selected tenders approved successfully.";
    }
    catch (Exception ex)
    {
        TempData["ErrorMessage"] = "Error approving tenders: " + ex.Message;
    }

    return RedirectToAction("Dashboard");
}
*/

        [HttpGet]
        public IActionResult ViewExcel()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult ViewExcel(ExcelViewUpload model)
        //{
        //    if (model.excelUpload == null || model.excelUpload.File.Length == 0)
        //    {
        //        ViewBag.ErrorMessage = "Please upload a valid Excel file.";
        //        return View(model);
        //    }

        //    List<RowData> rowList = new List<RowData>();
        //    try
        //    {
        //        using (var package = new ExcelPackage(model.excelUpload.File.OpenReadStream()))
        //        {
        //            var worksheet = package.Workbook.Worksheets[0]; // Get the first sheet
        //            var rowCount = worksheet.Dimension.Rows;

        //            for (int row = 2; row <= rowCount; row++) // Skip header
        //            {
        //                var rowData = new RowData
        //                {
        //                    Sl = worksheet.Cells[row, 1].Text,
        //                    Name = worksheet.Cells[row, 2].Text,//DateTime.TryParse(worksheet.Cells[row, 2].Text, out var parsedDate) ? parsedDate : (DateTime?)null,
        //                    Qualification = worksheet.Cells[row, 3].Text,
        //                    Experience = worksheet.Cells[row, 4].Text,
        //                    Skill = worksheet.Cells[row, 5].Text,
        //                    Current_CTC = worksheet.Cells[row, 6].Text,
        //                    Expected_CTC = worksheet.Cells[row, 7].Text,
        //                    Notice_Period = worksheet.Cells[row, 8].Text,
        //                    Contact_No = worksheet.Cells[row, 9].Text,
        //                    Mail_Id = worksheet.Cells[row, 10].Text,
        //                    Remarks = worksheet.Cells[row, 11].Text,
        //                    Hr_marks = worksheet.Cells[row, 12].Text,
        //                    Tech_marks = worksheet.Cells[row, 13].Text,
        //                    CV_Name = worksheet.Cells[row, 14].Text
        //                };
        //                rowList.Add(rowData);
        //            }
        //        }

        //        model.RowList = rowList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //public IActionResult SaveData([FromForm] ExcelViewUpload model)
        //{
        //    if (model.RowList == null || !model.RowList.Any())
        //    {
        //        ViewBag.ErrorMessage = "No data available to save.";
        //        return RedirectToAction("Dashboard");
        //    }

        //    try
        //    {
        //        int rowsAffected = _adminRepo.SaveExcelData(model.RowList);
        //        if (rowsAffected > 0)
        //        {
        //            TempData["SuccessMessage"] = "Uploaded Successfully";
        //            return RedirectToAction("Dashboard");
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrorMessage = $"Error while saving data: {ex.Message}";
        //    }

        //    TempData["ErrorMessage"] = "Uploading Failed, Email or Contact already Exists";
        //    return RedirectToAction("ViewExcel");
        //}

        public IActionResult AssignTenderList()
        {
            
            // Retrieve all records from the repository
            var dt = _adminRepo.Pendingforassigning();
            dt = dt.Replace("tender_file", "tender_file_name");  // Replace JSON key

            // Filter records based on user type
            //var filteredResult = result.Where(item => usertype != "3" || item.submitted_by == Name).ToList();
            var tenderDataList = JsonConvert.DeserializeObject<List<TenderModelData>>(dt);

            // Directly map the data to TenderModel
            var result = tenderDataList.Select(item => new TenderModel
            {
                rowData = item // Assigning entire TenderModelData object
            }).ToList();

            return View(result);
        }

        public IActionResult PendingforRemark()
        {
            var user_type = HttpContext.Session.GetString("id").ToString();
            var Name = HttpContext.Session.GetString("Name").ToString();
            int usertype = (int)Convert.ToInt64(user_type);
            ViewBag.user_type = user_type;
            var dt = _adminRepo.PendingforRemark(usertype);
            dt = dt.Replace("tender_file", "tender_file_name");  // Replace JSON key
            var tenderDataList = JsonConvert.DeserializeObject<List<TenderModelData>>(dt);

            // Directly map the data to TenderModel
            var result = tenderDataList.Select(item => new TenderModel
            {
                rowData = item // Assigning entire TenderModelData object
            }).ToList();
            return View(result);
        }
    }
}

