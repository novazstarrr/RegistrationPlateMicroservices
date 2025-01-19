using Microsoft.EntityFrameworkCore;
using Catalog.API.Data.Interface;
using Catalog.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Data.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
            return auditLog;
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .ToListAsync();
        }
    }
}