
using leave.api.infra.data;
using Microsoft.EntityFrameworkCore;

namespace members.api.infra;

public static class InfraDependencyInjectionExtension
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<LeaveContext>((sp, options) =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("leavedb"));
        });
        builder.EnrichNpgsqlDbContext<LeaveContext>(settings =>
        {
            settings.DisableRetry = false;
            settings.CommandTimeout = 30;
        });
        return builder;
    }
}