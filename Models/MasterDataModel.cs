using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dashboardtask.Models
{
    // CORE ENTITY MODELS

    // ===== Asset Type =====
    public class AssetType
    {
        [Key]
        public int AssetTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string? TypeName { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        // Navigation Properties
        public ICollection<AttributeDefinition> AttributeDefinitions { get; set; } = new List<AttributeDefinition>();
        public ICollection<MaintenanceControl> MaintenanceControls { get; set; } = new List<MaintenanceControl>();
        public ICollection<MaintenanceLink> MaintenanceLinks { get; set; } = new List<MaintenanceLink>();
    }

    // ===== Line =====
    public class Line
    {
        [Key]
        public int LineId { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        public string? Name { get; set; }

        [StringLength(20)]
        public string? Rank { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        public double? MTTR { get; set; }
        public double? MTBF { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        // Navigation Properties
        public ICollection<Machine> Machines { get; set; } = new List<Machine>();
        public ICollection<LineAttribute> LineAttributes { get; set; } = new List<LineAttribute>();
    }

    // ===== Machine =====
    public class Machine
    {
        [Key]
        public int MachineId { get; set; }

        [Required]
        [ForeignKey("Line")]
        public int LineId { get; set; }
        public Line Line { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        public string? Name { get; set; }

        [StringLength(20)]
        public string? Rank { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        public double? MTTR { get; set; }
        public double? MTBF { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        // Navigation Properties
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<MachineAttribute> MachineAttributes { get; set; } = new List<MachineAttribute>();
        public ICollection<MaintenanceControl> MaintenanceControls { get; set; } = new List<MaintenanceControl>();
    }

    // ===== Unit =====
    public class Unit
    {
        [Key]
        public int UnitId { get; set; }

        [Required]
        [ForeignKey("Machine")]
        public int MachineId { get; set; }
        public Machine Machine { get; set; }


        public string? Code { get; set; }

        [Required]
        public string? Name { get; set; }

        [StringLength(20)]
        public string? Rank { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        public double? MTTR { get; set; }
        public double? MTBF { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        // Navigation Properties
        public ICollection<Part> Parts { get; set; } = new List<Part>();
        public ICollection<UnitAttribute> UnitAttributes { get; set; } = new List<UnitAttribute>();
        public ICollection<MaintenanceControl> MaintenanceControls { get; set; } = new List<MaintenanceControl>();
    }

    // ===== Part =====
    public class Part
    {
        [Key]
        public int PartId { get; set; }

        [Required]
        [ForeignKey("Unit")]
        public int UnitId { get; set; }
        public Unit Unit { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        public string? Name { get; set; }

        [StringLength(20)]
        public string? Rank { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        public double? MTTR { get; set; }
        public double? MTBF { get; set; }
        public string? SectionCode { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        // Navigation Properties
        public ICollection<PartAttribute> PartAttributes { get; set; } = new List<PartAttribute>();
        public ICollection<MaintenanceControl> MaintenanceControls { get; set; } = new List<MaintenanceControl>();
    }


    // ===== Attribute Definition =====
    public class AttributeDefinition
    {
        [Key]
        public int AttributeId { get; set; }

        [Required]
        [ForeignKey("AssetType")]
        public int AssetTypeId { get; set; }
        public AssetType AssetType { get; set; }

        [Required]
        [StringLength(100)]
        public string? AttributeName { get; set; }

        [StringLength(50)]
        public string? AttributeCode { get; set; }

        [StringLength(20)]
        public string? DataType { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public string? DefaultValue { get; set; }

        public bool IsRequired { get; set; } = false;

        public int? DisplayOrder { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        // Navigation Properties
        public ICollection<LineAttribute> LineAttributes { get; set; } = new List<LineAttribute>();
        public ICollection<MachineAttribute> MachineAttributes { get; set; } = new List<MachineAttribute>();
        public ICollection<UnitAttribute> UnitAttributes { get; set; } = new List<UnitAttribute>();
        public ICollection<PartAttribute> PartAttributes { get; set; } = new List<PartAttribute>();
    }

    // ===== Line Attribute =====
    public class LineAttribute
    {
        [Key]
        public int LineAttributeId { get; set; }

        [Required]
        [ForeignKey("Line")]
        public int LineId { get; set; }
        public Line Line { get; set; }

        [Required]
        [ForeignKey("AttributeDefinition")]
        public int AttributeId { get; set; }
        public AttributeDefinition AttributeDefinition { get; set; }

        public string? AttributeValue { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }
    }

    // ===== Machine Attribute =====
    public class MachineAttribute
    {
        [Key]
        public int MachineAttributeId { get; set; }

        [Required]
        [ForeignKey("Machine")]
        public int MachineId { get; set; }
        public Machine Machine { get; set; }

        [Required]
        [ForeignKey("AttributeDefinition")]
        public int AttributeId { get; set; }
        public AttributeDefinition AttributeDefinition { get; set; }

        public string? AttributeValue { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }
    }

    // ===== Unit Attribute =====
    public class UnitAttribute
    {
        [Key]
        public int UnitAttributeId { get; set; }

        [Required]
        [ForeignKey("Unit")]
        public int UnitId { get; set; }
        public Unit Unit { get; set; }

        [Required]
        [ForeignKey("AttributeDefinition")]
        public int AttributeId { get; set; }
        public AttributeDefinition AttributeDefinition { get; set; }

        public string? AttributeValue { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }
    }

    // ===== Part Attribute =====
    public class PartAttribute
    {
        [Key]
        public int PartAttributeId { get; set; }

        [Required]
        [ForeignKey("Part")]
        public int PartId { get; set; }
        public Part Part { get; set; }

        [Required]
        [ForeignKey("AttributeDefinition")]
        public int AttributeId { get; set; }
        public AttributeDefinition AttributeDefinition { get; set; }

        public string? AttributeValue { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }
    }

    // =====================================================
    // MAINTENANCE MANAGEMENT MODELS
    // =====================================================

    // ===== Maintenance Standard =====
    public class MaintenanceStandard
    {
        [Key]
        public int StandardId { get; set; }

        [StringLength(50)]
        public string? StandardCode { get; set; }

        [Required]
        [StringLength(255)]
        public string? StandardDesc { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        // 🔹 Foreign key ที่ชัดเจน
        public int StandardTypeId { get; set; }

        [ForeignKey(nameof(StandardTypeId))]
        public MaintenanceStandardType StandardType { get; set; }

        public ICollection<MaintenanceSchedule> Schedules { get; set; } = new List<MaintenanceSchedule>();
        public ICollection<MaintenanceControl> Controls { get; set; } = new List<MaintenanceControl>();
    }

    public class MaintenanceStandardType
    {
        [Key]
        public int StandardTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string? StandardTypeCode { get; set; }

        [Required]
        [StringLength(255)]
        public string? StandardTypeDesc { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        public ICollection<MaintenanceStandard> Standards { get; set; } = new List<MaintenanceStandard>();
    }


    // ===== Maintenance Indicator Unit =====
    public class MaintenanceIndicatorUnit
    {
        [Key]
        public int UnitID { get; set; }

        [Required]
        [StringLength(255)]
        public string? UnitDesc { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        // Navigation Properties
        public ICollection<MaintenanceIndicator> MaintenanceIndicators { get; set; } = new List<MaintenanceIndicator>();
    }

    public class MaintenanceIndicator
    {
        [Key]
        public int IndicatorID { get; set; }

        [StringLength(50)]
        public string? IndicatorCode { get; set; }

        [Required]
        [StringLength(255)]
        public string? IndicatorDesc { get; set; }

        [Required]
        [ForeignKey(nameof(Unit))]
        public int UnitID { get; set; }
        public MaintenanceIndicatorUnit Unit { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        //  ชื่อปลอดภัย ไม่ชนคลาส
        public ICollection<MaintenanceSchedule> Schedules { get; set; } = new List<MaintenanceSchedule>();
    }



    // ===== Maintenance Control =====
    public class MaintenanceControl
    {
        [Key]
        public int ControlItemId { get; set; }

        [ForeignKey(nameof(AssetType))]
        public int? AssetTypeId { get; set; }
        public AssetType AssetType { get; set; }

        [ForeignKey(nameof(Machine))]
        public int? MachineId { get; set; }
        public Machine Machine { get; set; }

        [ForeignKey(nameof(Unit))]
        public int? UnitId { get; set; }
        public Unit Unit { get; set; }

        [ForeignKey(nameof(Part))]
        public int? PartId { get; set; }
        public Part Part { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }

        public ICollection<MaintenanceSchedule> Schedules { get; set; } = new List<MaintenanceSchedule>();
    }

    public class AssetControlRequest
    {
        public int? ControlItemId { get; set; }
        public int? AssetTypeId { get; set; }
        public int? AssetId { get; set; }
        public int? MachineId { get; set; }
        public int? UnitId { get; set; }
        public int? PartId { get; set; }

    }


    // ===== Maintenance Schedule =====
    public class MaintenanceSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        [ForeignKey(nameof(ControlItem))]
        public int ControlItemId { get; set; }
        public MaintenanceControl ControlItem { get; set; }

        [Required]
        [ForeignKey(nameof(Standard))]
        public int StandardId { get; set; }
        public MaintenanceStandard Standard { get; set; }

        [Required]
        [ForeignKey(nameof(Indicator))]


        [StringLength(5)]
        public string SectionCode { get; set; } = "ME";  // ← ต้องมีบรรทัดนี้

        [StringLength(5)]
        public string? JobPlanStatus { get; set; }


        public int IndicatorID { get; set; }
        public MaintenanceIndicator Indicator { get; set; }

        public bool IsRequired { get; set; } = true;
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }
    }


    // =====================================================
    // MAINTENANCE LINK MODELS
    // =====================================================

    // ===== Maintenance Link =====
    public class MaintenanceLink
    {
        [Key]
        public int LinkId { get; set; }

        [Required]
        [ForeignKey("AssetType")]
        public int AssetTypeId { get; set; }
        public AssetType AssetType { get; set; }

        [Required]
        public int AssetId { get; set; }

        [StringLength(50)]
        public string? LinkCategory { get; set; }

        [Required]
        [StringLength(200)]
        public string? LinkTitle { get; set; }

        public string? LinkURL { get; set; }

        [StringLength(50)]
        public string? LinkType { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? UpdatedTime { get; set; }
    }

    // =====================================================
    // REQUEST/RESPONSE DTO MODELS
    // =====================================================

    // ===== Asset Hierarchy DTOs =====
    public class AssetItem
    {
        public int id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? level { get; set; }
        public string? rank { get; set; }
        public string? status { get; set; }
        public bool hasChildren { get; set; }
        public List<AssetItem> children { get; set; } = new List<AssetItem>();
        public bool childrenLoaded { get; set; }
        public double? MTTR { get; set; }
        public double? MTBF { get; set; }
        public string? SectionCode { get; set; }
    }

    public class NodeRequest
    {
        public int? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Level { get; set; }
        public int? ParentId { get; set; }

        [JsonPropertyName("rank")] // ระบุให้ตรงกับที่ปรากฏในรูป Debug ของคุณ
        public string? Rank { get; set; }
        public string? Status { get; set; }
        public double? MTTR { get; set; }
        public double? MTBF { get; set; }

        public string? SectionCode { get; set; }
    }

    // ===== Attribute DTOs =====
    public class SetAttributeRequest
    {
        public int NodeId { get; set; }
        public string? Level { get; set; }  // Line, Machine, Unit, Part
        public int AttributeId { get; set; }
        public string? AttributeValue { get; set; }
    }

    public class GetAttributesRequest
    {
        [Required]
        public int NodeId { get; set; }

        [Required]
        [StringLength(20)]
        public string? Level { get; set; }
    }

    public class AttributeResponse
    {
        public int AttributeId { get; set; }
        public string? AttributeName { get; set; }
        public string? AttributeCode { get; set; }
        public string? DataType { get; set; }
        public string? Description { get; set; }
        public string? AttributeValue { get; set; }
        public bool IsRequired { get; set; }
        public int? DisplayOrder { get; set; }
    }

    // ===== Maintenance Link DTOs =====
    public class MaintenanceLinkRequest
    {
        [Required]
        public int AssetTypeId { get; set; }

        [Required]
        public int AssetId { get; set; }

        [StringLength(50)]
        public string? LinkCategory { get; set; }

        [Required]
        [StringLength(200)]
        public string? LinkTitle { get; set; }

        public string? LinkURL { get; set; }

        [StringLength(50)]
        public string? LinkType { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class MaintenanceLinkResponse
    {
        public int LinkId { get; set; }
        public int AssetTypeId { get; set; }
        public string? AssetTypeName { get; set; }
        public int AssetId { get; set; }
        public string? AssetCode { get; set; }
        public string? AssetName { get; set; }
        public string? LinkCategory { get; set; }
        public string? LinkTitle { get; set; }
        public string? LinkURL { get; set; }
        public string? LinkType { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

    public class GetLinksRequest
    {
        [Required]
        [StringLength(20)]
        public string? AssetType { get; set; } // "Line", "Machine", "Unit", "Part"

        [Required]
        public int AssetId { get; set; }

        [StringLength(50)]
        public string? LinkCategory { get; set; } // Optional filter

        public bool? IsActive { get; set; } = true; // Default to active links only
    }

    public class UpdateLinkRequest
    {
        [Required]
        public int LinkId { get; set; }

        [StringLength(50)]
        public string? LinkCategory { get; set; }

        [StringLength(200)]
        public string? LinkTitle { get; set; }

        public string? LinkURL { get; set; }

        [StringLength(50)]
        public string? LinkType { get; set; }

        public int? DisplayOrder { get; set; }

        public bool? IsActive { get; set; }
    }

    public class DeleteLinkRequest
    {
        [Required]
        public int LinkId { get; set; }

        public bool SoftDelete { get; set; } = true; // True = set IsActive = false, False = hard delete
    }

    public class BulkLinkRequest
    {
        [Required]
        public List<MaintenanceLinkRequest> Links { get; set; }
    }

    public class LinkStatisticsResponse
    {
        public string? AssetType { get; set; }
        public int AssetId { get; set; }
        public string? AssetCode { get; set; }
        public string? AssetName { get; set; }
        public int TotalLinks { get; set; }
        public int ActiveLinks { get; set; }
        public int InactiveLinks { get; set; }
        public Dictionary<string, int> LinksByCategory { get; set; }
        public Dictionary<string, int> LinksByType { get; set; }
    }

    // ===== Maintenance Schedule DTOs =====
    public class MaintenanceScheduleRequest
    {
        [Required]
        public int ControlItemId { get; set; }

        [Required]
        public int StandardId { get; set; }

        [Required]
        public int IndicatorID { get; set; }

        public bool IsRequired { get; set; } = true;
    }

    public class MaintenanceScheduleResponse
    {
        public int ScheduleId { get; set; }
        public int ControlItemId { get; set; }
        public string? StandardCode { get; set; }
        public string? StandardDesc { get; set; }
        public string? IndicatorCode { get; set; }
        public string? IndicatorDesc { get; set; }
        public string? UnitDesc { get; set; }
        public bool IsRequired { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    // =====================================================
    // CONSTANTS AND ENUMERATIONS
    // =====================================================

    // ===== Link Category Constants =====
    public static class LinkCategories
    {
        public const string OPL = "OPL";
        public const string MPInformation = "MP Information";
        public const string MTTRHist = "MTTR Hist";
        public const string DrawingManual = "Drawing & Manual";
        public const string MachineHistory = "ประวัติเครื่องจักร";
        public const string BreakdownTree = "Breakdown Tree Diagram";
        public const string Inspection = "Inspection";
        public const string RCMModel = "RCM Model 1";
        public const string FMEA = "RPN-FMEA";
        public const string dDocument = "dDocument";
        public const string ElementAnalysis = "Element Analysis Chart";
        public const string WorkInstruction = "Work Instruction";
        public const string DowntimeSummary = "Summary of Downtime";
        public const string BreakdownAnalysis = "Breakdown Analysis";
        public const string MIA = "Maintenance Information Assistant(MIA)";
        public const string QRCode = "Generat QR-Code(MIA)";
        public const string Quality = "Quality";
        public const string LinkExport = "Link Export";

        public static List<string> GetAllCategories()
        {
            return new List<string>
            {
                OPL, MPInformation, MTTRHist, DrawingManual, MachineHistory,
                BreakdownTree, Inspection, RCMModel, FMEA, dDocument,
                ElementAnalysis, WorkInstruction, DowntimeSummary, BreakdownAnalysis,
                MIA, QRCode, Quality, LinkExport
            };
        }
    }

    // ===== Link Type Constants =====
    public static class LinkTypes
    {
        public const string Document = "Document";
        public const string Drawing = "Drawing";
        public const string Video = "Video";
        public const string ExternalURL = "External URL";
        public const string Image = "Image";
        public const string Report = "Report";

        public static List<string> GetAllTypes()
        {
            return new List<string>
            {
                Document, Drawing, Video, ExternalURL, Image, Report
            };
        }
    }

    // ===== Asset Level Constants =====
    public static class AssetLevels
    {
        public const string Line = "Line";
        public const string Machine = "Machine";
        public const string Unit = "Unit";
        public const string Part = "Part";

        public static List<string> GetAllLevels()
        {
            return new List<string> { Line, Machine, Unit, Part };
        }

        public static bool IsValidLevel(string level)
        {
            return GetAllLevels().Contains(level);
        }
    }

    // ===== Rank Constants =====
    public static class RankLevels
    {
        public const string SuperCritical = "S";
        public const string Critical = "A";
        public const string Important = "B";
        public const string Normal = "";

        public static List<string> GetAllRanks()
        {
            return new List<string> { SuperCritical, Critical, Important, Normal };
        }

        public static bool IsValidRank(string rank)
        {
            return GetAllRanks().Contains(rank);
        }
    }

    // ===== Status Constants =====
    public static class AssetStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Maintenance = "Maintenance";
        public const string Retired = "Retired";

        public static List<string> GetAllStatuses()
        {
            return new List<string> { Active, Inactive, Maintenance, Retired };
        }

        public static bool IsValidStatus(string status)
        {
            return GetAllStatuses().Contains(status);
        }
    }

    // ===== Data Type Constants =====
    public static class DataTypes
    {
        public const string String = "String";
        public const string Number = "Number";
        public const string Date = "Date";
        public const string Boolean = "Boolean";

        public static List<string> GetAllDataTypes()
        {
            return new List<string> { String, Number, Date, Boolean };
        }

        public static bool IsValidDataType(string dataType)
        {
            return GetAllDataTypes().Contains(dataType);
        }
    }

    // =====================================================
    // HELPER/UTILITY CLASSES
    // =====================================================

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string> errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default(T),
                Errors = errors ?? new List<string>()
            };
        }
    }

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }

    // =====================================================
    // REQUEST/RESPONSE MODELS (Add to Models folder)
    // =====================================================

    public class AttributeDefinitionRequest
    {
        public int AssetTypeId { get; set; }
        public string? AttributeName { get; set; }
        public string? AttributeCode { get; set; }
        public string? DataType { get; set; }
        public string? Description { get; set; }
        public string? DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateAttributeDefinitionRequest
    {
        public int AttributeId { get; set; }
        public string? AttributeName { get; set; }
        public string? AttributeCode { get; set; }
        public string? DataType { get; set; }
        public string? Description { get; set; }
        public string? DefaultValue { get; set; }
        public bool? IsRequired { get; set; }
        public int? DisplayOrder { get; set; }
    }

    public class DeleteAttributeDefinitionRequest
    {
        public int AttributeId { get; set; }
    }

}
