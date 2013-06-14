using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;

namespace CWF.BAL
{
    class DataContractChecker
    {
        /// <summary>
        /// single path checking
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActivityLibrarySpecificExistsReplyDC ActivityLibrarySpecificExists(ActivityLibrarySpecificExistsRequestDC request)
        {
            ActivityLibrarySpecificExistsReplyDC checkerReply = CheckActivityLibrarySpecificExistsRequestDC(request);
            // at this point check the checkerReply.checkerList for errors.
            // if there any return the reply object
            // else call the DAL 
            return CWF.DAL.Activities.ActivityLibrarySpecificExists(request);
        }

        /// <summary>
        /// Specific checker for a RequestDC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private ActivityLibrarySpecificExistsReplyDC CheckActivityLibrarySpecificExistsRequestDC(ActivityLibrarySpecificExistsRequestDC request)
        {
            ActivityLibrarySpecificExistsReplyDC reply = new ActivityLibrarySpecificExistsReplyDC();
            return reply;
        }
    }
}
