using Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace Contract.Services
{
    public class CreateUpdateServiceDto
    {

        [Required(ErrorMessage = "Service Type must not be empty.")]
        public Guid? ServiceTypeId { get; set; }
        [Required(ErrorMessage = "This field must not be empty.")]
        public decimal Charges { get; set; }
        [Required(ErrorMessage = "This field must not be empty.")]
        public decimal OldCharges { get; set; }
        public string? Tags { get; set; }
        public string? SeoTitle { get; set; }
        public string? Slug { get; set; }
        public string? SeoKeyword { get; set; }
        public string? SeoDescription { get; set; }
        public string? Note { get; set; }
        public int Views { get; set; }
        public string? ImageUrl { get; set; }
        [Required(ErrorMessage = "Image must not be empty.")]
        public Guid? ImageId { get; set; }
        public int ODX { get; set; }
        [Required(ErrorMessage = "Code must not be empty.")]
        public string? Code { get; set; }
        [Required(ErrorMessage = "Name must not be empty.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHighLight { get; set; }
        public bool IsVisibled { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }

    }
}