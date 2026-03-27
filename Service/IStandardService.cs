using dashboardtask.Data;
using dashboardtask.Models;
using dashboardtask.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace dashboardtask.Service
{
    public interface IStandardService
    {
        Task<List<object>> GetStandardsAsync();
        Task<object> GetStandardAsync(int id);
        Task<List<object>> GetStandardTypesAsync();
        Task<object> CreateStandardAsync(CreateMaintenanceStandardRequest req);
        Task<string> UpdateStandardAsync(UpdateMaintenanceStandardRequest req);
        Task<string> DeleteStandardAsync(DeleteRequest req);
    }

    public class StandardService : IStandardService
    {
        private readonly AppDbContext _context;
        public StandardService(AppDbContext context) => _context = context;

        public async Task<List<object>> GetStandardsAsync()
        {
            return await _context.MaintenanceStandard
                .Include(s => s.StandardType)
                .AsNoTracking()
                .OrderBy(s => s.StandardCode)
                .Select(s => new {
                    s.StandardId,
                    s.StandardCode,
                    s.StandardDesc,
                    standardTypeDesc = s.StandardType.StandardTypeDesc,
                    s.CreatedTime,
                    s.UpdatedTime
                }).ToListAsync<object>();
        }

        public async Task<object> GetStandardAsync(int id)
        {
            var standard = await _context.MaintenanceStandard
                .AsNoTracking()
                .Where(s => s.StandardId == id)
                .Select(s => new { s.StandardId, s.StandardCode, s.StandardDesc, s.StandardTypeId })
                .FirstOrDefaultAsync();

            if (standard == null) throw new KeyNotFoundException("Standard not found");
            return standard;
        }

        public async Task<List<object>> GetStandardTypesAsync()
        {
            return await _context.MaintenanceStandardType
                .AsNoTracking()
                .OrderBy(t => t.StandardTypeDesc)
                .Select(t => new { t.StandardTypeId, t.StandardTypeDesc })
                .ToListAsync<object>();
        }

        public async Task<object> CreateStandardAsync(CreateMaintenanceStandardRequest req)
        {
            var exists = await _context.MaintenanceStandard.AnyAsync(s => s.StandardCode == req.StandardCode);
            if (exists) throw new ArgumentException($"Standard code '{req.StandardCode}' already exists");

            var standard = new MaintenanceStandard
            {
                StandardCode = req.StandardCode.Trim(),
                StandardDesc = req.StandardDesc.Trim(),
                StandardTypeId = req.StandardTypeId,
                CreatedTime = DateTime.UtcNow
            };

            _context.MaintenanceStandard.Add(standard);
            await _context.SaveChangesAsync();

            return new { standardId = standard.StandardId, message = "Standard created successfully" };
        }

        public async Task<string> UpdateStandardAsync(UpdateMaintenanceStandardRequest req)
        {
            var standard = await _context.MaintenanceStandard.FindAsync(req.StandardId);
            if (standard == null) throw new KeyNotFoundException("Standard not found");

            if (!string.IsNullOrWhiteSpace(req.StandardCode)) standard.StandardCode = req.StandardCode.Trim();
            if (!string.IsNullOrWhiteSpace(req.StandardDesc)) standard.StandardDesc = req.StandardDesc.Trim();
            standard.StandardTypeId = req.StandardTypeId;
            standard.UpdatedTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return "Standard updated successfully";
        }

        public async Task<string> DeleteStandardAsync(DeleteRequest req)
        {
            var standard = await _context.MaintenanceStandard.FindAsync(req.StandardId);
            if (standard == null) throw new KeyNotFoundException("Standard not found");

            _context.MaintenanceStandard.Remove(standard);
            await _context.SaveChangesAsync();
            return "Standard deleted successfully";
        }
    }

}
