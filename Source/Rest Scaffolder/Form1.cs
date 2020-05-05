using Rest_Scaffolder.CodeGenerator;
using Rest_Scaffolder.EF;
using Rest_Scaffolder.SqlModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rest_Scaffolder
{
    public partial class Form1 : Form
    {
        private DataBase db;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog save = new FolderBrowserDialog();
            save.Description = "Select project path";
            DialogResult open = DialogResult.Cancel;
            open = save.ShowDialog();
            if (open == DialogResult.OK)
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.InitialCatalog = textBoxCatalogName.Text;
                builder.UserID = textBoxUserName.Text;
                builder.Password = textBoxPassword.Text;
                builder.DataSource = textBoxServerAddress.Text;
                db = new DataBase(builder.ConnectionString);

                var relationsQuery = System.IO.File.ReadAllText("sqlCommands\\relations.sql");
                var properties = (db.Database.SqlQuery(typeof(SqlNames), "select * from INFORMATION_SCHEMA.COLUMNS").ToListAsync().Result);
                var tabels = db.Database.SqlQuery(typeof(SqlTableNames), "select * from INFORMATION_SCHEMA.TABLES where TABLE_TYPE <> N'VIEW'").ToListAsync().Result;
                var keys = db.Database.SqlQuery(typeof(SqlKeys), "select * from INFORMATION_SCHEMA.KEY_COLUMN_USAGE").ToListAsync().Result;
                var relations = db.Database.SqlQuery(typeof(SqlRelationNames), relationsQuery).ToListAsync().Result;


                var relationList = (from a in relations select (SqlRelationNames)a).ToList();
                var keyList = (from a in keys
                               let refrecedTable = relationList.FirstOrDefault(r => r.FK_NAME == ((SqlKeys)a).CONSTRAINT_NAME)
                               let obj = (SqlKeys)a
                               select new SqlKeys()
                               {
                                   COLUMN_NAME = obj.COLUMN_NAME,
                                   CONSTRAINT_NAME = obj.CONSTRAINT_NAME,
                                   CONSTRAINT_SCHEMA = obj.CONSTRAINT_SCHEMA,
                                   ORDINAL_POSITION = obj.ORDINAL_POSITION,
                                   REFRENCED_TABLE = refrecedTable?.REFRENCED_TABLE_NAME,
                                   TABLE_CATALOG = obj.TABLE_CATALOG,
                                   TABLE_NAME = obj.TABLE_NAME,
                                   TABLE_SCHEMA = obj.TABLE_SCHEMA
                               }
                               ).ToList();

                var GeneralDataModel = new GeneralData()
                {
                    Relations = relationList,
                    Properties = (from a in properties select (SqlNames)a).ToList(),
                    Tabels = (from a in tabels select (SqlTableNames)a).ToList(),
                    Keys = keyList,
                };
                if (!System.IO.Directory.Exists(save.SelectedPath))
                {
                    System.IO.Directory.CreateDirectory(save.SelectedPath);
                }
                var time = DateTime.Now.Ticks;
                var path = save.SelectedPath + "\\" + time + "-" + textBoxCatalogName.Text;
                System.IO.Directory.CreateDirectory(path);
                DirectoryInfo dir = new DirectoryInfo(path);
                MessageBox.Show("It will take a few minutes \r\n Press Ok to start build REST project.", "Build Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GeneralDataFileCreator.DirectoryCopy("tempProject//ApiProject", path, true, textBoxCatalogName.Text);
                GeneralDataFileCreator.SaveProjectFiles(GeneralDataModel, path, textBoxCatalogName.Text);
                MessageBox.Show("Project build finished", "Build Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
