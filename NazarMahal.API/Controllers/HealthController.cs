using Microsoft.AspNetCore.Mvc;
using NazarMahal.Infrastructure.Data;
using System.Diagnostics;

namespace NazarMahal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext dbContext, ILogger<HealthController> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Health check endpoint to verify API and database status
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            var (dbStatus, dbMessage, dbDuration) = await CheckDatabaseHealth();
            var isHealthy = dbStatus == "connected";
            var statusCode = isHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;

            var healthStatus = new
            {
                status = "API running",
                timestamp = DateTime.UtcNow,
                database = new
                {
                    status = dbStatus,
                    message = dbMessage,
                    connectionTimeMs = dbDuration
                }
            };

            return StatusCode(statusCode, healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "API running",
                timestamp = DateTime.UtcNow,
                database = new { status = "error", message = ex.Message, connectionTimeMs = (long?)null }
            });
        }
    }

    private async Task<(string status, string? message, long durationMs)> CheckDatabaseHealth()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();
            stopwatch.Stop();

            if (canConnect)
            {
                return ("connected", null, stopwatch.ElapsedMilliseconds);
            }
            return ("disconnected", null, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "Database health check failed");
            return ("error", ex.Message, stopwatch.ElapsedMilliseconds);
        }
    }
}
