//-----------------------------------------------------------------------
// <copyright file="StoreActivity.cs" company="Microsoft">
// Copyright
// StoreActivity DC validation
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DCValidation
{
    using CWF.DataContracts;

    /// <summary>
    /// StoreActivity validation class
    /// </summary>
    public static class StoreActivity
    {
        /// <summary>
        /// Checks the parameters for StoreActivity in the case of create, update, get, and delete
        /// </summary>
        /// <param name="action">create, update, get, or delete</param>
        /// <param name="sa">StoreActivity object</param>
        /// <returns>StatusReplyDC object</returns>
        public static CWF.DataContracts.StatusReplyDC CheckDCValidation(string action, StoreActivitiesDC sa)
        {
            StatusReplyDC reply = Primitives.BasicParameterCheck(sa, action);
            if (reply.Errorcode != 0)
                return reply;

            switch (action.ToLower())
            {
                case "get":
                    {
                        reply = Primitives.CheckGetStoreActivity(sa);
                        break;
                    }
                case "createorupdate":
                    {
                        //// determine if it is a create or updzate;
                        if (sa.Id == 0)
                        {
                            //// create
                            reply = Primitives.CheckCreateStoreActivity(sa);
                            if (reply.Errorcode != 0)
                                return reply;
                        }
                        else
                        {
                            //// update
                            reply = Primitives.CheckUpdateStoreActivity(sa);
                            if (reply.Errorcode != 0)
                                return reply;
                        }
                        break;
                    }
                case "delete":
                    {
                        reply = Primitives.CheckDeleteStoreActivity(sa);
                        break;
                    }
                default:
                    {
                        if (Primitives.IsStringNullEmptyOrWhitespace(action))
                            reply = Primitives.CreateStatusReplyObject(Constants.SprocValues.REQUEST_ACTION_OBJECT_IS_NULL_ID, Constants.SprocValues.REQUEST_ACTION_OBJECT_IS_NULL_MSG, string.Empty);
                        break;
                    }
            }
            return reply;
        }
    }
}
