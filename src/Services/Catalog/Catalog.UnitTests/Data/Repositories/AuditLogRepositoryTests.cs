using Xunit;
using Moq;
using Catalog.API.Data;
using Catalog.API.Data.Repositories;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Tests.Data.Repositories
{
    public class AuditLogRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly AuditLogRepository _repository;

        public AuditLogRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(_options);
            _repository = new AuditLogRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddAuditLogToDatabase()
        {
            // Arrange
            var auditLog = new AuditLog(
                plateId: Guid.NewGuid(),
                registration: "ABC123",
                action: "Created"
            );

            // Act
            var result = await _repository.AddAsync(auditLog);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(auditLog.Id, result.Id);
            Assert.Equal(1, await _context.AuditLogs.CountAsync());

            var savedAuditLog = await _context.AuditLogs.FirstAsync();
            Assert.Equal(auditLog.PlateId, savedAuditLog.PlateId);
            Assert.Equal(auditLog.Registration, savedAuditLog.Registration);
            Assert.Equal(auditLog.Action, savedAuditLog.Action);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAuditLogs()
        {
            // Arrange
            var auditLogs = new List<AuditLog>
            {
                new AuditLog(Guid.NewGuid(), "ABC123", "Created"),
                new AuditLog(Guid.NewGuid(), "XYZ789", "Updated"),
                new AuditLog(Guid.NewGuid(), "DEF456", "Deleted")
            };

            await _context.AuditLogs.AddRangeAsync(auditLogs);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(auditLogs.Select(x => x.Registration), result.Select(x => x.Registration));
            Assert.Equal(auditLogs.Select(x => x.Action), result.Select(x => x.Action));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoAuditLogsExist()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}