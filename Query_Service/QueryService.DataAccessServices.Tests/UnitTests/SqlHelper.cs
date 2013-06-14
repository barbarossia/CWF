using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;

namespace Microsoft.Support.Workflow.Service.DataAccessServices.Tests.UnitTests
{
    public static class SqlHelper
    {
        public static void CauseSqlException(int errorNumber)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand cmd = db.GetSqlStringCommand(String.Format("RAISERROR('<ErrorNumber>{0}</ErrorNumber>Manual SQL exception', 16, 1)", errorNumber));
            db.ExecuteNonQuery(cmd);
        }
    }
}
