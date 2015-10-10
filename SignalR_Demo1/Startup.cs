using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using SignalR_Demo1.Controllers;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(SignalR_Demo1.Startup))]
namespace SignalR_Demo1
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here

            var idProvider = new GameConnectionFactory();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);

            app.MapSignalR();
        }
    }
}