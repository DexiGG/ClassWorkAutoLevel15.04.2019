using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Configuration;


namespace AutoLevelpart2
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = ConfigurationManager.ConnectionStrings["appConnection"]; //Подключение через app.config
            var providerName = configuration.ProviderName;
            var connectionString = configuration.ConnectionString;

            var providerFactory = DbProviderFactories.GetFactory(providerName);
            using (var connection = providerFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                var dataSet = new DataSet("users"); //автономный уровень
                var dataAdapter = providerFactory.CreateDataAdapter();//автономный уровень(работает м/у таблицой и бд

                var selectUsersCommand = connection.CreateCommand();//автономный уровень
                selectUsersCommand.CommandText = "select * from Users";
                dataAdapter.SelectCommand = selectUsersCommand;//автономный уровень

                dataAdapter.Fill(dataSet, "Users");

                var commandBuilder = providerFactory.CreateCommandBuilder();
                commandBuilder.DataAdapter = dataAdapter;

                var usersTable = dataSet.Tables["Users"]; //Изменение данных строки таблицы
                var row = usersTable.Rows[0];
                row.BeginEdit();
                row["Login"] = "superUser";
                row.EndEdit();

                dataAdapter.Update(dataSet, "Users");
            }
        }
    }
}
