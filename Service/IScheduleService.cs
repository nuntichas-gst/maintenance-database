using dashboardtask.Data;
using dashboardtask.Models;
using dashboardtask.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace dashboardtask.Service
{
    public interface IScheduleService
    {
        Task<List<object>> GetSchedulesAsync(int controlItemId);
        Task<object> CreateScheduleAsync(CreateMaintenanceScheduleRequest req);
        Task<string> UpdateScheduleAsync(UpdateMaintenanceScheduleRequest req);
        Task<string> DeleteScheduleAsync(DeleteScheduleRequest req);
        Task<string> BulkCreateSchedulesAsync(BulkScheduleRequest req);
    }

    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;
        public ScheduleService(AppDbContext context) => _context = context;

        public async Task<List<object>> GetSchedulesAsync(int controlItemId)
        {
            var schedules = await _context.MaintenanceSchedule
                .AsNoTracking()
                .Include(s => s.Standard)
                .Include(s => s.Indicator).ThenInclude(i => i.Unit)
                .Where(s => s.ControlItemId == controlItemId)
                .OrderBy(s => s.ScheduleId)
                .Select(s => new {
                    s.ScheduleId,
                    s.ControlItemId,
                    s.StandardId,
                    standardCode = s.Standard.StandardCode ?? "",
                    standardDesc = s.Standard.StandardDesc ?? "",
                    s.IndicatorID,
                    indicatorCode = s.Indicator.IndicatorCode ?? "",
                    indicatorDesc = s.Indicator.IndicatorDesc ?? "",
                    unitDesc = s.Indicator.Unit.UnitDesc ?? "",
                    s.IsRequired,
                    sectionCode = s.SectionCode ?? "ME",
                    s.CreatedTime,
                    s.UpdatedTime
                }).ToListAsync();

            return schedules.Cast<object>().ToList();
        }

        public async Task<object> CreateScheduleAsync(CreateMaintenanceScheduleRequest req)
        {
            var controlExists = await _context.MaintenanceControl
                .AnyAsync(c => c.ControlItemId == req.ControlItemId);
            if (!controlExists) throw new ArgumentException("Control item not found");

            var sectionCode = req.SectionCode ?? "ME";

            var exists = await _context.MaintenanceSchedule.AnyAsync(s =>
                s.ControlItemId == req.ControlItemId &&
                s.StandardId == req.StandardId &&
                s.SectionCode == sectionCode);

            if (exists) throw new ArgumentException("Schedule for this standard/section already exists");

            var schedule = new MaintenanceSchedule
            {
                ControlItemId = req.ControlItemId,
                StandardId = req.StandardId,
                IndicatorID = (int)req.IndicatorId,
                IsRequired = req.IsRequired,
                SectionCode = sectionCode,
                CreatedTime = DateTime.UtcNow
            };

            _context.MaintenanceSchedule.Add(schedule);
            await _context.SaveChangesAsync();

            return new { scheduleId = schedule.ScheduleId, message = "Schedule created successfully" };
        }

        public async Task<string> UpdateScheduleAsync(UpdateMaintenanceScheduleRequest req)
        {
            var schedule = await _context.MaintenanceSchedule.FindAsync(req.ScheduleId);
            if (schedule == null) throw new KeyNotFoundException("Schedule not found");

            if (req.IndicatorId.HasValue) schedule.IndicatorID = req.IndicatorId.Value;
            if (req.IsRequired.HasValue) schedule.IsRequired = req.IsRequired.Value;
            if (!string.IsNullOrEmpty(req.SectionCode)) schedule.SectionCode = req.SectionCode;

            schedule.UpdatedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return "Schedule updated successfully";
        }

        public async Task<string> DeleteScheduleAsync(DeleteScheduleRequest req)
        {
            var schedule = await _context.MaintenanceSchedule.FindAsync(req.ScheduleId);
            if (schedule == null) throw new KeyNotFoundException("Schedule not found");

            _context.MaintenanceSchedule.Remove(schedule);
            await _context.SaveChangesAsync();

            return "Schedule deleted successfully";
        }

        public async Task<string> BulkCreateSchedulesAsync(BulkScheduleRequest req)
        {
            if (req.Schedules == null || !req.Schedules.Any())
                throw new ArgumentException("No schedules provided");

            var schedules = req.Schedules.Select(s => new MaintenanceSchedule
            {
                ControlItemId = s.ControlItemId,
                StandardId = s.StandardId,
                IndicatorID = (int)s.IndicatorId,
                IsRequired = s.IsRequired,
                SectionCode = s.SectionCode ?? "ME",
                CreatedTime = DateTime.UtcNow
            }).ToList();

            _context.MaintenanceSchedule.AddRange(schedules);
            await _context.SaveChangesAsync();

            return $"{schedules.Count} schedules created successfully";
        }
    }

}
