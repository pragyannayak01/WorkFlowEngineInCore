using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkFlowEngine.IRepository.DapperConfiguration;
using WorkFlowEngine.IRepository.WFEngine.ReportDesign;
using WorkFlowEngine.IRepository.WFERender;
using WorkFlowEngine.Repository.DapperConfiguration;
using WorkFlowEngine.Repository.EFConfig;
using WorkFlowEngine.Repository.WFEngine.ReportDesign;
using WorkFlowEngine.Repository.WFERender;

namespace WorkFlowEngine.API.DIContainer
{
    public static class CustomContainer
    {
        public static void AddCustomContainer(this IServiceCollection services, IConfiguration configuration)
        {
            #region For EFConfig(Entity Framework Configuration)
            services.AddDbContext<WFContext>(x => x.UseSqlServer(configuration.GetConnectionString("WFEConnectionString")));
            #endregion
            #region For DapperConfig
            IConnectionFactory connectionFactory = new ConnectionFactory(configuration.GetConnectionString("WFEConnectionString"));
            services.AddSingleton(connectionFactory);
            services.AddSingleton<IDFormRepository, DFormRepository>();
            services.AddSingleton<IFormRepository, FormRepository>(); 
            #endregion
        }
    }
}
