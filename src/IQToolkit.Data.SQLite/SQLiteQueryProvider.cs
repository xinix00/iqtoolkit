using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using System.IO;

namespace IQToolkit.Data.SQLite
{
    using IQToolkit.Data.Common;

    /// <summary>
    /// A <see cref="DbEntityProvider"/> for SQLite databases
    /// </summary>
    public class SQLiteQueryProvider : DbEntityProvider
    {
        private readonly Dictionary<QueryCommand, SqliteCommand> commandCache = new Dictionary<QueryCommand, SqliteCommand>();

        /// <summary>
        /// Constructs a <see cref="SQLiteQueryProvider"/>
        /// </summary>
        public SQLiteQueryProvider(SqliteConnection connection, QueryMapping mapping = null, QueryPolicy policy = null)
            : base(connection, SQLiteLanguage.Default, mapping, policy)
        {
        }

        /// <summary>
        /// Constructs a <see cref="SQLiteQueryProvider"/>
        /// </summary>
        public SQLiteQueryProvider(string connectionStringOrDatabaseFile, QueryMapping mapping = null, QueryPolicy policy = null)
            : this(CreateConnection(connectionStringOrDatabaseFile), mapping, policy)
        {
        }

        /// <summary>
        /// Creates a <see cref="SQLiteConnection"/> given a connection string or a database file.
        /// </summary>
        public static SqliteConnection CreateConnection(string connectionStringOrDatabaseFile)
        {
            if (!connectionStringOrDatabaseFile.Contains("="))
            {
                connectionStringOrDatabaseFile = GetConnectionString(connectionStringOrDatabaseFile);
            }

            return new SqliteConnection(connectionStringOrDatabaseFile);
        }
        /// <summary>
        /// Gets a connection string that will access specified the database file.
        /// </summary>
        public static string GetConnectionString(string databaseFile)
        {
            databaseFile = Path.GetFullPath(databaseFile);
            return string.Format("Data Source={0};", databaseFile);
        }

        /// <summary>
        /// Gets a connection string that will access specified the database file.
        /// </summary>
        public static string GetConnectionString(string databaseFile, string password)
        {
            databaseFile = Path.GetFullPath(databaseFile);
            return string.Format("Data Source={0};Password={1};", databaseFile, password);
        }

        /// <summary>
        /// Gets a connection string that will access specified the database file.
        /// </summary>
        public static string GetConnectionString(string databaseFile, bool failIfMissing)
        {
            databaseFile = Path.GetFullPath(databaseFile);
            return string.Format("Data Source={0};FailIfMissing={1};", databaseFile, failIfMissing ? bool.TrueString : bool.FalseString);
        }

        /// <summary>
        /// Gets a connection string that will access specified the database file.
        /// </summary>
        public static string GetConnectionString(string databaseFile, string password, bool failIfMissing)
        {
            databaseFile = Path.GetFullPath(databaseFile);
            return string.Format("Data Source={0};Password={1};FailIfMissing={2};", databaseFile, password, failIfMissing ? bool.TrueString : bool.FalseString);
        }

        protected override DbEntityProvider New(DbConnection connection, QueryMapping mapping, QueryPolicy policy)
        {
            return new SQLiteQueryProvider((SqliteConnection)connection, mapping, policy);
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }

        new class Executor : DbEntityProvider.Executor
        {
            SQLiteQueryProvider provider;

            public Executor(SQLiteQueryProvider provider)
                : base(provider)
            {
                this.provider = provider;
            }

            protected override DbCommand GetCommand(QueryCommand query, object[] paramValues)
            {
                SqliteCommand cmd;
#if false
                if (!this.provider.commandCache.TryGetValue(query, out cmd))
                {
                    cmd = (SqliteCommand)this.provider.Connection.CreateCommand();
                    cmd.CommandText = query.CommandText;
                    this.SetParameterValues(query, cmd, paramValues);
                    cmd.Prepare();
                    this.provider.commandCache.Add(query, cmd);
                    if (this.provider.Transaction != null)
                    {
                        cmd = new SqliteCommand(cmd.CommandText, (SqliteConnection)this.provider.Connection, (SqliteTransaction)this.provider.Transaction);
                        cmd.Transaction = (SqliteTransaction)this.provider.Transaction;
                    }
                }
                else
                {
                    cmd = new SqliteCommand(cmd.CommandText, (SqliteConnection)this.provider.Connection, (SqliteTransaction)this.provider.Transaction);
                    this.SetParameterValues(query, cmd, paramValues);
                }
#else
                cmd = (SqliteCommand)this.provider.Connection.CreateCommand();
                cmd.CommandText = query.CommandText;
                this.SetParameterValues(query, cmd, paramValues);
                cmd.Prepare();

                if (this.provider.Transaction != null)
                {
                    cmd.Transaction = (SqliteTransaction)this.provider.Transaction;
                }
#endif
                return cmd;
            }

            protected override void AddParameter(DbCommand command, QueryParameter parameter, object value)
            {
                QueryType qt = parameter.QueryType;
                if (qt == null)
                    qt = this.provider.Language.TypeSystem.GetColumnType(parameter.Type);
                
                var p = ((SqliteCommand)command).Parameters.Add(
                    parameter.Name, 
                    SQLiteTypeSystem.GetSqliteType(((DbQueryType)qt).DbType), 
                    qt.Length);

                if (qt.Length != 0)
                {
                    p.Size = qt.Length;
                }
                else if (qt.Scale != 0)
                {
                    p.Size = qt.Scale;
                }
                p.Value = value ?? DBNull.Value;
            }
        }
    }
}
