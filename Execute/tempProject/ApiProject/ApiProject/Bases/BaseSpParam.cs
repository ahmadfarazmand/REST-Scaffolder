using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiProject.Bases
{
    public interface IBaseSqlSpParam
    {
        string GetSpParams();
    }

    public class BaseSqlFilterParam : IBaseSqlSpParam
    {
        public virtual string GetSpParams()
        {
            return "";
        }
    }

    public class BaseSqlListFilterParam : IBaseSqlSpParam
    {
        public virtual int? TopCount { get; set; } = 100;
        public virtual string GetSpParams()
        {
            return "";
        }
    }

    public class BaseSqlPaginationFilterParam : IBaseSqlSpParam
    {
        public virtual int? OffsetSkip { get; set; } = 0;
        public virtual int? OffsetSize { get; set; } = 10;
        public virtual string GetSpParams()
        {
            return "";
        }
    }

    public class BaseSqlAddParam : IBaseSqlSpParam
    {
        public virtual string GetSpParams()
        {
            return "";
        }
    }

    public class BaseSqlUpdateParam : IBaseSqlSpParam
    {
        public virtual string GetSpParams()
        {
            return "";
        }
    }
}