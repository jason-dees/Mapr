using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MapR.Data.Extensions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
            where T : class, IStartupTask
            => services.AddTransient<IStartupTask, T>();
    }

    //Shamelessly stolen from https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/
    public interface IStartupTask {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
