using Contract.CustomAttribute;
using Core.Const;
using Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace Contract.Identity.UserManager
{
    public class UpdateUserDto
    {
        public string UserName { get; set; }
        
        
        [Required(ErrorMessage = "First Name is required")]
        [MinLength(2,ErrorMessage = "First Name is at least 2 character")]
        [RegularExpression(ContentRegularExpression.NAME,ErrorMessage = "First Name must be in text format")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last Name is required")]
        [MinLength(2,ErrorMessage = "Last Name is at least 2 character")]
        [RegularExpression(ContentRegularExpression.NAME,ErrorMessage = "Last Name must be in text format")]
        public string LastName { get; set; }
        
        
        [Required(ErrorMessage = "User Code is required")]
        [MinLength(2,ErrorMessage = "User Code is at least 2 character")]
        public string UserCode { get; set;}
        

        
        public Gender Gender { get; set; } 
        public DateTime DOB { get; set; }
        
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(ContentRegularExpression.NUMBER_PHONE,ErrorMessage = "Phone Number has to 10 number")]
        public string PhoneNumber { get; set; }
        
        public bool IsSetPassword { get; set; } = false;
        
        [RequiredIf(nameof(IsSetPassword),true)]
        [MinLength(6)]
        [RegularExpression(ContentRegularExpression.PASSWORD,ErrorMessage = "Password must not have spaces")]
        public string Password { get; set; }

        [RequiredIf(nameof(IsSetPassword),true)]
        [Compare(nameof(Password),ErrorMessage = "Password confirm do not match")]
        [RegularExpression(ContentRegularExpression.PASSWORD,ErrorMessage = "Password Confirm must not have spaces")]
        public string PasswordConfirm { get; set; }

        
        [RegularExpression(ContentRegularExpression.EMAIL,ErrorMessage = "Email has to @gmail.com format")]
        public string? Email { get; set; }
        
        public List<string> Roles { get; set; } = new List<string>();
        
        public Guid? PositionId { get; set; }
        public string? AvatarURL { get; set; }

        public List<Guid> DepartmentIds { get; set; } = new List<Guid>();

        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }

        public Guid? BloodTypeId { set; get; }
        public string? CitizenIDNumber { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public string? Address { get; set; }
        public Guid? DependsId { get; set; }
        public int? Relationship { get; set; }
        public int ODX { get; set; } = 0;
    }
}