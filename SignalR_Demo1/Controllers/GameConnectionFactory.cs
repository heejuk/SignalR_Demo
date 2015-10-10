using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_Demo1.Controllers
{
    public class GameConnectionFactory : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            if (request.Cookies.ContainsKey("srconnectionid") && request.Cookies["srconnectionid"] != null)
            {
                //HttpSessionStateBase[]
                
                
                return request.Cookies["srconnectionid"].ToString();
            }

            return Guid.NewGuid().ToString();
        }
    }
}