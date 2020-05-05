using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest_Scaffolder.CodeGenerator
{
    public class SqlRelationNames
    {
        public string FK_NAME { get; set; }
        public string TABLE_NAME { get; set; }
        public string PARENT_COL_NAME { get; set; }
        public int? PARENT_COL_ID { get; set; }
        public string REFRENCED_TABLE_NAME { get; set; }
        public string REFRENCED_COL_NAME { get; set; }
        public int? REFRENCED_COL_ID { get; set; }
    }

    public class SqlTableNames
    {
        public string TABLE_CATALOG { get; set; }
        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }
    }

    public class SqlKeys
    {
        public string TABLE_CATALOG { get; set; }
        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public int? ORDINAL_POSITION { get; set; }
        public string CONSTRAINT_NAME { get; set; }
        public string CONSTRAINT_SCHEMA { get; set; }
        public string REFRENCED_TABLE { get; set; }
        public KeyType KeyType
        {
            get
            {
                if (CONSTRAINT_NAME.StartsWith("FK"))
                    return KeyType.FK;
                else if (CONSTRAINT_NAME.StartsWith("UK"))
                    return KeyType.UK;
                else if (CONSTRAINT_NAME.StartsWith("PK"))
                    return KeyType.PK;
                else
                    return KeyType.None;
            }
        }
    }

    public class SqlNames
    {
        public string TABLE_CATALOG { get; set; }
        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public int? ORDINAL_POSITION { get; set; }
        public string IS_NULLABLE { get; set; }
        public string DATA_TYPE { get; set; }
        public int? CHARACTER_MAXIMUM_LENGTH { get; set; }
        public int? CHARACTER_OCTET_LENGTH { get; set; }
        public int? DATETIME_PRECISION { get; set; }
        public int? NUMERIC_PRECISION { get; set; }
        public int? NUMERIC_SCALE { get; set; }
    }

    public enum KeyType
    {
        FK = 0,
        UK = 1,
        PK = 2,
        None = 3
    }
}
