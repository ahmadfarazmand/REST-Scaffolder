using Rest_Scaffolder.SqlModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest_Scaffolder.CodeGenerator
{
    public static class GeneralDataFileCreator
    {
        public static void SaveProjectFiles(GeneralData GeneralDataModel, string pathOfProject,string projectName)
        {
            var entity = GeneralDataModel.EntityCodeList;
            var folderPath = pathOfProject + "\\" + projectName + "\\";
            var controllerFolder = folderPath + "Controllers\\";
            var allModelsFolder = folderPath + "Models\\MetaData\\";
            var allModel = folderPath + "Models\\";
            var allSqlFolder = folderPath + "App_Data\\";
            var sqlDataClassFolder = folderPath + "SqlData\\";
            var allSp = "";
            var allInclude = "";
            var allContentInclude = "";
            var availableTables = GeneralDataModel.EntityCodeList.Where(a => a.TABLE_NAME.ToLower() != "sysdiagrams" && (a.PkName != null && a.PkName != "") && a.Properties.Count >= 2);
            foreach (var item in availableTables)
            {
                allInclude += item.IncludeFolders;
                allContentInclude += item.IncludeContentFolders;

                var getKeys = item.Keys.Where(a => a.KeyType == KeyType.FK).ToList();
                string relationText = ",";
                for (int i = 0; i < getKeys.Count && item.Relations.Count > 0; i++)
                {
                    var ef = entity.FirstOrDefault(a => a.TABLE_NAME == getKeys[i].REFRENCED_TABLE);
                    relationText += ef.RelationalJsonSelectQuery(getKeys[i].COLUMN_NAME, ef.PkName, item.SqlConstString) + (i == (getKeys.Count - 1) ? "" : " ,\r\n -- End Relation \r\n");
                }
                if (relationText.Length < 2 && relationText.EndsWith(","))
                    relationText = "";
                var man = GeneralDataModel.EntityCodeList.SelectMany(a => a.Keys).Where(a => a.KeyType == KeyType.FK && a.REFRENCED_TABLE == item.TABLE_NAME).ToList();

                string manyRelations = relationText.EndsWith(",") ? "" : ",";
                for (int i = 0; i < man.Count; i++)
                {
                    var fk = man[i].COLUMN_NAME;
                    var pk = item.PkName;
                    var ef = entity.FirstOrDefault(a => a.TABLE_NAME == man[i].TABLE_NAME);
                    manyRelations += ef.ManyRelationalJsonSelectQuery(item.SqlConstString, fk, pk, entity, GeneralDataModel.Relations) + (i == (man.Count - 1) ? "" : " ,\r\n -- End Relation \r\n");
                }

                string manyRelationsCount = relationText.EndsWith(",") ? "" : ",";
                for (int i = 0; i < man.Count; i++)
                {
                    var fk = man[i].COLUMN_NAME;
                    var pk = item.PkName;
                    var ef = entity.FirstOrDefault(a => a.TABLE_NAME == man[i].TABLE_NAME);
                    manyRelationsCount += ef.ManyRelationalCountSelectQuery(item.SqlConstString, fk, pk, entity, GeneralDataModel.Relations) + (i == (man.Count - 1) ? "" : " ,\r\n -- End Relation \r\n");
                }

                if ((manyRelations.Length < 2 || manyRelationsCount.Length < 2) && relationText.EndsWith(","))
                    relationText = relationText.Length > 0 ? relationText.Substring(0, relationText.Length - 1) : "";


                string SelectSingleData = item.SelectSingleData(relationText == "," ? "" : relationText, manyRelations == "," ? "" : manyRelations);
                string SelectByIdData = item.SelectByIdData(relationText == "," ? "" : relationText, manyRelations == "," ? "" : manyRelations);
                string SelectListData = item.SelectListData(relationText == "," ? "" : relationText, manyRelationsCount == "," ? "" : manyRelationsCount);
                string SelectPaginationData = item.SelectPaginationData(relationText == "," ? "" : relationText, manyRelations == "," ? "" : manyRelations);
                string AddSingleData = item.AddSingleData(relationText == "," ? "" : relationText, manyRelations == "," ? "" : manyRelations);
                string UpdateSingleData = item.UpdateSingleData(relationText == "," ? "" : relationText, manyRelations == "," ? "" : manyRelations);
                string DeleteSingleData = item.DeleteSingleData(relationText == "," ? "" : relationText, manyRelations == "," ? "" : manyRelations);
                string SelectAutoComplete = item.SelectAutoComplete();
                string SelectCount = item.SelectCount();


                allSp += SelectSingleData;
                allSp += SelectByIdData;
                allSp += SelectListData;
                allSp += SelectPaginationData;
                allSp += AddSingleData;
                allSp += UpdateSingleData;
                allSp += DeleteSingleData;
                allSp += SelectAutoComplete;
                allSp += SelectCount;

                var tableFolder = allSqlFolder + item.TABLE_NAME + "\\";
                System.IO.Directory.CreateDirectory(tableFolder);

                System.IO.File.WriteAllText(tableFolder + "\\FirstOrDefaultJson.sql", SelectSingleData);
                System.IO.File.WriteAllText(tableFolder + "\\ByIdJson.sql", SelectByIdData);
                System.IO.File.WriteAllText(tableFolder + "\\ListJson.sql", SelectListData);
                System.IO.File.WriteAllText(tableFolder + "\\PaginationListJson.sql", SelectPaginationData);
                System.IO.File.WriteAllText(tableFolder + "\\InsertReturnJson.sql", AddSingleData);
                System.IO.File.WriteAllText(tableFolder + "\\UpdateReturnJson.sql", UpdateSingleData);
                System.IO.File.WriteAllText(tableFolder + "\\Delete.sql", DeleteSingleData);
                System.IO.File.WriteAllText(tableFolder + "\\AutoCompleteJson.sql", SelectAutoComplete);
                System.IO.File.WriteAllText(tableFolder + "\\CountJson.sql", SelectCount);

                var modelFolder = allModelsFolder + item.TABLE_NAME + "Models\\";
                System.IO.Directory.CreateDirectory(modelFolder);

                System.IO.File.WriteAllText(modelFolder + "\\" + item.TABLE_NAME + "FilterModels.cs", item.FilterClass);
                System.IO.File.WriteAllText(modelFolder + "\\" + item.TABLE_NAME + "ListFilterModels.cs", item.ListFilterClass);
                System.IO.File.WriteAllText(modelFolder + "\\" + item.TABLE_NAME + "PaginationFilterModels.cs", item.PaginationFilterClass);
                System.IO.File.WriteAllText(modelFolder + "\\" + item.TABLE_NAME + "AddModels.cs", item.AddClass);
                System.IO.File.WriteAllText(modelFolder + "\\" + item.TABLE_NAME + "UpdateModels.cs", item.UpdateClass);

                System.IO.File.WriteAllText(controllerFolder + item.TABLE_NAME + "Controller.cs", item.SqlController);

                var sqlDataFolder = sqlDataClassFolder + item.TABLE_NAME + "Sql\\";
                System.IO.Directory.CreateDirectory(sqlDataFolder);
                System.IO.File.WriteAllText(sqlDataFolder + "\\" + item.TABLE_NAME + "SqlData.cs", item.SqlClass);
            }


            System.IO.File.WriteAllText(allSqlFolder + "\\All Sp Data.sql", allSp);
            var perFile = SqlPermissions(availableTables);

            System.IO.File.WriteAllText(folderPath + "Auth\\RolePermission.cs", perFile);

            System.IO.File.WriteAllText(sqlDataClassFolder + "\\SqlDataManager.cs", SqlManager(availableTables));

            System.IO.File.WriteAllText(allModel + "\\EntityNames.cs", EntityNames(availableTables));

            var readSelectTemplateDataBase = System.IO.File.ReadAllText("sqlTemplates\\dataBase.txt");

            string dataBaseText = readSelectTemplateDataBase
                .Replace("#CATALOG#", projectName)
                .Replace("#JSSP#", allSp);

            System.IO.File.WriteAllText(allSqlFolder + "\\DataBase.sql", dataBaseText);

            var readProjectFile = System.IO.File.ReadAllText(pathOfProject + "\\" + projectName + "\\" + projectName + ".csproj");
            var csWithIncludes = readProjectFile.Replace("#INCLUDE#", allInclude).Replace("#SQLINCLUDE#", allContentInclude);

            System.IO.File.WriteAllText(pathOfProject + "\\" + projectName + "\\" + projectName + ".csproj", csWithIncludes);
        }

        private static string SqlPermissions(IEnumerable<EntityCode> list)
        {
            var result = "";
            var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\permissionList.txt");
            var readSelectTemplateUnit = System.IO.File.ReadAllText("sharpTemplates\\permissionUnit.txt");
            var unitResult = "";
            foreach (var item in list)
            {
                unitResult += readSelectTemplateUnit.
                    Replace("#NAME#", item.TABLE_NAME);
            }

            result += readSelectTemplate.
                Replace("#PROJECTNAME#", list.First().TABLE_CATALOG).
                Replace("#LIST#", unitResult);


            return result;
        }

        private static string SqlManager(IEnumerable<EntityCode> list)
        {
            var result = "";
            var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\sqlManagerClass.txt");
            var readSelectTemplateUnit = System.IO.File.ReadAllText("sharpTemplates\\sqlManagerUnit.txt");
            var unitResult = "";
            foreach (var item in list)
            {
                unitResult += readSelectTemplateUnit.
                    Replace("#NAME#", item.TABLE_NAME);
            }

            result += readSelectTemplate.
                Replace("#PROJECTNAME#", list.First().TABLE_CATALOG).
                Replace("#PROPERTY#", unitResult);


            return result;
        }

        private static string EntityNames(IEnumerable<EntityCode> list)
        {
            var result = "";
            var readSelectTemplate = System.IO.File.ReadAllText("sharpTemplates\\entityNames.txt");

            var unitResult = "";
            foreach (var item in list)
            {
                unitResult += "            public static string " + item.TABLE_NAME + " { get { return \"" + item.TABLE_NAME + "\"; } } \r\n";
            }

            result += readSelectTemplate.
                Replace("#PROJECTNAME#", list.First().TABLE_CATALOG).
                Replace("#PROPERTY#", unitResult);


            return result;
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs,string projectName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: \r\n"
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                var dirPath = destDirName.Replace("ApiProject", projectName);
                Directory.CreateDirectory(dirPath);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                var read = File.ReadAllText(file.FullName);
                read = read.Replace("ApiProject", projectName);

                string temppath = Path.Combine(destDirName, file.Name);
                temppath = temppath.Replace("ApiProject", projectName);
                File.WriteAllText(temppath, read);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    //temppath = temppath.Replace("ApiProject", projectName);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, projectName);
                }
            }
        }

    }
}
