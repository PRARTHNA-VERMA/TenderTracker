using System.ComponentModel.DataAnnotations;


namespace TenderTracker.Models
{
    public class TenderModel
    {
        public TenderModelData rowData { get; set; }
        //public List<State> states { get; set; }
        public bool IsSelected { get; set; }
        public string? tender_remark { get; set; }

        public int tender_id { get; set; }

        public List<TenderModel> Tendermodel { get; set; }
    }
    public class ExcelViewUpload
    {
       
        public IFormFile? ExcelUpload { get; set; }

        public List<TenderData> TenderList { get; set; }
    }
    public class TenderData
    {
        public string? Empanelment_type { get; set; }
        public int? Tender { get; set; }
        public string? Department { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Manpower { get; set; }
        public string? filename { get; set; }
        public decimal? EMD { get; set; }
        public decimal? Tender_fee { get; set; }

        public DateOnly? Pre_bid_date { get; set; }  // Nullable DateTime
        public DateOnly? Tender_due_date { get; set; }  // Nullable DateTime
    }

    public class TenderModelData
    {
        public int id { get; set; }

        [Required(ErrorMessage = "*")]
        public string? Empanelment_type { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^[0-9]\d*$", ErrorMessage = "Invalid Tender No. Format")]
        public string? Tender { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^[a-zA-Z_]+$", ErrorMessage = "Invalid Department Format")]
        public string? Department { get; set; }

        [Required(ErrorMessage = "*")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "*")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "*")]
        public string? State { get; set; }

        [Required(ErrorMessage = "*")]
        public string? City { get; set; }
        public string? tender_remark { get; set; }  

        [Required(ErrorMessage = "*")]  
        public string? Manpower { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^[0-9]\d*$", ErrorMessage = "Invalid EMD Format")]
        public string? EMD { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^(?!0\d)\d+(\.\d+)?$", ErrorMessage = "Invalid Tender Fee Format")]
        public string? Tender_fee { get; set; }

        [Required(ErrorMessage = "*")]
        public string? Pre_bid_date { get; set; }

        [Required(ErrorMessage = "*")]
        public string? Tender_due_date { get; set; }

        [Required(ErrorMessage = "*")]
        public string? Remarks { get; set; }

        public int? remark_status { get; set; } = 0;

        public string? created_date { get; set; }

        public string? submitted_by { get; set; }

        public int? status { get; set; }

        public string? Name { get; set; }

        [Required(ErrorMessage = "*")]
        public IFormFile? tender_file { get; set; }
        public string?tender_file_name { get; set; }
       
        public List<State> states { get; set; }

        public List<City> cities { get; set; }
    }

    public class State
    {

        public int StateId { get; set; }
        public string StateName { get; set; }
    }
    public class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int StateId { get; set; }
    }
}
