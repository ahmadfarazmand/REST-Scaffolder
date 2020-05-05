using Rest_Scaffolder.SqlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest_Scaffolder.CodeGenerator
{
    public class GeneralData
    {
        public List<SqlRelationNames> Relations { get; set; }

        public List<SqlTableNames> Tabels { get; set; }

        public List<SqlNames> Properties { get; set; }

        public List<SqlKeys> Keys { get; set; }

        public IEnumerable<EntityCode> EntityCodeList
        {
            get
            {
                foreach (var item in Tabels)
                {

                    var newItem = new EntityCode()
                    {
                        Keys = Keys.Where(a => a.TABLE_NAME == item.TABLE_NAME).ToList(),
                        Properties = Properties.Where(a => a.TABLE_NAME == item.TABLE_NAME).ToList(),
                        Relations = Relations.Where(a => a.TABLE_NAME == item.TABLE_NAME).ToList(),
                        TABLE_CATALOG = item.TABLE_CATALOG,
                        TABLE_NAME = item.TABLE_NAME,
                        TABLE_SCHEMA = item.TABLE_SCHEMA
                    };

                    yield return newItem;
                }
            }
        }
    }

    public class EntityCode
    {
        public List<SqlKeys> Keys { get; set; }

        public List<SqlNames> Properties { get; set; }

        public string PkName
        {
            get
            {
                return Keys.FirstOrDefault(a => a.KeyType == KeyType.PK)?.COLUMN_NAME;
            }
        }

        public string PKType
        {
            get
            {
                var pk = Properties.FirstOrDefault(a => a.COLUMN_NAME == PkName);
                return TableSqlCodeInfoes.DataTypeDeclare(pk, pk?.DATA_TYPE);
            }
        }

        public string PKSysType
        {
            get
            {
                var pk = Properties.FirstOrDefault(a => a.COLUMN_NAME == PkName);
                return TableSqlCodeInfoes.SystemType(pk?.DATA_TYPE);
            }
        }

        public List<SqlRelationNames> Relations { get; set; }

        public string TABLE_CATALOG { get; set; }

        public string TABLE_SCHEMA { get; set; }

        public string TABLE_NAME { get; set; }

        public string SqlConstString { get { return TABLE_NAME.ToLower(); } }

        public string SqlSimpleSelectNames { get { return TableSqlCodeInfoes.SelectNameSingleInfo(Properties, SqlConstString); } }

        public string RelationalJsonSelectQuery(string fk, string pk, string conditionConstValue)
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\relationJsonTemplate.txt");
            var result =
            readSelectTemplate.
            Replace("#RELATIONCONST#", (conditionConstValue + "_" + SqlConstString)).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#RELATIONCONSTANDKEY#", conditionConstValue).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#FK#", fk).
            Replace("#PK#", pk).
            Replace("#SELECTNAMES#", TableSqlCodeInfoes.SelectNameSingleInfo(Properties, (conditionConstValue + "_" + SqlConstString)));

            return result;
        }

        public string ManyRelationalJsonSelectQuery(string conditionConstValue, string fkCol, string pkCol, IEnumerable<EntityCode> EntityCodeList, IEnumerable<SqlRelationNames> AllKeys)
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\selectManyRelationJson.txt");
            var result =
            readSelectTemplate.
            Replace("#RELATIONCONST#", (conditionConstValue + "_" + SqlConstString)).
            Replace("#MANYCONST#", (conditionConstValue + "_" + SqlConstString)).
            Replace("#RELATIONSJSON#", "").
            Replace("#TABLE#", TABLE_NAME).
            Replace("#NAME#", TABLE_NAME).
            Replace("#RELATIONCONSTANDKEY#", conditionConstValue).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#SELECTNAMES#", TableSqlCodeInfoes.SelectNameSingleInfo(Properties, (conditionConstValue + "_" + SqlConstString))).
            Replace("#PKEYCOLUMN#", pkCol).
            Replace("#FKEYCOLUMN#", fkCol).
            Replace("#SELECTCONST#", conditionConstValue);

            return result;
        }

        public string ManyRelationalCountSelectQuery(string conditionConstValue, string fkCol, string pkCol, IEnumerable<EntityCode> EntityCodeList, IEnumerable<SqlRelationNames> AllKeys)
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\selectCountManyRelation.txt");
            var result =
            readSelectTemplate.
            Replace("#RELATIONCONST#", (conditionConstValue + "_" + SqlConstString)).
            Replace("#MANYCONST#", (conditionConstValue + "_" + SqlConstString)).
            Replace("#RELATIONSJSON#", "").
            Replace("#TABLE#", TABLE_NAME).
            Replace("#NAME#", TABLE_NAME).
            Replace("#RELATIONCONSTANDKEY#", conditionConstValue).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#SELECTNAMES#", TableSqlCodeInfoes.SelectNameSingleInfo(Properties, (conditionConstValue + "_" + SqlConstString))).
            Replace("#PKEYCOLUMN#", pkCol).
            Replace("#FKEYCOLUMN#", fkCol).
            Replace("#SELECTCONST#", conditionConstValue);

            return result;
        }

        public string SelectSingleData(string relations, string manyRelations)
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\selectOneSample.txt");
            var result =
            readSelectTemplate.
            Replace("#TABLECATALOG#", TABLE_CATALOG).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#PROCNAME#", TABLE_NAME + "_FirstOrDefault").
            Replace("#SPPARAMS#", SPParams).
            Replace("#SELECTNAMES#", SqlSimpleSelectNames).
            //Replace("#RELATIONSJSON#", RelationalJsonSelectQuery(SqlConstString)).
            Replace("#RELATIONSJSON#", relations).
            Replace("#MANYRELATIONSJSON#", manyRelations).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#SELECTMULTIPARAMS#", SPMultiConvertParams).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#CONST#", SqlConstString).
            Replace("#SELECTCONDITIONS#", SPConditions);

            return result;
        }

        public string SelectByIdData(string relations, string manyRelations)
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\byIdSample.txt");
            var result =
            readSelectTemplate.
            Replace("#TABLECATALOG#", TABLE_CATALOG).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#PKNAME#", PkName).
            Replace("#PKTYPE#", PKType).
            Replace("#PROCNAME#", TABLE_NAME + "_FirstOrDefaultById").
            Replace("#SPPARAMS#", SPParams).
            Replace("#SELECTNAMES#", SqlSimpleSelectNames).
            Replace("#RELATIONSJSON#", relations).
            Replace("#MANYRELATIONSJSON#", manyRelations).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#CONST#", SqlConstString);

            return result;
        }

        public string AddSingleData(string relations, string manyRelations)
        {
            var readSelectTemplate = "";
            if(PKSysType == "Guid?")
                readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\guidInsertSample.txt");
            else
                readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\insertSample.txt");

            var result =
            readSelectTemplate.
            Replace("#TABLECATALOG#", TABLE_CATALOG).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#PKNAME#", PkName).
            Replace("#PKTYPE#", PKType).
            Replace("#PROCNAME#", TABLE_NAME + "_AddNewData").
            Replace("#SPPARAMS#", SPInsertParams).
            Replace("#SELECTNAMES#", SqlSimpleSelectNames).
            Replace("#RELATIONSJSON#", relations).
            Replace("#MANYRELATIONSJSON#", manyRelations).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#SELECTMULTIPARAMS#", SPMultiConvertParams).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#CONST#", SqlConstString).
            Replace("#SPNAMES#", SPInsertParamsValueNames).
            Replace("#SPVALUE#", SPInsertParamsValues);

            return result;
        }

        public string UpdateSingleData(string relations, string manyRelations)
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\updateSample.txt");
            var result =
            readSelectTemplate.
            Replace("#TABLECATALOG#", TABLE_CATALOG).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#PKNAME#", PkName).
            Replace("#PKTYPE#", PKType).
            Replace("#PROCNAME#", TABLE_NAME + "_UpdateData").
            Replace("#SPPARAMS#", SPUpdateParams).
            Replace("#SELECTNAMES#", SqlSimpleSelectNames).
            Replace("#PKNAME#", PkName).
            Replace("#RELATIONSJSON#", relations).
            Replace("#MANYRELATIONSJSON#", manyRelations).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#SELECTMULTIPARAMS#", SPMultiConvertParams).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#CONST#", SqlConstString).
            Replace("#UPDATENAMES#", SPUpdateParamsValues);

            return result;
        }

        public string DeleteSingleData(string relations, string manyRelations)
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\deleteItem.txt");
            var result =
            readSelectTemplate.
            Replace("#TABLECATALOG#", TABLE_CATALOG).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#PKNAME#", PkName).
            Replace("#PKTYPE#", PKType).
            Replace("#PROCNAME#", TABLE_NAME + "_DeleteData").
            Replace("#SPPARAMS#", SPUpdateParams).
            Replace("#SELECTNAMES#", SqlSimpleSelectNames).
            Replace("#RELATIONSJSON#", relations).
            Replace("#MANYRELATIONSJSON#", manyRelations).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#SELECTMULTIPARAMS#", SPMultiConvertParams).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#CONST#", SqlConstString).
            Replace("#UPDATENAMES#", SPUpdateParamsValues);

            return result;
        }

        public string SelectAutoComplete()
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\selectAutoCompleteSample.txt");
            var fkList = Keys.Where(a => a.KeyType == KeyType.FK);
            var result =
            readSelectTemplate.
            Replace("#TABLECATALOG#", TABLE_CATALOG).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#PROCNAME#", TABLE_NAME + "_AutoComplete").
            Replace("#SPPARAMS#", SPParams).
            Replace("#SELECTNAMES#", SqlSimpleSelectNames).
            Replace("#SELECTMULTIPARAMS#", SPMultiConvertParams).
            Replace("#PKNAME#", PkName).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#CONST#", SqlConstString).
            Replace("#SELECTCONDITIONS#", SPConditions);

            return result;
        }

        public string IncludeFolders
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\includeProject.txt");
                result = readSelectTemplate.Replace("#NAME#", TABLE_NAME);

                return result;
            }
        }

        public string IncludeContentFolders
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\includeContentProjec.txt");
                result = readSelectTemplate.Replace("#NAME#", TABLE_NAME);

                return result;
            }
        }

        public string SelectCount()
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\countDataSample.txt");
            var result =
            readSelectTemplate.
            Replace("#TABLECATALOG#", TABLE_CATALOG).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#PKNAME#", PkName).
            Replace("#PROCNAME#", TABLE_NAME + "_Count").
            Replace("#SPPARAMS#", SPParams).
            Replace("#SELECTMULTIPARAMS#", SPMultiConvertParams).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#CONST#", SqlConstString).
            Replace("#SELECTCONDITIONS#", SPConditions);

            return result;
        }

        public string SelectListData(string relations, string manyRelations)
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\selectListSample.txt");
            var result =
            readSelectTemplate.
            Replace("#TABLECATALOG#", TABLE_CATALOG).
            Replace("#PKNAME#", PkName).
            Replace("#PROCNAME#", TABLE_NAME + "_ToList").
            Replace("#SPPARAMS#", SPParams).
            Replace("#SELECTNAMES#", SqlSimpleSelectNames).
            Replace("#SELECTMULTIPARAMS#", SPMultiConvertParams).
            Replace("#RELATIONSJSON#", relations).
            Replace("#MANYRELATIONSJSONCOUNT#", manyRelations).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#CONST#", SqlConstString).
            Replace("#SELECTCONDITIONS#", SPConditions);

            return result;
        }

        public string SelectPaginationData(string relations, string manyRelations)
        {
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\paginationSample.txt");
            var result =
            readSelectTemplate.
            Replace("#TABLECATALOG#", TABLE_CATALOG).
            Replace("#PKNAME#", PkName).
            Replace("#PROCNAME#", TABLE_NAME + "_PageList").
            Replace("#SPPARAMS#", SPParams).
            Replace("#SELECTNAMES#", SqlSimpleSelectNames).
            Replace("#SELECTMULTIPARAMS#", SPMultiConvertParams).
            Replace("#RELATIONSJSON#", relations).
            Replace("#MANYRELATIONSJSON#", manyRelations).
            Replace("#SCHEMA#", TABLE_SCHEMA).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#CONST#", SqlConstString).
            Replace("#SELECTCONDITIONS#", SPConditions);

            return result;
        }

        public string SPParams
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptFilter(a.DATA_TYPE)).OrderBy(a => a.ORDINAL_POSITION).ToList();
                for (int i = 0; i < filters.Count; i++)
                {
                    result += TableSqlCodeInfoes.ConditionFilters(filters[i], (i == (filters.Count - 1) ? false : true));
                }

                return result;
            }
        }

        public string SPInsertParams
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptAdd(a.DATA_TYPE) && a.COLUMN_NAME != PkName).OrderBy(a => a.ORDINAL_POSITION).ToList();
                for (int i = 0; i < filters.Count; i++)
                {
                    result += TableSqlCodeInfoes.AddFilters(filters[i], (i == (filters.Count - 1) ? false : true));
                }

                return result;
            }
        }

        public string SPUpdateParams
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptAdd(a.DATA_TYPE)).OrderBy(a => a.ORDINAL_POSITION).ToList();
                for (int i = 0; i < filters.Count; i++)
                {
                    result += TableSqlCodeInfoes.AddFilters(filters[i], (i == (filters.Count - 1) ? false : true));
                }

                return result;
            }
        }

        public string SPUpdateParamsValues
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptAdd(a.DATA_TYPE) && a.COLUMN_NAME != PkName).OrderBy(a => a.ORDINAL_POSITION).ToList();
                for (int i = 0; i < filters.Count; i++)
                {
                    result += "[" + filters[i].COLUMN_NAME + "] = @" + filters[i].COLUMN_NAME + (i == (filters.Count - 1) ? " \r\n \r\n" : ", \r\n");
                }

                return result;
            }
        }

        public string SPInsertParamsValues
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptAdd(a.DATA_TYPE) && a.COLUMN_NAME != PkName).OrderBy(a => a.ORDINAL_POSITION).ToList();
                for (int i = 0; i < filters.Count; i++)
                {
                    result += "@" + filters[i].COLUMN_NAME + (i == (filters.Count - 1) ? "" : ",");
                }

                return result;
            }
        }

        public string SPInsertParamsValueNames
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptAdd(a.DATA_TYPE) && a.COLUMN_NAME != PkName).OrderBy(a => a.ORDINAL_POSITION).ToList();
                for (int i = 0; i < filters.Count; i++)
                {
                    result += "[" + filters[i].COLUMN_NAME + "]" + (i == (filters.Count - 1) ? "" : ",");
                }

                return result;
            }
        }

        public string SPMultiConvertParams
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptFilter(a.DATA_TYPE)).OrderBy(a => a.ORDINAL_POSITION).ToList();
                var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\multiSelectTemplate.txt");
                for (int i = 0; i < filters.Count; i++)
                {
                    result +=
                    readSelectTemplate
                    .Replace("#COLNAME#", filters[i].COLUMN_NAME)
                    .Replace("#TYPE#", TableSqlCodeInfoes.DataTypeDeclare(filters[i], filters[i].DATA_TYPE))
                    ;
                }

                return result;
            }
        }

        public string SPConditions
        {
            get
            {
                var result = "-- || Conditions || -- \r\n";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptFilter(a.DATA_TYPE)).OrderBy(a => a.ORDINAL_POSITION).ToList();
                for (int i = 0; i < filters.Count; i++)
                {
                    result += TableSqlCodeInfoes.ConditionStatement(filters[i], (i == (filters.Count - 1) ? false : true), SqlConstString) + "\r\n";
                }

                return result;
            }
        }

        public string SqlClass
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\sqlDataSelect.txt");

                result = readSelectTemplate.
                        Replace("#PROJECTNAME#", TABLE_CATALOG).
                        Replace("#NAME#", TABLE_NAME).
                        Replace("#SCHEMA#", TABLE_SCHEMA);

                return result;
            }
        }

        public string SqlController
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\controllerClass.txt");
                result = readSelectTemplate.
                        Replace("#PROJECTNAME#", TABLE_CATALOG).
                        Replace("#PKSYSTYPE#", PKSysType).
                        Replace("#NAME#", TABLE_NAME);

                return result;
            }
        }

        public string SqlPermissions
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\controllerClass.txt");
                var readSelectTemplateUnit = System.IO.File.ReadAllText("sharpTemplates\\controllerClass.txt");
                var currentUser = Properties.Any(a => a.COLUMN_NAME.ToLower() == "submiteruserid");
                result = readSelectTemplate.
                        Replace("#PROJECTNAME#", TABLE_CATALOG).
                        Replace("#SUBMITER#", currentUser ? "item.SubmiterUserId = currentUser.Id;" : "").
                        Replace("#NAME#", TABLE_NAME);

                return result;
            }
        }

        public string AddClass
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\addClassModel.txt");

                result = readSelectTemplate.
                        Replace("#PROJECTNAME#", TABLE_CATALOG).
                        Replace("#NAME#", TABLE_NAME + "Add").
                        Replace("#PROPERTY#", AddModelName).
                        Replace("#PROPERTYCONVERT#", AddConvertModelName);

                return result;
            }
        }

        public string UpdateClass
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\updateClassModel.txt");

                result = readSelectTemplate.
                        Replace("#PROJECTNAME#", TABLE_CATALOG).
                        Replace("#NAME#", TABLE_NAME + "Update").
                        Replace("#PROPERTY#", UpdateModelName).
                        Replace("#PROPERTYCONVERT#", UpdateConvertModelName);

                return result;
            }
        }

        public string PaginationFilterClass
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\paginationClassModel.txt");

                result = readSelectTemplate.
                        Replace("#PROJECTNAME#", TABLE_CATALOG).
                        Replace("#NAME#", TABLE_NAME + "PaginationFilter").
                        Replace("#PROPERTY#", FilterModelName).
                        Replace("#PROPERTYCONVERT#", FilterConvertModelName);

                return result;
            }
        }

        public string ListFilterClass
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\listClassModel.txt");

                result = readSelectTemplate.
                        Replace("#PROJECTNAME#", TABLE_CATALOG).
                        Replace("#NAME#", TABLE_NAME + "ListFilter").
                        Replace("#PROPERTY#", FilterModelName).
                        Replace("#PROPERTYCONVERT#", FilterConvertModelName);

                return result;
            }
        }

        public string FilterClass
        {
            get
            {
                var result = "";
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\filtercalssModel.txt");

                result = readSelectTemplate.
                        Replace("#PROJECTNAME#", TABLE_CATALOG).
                        Replace("#NAME#", TABLE_NAME + "Filter").
                        Replace("#PROPERTY#", FilterModelName).
                        Replace("#PROPERTYCONVERT#", FilterConvertModelName);

                return result;
            }
        }

        public string UpdateConvertModelName
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptAdd(a.DATA_TYPE) && a.COLUMN_NAME != "SubmitDate").OrderBy(a => a.ORDINAL_POSITION).ToList();
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\unitCovert.txt");
                for (int i = 0; i < filters.Count; i++)
                {
                    result +=
                        readSelectTemplate.
                        Replace("#NAME#", filters[i].COLUMN_NAME).
                        Replace("#COMA#", (i == (filters.Count - 1) ? "" : ", "));
                }

                return result;
            }
        }

        public string AddConvertModelName
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptAdd(a.DATA_TYPE) && a.COLUMN_NAME != "Id").OrderBy(a => a.ORDINAL_POSITION).ToList();
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\unitCovert.txt");
                for (int i = 0; i < filters.Count; i++)
                {
                    result +=
                        readSelectTemplate.
                        Replace("#NAME#", filters[i].COLUMN_NAME).
                        Replace("#COMA#", (i == (filters.Count - 1) ? "" : ", "));
                }

                return result;
            }
        }

        public string FilterConvertModelName
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptFilter(a.DATA_TYPE)).OrderBy(a => a.ORDINAL_POSITION).ToList();
                var readSelectTemplateMinMax = System.IO.File.ReadAllText("sharpTemplates\\minMaxConvertFilter.txt");
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\simpleConvertFilter.txt");
                for (int i = 0; i < filters.Count; i++)
                {
                    if (TableSqlCodeInfoes.MinMaxDataTypes(filters[i].DATA_TYPE))
                    {
                        result +=
                        readSelectTemplateMinMax.
                        Replace("#NAME#", filters[i].COLUMN_NAME).
                        Replace("#COMA#", (i == (filters.Count - 1) ? "" : ", "));
                    }
                    else
                    {
                        result +=
                        readSelectTemplate.
                        Replace("#NAME#", filters[i].COLUMN_NAME).
                        Replace("#COMA#", (i == (filters.Count - 1) ? "" : ", "));
                    }
                }

                return result;
            }
        }

        public string UpdateModelName
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptAdd(a.DATA_TYPE) && a.COLUMN_NAME != "SubmitDate").OrderBy(a => a.ORDINAL_POSITION).ToList();
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\unitProperty.txt");
                for (int i = 0; i < filters.Count; i++)
                {
                    result +=
                        readSelectTemplate.
                        Replace("#NAME#", filters[i].COLUMN_NAME).
                        Replace("#LENGTH#", (filters[i].CHARACTER_MAXIMUM_LENGTH.HasValue ? filters[i].CHARACTER_MAXIMUM_LENGTH.Value.ToString() : "0")).
                        Replace("#ORDER#", filters[i].ORDINAL_POSITION.Value.ToString()).
                        Replace("#TYPE#", TableSqlCodeInfoes.SystemType(filters[i].DATA_TYPE));
                }

                return result;
            }
        }

        public string AddModelName
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptAdd(a.DATA_TYPE) && a.COLUMN_NAME != "Id").OrderBy(a => a.ORDINAL_POSITION).ToList();
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\unitProperty.txt");
                for (int i = 0; i < filters.Count; i++)
                {
                    result +=
                        readSelectTemplate.
                        Replace("#NAME#", filters[i].COLUMN_NAME).
                        Replace("#LENGTH#", (filters[i].CHARACTER_MAXIMUM_LENGTH.HasValue ? filters[i].CHARACTER_MAXIMUM_LENGTH.Value.ToString() : "0")).
                        Replace("#ORDER#", filters[i].ORDINAL_POSITION.Value.ToString()).
                        Replace("#TYPE#", TableSqlCodeInfoes.SystemType(filters[i].DATA_TYPE));
                }

                return result;
            }
        }

        public string FilterModelName
        {
            get
            {
                var result = "";
                var filters = Properties.Where(a => TableSqlCodeInfoes.AcceptFilter(a.DATA_TYPE)).OrderBy(a => a.ORDINAL_POSITION).ToList();
                var readSelectTemplateMinMax = System.IO.File.ReadAllText("sharpTemplates\\minMaxProperty.txt");
                var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\simpleProperty.txt");
                for (int i = 0; i < filters.Count; i++)
                {
                    if (TableSqlCodeInfoes.MinMaxDataTypes(filters[i].DATA_TYPE))
                    {
                        result +=
                        readSelectTemplateMinMax.
                        Replace("#NAME#", filters[i].COLUMN_NAME).
                        Replace("#TYPE#", TableSqlCodeInfoes.SystemType(filters[i].DATA_TYPE));
                    }
                    else
                    {
                        result +=
                        readSelectTemplate.
                        Replace("#NAME#", filters[i].COLUMN_NAME).
                        Replace("#TYPE#", TableSqlCodeInfoes.SystemType(filters[i].DATA_TYPE));
                    }
                }

                return result;
            }
        }
    }

    public static class TableSqlCodeInfoes
    {
        public static string Enter = "\r\n";

        public static string ConditionStatement(SqlNames Property, bool enableComa, string constName)
        {
            var result = "-- || Auto Generated Conditions || --" + Enter;
            var readSelectTemplateMinMax = System.IO.File.ReadAllText("sqlTemplates\\minMaxConditionsTemplate.txt");
            var readSelectTemplateSimple = System.IO.File.ReadAllText("sqlTemplates\\simpleConditionsTemplate.txt");
            var readSelectTemplateChar = System.IO.File.ReadAllText("sqlTemplates\\charConditionTemplate.txt");
            if (MinMaxDataTypes(Property.DATA_TYPE))
            {
                result +=
                readSelectTemplateMinMax.
                Replace("#COLNAME#", Property.COLUMN_NAME).
                Replace("#CONST#", constName).
                Replace("#AND#", (enableComa ? "And" : ""))
                + Enter + Enter;
            }
            else if (!CharType(Property.DATA_TYPE))
            {
                result +=
                readSelectTemplateSimple.
                Replace("#COLNAME#", Property.COLUMN_NAME).
                Replace("#CONST#", constName).
                Replace("#AND#", (enableComa ? "And" : ""))
                + Enter + Enter;
            }
            else
            {
                result +=
                readSelectTemplateChar.
                Replace("#COLNAME#", Property.COLUMN_NAME).
                Replace("#CONST#", constName).
                Replace("#AND#", (enableComa ? "And" : ""))
                + Enter + Enter;
            }
            return result;
        }

        public static string AddFilters(SqlNames Property, bool enableComa)
        {
            var result = "";
            result += "@" + Property.COLUMN_NAME + " " + DataTypeDeclare(Property, Property.DATA_TYPE) + (enableComa ? " = NULL ," : " = NULL") + Enter;

            return result;
        }

        public static string ConditionFilters(SqlNames Property, bool enableComa)
        {
            var result = "";
            if (MinMaxDataTypes(Property.DATA_TYPE))
            {

                result += Enter + "@" + Property.COLUMN_NAME + " " + DataTypeDeclare(Property, Property.DATA_TYPE) + " = NULL ," + Enter;
                result += "@MultiText_" + Property.COLUMN_NAME + " nvarchar(MAX) = '' ," + Enter;
                result += "@Min" + Property.COLUMN_NAME + " " + DataTypeDeclare(Property, Property.DATA_TYPE) + " = NULL ," + Enter;
                result += "@Max" + Property.COLUMN_NAME + " " + DataTypeDeclare(Property, Property.DATA_TYPE) + (enableComa ? " = NULL ," : " = NULL") + Enter + Enter;
            }
            else
            {

                result += Enter + "@" + Property.COLUMN_NAME + " " + DataTypeDeclare(Property, Property.DATA_TYPE) + " = NULL ," + Enter;
                result += "@MultiText_" + Property.COLUMN_NAME + " nvarchar(MAX) = '' " + (enableComa ? "," : "") + Enter + Enter;
            }

            return result;
        }

        public static string CreateSample(SqlNames Property, bool enableComa)
        {
            var result = "";

            result += "[" + Property.COLUMN_NAME + "] " + DataTypeDeclare(Property, Property.DATA_TYPE) + " " + (enableComa ? "," : "") + Enter;

            return result;
        }

        public static string CRUDParams(SqlNames Property, bool enableComa)
        {
            var result = "";
            result += "@" + Property.COLUMN_NAME + " " + DataTypeDeclare(Property, Property.DATA_TYPE) + (enableComa ? " ," : "") + Enter;

            return result;
        }

        public static string SqlUpdateNames(List<SqlNames> Properties)
        {
            var result = "";
            var ordered = Properties.OrderBy(a => a.ORDINAL_POSITION).Where(a => a.COLUMN_NAME != "Id" && a.COLUMN_NAME != "SubmitDate").ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                result += "Set [" + ordered[i].COLUMN_NAME + "] = @" + ordered[i].COLUMN_NAME + (i == (ordered.Count - 1) ? "" : " ,") + Enter;
            }

            return result;
        }

        public static string SelectNameSingleInfo(List<SqlNames> Properties, string selectConstName)
        {
            var result = "";
            var ordered = Properties.Where(a=> AcceptJson(a.DATA_TYPE)).OrderBy(a => a.ORDINAL_POSITION).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                result += "[" + selectConstName + "].[" + ordered[i].COLUMN_NAME + "]" + (i == (ordered.Count - 1) ? "" : " ,") + Enter;
            }

            return result;
        }

        public static string CreateSimpleSelect(List<SqlNames> Properties, string TABLE_NAME, string SCHEMA)
        {
            var tabels = Properties.DistinctBy(a => a.TABLE_NAME);
            var readSelectTemplate = System.IO.File.ReadAllText("sqlTemplates\\simpleSelect.txt");
            var selectConstName = TABLE_NAME.ToLower();
            var infoNames = SelectNameSingleInfo(Properties, selectConstName);
            var result =
            readSelectTemplate.
            Replace("#CONSTNAME#", selectConstName).
            Replace("#TABLE#", TABLE_NAME).
            Replace("#SCHEMA#", SCHEMA).
            Replace("#SELECTNAMES#", infoNames);
            return result;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var known = new HashSet<TKey>();
            return source.Where(element => known.Add(keySelector(element)));
        }

        public static string DataTypeDeclare(SqlNames Property, string type)
        {
            var result = type;
            switch (type)
            {
                case "datetime2":
                    {
                        result += "(" + Property.DATETIME_PRECISION + ")";
                        return result;
                    }
                case "datetimeoffset":
                    {
                        result += "(" + Property.DATETIME_PRECISION + ")";
                        return result;
                    }
                case "decimal":
                    {
                        //result += "(" + Property.NUMERIC_PRECISION + "," + Property.NUMERIC_SCALE + ")";
                        return result;
                    }
                case "nchar":
                    {
                        result += "(" + Property.CHARACTER_MAXIMUM_LENGTH + ")";
                        return result;
                    }
                case "numeric":
                    {
                        //result += "(" + Property.NUMERIC_PRECISION + "," + Property.NUMERIC_SCALE + ")";
                        return result;
                    }
                case "nvarchar":
                    {
                        result += "(" + (Property.CHARACTER_MAXIMUM_LENGTH == -1 ? "MAX" : Property.CHARACTER_MAXIMUM_LENGTH.ToString()) + ")";
                        return result;
                    }
                case "time":
                    {
                        result += "(" + Property.DATETIME_PRECISION + ")";
                        return result;
                    }
                case "varbinary":
                    {
                        result += "(" + (Property.CHARACTER_MAXIMUM_LENGTH == -1 ? "MAX" : Property.CHARACTER_MAXIMUM_LENGTH.ToString()) + ")";
                        return result;
                    }
                case "varchar":
                    {
                        result += "(" + (Property.CHARACTER_MAXIMUM_LENGTH == -1 ? "MAX" : Property.CHARACTER_MAXIMUM_LENGTH.ToString()) + ")";
                        return result;
                    }
                case "binary":
                    {
                        result += "(" + Property.CHARACTER_OCTET_LENGTH + ")";
                        return result;
                    }
                case "char":
                    {
                        result += "(" + Property.CHARACTER_OCTET_LENGTH + ")";
                        return result;
                    }
                default:
                    return result;
            }
        }

        public static bool MinMaxDataTypes(string type)
        {
            switch (type)
            {
                case "int":
                    return true;
                case "float":
                    return true;
                case "decimal":
                    return true;
                case "datetimeoffset":
                    return true;
                case "datetime":
                    return true;
                case "bigint":
                    return true;
                case "date":
                    return true;
                case "datetime2":
                    return true;
                case "money":
                    return true;
                case "numeric":
                    return true;
                case "real":
                    return true;
                case "smalldatetime":
                    return true;
                case "smallint":
                    return true;
                case "smallmoney":
                    return true;
                case "time":
                    return true;
                case "timestamp":
                    return true;
                case "tinyint":
                    return true;
                default:
                    return false;
            }
        }

        public static bool CharType(string type)
        {
            switch (type)
            {
                case "char":
                    return true;
                case "nchar":
                    return true;
                case "ntext":
                    return true;
                case "nvarchar":
                    return true;
                case "text":
                    return true;
                case "varchar":
                    return true;
                case "xml":
                    return true;
                default:
                    return false;
            }
        }

        public static bool AcceptJson(string type)
        {
            switch (type)
            {
                case "bigint":
                    return true;
                case "binary":
                    return false;
                case "bit":
                    return true;
                case "char":
                    return true;
                case "date":
                    return true;
                case "datetime":
                    return true;
                case "datetime2":
                    return true;
                case "datetimeoffset":
                    return true;
                case "decimal":
                    return true;
                case "float":
                    return true;
                case "geography":
                    return false;
                case "geometry":
                    return false;
                case "hierarchyid":
                    return false;
                case "image":
                    return false;
                case "int":
                    return true;
                case "money":
                    return true;
                case "nchar":
                    return true;
                case "ntext":
                    return true;
                case "numeric":
                    return true;
                case "nvarchar":
                    return true;
                case "real":
                    return true;
                case "smalldatetime":
                    return true;
                case "smallint":
                    return true;
                case "smallmoney":
                    return true;
                case "sql_variant":
                    return false;
                case "text":
                    return true;
                case "time":
                    return true;
                case "timestamp":
                    return true;
                case "tinyint":
                    return true;
                case "uniqueidentifier":
                    return true;
                case "varbinary":
                    return false;
                case "varchar":
                    return true;
                case "xml":
                    return false;
                default:
                    return false;
            }
        }

        public static bool AcceptFilter(string type)
        {
            switch (type)
            {
                case "bigint":
                    return true;
                case "binary":
                    return false;
                case "bit":
                    return true;
                case "char":
                    return true;
                case "date":
                    return true;
                case "datetime":
                    return true;
                case "datetime2":
                    return true;
                case "datetimeoffset":
                    return true;
                case "decimal":
                    return true;
                case "float":
                    return true;
                case "geography":
                    return false;
                case "geometry":
                    return false;
                case "hierarchyid":
                    return false;
                case "image":
                    return false;
                case "int":
                    return true;
                case "money":
                    return true;
                case "nchar":
                    return true;
                case "ntext":
                    return true;
                case "numeric":
                    return true;
                case "nvarchar":
                    return true;
                case "real":
                    return true;
                case "smalldatetime":
                    return true;
                case "smallint":
                    return true;
                case "smallmoney":
                    return true;
                case "sql_variant":
                    return false;
                case "text":
                    return true;
                case "time":
                    return true;
                case "timestamp":
                    return false;
                case "tinyint":
                    return true;
                case "uniqueidentifier":
                    return true;
                case "varbinary":
                    return false;
                case "varchar":
                    return true;
                case "xml":
                    return false;
                default:
                    return false;
            }
        }

        public static bool AcceptAdd(string type)
        {
            switch (type)
            {
                case "bigint":
                    return true;
                case "binary":
                    return false;
                case "bit":
                    return true;
                case "char":
                    return true;
                case "date":
                    return true;
                case "datetime":
                    return true;
                case "datetime2":
                    return true;
                case "datetimeoffset":
                    return true;
                case "decimal":
                    return true;
                case "float":
                    return true;
                case "geography":
                    return false;
                case "geometry":
                    return false;
                case "hierarchyid":
                    return false;
                case "image":
                    return false;
                case "int":
                    return true;
                case "money":
                    return true;
                case "nchar":
                    return true;
                case "ntext":
                    return true;
                case "numeric":
                    return true;
                case "nvarchar":
                    return true;
                case "real":
                    return true;
                case "smalldatetime":
                    return true;
                case "smallint":
                    return true;
                case "smallmoney":
                    return true;
                case "sql_variant":
                    return false;
                case "text":
                    return true;
                case "time":
                    return true;
                case "timestamp":
                    return false;
                case "tinyint":
                    return true;
                case "uniqueidentifier":
                    return true;
                case "varbinary":
                    return false;
                case "varchar":
                    return true;
                case "xml":
                    return false;
                default:
                    return false;
            }
        }

        public static string SystemType(string type)
        {
            switch (type)
            {
                case "bigint":
                    return "long?";
                case "binary":
                    return "byte[]";
                case "bit":
                    return "bool?";
                case "char":
                    return "string";
                case "date":
                    return "DateTime?";
                case "datetime":
                    return "DateTime?";
                case "datetime2":
                    return "DateTime?";
                case "datetimeoffset":
                    return "DateTimeOffset?";
                case "decimal":
                    return "decimal?";
                case "float":
                    return "double?";
                case "geography":
                    return "DbGeography";
                case "geometry":
                    return "DbGeometry";
                case "hierarchyid":
                    return "object";
                case "image":
                    return "byte[]";
                case "int":
                    return "int?";
                case "money":
                    return "decimal?";
                case "nchar":
                    return "string";
                case "ntext":
                    return "string";
                case "numeric":
                    return "decimal?";
                case "nvarchar":
                    return "string";
                case "real":
                    return "float?";
                case "smalldatetime":
                    return "DateTime?";
                case "smallint":
                    return "short?";
                case "smallmoney":
                    return "decimal?";
                case "sql_variant":
                    return "object";
                case "text":
                    return "string";
                case "time":
                    return "TimeSpan?";
                case "timestamp":
                    return "byte[]";
                case "tinyint":
                    return "byte?";
                case "uniqueidentifier":
                    return "Guid?";
                case "varbinary":
                    return "byte[]";
                case "varchar":
                    return "string";
                case "xml":
                    return "string";
                default:
                    return "object";
            }
        }

    }

}
