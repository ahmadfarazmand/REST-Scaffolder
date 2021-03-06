﻿using ApiProject.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ApiProject
{

    [JwtAuth]
    public class BaseApiController : ApiController
    {
        protected SqlDataManager _sqlData;
        protected string _entityName = "";
        protected BaseSpName _spName;

        public BaseApiController(string entityName)
        {
            BaseSpName spNames = new BaseSpName();
            _entityName = entityName;
            _sqlData = new SqlDataManager(entityName, spNames);
        }
    }
}