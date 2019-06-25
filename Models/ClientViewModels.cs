using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingConfirm.Models
{
    public class ClientViewModel {
        
        [Display(Name = "First Name")]
        public string fname { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage = "The field Name is required")]
        [Display(Name = "Last Name")]
        public string lname { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The Reservation Number is required")]
        [RegularExpression(@"([0-9]+)", ErrorMessage = "Reservation Number must be a number.")]
        [StringLength(9, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Reservation Number")]
        public string reservNumber { get; set; }

        public string propCode { get; set; }
    }

    public class BookingViewModel
    {
        public string property { get; set; }
        public string index { get; set; }

        [Required]
        [Display(Name = "salutation", ResourceType = typeof(Resources.Resources))]
        public string salutation { get; set; }

        [Required]
        [Display(Name = "bookingNumber", ResourceType = typeof(Resources.Resources))]
        public string bookingNumber { get; set; }

        [Required]
        [Display(Name = "firstName", ResourceType = typeof(Resources.Resources))]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "lastName", ResourceType = typeof(Resources.Resources))]
        public string lastName { get; set; }

        [Display(Name = "secu", ResourceType = typeof(Resources.Resources))]
        public string secu { get; set; }

        [Display(Name = "gender", ResourceType = typeof(Resources.Resources))]
        public string gender { get; set; }

        [Required]
        [Display(Name = "email", ResourceType = typeof(Resources.Resources))]
        public string email { get; set; }

        [Required]
        [Display(Name = "phonenumber", ResourceType = typeof(Resources.Resources))]
        //[RegularExpression(@"([0-9]+)", ErrorMessage = "Phone number must be a number.")]
        public string phonenumber { get; set; }

        [Display(Name = "cellnumber", ResourceType = typeof(Resources.Resources))]
        //[RegularExpression(@"([0-9]+)", ErrorMessage = "Cell Number must be a number.")]
        public string cellnumber { get; set; }

        [Display(Name = "company", ResourceType = typeof(Resources.Resources))]
        public string company { get; set; }

        [Display(Name = "postcode", ResourceType = typeof(Resources.Resources))]
        public string postcode { get; set; }

        [Display(Name = "state", ResourceType = typeof(Resources.Resources))]
        public string state { get; set; }

        [Display(Name = "address1", ResourceType = typeof(Resources.Resources))]
        public string address1 { get; set; }

        [Display(Name = "address2", ResourceType = typeof(Resources.Resources))]
        public string address2 { get; set; }

        [Display(Name = "city", ResourceType = typeof(Resources.Resources))]
        public string city { get; set; }

        [Required]
        [Display(Name = "country", ResourceType = typeof(Resources.Resources))]
        public string country { get; set; }


        [Display(Name = "nationality", ResourceType = typeof(Resources.Resources))]
        public string nationality { get; set; }

        [Display(Name = "idnumber", ResourceType = typeof(Resources.Resources))]
        public string idnumber { get; set; }

        [Display(Name = "document", ResourceType = typeof(Resources.Resources))]
        public string document { get; set; }

        [Display(Name = "expdate", ResourceType = typeof(Resources.Resources))]
        public string expdate { get; set; }

        public string bookingDate { get; set; }

        [Required]
        [Display(Name = "arrivalDate", ResourceType = typeof(Resources.Resources))]
        public string arrivalDate { get; set; }

        [Required]
        [Display(Name = "departureDate", ResourceType = typeof(Resources.Resources))]
        public string departureDate { get; set; }

        [Display(Name = "timearrival", ResourceType = typeof(Resources.Resources))]
        public string timearrival { get; set; }

        [Display(Name = "roomType", ResourceType = typeof(Resources.Resources))]
        public string roomType { get; set; }

        [Display(Name = "numberRoom", ResourceType = typeof(Resources.Resources))]
        public int numberRoom { get; set; }

        [Display(Name = "adults", ResourceType = typeof(Resources.Resources))]
        public int adults { get; set; }

        [Display(Name = "childs", ResourceType = typeof(Resources.Resources))]
        public int childs { get; set; }

        [Display(Name = "paym", ResourceType = typeof(Resources.Resources))]
        public string paym { get; set; }

        [Display(Name = "total", ResourceType = typeof(Resources.Resources))]
        public string total { get; set; }

        [Display(Name = "cardType", ResourceType = typeof(Resources.Resources))]
        public string cardType { get; set; }

        [Display(Name = "cardHolder", ResourceType = typeof(Resources.Resources))]
        public string cardHolder { get; set; }

        [Display(Name = "cardNumber", ResourceType = typeof(Resources.Resources))]
        public string cardNumber { get; set; }

        [Display(Name = "cardEndDate", ResourceType = typeof(Resources.Resources))]
        public string cardEndDate { get; set; }

        [Display(Name ="cardCode",ResourceType = typeof(Resources.Resources))]
        public string cardCode { get; set; }
    }


    public class PaymentViewModel
    {
        public string property { get; set; }
        public string index { get; set; }
        
        [Display(Name = "bookingNumber", ResourceType = typeof(Resources.Resources))]
        public string bookingNumber { get; set; }
        
        [Display(Name = "firstName", ResourceType = typeof(Resources.Resources))]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "lastName", ResourceType = typeof(Resources.Resources))]
        public string lastName { get; set; }

        [Display(Name = "secu", ResourceType = typeof(Resources.Resources))]
        public string secu { get; set; }

        [Display(Name = "gender", ResourceType = typeof(Resources.Resources))]
        public string gender { get; set; }
        
        [Display(Name = "email", ResourceType = typeof(Resources.Resources))]
        public string email { get; set; }
        
        [Display(Name = "phonenumber", ResourceType = typeof(Resources.Resources))]
        //[RegularExpression(@"([0-9]+)", ErrorMessage = "Phone number must be a number.")]
        public string phonenumber { get; set; }

        [Display(Name = "company", ResourceType = typeof(Resources.Resources))]
        public string company { get; set; }

        [Display(Name = "postcode", ResourceType = typeof(Resources.Resources))]
        public string postcode { get; set; }

        [Display(Name = "state", ResourceType = typeof(Resources.Resources))]
        public string state { get; set; }

        [Display(Name = "address1", ResourceType = typeof(Resources.Resources))]
        public string address1 { get; set; }

        [Display(Name = "address2", ResourceType = typeof(Resources.Resources))]
        public string address2 { get; set; }

        [Display(Name = "city", ResourceType = typeof(Resources.Resources))]
        public string city { get; set; }
        
        [Display(Name = "country", ResourceType = typeof(Resources.Resources))]
        public string country { get; set; }

        [Display(Name = "paym", ResourceType = typeof(Resources.Resources))]
        public string paym { get; set; }

        [Display(Name = "total", ResourceType = typeof(Resources.Resources))]
        public string total { get; set; }

        [Display(Name = "cardType", ResourceType = typeof(Resources.Resources))]
        public string cardType { get; set; }

        [Display(Name = "cardHolder", ResourceType = typeof(Resources.Resources))]
        public string cardHolder { get; set; }

        [Display(Name = "cardNumber", ResourceType = typeof(Resources.Resources))]
        public string cardNumber { get; set; }

        [Display(Name = "cardEndDate", ResourceType = typeof(Resources.Resources))]
        public string cardEndDate { get; set; }

        [Display(Name = "cardCode", ResourceType = typeof(Resources.Resources))]
        public string cardCode { get; set; }
    }
    
    public class DocumentTypeModel {
        public string value { get; set; }
        public string text { get; set; }
    }
}
