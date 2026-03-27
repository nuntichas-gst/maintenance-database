using System.ComponentModel.DataAnnotations;

namespace dashboardtask.Models.Request
{

    #region Standard Request
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

    #endregion



#region Indicator Unit Request

public class CreateIndicatorUnitRequest
    {
        [Required(ErrorMessage = "UnitDesc is required")]
        [StringLength(100, ErrorMessage = "UnitDesc must not exceed 100 characters")]
        public string UnitDesc { get; set; } = "";
    }

    public class UpdateIndicatorUnitRequest
    {
        [Required(ErrorMessage = "UnitId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "UnitId must be a valid ID")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "UnitDesc is required")]
        [StringLength(100, ErrorMessage = "UnitDesc must not exceed 100 characters")]
        public string UnitDesc { get; set; } = "";
    }

    public class DeleteIndicatorUnitRequest
    {
        [Required(ErrorMessage = "UnitId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "UnitId must be a valid ID")]
        public int UnitId { get; set; }
    }

    #endregion


    #region Indicator Request

    public class MaintenanceIndicatorRequest
    {
        [Required(ErrorMessage = "IndicatorCode is required")]
        [StringLength(50, ErrorMessage = "IndicatorCode must not exceed 50 characters")]
        public string IndicatorCode { get; set; } = "";

        [Required(ErrorMessage = "IndicatorDesc is required")]
        [StringLength(255, ErrorMessage = "IndicatorDesc must not exceed 255 characters")]
        public string IndicatorDesc { get; set; } = "";

        [Required(ErrorMessage = "UnitId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "UnitId must be a valid ID")]
        public int UnitId { get; set; }
    }

    public class UpdateMaintenanceIndicatorRequest
    {
        [Required(ErrorMessage = "IndicatorId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "IndicatorId must be a valid ID")]
        public int IndicatorId { get; set; }

        [StringLength(50, ErrorMessage = "IndicatorCode must not exceed 50 characters")]
        public string? IndicatorCode { get; set; }

        [StringLength(255, ErrorMessage = "IndicatorDesc must not exceed 255 characters")]
        public string? IndicatorDesc { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UnitId must be a valid ID")]
        public int? UnitId { get; set; }
    }

    public class DeleteIndicatorRequest
    {
        [Required(ErrorMessage = "IndicatorId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "IndicatorId must be a valid ID")]
        public int IndicatorId { get; set; }
    }

    #endregion


    #region Asset Control Request

    public class CreateAssetControlRequest
    {
        [Required(ErrorMessage = "AssetTypeId is required")]
        [Range(2, 4, ErrorMessage = "AssetTypeId must be 2 (Machine), 3 (Unit), or 4 (Part)")]
        public int AssetTypeId { get; set; }

        [Required(ErrorMessage = "AssetId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "AssetId must be a valid ID")]
        public int AssetId { get; set; }
    }

    public class UpdateAssetControlRequest
    {
        [Required(ErrorMessage = "ControlItemId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ControlItemId must be a valid ID")]
        public int ControlItemId { get; set; }

        [Required(ErrorMessage = "AssetTypeId is required")]
        [Range(2, 4, ErrorMessage = "AssetTypeId must be 2 (Machine), 3 (Unit), or 4 (Part)")]
        public int AssetTypeId { get; set; }

        [Required(ErrorMessage = "AssetId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "AssetId must be a valid ID")]
        public int AssetId { get; set; }
    }

    public class DeleteAssetControlRequest
    {
        [Required(ErrorMessage = "ControlItemId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ControlItemId must be a valid ID")]
        public int ControlItemId { get; set; }
    }

    #endregion


    #region Control Item Request

    public class MaintenanceControlRequest
    {
        [Required(ErrorMessage = "AssetTypeId is required")]
        [Range(2, 4, ErrorMessage = "AssetTypeId must be 2 (Machine), 3 (Unit), or 4 (Part)")]
        public int AssetTypeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MachineId must be a valid ID")]
        public int? MachineId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UnitId must be a valid ID")]
        public int? UnitId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PartId must be a valid ID")]
        public int? PartId { get; set; }
    }

    public class DeleteControlItemRequest
    {
        [Required(ErrorMessage = "ControlItemId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ControlItemId must be a valid ID")]
        public int ControlItemId { get; set; }
    }

    #endregion


    #region Schedule Request

    public class CreateMaintenanceScheduleRequest
    {
        [Required(ErrorMessage = "ControlItemId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ControlItemId must be a valid ID")]
        public int ControlItemId { get; set; }

        [Required(ErrorMessage = "StandardId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "StandardId must be a valid ID")]
        public int StandardId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "IndicatorId must be a valid ID")]
        public int? IndicatorId { get; set; }

        public bool IsRequired { get; set; } = false;

        [StringLength(10, ErrorMessage = "SectionCode must not exceed 10 characters")]
        public string? SectionCode { get; set; }
    }

    public class UpdateMaintenanceScheduleRequest
    {
        [Required(ErrorMessage = "ScheduleId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ScheduleId must be a valid ID")]
        public int ScheduleId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "IndicatorId must be a valid ID")]
        public int? IndicatorId { get; set; }

        public bool? IsRequired { get; set; }

        [StringLength(10, ErrorMessage = "SectionCode must not exceed 10 characters")]
        public string? SectionCode { get; set; }
    }

    public class DeleteScheduleRequest
    {
        [Required(ErrorMessage = "ScheduleId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ScheduleId must be a valid ID")]
        public int ScheduleId { get; set; }
    }

    #endregion


    #region Bulk Schedule Request

    public class BulkScheduleRequest
    {
        [Required(ErrorMessage = "Schedules is required")]
        public List<BulkScheduleItemRequest> Schedules { get; set; } = new();
    }

    public class BulkScheduleItemRequest
    {
        [Required(ErrorMessage = "ControlItemId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ControlItemId must be a valid ID")]
        public int ControlItemId { get; set; }

        [Required(ErrorMessage = "StandardId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "StandardId must be a valid ID")]
        public int StandardId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "IndicatorId must be a valid ID")]
        public int? IndicatorId { get; set; }

        public bool IsRequired { get; set; } = false;

        [StringLength(10, ErrorMessage = "SectionCode must not exceed 10 characters")]
        public string? SectionCode { get; set; }
    }

    #endregion


    #region Control From Asset Request

    public class CreateControlFromAssetRequest
    {
        [Required(ErrorMessage = "AssetId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "AssetId must be a valid ID")]
        public int AssetId { get; set; }

        [Required(ErrorMessage = "AssetTypeId is required")]
        [Range(2, 4, ErrorMessage = "AssetTypeId must be 2 (Machine), 3 (Unit), or 4 (Part)")]
        public int AssetTypeId { get; set; }
    }

    public class ClearSchedulesByAssetRequest
    {
        [Required(ErrorMessage = "AssetId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "AssetId must be a valid ID")]
        public int AssetId { get; set; }

        [Required(ErrorMessage = "AssetTypeId is required")]
        [Range(2, 4, ErrorMessage = "AssetTypeId must be 2 (Machine), 3 (Unit), or 4 (Part)")]
        public int AssetTypeId { get; set; }
    }

    #endregion


    #region Reset / AutoGenerate Request

    public class ResetControlsRequest
    {
        [Required(ErrorMessage = "ConfirmDelete is required")]
        public bool ConfirmDelete { get; set; }
    }

    public class AutoGenerateControlsRequest
    {
        // optional
        public List<int>? AssetTypeIds { get; set; }
    }

    #endregion
}