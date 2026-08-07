using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

/// <summary>
/// مؤقّت خلفي يشغّل النسخ الاحتياطي التلقائي المجدول حسب إعدادات
/// <c>BackupSettings</c> (يومي/أسبوعي/شهري). يفحص الاستحقاق كل دقيقة.
/// </summary>
public class BackupSchedulerHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<BackupSchedulerHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();
                await backupService.RunScheduledBackupIfDueAsync();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Backup scheduler tick failed");
            }
        }
    }
}
