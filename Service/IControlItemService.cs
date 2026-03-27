using dashboardtask.Data;
using dashboardtask.Models;
using dashboardtask.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace dashboardtask.Service
{
    public interface IControlService
    {
        Task<List<object>> GetAssetControlsAsync(int? assetTypeId, string assetName);
        Task<object> GetAssetControlAsync(int id);
        Task<string> CreateAssetControlAsync(AssetControlRequest req);
        Task<string> UpdateAssetControlAsync(AssetControlRequest req);
        Task<string> DeleteAssetControlAsync(DeleteAssetControlRequest req);
    }

    public class ControlService : IControlService
    {
        private readonly AppDbContext _context;
        public ControlService(AppDbContext context) => _context = context;

        public async Task<List<object>> GetAssetControlsAsync(int? assetTypeId, string assetName)
        {
            var query = _context.MaintenanceControl.AsNoTracking()
                .Include(c => c.AssetType)
                .Include(c => c.Machine)
                .Include(c => c.Unit)
                .Include(c => c.Part)
                .AsQueryable();

            if (assetTypeId.HasValue)
                query = query.Where(c => c.AssetTypeId == assetTypeId);

            if (!string.IsNullOrWhiteSpace(assetName))
            {
                var name = assetName.Trim().ToLower();
                query = query.Where(c =>
                    (c.Machine != null && c.Machine.Name.ToLower().Contains(name)) ||
                    (c.Unit != null && c.Unit.Name.ToLower().Contains(name)) ||
                    (c.Part != null && c.Part.Name.ToLower().Contains(name)));
            }

            return await query.OrderBy(c => c.ControlItemId)
                .Select(c => new {
                    c.ControlItemId,
                    c.AssetTypeId,
                    assetTypeName = c.AssetType != null ? c.AssetType.TypeName : "-",
                    c.MachineId,
                    c.UnitId,
                    c.PartId,
                    assetName = c.AssetTypeId == 4 ? c.Part.Name :
                                c.AssetTypeId == 3 ? c.Unit.Name :
                                c.AssetTypeId == 2 ? c.Machine.Name : "-",
                    c.CreatedTime
                }).ToListAsync<object>();
        }

        public async Task<object> GetAssetControlAsync(int id)
        {
            var control = await _context.MaintenanceControl.AsNoTracking()
                .Include(c => c.AssetType)
                .Include(c => c.Machine)
                .Include(c => c.Unit)
                .Include(c => c.Part)
                .Where(c => c.ControlItemId == id)
                .Select(c => new {
                    c.ControlItemId,
                    c.AssetTypeId,
                    assetTypeName = c.AssetType != null ? c.AssetType.TypeName : "-",
                    c.MachineId,
                    c.UnitId,
                    c.PartId,
                    assetName = c.AssetTypeId == 4 ? c.Part.Name :
                                c.AssetTypeId == 3 ? c.Unit.Name :
                                c.AssetTypeId == 2 ? c.Machine.Name : "-",
                    c.CreatedTime
                }).FirstOrDefaultAsync();

            if (control == null) throw new KeyNotFoundException("Asset control not found");
            return control;
        }

        public async Task<string> CreateAssetControlAsync(AssetControlRequest req)
        {
            var control = new MaintenanceControl
            {
                AssetTypeId = req.AssetTypeId,
                MachineId = req.AssetTypeId == 2 ? req.AssetId : null,
                UnitId = req.AssetTypeId == 3 ? req.AssetId : null,
                PartId = req.AssetTypeId == 4 ? req.AssetId : null,
                CreatedTime = DateTime.UtcNow
            };
            _context.MaintenanceControl.Add(control);
            await _context.SaveChangesAsync();
            return "Asset Control created successfully";
        }

        public async Task<string> UpdateAssetControlAsync(AssetControlRequest req)
        {
            var control = await _context.MaintenanceControl.FindAsync(req.ControlItemId);
            if (control == null) throw new KeyNotFoundException("Asset Control not found");

            control.AssetTypeId = req.AssetTypeId;
            control.MachineId = req.AssetTypeId == 2 ? req.AssetId : null;
            control.UnitId = req.AssetTypeId == 3 ? req.AssetId : null;
            control.PartId = req.AssetTypeId == 4 ? req.AssetId : null;
            control.UpdatedTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return "Asset Control updated successfully";
        }

        public async Task<string> DeleteAssetControlAsync(DeleteAssetControlRequest req)
        {
            var control = await _context.MaintenanceControl.FindAsync(req.ControlItemId);
            if (control == null) throw new KeyNotFoundException("Asset Control not found");

            _context.MaintenanceControl.Remove(control);
            await _context.SaveChangesAsync();
            return "Asset Control deleted successfully";
        }
    }

}
