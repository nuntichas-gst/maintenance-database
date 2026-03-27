using dashboardtask.Data;
using dashboardtask.Models;
using Microsoft.EntityFrameworkCore;

namespace dashboardtask.Service
{
    public interface IAssetService
    {
        Task<List<object>> GetAssetTypesAsync();
        Task<object> GetAssetDataAsync();
        Task<object> GetStatisticsAsync();
        Task<List<object>> GetAssetsByTypeAsync(int assetTypeId);
    }

    public class AssetService : IAssetService
    {
        private readonly AppDbContext _context;
        public AssetService(AppDbContext context) => _context = context;

        public async Task<List<object>> GetAssetTypesAsync() =>
            await _context.AssetType.AsNoTracking()
                .OrderBy(t => t.TypeName)
                .Select(t => new { t.AssetTypeId, t.TypeName })
                .ToListAsync<object>();

        public async Task<object> GetAssetDataAsync()
        {
            var lines = await _context.Line.AsNoTracking().OrderBy(l => l.Code).ThenBy(l => l.Name).ToListAsync();
            var machineCountByLine = await _context.Machine.AsNoTracking()
                .GroupBy(m => m.LineId).Select(g => new { LineId = g.Key, Count = g.Count() }).ToListAsync();

            var treeData = lines.Select(l => new AssetItem
            {
                id = l.LineId,
                name = l.Name,
                code = l.Code,
                level = "Line",
                hasChildren = machineCountByLine.Any(m => m.LineId == l.LineId),
                children = null,
                childrenLoaded = false,
                MTTR = l.MTTR ?? 0,
                MTBF = l.MTBF ?? 0,
                rank = string.IsNullOrEmpty(l.Rank) ? "" : l.Rank,
                status = string.IsNullOrEmpty(l.Status) ? "Active" : l.Status
            }).ToList();

            return new
            {
                nodes = treeData,
                stats = new
                {
                    totalLines = await _context.Line.CountAsync(),
                    totalMachines = await _context.Machine.CountAsync(),
                    totalUnits = await _context.Unit.CountAsync(),
                    totalParts = await _context.Part.CountAsync()
                }
            };
        }

        public async Task<object> GetStatisticsAsync()
        {
            return new
            {
                totalStandards = await _context.MaintenanceStandard.CountAsync(),
                totalIndicators = await _context.MaintenanceIndicator.CountAsync(),
                totalControlItems = await _context.MaintenanceControl.CountAsync(),
                totalSchedules = await _context.MaintenanceSchedule.CountAsync(),
                controlsByAssetType = await _context.MaintenanceControl
                    .Where(c => c.AssetTypeId != null)
                    .GroupBy(c => c.AssetType.TypeName)
                    .Select(g => new { assetType = g.Key, count = g.Count() }).ToListAsync(),
                schedulesByIndicator = await _context.MaintenanceSchedule
                    .Include(s => s.Indicator)
                    .GroupBy(s => s.Indicator.IndicatorCode)
                    .Select(g => new { indicator = g.Key, count = g.Count() }).ToListAsync(),
                controlsByStandardChart = await _context.MaintenanceSchedule
                    .Include(s => s.Standard)
                    .GroupBy(s => s.Standard.StandardDesc)
                    .Select(g => new { Standard = g.Key, count = g.Count() }).ToListAsync()
            };
        }

        public async Task<List<object>> GetAssetsByTypeAsync(int assetTypeId)
        {
            return assetTypeId switch
            {
                2 => await _context.Machine.AsNoTracking()
                    .Where(m => !_context.MaintenanceControl.Any(c => c.MachineId == m.MachineId))
                    .OrderBy(m => m.Name)
                    .Select(m => (object)new { m.MachineId, m.Code, m.Name, type = "Machine" })
                    .ToListAsync(),
                3 => await _context.Unit.AsNoTracking().Include(u => u.Machine)
                    .Where(u => !_context.MaintenanceControl.Any(c => c.UnitId == u.UnitId))
                    .OrderBy(u => u.Name)
                    .Select(u => (object)new { u.UnitId, u.Code, u.Name, type = "Unit", machineName = u.Machine.Name })
                    .ToListAsync(),
                4 => await _context.Part.AsNoTracking().Include(p => p.Unit).ThenInclude(u => u.Machine)
                    .Where(p => !_context.MaintenanceControl.Any(c => c.PartId == p.PartId))
                    .OrderBy(p => p.Name)
                    .Select(p => (object)new { p.PartId, p.Code, p.Name, type = "Part", unitName = p.Unit.Name, machineName = p.Unit.Machine.Name })
                    .ToListAsync(),
                _ => throw new ArgumentException("Invalid asset type ID")
            };
        }
    }

}
