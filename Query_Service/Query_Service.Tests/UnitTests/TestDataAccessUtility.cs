using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data.Common;

namespace Query_Service.UnitTests
{
    public static class TestDataAccessUtility
    {
        public static void CreateAuthoriztionGroup(string name, int roleId)
        {
            Database database = DatabaseFactory.CreateDatabase();
            string commandFormat = "if (select COUNT(1) from AuthorizationGroup" +
            " where Name = '{0}' and RoleId = {1} and Enabled =1) = 0" +
            " INSERT INTO [dbo].[AuthorizationGroup]" +
            " ([Guid],[Name],[AuthoringToolLevel],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias] ,[UpdatedDateTime] ,[RoleId] ,[Enabled])" +
            " VALUES ('{2}', '{0}', 0, 0, 'v-bobzh', '2013-01-01', 'v-bobzh', '2013-01-01', {1}, 1)";
            DbCommand command = database.GetSqlStringCommand(string.Format(commandFormat, name, roleId, Guid.NewGuid()));
            database.ExecuteNonQuery(command);
        }
    }
}
