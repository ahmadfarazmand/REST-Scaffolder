﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace ApiProject
{
    public class JwtAuth : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.Request.Headers.Authorization != null)
                {
                    if(actionContext.Request.Headers.Authorization.Scheme == "Bearer")
                    {
                        var authToken = actionContext.Request.Headers.Authorization.Parameter;

                        var decoAuthToken = EncryptCode.Decrypt(authToken);

                        //split by colon : and store in variable  
                        //var UserNameAndPassword = decoAuthToken.Split(':');
                        //Passing to a function for authorization  

                        //var controller = (string)actionContext.RequestContext.RouteData.Values["controller"];
                        var action = (HttpActionDescriptor[])actionContext.RequestContext.RouteData.Route.DataTokens["actions"];
                        if (action.Length > 0 && action[0].ControllerDescriptor != null)
                        {
                            var rightPlace = action[0];
                            var controllerName = rightPlace.ControllerDescriptor.ControllerName;
                            var actionName = rightPlace.ActionName;
                            var infoes = decoAuthToken.Split('|');
                            if(infoes != null && infoes.Length == 3)
                            {
                                DateTime expDate = DateTime.Now.AddDays(-30);
                                bool haveDate = false;
                                haveDate = DateTime.TryParse(infoes[2], out expDate);
                                if (infoes[0].StartsWith("userId=") && haveDate && (expDate > DateTime.Now))
                                {
                                    if(RolePermission.RoleList.Any(a=> a.Name == controllerName && a.Roles.Any(r=> r == infoes[1]) && a.Actions.Any(ac=> ac.Name == actionName && ac.Roles.Any(ro=> ro == infoes[1]))))
                                    {
                                        var genericIdentity = new GenericIdentity(decoAuthToken);
                                        var princ = new GenericPrincipal(genericIdentity, null);
                                        Thread.CurrentPrincipal = princ;
                                    }
                                        
                                    else
                                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                                }
                                else
                                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                            }

                            else
                                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        }
                        else
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                    else
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

                }
                else
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                //ex.Message.ToString();
            }
        }


        public static bool IsAuthorizedUser(string Username, string Password)
        {
            // In this method we can handle our database logic here...  
            //Here we have given the hard-coded values   
            return Username == "shahbaz" && Password == "abc123";
        }

        protected  void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Home" },
                    { "action", "UnAuthorized" }
               });
        }
    }

    
}