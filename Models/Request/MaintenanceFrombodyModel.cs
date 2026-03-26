using System.ComponentModel.DataAnnotations;

namespace dashboardtask.Models.Request
{
    public class CreateMaintenanceStandardRequest
    {
        [Required(ErrorMessage = "StandardCode is required")]
        [StringLength(50, ErrorMessage = "StandardCode must not exceed 50 characters")]
        public string StandardCode { get; set; } = "";

        [Required(ErrorMessage = "StandardDesc is required")]
        [StringLength(255, ErrorMessage = "StandardDesc must not exceed 255 characters")]
        public string StandardDesc { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "StandardTypeId must be a valid ID")]
        public int StandardTypeId { get; set; }   
    }

    public class UpdateMaintenanceStandardRequest
    {
        [Required]
        public int StandardId { get; set; }

        [StringLength(50)]
        public string? StandardCode { get; set; }

        [StringLength(255)]
        public string? StandardDesc { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "StandardTypeId must be a valid ID")]
        public int StandardTypeId { get; set; }   
    }

    public class DeleteRequest
    {
        [Required]
        public int StandardId { get; set; }

    }
}
