using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
namespace Microsoft.Support.Workflow.Service.DataAccessServices.Tests.Utilities
{
    public static class WorkflowTypeTestDataAccessUtility
    {
        public static void DeleteWorkflowType(int id)
        {
            Database database = DatabaseFactory.CreateDatabase();
            string commandFormat = "DELETE FROM WorkflowType WHERE Id={0}";
            DbCommand command = database.GetSqlStringCommand(string.Format(commandFormat, id));
            database.ExecuteNonQuery(command);
        }
    }
}