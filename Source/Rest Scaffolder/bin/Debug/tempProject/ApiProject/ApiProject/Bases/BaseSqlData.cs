using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ApiProject.Bases
{
    public class BaseSqlData
    <
        BaseSqlFilterParam,
        BaseSqlListFilterParam,
        BaseSqlPaginationFilterParam,
        BaseSqlAddParam,
        BaseSqlUpdateParam,
        BaseSpName
    > 
    : IDisposable
    where BaseSqlFilterParam : IBaseSqlSpParam
    where BaseSqlListFilterParam : class, IBaseSqlSpParam
    where BaseSqlPaginationFilterParam : class, IBaseSqlSpParam
    where BaseSqlAddParam : class, IBaseSqlSpParam
    where BaseSqlUpdateParam : class, IBaseSqlSpParam
    where BaseSpName : class, IBaseSpName
    {
        protected BaseSpName _spNames;

        public BaseSqlData(string entityName,BaseSpName spNames)
        {
            _spNames = spNames;
            _spNames.EntityName = entityName;
        }

        public virtual async Task<string> Count(BaseSqlFilterParam filter)
        {
            var result = await SqlService.GetDataResult(_spNames.Count + " " + filter.GetSpParams());
            return result;
        }

        public virtual async Task<string> ToList(BaseSqlListFilterParam filter)
        {
            var result = await SqlService.GetDataResult(_spNames.ToList + " " + filter.GetSpParams());
            return result;
        }

        public virtual async Task<string> PagedList(BaseSqlPaginationFilterParam filter)
        {
            var result = await SqlService.GetDataResult(_spNames.PagedList + " " + filter.GetSpParams());
            return result;
        }

        public virtual async Task<string> GetDeletedList(BaseSqlListFilterParam filter)
        {
            var result = await SqlService.GetDataResult(_spNames.GetDeletedList + " " + filter.GetSpParams());
            return result;
        }

        public virtual async Task<string> FirstOrDefault(BaseSqlFilterParam filter)
        {
            var result = await SqlService.GetDataResult(_spNames.FirstOrDefault + " " + filter.GetSpParams());
            return result;
        }

        public virtual async Task<string> FirstOrDefaultById<T>(T id)
        {
            var idString = id.ToString();
            Guid GuidType = Guid.Empty;
            if (Guid.TryParse(idString, out GuidType))
                idString = $"'{GuidType.ToString()}'";
            var result = await SqlService.GetDataResult(_spNames.FirstOrDefaultById + " " + idString);
            return result;
        }

        public virtual async Task<string> DeletedFirstOrDefault(BaseSqlFilterParam filter)
        {
            var result = await SqlService.GetDataResult(_spNames.DeletedFirstOrDefault + " " + filter.GetSpParams());
            return result;
        }

        public virtual async Task<string> AutoComplete(BaseSqlListFilterParam filter)
        {
            var result = await SqlService.GetDataResult(_spNames.AutoComplete + " " + filter.GetSpParams());
            return result;
        }

        public virtual async Task<string> Add(BaseSqlAddParam item)
        {
            var result = await SqlService.GetDataResult(_spNames.Add + " " + item.GetSpParams());
            return result;
        }

        public virtual async Task<string> Update(BaseSqlUpdateParam item)
        {
            var result = await SqlService.GetDataResult(_spNames.Update + " " + item.GetSpParams());
            return result;
        }

        public virtual async Task<string> Delete<T>(T id)
        {
            var idString = id.ToString();
            Guid GuidType = Guid.Empty;
            if (Guid.TryParse(idString, out GuidType))
                idString = $"'{GuidType.ToString()}'";
            var result = await SqlService.GetDataResult(_spNames.Delete + " " + idString);
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                //to do
            }
        }
    }
}