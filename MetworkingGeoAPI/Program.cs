using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MetWorkingGeo.API.Models;

namespace MetWorkingGeo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var host = BuildWebHost(args);

			host.Run();
		}

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
