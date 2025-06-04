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

        public City? Citymodel { get; set; }
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

        [Required(ErrorMessage = "Empanelment Type is required.")]
        public string? Empanelment_type { get; set; }

        [Required(ErrorMessage = "Tender No. is required.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Tender No. Format")]
        public string? Tender { get; set; }

        [Required]
        [RegularExpression(@"^(?!\s*$)(?:_?[a-zA-Z]+(?:[_ ][a-zA-Z]+)*)$", ErrorMessage = "Invalid Department Format")]
        public string? Department { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int? StateId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int? CityId { get; set; }

        [Required]
        public string? State { get; set; }

        [Required]
        public string? City { get; set; }
        public string? tender_remark { get; set; }  

        [Required]  
        public string? Manpower { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]\d*$", ErrorMessage = "Invalid EMD Format")]
        public string? EMD { get; set; }

        [Required]
        [RegularExpression(@"^(?!0\d)\d+(\.\d+)?$", ErrorMessage = "Invalid Tender Fee Format")]
        public string? Tender_fee { get; set; }

        [Required]
        public string? Pre_bid_date { get; set; }

        [Required]
        public string? Tender_due_date { get; set; }

        [Required]
        public string? Remarks { get; set; }

        public int? remark_status { get; set; } = 0;

        public string? created_date { get; set; }

        public string? submitted_by { get; set; }

        public int? status { get; set; }

        public string? Name { get; set; }

        [Required]
        public IFormFile? tender_file { get; set; }
        public string?tender_file_name { get; set; }
        //public string? Base64Pdf { get; set; } // Optional: for re-rendering the PDF

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

        [Required(ErrorMessage = "City is required.")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Invalid City Format.")]
        public string? CityName { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int? StateId { get; set; }
    }
}
