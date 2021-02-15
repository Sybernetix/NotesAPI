using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
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
using System.Web.Script.Serialization;

namespace NotesAPI.Classes
{
    public class NotesAuthorizationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            bool authorizationSuccess = false;
            try
            {
                string body = actionContext.Request.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(body);
                string token = Convert.ToString(jsonObject["token"]);

                if (Database.TokenExist(token))
                {
                    authorizationSuccess = true;
                }
            }
            catch (Exception)
            {
                //Error With JSON payload
            }

            if (!authorizationSuccess)
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}