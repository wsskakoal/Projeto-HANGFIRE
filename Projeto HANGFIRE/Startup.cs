using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projeto_HANGFIRE
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddHangfire(op =>
            {
                op.UseMemoryStorage();
            });

            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHangfireDashboard();

            // São executados uma unica vez
            BackgroundJob.Enqueue(() => MeuPrimeiroFireAndForget());

            // Faz um agendamento do job e faz de tempos em tempos que foi definido. Pode ser por hora/ dias/mes /ano
            RecurringJob.AddOrUpdate(() => Console.Write("Recurring funcionando"), Cron.Daily);

            // Job para realizar em dois dias
            BackgroundJob.Schedule(() => Console.Write("Schedule job funcionando"), TimeSpan.FromDays(2));

            // Continuations são as tarefas filhas que sao agendada para ser executada apos a execução de uma tarefa pai.
            string jobId = BackgroundJob.Enqueue(() => Console.Write("Tarefa pai"));
            BackgroundJob.ContinueJobWith(jobId, () => Console.Write("tarefa filha"));

        }

        public async Task MeuPrimeiroFireAndForget()
        {
            await Task.Run(() =>
            {
                Console.Write("Meu primeiro fire and forget funcionando");
            });
        }
    }
}
