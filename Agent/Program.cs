namespace Agent;

using Application.Services;
using Domain.Interfaces;
using Infrastructure.Jobs;
// MyApplication/Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using Quartz;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(async (context, services) =>
            {
                // پیکربندی Elasticsearch
                var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
                    .DefaultIndex("stockdata")
                        .BasicAuthentication("elastic", "Aa@123456") // نام کاربری و رمز عبور
                        .ServerCertificateValidationCallback((o, certificate, chain, errors) => true); // نادیده گرفتن اعتبارسنجی گواهی‌نامه
                ;
                var client = new ElasticClient(settings);
                services.AddSingleton<IElasticClient>(client);
                services.AddScoped<IElasticSearchService, ElasticSearchService>();
                services.AddScoped<IStockService, StockService>();

                services.AddHttpClient();

                // پیکربندی Quartz
                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();

                    // جاب روزانه
                    var dailyJobKey = new JobKey("DailyStockDataJob");
                q.AddJob<DailyStockDataJob>(opts => opts.WithIdentity(dailyJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(dailyJobKey)
                    .WithIdentity("DailyStockDataJob-trigger")
                        .StartNow()); // اجرای فوری
                    //.WithCronSchedule("0 0 14 * * ?")); // هر روز ساعت 2 بعد از ظهر

                // جاب هفتگی
                var weeklyJobKey = new JobKey("WeeklyStockDataJob");
                    q.AddJob<WeeklyStockDataJob>(opts => opts.WithIdentity(weeklyJobKey));
                    q.AddTrigger(opts => opts
                        .ForJob(weeklyJobKey)
                        .WithIdentity("WeeklyStockDataJob-trigger")
                                               .StartNow()); // اجرای فوری
                        //.WithCronSchedule("0 0 16 ? * WED"));  // هر چهارشنبه ساعت 4 بعد از ظهر

                // جاب ماهانه
                var monthlyJobKey = new JobKey("MonthlyStockDataJob");
                q.AddJob<MonthlyStockDataJob>(opts => opts.WithIdentity(monthlyJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(monthlyJobKey)
                    .WithIdentity("MonthlyStockDataJob-trigger")
                    .WithCronSchedule("0 0 0 1 * ?")); // هر اول ماه ساعت 12 صبح
                });

                services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            })
            .Build();

        await host.RunAsync();
    }
}
