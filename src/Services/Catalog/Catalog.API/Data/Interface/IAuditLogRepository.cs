using Catalog.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Data.Interface
{
    public interface IAuditLogRepository
    {
        Task<AuditLog> AddAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAllAsync();
    }
}