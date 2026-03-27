using dashboardtask.Data;
using dashboardtask.Models;
using dashboardtask.Models.Request;
using dashboardtask.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dashboardtask.Controllers
{
    public class MasterDataController : Controller
    {
        private readonly IStandardService _standardService;
        private readonly IAssetService _assetService;
        private readonly IControlService _controlService;
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<MasterDataController> _logger;

        public MasterDataController(
            IStandardService standardService,
            IAssetService assetService,
            IControlService controlService,
            IScheduleService scheduleService,
            ILogger<MasterDataController> logger)
        {
            _standardService = standardService;
            _assetService = assetService;
            _controlService = controlService;
            _scheduleService = scheduleService;
            _logger = logger;
        }

        // -------------------- Views --------------------
        public IActionResult Index() => View();
        public IActionResult MaintenanceScheduleIndex() => View();
        public IActionResult EmployeeIndex() => View();
        public IActionResult ApprovalConfigIndex() => View();

        public IActionResult StandardsIndex()
        {
            ViewBag.Title = "Maintenance Standards";
            ViewBag.SubTitle = "Overview of Maintenance Standards";
            ViewBag.TextTitle = "Standard";
            ViewBag.Columns = new[] { "#", "Code", "Standards Name", "Category", "Date", "Action" };
            return View("StandardsIndex");
        }

        public IActionResult IndicatorsIndex()
        {
            ViewBag.Title = "Maintenance Indicators";
            ViewBag.SubTitle = "Overview of Maintenance Indicators";
            ViewBag.TextTitle = "Indicator";
            ViewBag.Columns = new[] { "#", "Code", "Description", "Unit", "Date", "Action" };
            return View("StandardsIndex");
        }

        public IActionResult ControlItemsIndex()
        {
            ViewBag.Title = "Control Asset & Schedule Management";
            ViewBag.SubTitle = "Overview of Control Asset & Schedule Management";
            ViewBag.TextTitle = "Control";
            ViewBag.Columns = new[] { "#", "Code", "Description", "Unit", "Date", "Action" };
            return View("StandardsIndex");
        }

        #region standard endpoints
            // -------------------- Standard Endpoints --------------------
            [HttpGet("Masterdata/GetStandards")]
            public async Task<IActionResult> GetStandards() =>
                await HandleRequest(() => _standardService.GetStandardsAsync(), "Failed to get standards");

            [HttpGet("Masterdata/GetStandard/{id}")]
            public async Task<IActionResult> GetStandard(int id) =>
                await HandleRequest(() => _standardService.GetStandardAsync(id), "Failed to get standard");

            [HttpGet("Masterdata/GetStandardTypes")]
            public async Task<IActionResult> GetStandardTypes() =>
                await HandleRequest(() => _standardService.GetStandardTypesAsync(), "Failed to get standard types");

            [HttpPost("Masterdata/CreateStandard")]
            public async Task<IActionResult> CreateStandard([FromBody] CreateMaintenanceStandardRequest req) =>
                await HandleRequest(() => _standardService.CreateStandardAsync(req), "Failed to create standard");

            [HttpPost("Masterdata/UpdateStandard")]
            public async Task<IActionResult> UpdateStandard([FromBody] UpdateMaintenanceStandardRequest req) =>
                await HandleRequest(() => _standardService.UpdateStandardAsync(req), "Failed to update standard");

            [HttpPost("Masterdata/DeleteStandard")]
            public async Task<IActionResult> DeleteStandard([FromBody] DeleteRequest req) =>
                await HandleRequest(() => _standardService.DeleteStandardAsync(req), "Failed to delete standard");

        #endregion

        #region asset endpoints

        // -------------------- Asset Endpoints --------------------
        [HttpGet("Masterdata/GetAssetTypes")]
            public async Task<IActionResult> GetAssetTypes() =>
                await HandleRequest(() => _assetService.GetAssetTypesAsync(), "Failed to get asset types");

            [HttpGet("Masterdata/GetAssetData")]
            public async Task<IActionResult> GetAssetData() =>
                await HandleRequest(() => _assetService.GetAssetDataAsync(), "Failed to get asset data");

            [HttpGet("Masterdata/GetStatistics")]
            public async Task<IActionResult> GetStatistics() =>
                await HandleRequest(() => _assetService.GetStatisticsAsync(), "Failed to get statistics");

            [HttpGet("Masterdata/GetAssetsByType")]
            public async Task<IActionResult> GetAssetsByType(int assetTypeId) =>
                await HandleRequest(() => _assetService.GetAssetsByTypeAsync(assetTypeId), "Failed to get assets by type");

        #endregion

        #region control endpoints
        // -------------------- Control Endpoints --------------------
        [HttpGet("Masterdata/GetAssetControls")]
            public async Task<IActionResult> GetAssetControls(int? assetTypeId, string assetName) =>
                await HandleRequest(() => _controlService.GetAssetControlsAsync(assetTypeId, assetName), "Failed to get asset controls");

            [HttpGet("Masterdata/GetAssetControl/{id}")]
            public async Task<IActionResult> GetAssetControl(int id) =>
                await HandleRequest(() => _controlService.GetAssetControlAsync(id), "Failed to get asset control");

            [HttpPost("Masterdata/CreateAssetControl")]
            public async Task<IActionResult> CreateAssetControl([FromBody] AssetControlRequest req) =>
                await HandleRequest(() => _controlService.CreateAssetControlAsync(req), "Failed to create asset control");

            [HttpPost("Masterdata/UpdateAssetControl")]
            public async Task<IActionResult> UpdateAssetControl([FromBody] AssetControlRequest req) =>
                await HandleRequest(() => _controlService.UpdateAssetControlAsync(req), "Failed to update asset control");

            [HttpPost("Masterdata/DeleteAssetControl")]
            public async Task<IActionResult> DeleteAssetControl([FromBody] DeleteAssetControlRequest req) =>
                await HandleRequest(() => _controlService.DeleteAssetControlAsync(req), "Failed to delete asset control");

        #endregion

        #region schedule endpoints
        // -------------------- Schedule Endpoints --------------------
        [HttpGet("Masterdata/GetSchedules")]
            public async Task<IActionResult> GetSchedules(int controlItemId) =>
                await HandleRequest(() => _scheduleService.GetSchedulesAsync(controlItemId), "Failed to get schedules");

            [HttpPost("Masterdata/CreateSchedule")]
            public async Task<IActionResult> CreateSchedule([FromBody] CreateMaintenanceScheduleRequest req) =>
                await HandleRequest(() => _scheduleService.CreateScheduleAsync(req), "Failed to create schedule");

            [HttpPost("Masterdata/UpdateSchedule")]
            public async Task<IActionResult> UpdateSchedule([FromBody] UpdateMaintenanceScheduleRequest req) =>
                await HandleRequest(() => _scheduleService.UpdateScheduleAsync(req), "Failed to update schedule");

            [HttpPost("Masterdata/DeleteSchedule")]
            public async Task<IActionResult> DeleteSchedule([FromBody] DeleteScheduleRequest req) =>
                await HandleRequest(() => _scheduleService.DeleteScheduleAsync(req), "Failed to delete schedule");

            [HttpPost("Masterdata/BulkCreateSchedules")]
            public async Task<IActionResult> BulkCreateSchedules([FromBody] BulkScheduleRequest req) =>
                await HandleRequest(() => _scheduleService.BulkCreateSchedulesAsync(req), "Failed to bulk create schedules");

        #endregion

        #region helper method
        // -------------------- Helper Method --------------------
        private async Task<IActionResult> HandleRequest<T>(Func<Task<T>> action, string errorMessage)
            {
                try
                {
                    var result = await action();
                    return Ok(ApiResponse<T>.SuccessResponse(result));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, errorMessage);
                    return StatusCode(500, ApiResponse<T>.ErrorResponse(errorMessage));
                }
            }
        #endregion





       
  



        [HttpGet]
        public IActionResult GetPanelData(string key)
        {
            object data = key switch
            {
                "line" => new
                {
                    title = "Line Overview",
                    sub = "All Lines",
                    total = 26,
                    labels = new[] { "Active", "Maintenance", "Inactive" },
                    colors = new[] { "#22c55e", "#f59e0b", "#94a3b8" },
                    donut = new[] { 20, 4, 2 },
                    rows = new[]
                    {
                        new { name = "Line A1 — Assembly",      status = "Active",      badge = "tag-done",       info = "Floor 1" },
                        new { name = "Line B2 — Welding",       status = "Maintenance", badge = "tag-inprogress", info = "Floor 2" },
                        new { name = "Line C3 — Painting",      status = "Active",      badge = "tag-done",       info = "Floor 2" },
                        new { name = "Line D4 — Packaging",     status = "Inactive",    badge = "tag-overdue",    info = "Floor 3" },
                        new { name = "Line E5 — Quality Check", status = "Active",      badge = "tag-done",       info = "Floor 1" },
                    }
                },
                "machine" => new
                {
                    title = "Machine Overview",
                    sub = "All Machines",
                    total = 14,
                    labels = new[] { "Running", "Maintenance", "Breakdown" },
                    colors = new[] { "#22c55e", "#f59e0b", "#ef4444" },
                    donut = new[] { 9, 3, 2 },
                    rows = new[]
                    {
                        new { name = "CNC Machine #01",     status = "Running",     badge = "tag-done",       info = "Line A1" },
                        new { name = "Hydraulic Press #02", status = "Maintenance", badge = "tag-inprogress", info = "Line B2" },
                        new { name = "Conveyor Belt #03",   status = "Running",     badge = "tag-done",       info = "Line C3" },
                        new { name = "Robot Arm #04",       status = "Breakdown",   badge = "tag-overdue",    info = "Line D4" },
                        new { name = "Laser Cutter #05",    status = "Running",     badge = "tag-done",       info = "Line A1" },
                    }
                },
                "units" => new
                {
                    title = "Unit Overview",
                    sub = "All Units",
                    total = 27,
                    labels = new[] { "Active", "In Progress", "Pending" },
                    colors = new[] { "#22c55e", "#4f6ef7", "#94a3b8" },
                    donut = new[] { 18, 6, 3 },
                    rows = new[]
                    {
                        new { name = "Unit Alpha — Motor Assembly", status = "Active",      badge = "tag-done",       info = "Zone 1" },
                        new { name = "Unit Beta — Frame Welding",   status = "In Progress", badge = "tag-inprogress", info = "Zone 2" },
                        new { name = "Unit Gamma — Surface Coat",   status = "Active",      badge = "tag-done",       info = "Zone 3" },
                        new { name = "Unit Delta — Final Test",     status = "Pending",     badge = "tag-todo",       info = "Zone 4" },
                        new { name = "Unit Epsilon — Packaging",    status = "Active",      badge = "tag-done",       info = "Zone 5" },
                    }
                },
                _ => new  // parts (default)
                {
                    title = "Parts Overview",
                    sub = "All Parts",
                    total = 7,
                    labels = new[] { "In Stock", "Low Stock", "Ordered" },
                    colors = new[] { "#22c55e", "#ef4444", "#f59e0b" },
                    donut = new[] { 4, 2, 1 },
                    rows = new[]
                    {
                        new { name = "Bearing SKF 6205",    status = "In Stock",  badge = "tag-done",       info = "240 pcs" },
                        new { name = "V-Belt A42",           status = "Low Stock", badge = "tag-overdue",    info = "3 pcs"   },
                        new { name = "Oil Filter OF-220",    status = "In Stock",  badge = "tag-done",       info = "56 pcs"  },
                        new { name = "Seal Kit SK-14",       status = "Ordered",   badge = "tag-inprogress", info = "0 pcs"   },
                        new { name = "Hydraulic Hose 1/2\"", status = "In Stock",  badge = "tag-done",       info = "18 pcs"  },
                    }
                }
            };

            return Json(data);
        }

       
        // GET: /Org/GetPlannerGroups
        [HttpGet]
        public IActionResult GetPlannerGroups()
        {
            var data = new[]
            {
        new { id = 1, name = "Steel Making",   code = "SM", color = "navy", sub = "3 planners" },
        new { id = 2, name = "Rolling Mill",   code = "RM", color = "sky",  sub = "2 planners" },
        new { id = 3, name = "Maintenance",    code = "MT", color = "teal", sub = "2 planners" },
        new { id = 4, name = "Quality Control",code = "QC", color = "warm", sub = "2 planners" },
    };
            return Json(data);
        }

        // GET: /Org/GetPlanners?pgId=1
        [HttpGet]
        public IActionResult GetPlanners(int pgId)
        {
            var data = pgId switch
            {
                1 => new[] {
            new { id = 11, name = "Somchai S.", role = "Production Engineer", av = "SS", color = "navy" },
            new { id = 12, name = "Anucha T.",  role = "Shift Supervisor",    av = "AT", color = "navy" },
            new { id = 13, name = "Krit P.",    role = "Planner",             av = "KP", color = "navy" },
        },
                2 => new[] {
            new { id = 21, name = "Wichai L.", role = "Rolling Supervisor", av = "WL", color = "sky" },
            new { id = 22, name = "Narin K.",  role = "Planner",            av = "NK", color = "sky" },
        },
                3 => new[] {
            new { id = 31, name = "Prasit M.", role = "Maintenance Engineer", av = "PM", color = "teal" },
            new { id = 32, name = "Somyot R.", role = "Supervisor",           av = "SR", color = "teal" },
        },
                4 => new[] {
            new { id = 41, name = "Kanya W.", role = "QC Engineer", av = "KW", color = "warm" },
            new { id = 42, name = "Ladda S.", role = "QC Supervisor", av = "LS", color = "warm" },
        },
                _ => Array.Empty<object>()
            };
            return Json(data);
        }

        // GET: /Org/GetWorkCenters?plannerId=11
        [HttpGet]
        public IActionResult GetWorkCenters(int plannerId)
        {
            var data = plannerId switch
            {
                11 => new[] {
            new { id = 111, name = "Blast Furnace", type = "Iron Making" },
            new { id = 112, name = "Basic Oxygen Furnace", type = "Steel Making" },
        },
                12 => new[] {
            new { id = 121, name = "Continuous Casting", type = "Casting" },
        },
                21 => new[] {
            new { id = 211, name = "Hot Rolling Mill", type = "Rolling" },
            new { id = 212, name = "Cold Rolling Mill", type = "Rolling" },
        },
                31 => new[] {
            new { id = 311, name = "Mechanical Workshop", type = "Maintenance" },
            new { id = 312, name = "Electrical Workshop", type = "Maintenance" },
        },
                41 => new[] {
            new { id = 411, name = "Chemical Lab", type = "QC Lab" },
            new { id = 412, name = "Mechanical Testing Lab", type = "QC Lab" },
        },
                _ => Array.Empty<object>()
            };
            return Json(data);
        }

        // GET: /Org/GetWorkers?wcId=111
        [HttpGet]
        public IActionResult GetWorkers(int wcId)
        {
            var data = wcId switch
            {
                111 => new[] {
            new { id = 1001, name = "Chaiwat P.", role = "Furnace Operator", av = "CP" },
            new { id = 1002, name = "มนตรี T.",  role = "Crane Operator",   av = "MT" },
        },
                112 => new[] {
            new { id = 1003, name = "Sakda K.", role = "Steel Operator", av = "SK" },
        },
                211 => new[] {
            new { id = 1004, name = "Anan P.", role = "Rolling Operator", av = "AP" },
            new { id = 1005, name = "Decha M.", role = "Technician", av = "DM" },
        },
                311 => new[] {
            new { id = 1006, name = "Korn S.", role = "Mechanic", av = "KS" },
        },
                312 => new[] {
            new { id = 1007, name = "Nok S.", role = "Electrician", av = "NS" },
        },
                411 => new[] {
            new { id = 1008, name = "Om W.", role = "Lab Technician", av = "OW" },
        },
                _ => Array.Empty<object>()
            };
            return Json(data);
        }

        // GET: /Org/GetAvailableWorkers?wcId=111
        [HttpGet]
        public IActionResult GetAvailableWorkers(int wcId)
        {
            var pool = new[]
            {
        new { id = 2001, name = "Manee P.", role = "Technician", av = "MP" },
        new { id = 2002, name = "Pong T.",  role = "Electrician", av = "PT" },
        new { id = 2003, name = "Rat K.",   role = "Welder", av = "RK" },
        new { id = 2004, name = "Somchai B.", role = "Operator", av = "SB" },
        new { id = 2005, name = "Wut K.", role = "Crane Operator", av = "WK" },
        new { id = 2006, name = "Dao N.", role = "QC Inspector", av = "DN" },
    };
            return Json(pool);
        }

        // POST: /Org/AddWorker
        [HttpPost]
        public IActionResult AddWorker([FromBody] AddWorkerRequest req)
        {
            // บันทึก DB
            // _service.AddWorkerToWorkCenter(req.WcId, req.WorkerIds);
            return Json(new { success = true, message = $"Added {req.WorkerIds.Length} worker(s) to WC {req.WcId}" });
        }

        // POST: /Org/RemoveWorker
        [HttpPost]
        public IActionResult RemoveWorker([FromBody] RemoveWorkerRequest req)
        {
            // ลบออกจาก DB
            // _service.RemoveWorkerFromWorkCenter(req.WcId, req.WorkerId);
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetEmployees()
        {
            var data = new List<Approver>
        {
            new Approver { id = "EMP001", name = "Somchai Jaidee" },
            new Approver { id = "EMP002", name = "Suda Srisawat" },
            new Approver { id = "EMP003", name = "Anan Phrom" },
            new Approver { id = "EMP004", name = "Nida Kanya" },
            new Approver { id = "EMP005", name = "Krit Chaiyo" }
        };
            return Json(data);
        }

        // GET: /ApprovalAuthority/GetHierarchy
        [HttpGet]
        public IActionResult GetHierarchy()
        {
            var data = new List<HierarchyData>
            {
                new HierarchyData { id=0, div="Accounting", dept="Accounting",sec="Accounting", hier=1, ap=new ApprovalLevel{ l1= new Approver {id="768026",name="คุณเฉลิมพงษ์"},l2= new Approver {id="64030",name="คุณพนิดา"}}},
                new HierarchyData { id=1, div="Accounting", dept="Budgeting & Costing", sec="Budgeting & Costing", hier=1,ap=new ApprovalLevel{l1= new Approver {id="648109",name="คุณวรนุช"},  l2= new Approver {id="59065",name="คุณวราภรณ์"}}},
                new HierarchyData { id=2, div="Accounting", dept="Budgeting & Costing", sec="Budgeting & Costing", hier=2,ap=new ApprovalLevel{}},
                new HierarchyData { id=3, div="Accounting", dept="Budgeting & Costing", sec="Budgeting & Costing", hier=3,ap=new ApprovalLevel{}},
                new HierarchyData { id=4, div="Accounting", dept="Budgeting & Costing", sec="Budgeting & Costing", hier=4,ap=new ApprovalLevel{}},
                new HierarchyData { id=5, div="Accounting", dept="Budgeting & Costing", sec="Budgeting & Costing", hier=5,ap=new ApprovalLevel{l1= new Approver {id="249037",name="คุณธิดา"},  l2= new Approver {id="65022",name="คุณชินอิจิ"},}},
                new HierarchyData { id=6, div="Accounting", dept="Budgeting & Costing", sec="Budgeting & Costing", hier=6,ap=new ApprovalLevel{}},
                new HierarchyData { id=7, div="Melt Shop & Caster", dept="Caster", sec="Caster", hier=1,ap=new ApprovalLevel{l1= new Approver {id="246015",name="คุณพิสุทธิ์"},l2= new Approver {id="47227",name="คุณสุดชาย"}}},
                new HierarchyData { id=8, div="Melt Shop & Caster", dept="Caster", sec="Caster", hier=2,ap=new ApprovalLevel{l1= new Approver {id="558030",name="คุณจุฬาพรรณ"},l4=new Approver{id="54084",name="คุณศรัญญา"}}},
                new HierarchyData { id=9, div="Melt Shop & Caster", dept="Caster", sec="Caster", hier=3,ap=new ApprovalLevel{}},
                new HierarchyData { id=10, div="Melt Shop & Caster", dept="Caster", sec="Caster",hier=4,ap=new ApprovalLevel{}},
                new HierarchyData { id=11, div="Melt Shop & Caster", dept="Caster", sec="Caster", hier=5,ap=new ApprovalLevel{}},
                new HierarchyData { id=12, div="Melt Shop & Caster", dept="Caster", sec="Caster", hier=6,ap=new ApprovalLevel{}},
                new HierarchyData { id=13, div="Melt Shop & Caster", dept="Caster", sec="Caster", hier=7,ap=new ApprovalLevel{}},
                new HierarchyData { id=14, div="Maintenance & CES", dept="Central Engineering Services", sec="Air Condition", hier=5,ap=new ApprovalLevel{}},
                new HierarchyData { id=15, div="Maintenance & CES", dept="Central Engineering Services", sec="Central Electrical And Instrument", hier=1,ap=new ApprovalLevel{l1= new Approver {id="246015",name="คุณพิสุทธิ์"}}},
                new HierarchyData { id=16, div="Maintenance & CES", dept="Central Engineering Services", sec="Central Electrical And Instrument", hier=2,ap=new ApprovalLevel{}},
                new HierarchyData { id=17, div="Maintenance & CES", dept="Central Engineering Services", sec="Central Engineering Services", hier=1,ap=new ApprovalLevel{l1= new Approver {id="246015",name="คุณพิสุทธิ์"},l2= new Approver {id="47227",name="คุณสุดชาย"},}},
                new HierarchyData { id=18, div="Maintenance & CES", dept="Central Engineering Services", sec="Central Engineering Services", hier=2,ap=new ApprovalLevel{}},
                new HierarchyData { id=19, div="Maintenance & CES", dept="Central Engineering Services", sec="Central Machinery Shop Services", hier=1,ap=new ApprovalLevel{}},
                new HierarchyData { id=20, div="Maintenance & CES", dept="Central Engineering Services", sec="Central Machinery Shop Services", hier=2,ap=new ApprovalLevel{}},
                new HierarchyData { id=21, div="Maintenance & CES", dept="Central Engineering Services", sec="Central Machinery Shop Services", hier=3,ap=new ApprovalLevel{}},
                new HierarchyData { id=22, div="Maintenance & CES", dept="Central Engineering Services", sec="Central Mechanical Service", hier=1,ap=new ApprovalLevel{}},
                new HierarchyData { id=23, div="Maintenance & CES", dept="Central Engineering Services", sec="Central Mechanical Service", hier=2,ap=new ApprovalLevel{}},
                new HierarchyData { id=24, div="Central Warehouse", dept="Central Warehouse", sec="Central Warehouse", hier=1,ap=new ApprovalLevel{}},
                new HierarchyData { id=25, div="Central Warehouse", dept="Central Warehouse", sec="Central Warehouse", hier=2,ap=new ApprovalLevel{}},
                new HierarchyData { id=26, div="Central Warehouse", dept="Central Warehouse", sec="Central Warehouse", hier=3,ap=new ApprovalLevel{}},
                new HierarchyData { id=27, div="Central Warehouse", dept="Central Warehouse", sec="Central Warehouse", hier=4,ap=new ApprovalLevel{}},
                new HierarchyData { id=28, div="Marketing", dept="Corporate Communication", sec="Corporate Communication", hier=1,ap=new ApprovalLevel{}},
                new HierarchyData { id=29, div="Procurement (Domestic & Import Scrap)", dept="Domestic Scrap", sec="Domestic Scrap", hier=1,ap=new ApprovalLevel{}},
            };

            return Json(data);
        }

        // POST: /ApprovalAuthority/SaveApprover
        [HttpPost]
        public IActionResult SaveApprover([FromBody] SaveApproverRequest req)
        {
            // บันทึก DB จริง
            // _service.SaveApprover(req.RowId, req.Level, req.EmployeeId, req.Name);
            return Json(new { success = true, message = "บันทึกเรียบร้อย" });
        }

        // POST: /ApprovalAuthority/ClearApprover
        [HttpPost]
        public IActionResult ClearApprover([FromBody] ClearApproverRequest req)
        {
            // ลบออกจาก DB
            // _service.ClearApprover(req.RowId, req.Level);
            return Json(new { success = true });
        }
    }

    public class SaveApproverRequest
    {
        public int RowId { get; set; }
        public string Level { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public class ClearApproverRequest
    {
        public int RowId { get; set; }
        public string Level { get; set; } = "";
    }


    public class AddWorkerRequest
    {
        public int WcId { get; set; }
        public int[] WorkerIds { get; set; } = Array.Empty<int>();
    }

    public class RemoveWorkerRequest
    {
        public int WcId { get; set; }
        public int WorkerId { get; set; }
    }


    public class Approver
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class ApprovalLevel
    {
        public Approver l1 { get; set; }
        public Approver l2 { get; set; }
        public Approver l3 { get; set; }
        public Approver l4 { get; set; }
        
    }

    public class HierarchyData
    {
        public int id { get; set; }
        public string div { get; set; }
        public string dept { get; set; }
        public string sec { get; set; }
        public int hier { get; set; }
        public ApprovalLevel ap { get; set; }
    }


}
