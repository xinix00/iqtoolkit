using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;

namespace IQToolkit.Data.SQLite
{
    using IQToolkit.Data.Common;

    public class SQLiteTypeSystem : DbTypeSystem
    {
        public static SqliteType GetSqliteType(DbType type)
        {
            switch(type)
            {
                default:
                    throw new Exception("Unknown type!");

                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Xml:
                    return SqliteType.Text;

                case DbType.Binary:
                case DbType.Object:
                    return SqliteType.Blob;

                case DbType.Boolean:
                case DbType.Byte:
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Guid:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.SByte:
                case DbType.Single:
                case DbType.Time:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.VarNumeric:
                    return SqliteType.Integer;

                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                    return SqliteType.Real;
            }

        }

        public override SqlDbType GetSqlType(string typeName)
        {
            if (string.Compare(typeName, "TEXT", true) == 0 ||
                string.Compare(typeName, "CHAR", true) == 0 ||
                string.Compare(typeName, "CLOB", true) == 0 ||
                string.Compare(typeName, "VARYINGCHARACTER", true) == 0 ||
                string.Compare(typeName, "NATIONALVARYINGCHARACTER", true) == 0)
            {
                return SqlDbType.VarChar;
            }
            else if (string.Compare(typeName, "INT", true) == 0 ||
                string.Compare(typeName, "INTEGER", true) == 0)
            {
                return SqlDbType.BigInt;
            }
            else if (string.Compare(typeName, "BLOB", true) == 0)
            {
                return SqlDbType.Binary;
            }
            else if (string.Compare(typeName, "BOOLEAN", true) == 0)
            {
                return SqlDbType.Bit;
            }
            else if (string.Compare(typeName, "NUMERIC", true) == 0)
            {
                return SqlDbType.Decimal;
            }
            else
            {
                return base.GetSqlType(typeName);
            }
        }

        public override string Format(QueryType type, bool suppressSize)
        {
            StringBuilder sb = new StringBuilder();
            DbQueryType sqlType = (DbQueryType)type;
            SqlDbType sqlDbType = sqlType.SqlDbType;

            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                case SqlDbType.SmallInt:
                case SqlDbType.Int:
                case SqlDbType.TinyInt:
                    sb.Append("INTEGER");
                    break;
                case SqlDbType.Bit:
                    sb.Append("BOOLEAN");
                    break;
                case SqlDbType.SmallDateTime:
                    sb.Append("DATETIME");
                    break;
                case SqlDbType.Char:
                case SqlDbType.NChar:
                    sb.Append("CHAR");
                    if (type.Length > 0 && !suppressSize)
                    {
                        sb.Append("(");
                        sb.Append(type.Length);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Variant:
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.UniqueIdentifier: //There is a setting to make it string, look at later
                    sb.Append("BLOB");
                    if (type.Length > 0 && !suppressSize)
                    {
                        sb.Append("(");
                        sb.Append(type.Length);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Xml:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarBinary:
                case SqlDbType.VarChar:
                    sb.Append("TEXT");
                    if (type.Length > 0 && !suppressSize)
                    {
                        sb.Append("(");
                        sb.Append(type.Length);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    sb.Append("NUMERIC");
                    if (type.Precision != 0)
                    {
                        sb.Append("(");
                        sb.Append(type.Precision);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Float:
                case SqlDbType.Real:
                    sb.Append("FLOAT");
                    if (type.Precision != 0)
                    {
                        sb.Append("(");
                        sb.Append(type.Precision);
                        sb.Append(")");
                    }
                    break;
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.Timestamp:
                default:
                    sb.Append(sqlDbType);
                    break;
            }
            return sb.ToString();
        }
    }
}
