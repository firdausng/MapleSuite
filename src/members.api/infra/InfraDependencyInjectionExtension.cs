using members.api.infra.data;
using members.api.infra.data.interceptors;
using Microsoft.EntityFrameworkCore;

namespace members.api.infra;

public static class InfraDependencyInjectionExtension
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<InsertOutboxMessagesInterceptor>();
        builder.Services.AddDbContext<MemberContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<InsertOutboxMessagesInterceptor>());
            options.UseNpgsql(builder.Configuration.GetConnectionString("memberdb"));
        });
        builder.EnrichNpgsqlDbContext<MemberContext>(settings =>
        {
            settings.DisableRetry = false;
            settings.CommandTimeout = 30;
        });
        // builder.AddNpgsqlDbContext<MemberContext>(connectionName: "memberdb",
        //     settings =>
        //     {
        //         settings.DisableRetry = false;
        //         settings.CommandTimeout = 30;
        //     });
        return builder;
    }
}