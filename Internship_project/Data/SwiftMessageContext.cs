using Internship_project.ViewModels;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Internship_project.Data
{
    public class SwiftMessageContext
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _dbProviderFactory;

        public SwiftMessageContext(string connectionString)
        {
            _connectionString = connectionString;
            _dbProviderFactory = SqliteFactory.Instance;
        }

        public void EnsureDatabaseCreated()
        {
            using (var connection = CreateConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS SwiftMessages (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Field1 TEXT,
                            Field2 TEXT,
                            Field20 TEXT,
                            Field21 TEXT,
                            Field79 TEXT,
                            MAC TEXT,
                            CHK TEXT
                        )";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public void AddSwiftMessage(SwiftMessage swiftMessage)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO SwiftMessages (Field1, Field2, Field20, Field21, Field79, MAC, CHK)
                        VALUES (@Field1, @Field2, @Field20, @Field21, @Field79, @MAC, @CHK)";

                    
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@Field1";
                    parameter.Value = swiftMessage.Field1;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@Field2";
                    parameter.Value = swiftMessage.Field2;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@Field20";
                    parameter.Value = swiftMessage.Field20;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@Field21";
                    parameter.Value = swiftMessage.Field21;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@Field79";
                    parameter.Value = swiftMessage.Field79;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@MAC";
                    parameter.Value = swiftMessage.MAC;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@CHK";
                    parameter.Value = swiftMessage.CHK;
                    command.Parameters.Add(parameter);

                    

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private DbConnection CreateConnection()
        {
            var connection = _dbProviderFactory.CreateConnection();
            connection.ConnectionString = _connectionString;    
            return connection;
        }
    }
}

