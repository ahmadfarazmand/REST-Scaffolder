﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiProject
{
    public class RoleControllerModel
    {
        public IEnumerable<RoleActionModel> Actions { get; set; }

        public string Name { get; set; }

        public string[] Roles { get; set; }
    }

    public class RoleActionModel
    {
        public string Name { get; set; }

        public string[] Roles { get; set; }
    }
}