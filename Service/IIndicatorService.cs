using dashboardtask.Data;
using dashboardtask.Models;
using dashboardtask.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace dashboardtask.Service
{
    public interface IIndicatorService
    {
        Task<List<object>> GetIndicatorsAsync(int? unitId);
        Task<object> GetIndicatorAsync(int id);
        Task<object> CreateIndicatorAsync(MaintenanceIndicatorRequest req);
        Task<string> UpdateIndicatorAsync(UpdateMaintenanceIndicatorRequest req);
        Task<string> DeleteIndicatorAsync(DeleteIndicatorRequest req);
        Task<object> BulkCreateIndicatorsAsync(List<MaintenanceIndicatorRequest> indicators);
    }

    public class IndicatorService : IIndicatorService
    {
        private readonly AppDbContext _context;
        public IndicatorService(AppDbContext context) => _context = context;

        public async Task<List<object>> GetIndicatorsAsync(int? unitId) =>
            await _context.MaintenanceIndicator.AsNoTracking()
                .Include(i => i.Unit)
                .Where(i => unitId == null || i.UnitID == unitId)
                .OrderBy(i => i.IndicatorID)
                .Select(i => new {
                    i.IndicatorID,
                    i.IndicatorCode,
                    i.IndicatorDesc,
                    i.UnitID,
                    unitDesc = i.Unit != null ? i.Unit.UnitDesc : null,
                    i.CreatedTime,
                    i.UpdatedTime
                }).ToListAsync<object>();

        public async Task<object> GetIndicatorAsync(int id)
        {
            var indicator = await _context.MaintenanceIndicator.AsNoTracking()
                .Include(i => i.Unit)
                .Where(i => i.IndicatorID == id)
                .Select(i => new {
                    i.IndicatorID,
                    i.IndicatorCode,
                    i.IndicatorDesc,
                    i.UnitID,
                    unitDesc = i.Unit != null ? i.Unit.UnitDesc : null,
                    i.CreatedTime,
                    i.UpdatedTime
                }).FirstOrDefaultAsync();
            if (indicator == null) throw new KeyNotFoundException("Indicator not found");
            return indicator;
        }

        public async Task<object> CreateIndicatorAsync(MaintenanceIndicatorRequest req)
        {
            if (await _context.MaintenanceIndicator.AnyAsync(i => i.IndicatorCode == req.IndicatorCode))
                throw new ArgumentException($"Indicator code '{req.IndicatorCode}' already exists");

            var indicator = new MaintenanceIndicator
            {
                IndicatorCode = req.IndicatorCode,
                IndicatorDesc = req.IndicatorDesc,
                UnitID = req.UnitId,
                CreatedTime = DateTime.UtcNow
            };
            _context.MaintenanceIndicator.Add(indicator);
            await _context.SaveChangesAsync();
            return new { indicatorId = indicator.IndicatorID, message = "Indicator created successfully" };
        }

        public async Task<string> UpdateIndicatorAsync(UpdateMaintenanceIndicatorRequest req)
        {
            var indicator = await _context.MaintenanceIndicator.FindAsync(req.IndicatorId);
            if (indicator == null) throw new KeyNotFoundException("Indicator not found");

            if (!string.IsNullOrEmpty(req.IndicatorCode)) indicator.IndicatorCode = req.IndicatorCode;
            if (!string.IsNullOrEmpty(req.IndicatorDesc)) indicator.IndicatorDesc = req.IndicatorDesc;
            if (req.UnitId.HasValue) indicator.UnitID = req.UnitId.Value;
            indicator.UpdatedTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return "Indicator updated successfully";
        }

        public async Task<string> DeleteIndicatorAsync(DeleteIndicatorRequest req)
        {
            var inUse = await _context.MaintenanceSchedule.AnyAsync(s => s.IndicatorID == req.IndicatorId);
            if (inUse) throw new InvalidOperationException("Cannot delete indicator. It is being used in maintenance schedules.");

            var indicator = await _context.MaintenanceIndicator.FindAsync(req.IndicatorId);
            if (indicator == null) throw new KeyNotFoundException("Indicator not found");

            _context.MaintenanceIndicator.Remove(indicator);
            await _context.SaveChangesAsync();
            return "Indicator deleted successfully";
        }

        public async Task<object> BulkCreateIndicatorsAsync(List<MaintenanceIndicatorRequest> indicators)
        {
            if (indicators == null || !indicators.Any()) throw new ArgumentException("No indicators provided");

            var incomingCodes = indicators.Select(i => i.IndicatorCode).ToList();
            var existingCodes = await _context.MaintenanceIndicator
                .Where(i => incomingCodes.Contains(i.IndicatorCode))
                .Select(i => i.IndicatorCode)
                .ToHashSetAsync();

            var toCreate = indicators.Where(req => !existingCodes.Contains(req.IndicatorCode))
                .Select(req => new MaintenanceIndicator
                {
                    IndicatorCode = req.IndicatorCode,
                    IndicatorDesc = req.IndicatorDesc,
                    UnitID = req.UnitId,
                    CreatedTime = DateTime.UtcNow
                }).ToList();

            if (toCreate.Any())
            {
                _context.MaintenanceIndicator.AddRange(toCreate);
                await _context.SaveChangesAsync();
            }

            return new
            {
                totalRequested = indicators.Count,
                created = toCreate.Count,
                skipped = indicators.Count - toCreate.Count,
                createdIds = toCreate.Select(i => i.IndicatorID).ToList(),
                skippedReasons = indicators.Where(req => existingCodes.Contains(req.IndicatorCode))
                    .Select(req => $"Code '{req.IndicatorCode}' already exists").ToList(),
                message = $"Successfully created {toCreate.Count} indicator(s)"
            };
        }
    }





}
