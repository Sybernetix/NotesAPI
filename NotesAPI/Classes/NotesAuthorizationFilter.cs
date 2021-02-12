using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace NotesAPI.Classes
{
    public class NotesAuthorizationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            bool authorizationSuccess = false;
            if (actionContext.Request.Headers.Authorization != null)
            {
                string body = actionContext.Request.Content.ReadAsStringAsync().Result;
                string authenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(actionContext.Request.Headers.Authorization.Parameter));
                string username = authenticationToken.Split(':')[0];
                string password = authenticationToken.Substring(username.Length + 1);

                if (Database.VerifyUserCredentials(username, password))
                {
                    authorizationSuccess = true;
                }
            }


            if (!authorizationSuccess)
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}