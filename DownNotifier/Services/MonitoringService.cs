using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DownNotifier.Data;
using DownNotifier.Models;
using Microsoft.Extensions.DependencyInjection;
using DownNotifier.Common;
using Microsoft.Extensions.Configuration;
using Azure.Core;

namespace DownNotifier.Services
{
    public class MonitoringService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly Helpers _helpers;

        public MonitoringService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _helpers = new Helpers(configuration);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckApps, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private void CheckApps(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var httpClient = new HttpClient();

                var targetApps = context.TargetApps.ToList();


                foreach (var app in targetApps)
                {
                    if (app.LastChecked.Add(app.Interval) < DateTime.Now)
                    {
                        var notification = new Notification
                        {
                            TargetAppId = app.Id,
                            SentAt = DateTime.Now
                        };

                        HttpResponseMessage response = new HttpResponseMessage();
                        try
                        {
                            response = httpClient.GetAsync(app.Url).Result;
                        }
                        catch (Exception ex)
                        {
                            notification.Message = ex.Message;
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            notification.Message = $"The application {app.Name} is down! Status code:{response.StatusCode}";
                            _helpers.MailSend("ertugrulakdag@gmail.com"
                                , $"The application {app.Name} is down!"
                                , $"The application {app.Name} is down! Status code:{response.StatusCode} Message:{notification.Message}", DateTime.Now);
                            context.Notifications.Add(notification);
                            app.LastChecked = DateTime.Now;
                            app.IsUp = false;
                            context.TargetApps.Update(app);
                            context.SaveChanges();
                        }
                        else
                        {
                            app.LastChecked = DateTime.Now;
                            app.IsUp = true;
                            context.TargetApps.Update(app);
                            context.SaveChanges();
                        }
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}