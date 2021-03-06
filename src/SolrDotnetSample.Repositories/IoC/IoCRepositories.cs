using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SolrDotnetSample.Repositories.Contexts;
using SolrDotnetSample.Repositories.IoC.Options;
using SolrDotnetSample.Repositories.Mappers;
using SolrNet;

namespace SolrDotnetSample.Repositories.IoC
{
    public static class IoCRepositories
    {
        private static readonly SolrOptions SolrOptions = new SolrOptions();
        private static readonly RepositoryOptions RepositoryOptions = new RepositoryOptions();

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
            => services.AddAutoMapper(typeof(ModelToDomainProfile), typeof(DomainToModelProfile));

        public static IServiceCollection AddDbContext(this IServiceCollection services, Action<RepositoryOptions> options)
        {
            options.Invoke(RepositoryOptions);

            return services.AddDbContext<SolrDotnetSampleContext>(dbContextOptions
                => dbContextOptions
                   .UseSqlServer(RepositoryOptions.ConnectionString, SqlServerOptionsAction)
                   .UseLazyLoadingProxies());
        }

        private static void SqlServerOptionsAction(SqlServerDbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
            optionsBuilder.MigrationsAssembly(typeof(SolrDotnetSampleContext).Assembly.GetName().Name);
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
            => services.AddScoped<IPostNoSqlRepository, PostNoSqlRepository>()
               .AddScoped<IPostRelationalRepository, PostRelationRepository>();

        public static IServiceCollection AddSolr(this IServiceCollection services, Action<SolrOptions> options)
        {
            options.Invoke(SolrOptions);
            return services.AddSolrNet(SolrOptions.Url);
        }
    }
}