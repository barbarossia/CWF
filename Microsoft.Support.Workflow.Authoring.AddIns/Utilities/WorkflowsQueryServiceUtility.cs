using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CWF.DataContracts;

namespace Microsoft.Support.Workflow.Authoring.Services
{
    public static class WorkflowsQueryServiceUtility
    {
        /// <summary>
        /// Gets an instance of WorkflowsQueryServiceClient, for injection in unit tests
        /// </summary>
        public static Func<WorkflowsQueryServiceClient> GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();

           /// <summary>
        /// Encapsulate "safe" pattern for using WCF client and then disposing of it.
        /// </summary>
        /// <param name="action">What to do with the client after creating it and before disposing of it.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000")] // .Dispose() is not the appropriate idiom for generated clients
        public static void UsingClient(Action<IWorkflowsQueryService> action)
        {
            // Explicitly use TaskScheduler.Default to make sure that the client gets created on a threadpool thread 
            // (so it is non-STA) even if we are executing on a Task scheduled on the UI thread.
            var createTask = Task.Factory.StartNew(
                GetWorkflowQueryServiceClient, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            UsingClient<WorkflowsQueryServiceClient>(createTask.ResultOrException(), action);
        }

        public static T UsingClientReturn<T>(Func<IWorkflowsQueryService, T> actionWithReturn)
        {
            T result = default(T);
            UsingClient(client => result = actionWithReturn(client));
            return result;
        }

        /// <summary>
        /// Encapsulate "safe" pattern for using WCF client and then disposing of it.
        /// </summary>
        /// <param name="client">Type of client to create</param>
        /// <param name="action">What to do with the client after creating it and before disposing of it.</param>
        public static void UsingClient<T>(T client, Action<T> action) where T : ICommunicationObject
        {
            bool success = false;
            try
            {
                action(client);
                client.Close();
                success = true;
            }
            finally
            {
                if (!success)
                {
                    client.Abort();
                }
            }
        }

        /// <summary>
        /// Fluent pattern for converting error codes into CommunicationExceptions
        /// </summary>
        /// <exception cref="CommunicationException">Wraps ErrorMessage</exception>
        public static T CheckErrors<T>(this T reply) where T : StatusReplyDC
        {
            if (reply.Errorcode != 0)
            {
                throw new CommunicationException(reply.ErrorMessage);
            }
            return reply;
        }

        /// <summary>
        /// Fluent pattern for setting the ubiquitous "Incaller" parameters to the correct value 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns>The original request, with Incaller/IncallerVersion set</returns>
        public static T SetIncaller<T>(this T request) where T : RequestHeader
        {
            request.Incaller = Assembly.GetExecutingAssembly().GetName().Name;
            request.IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            request.InInsertedByUserAlias = Environment.UserName;
            request.InUpdatedByUserAlias = Environment.UserName;
            return request;
        }

        public static T CommonHeaderSetIncaller<T>(this T request) where T : RequestReplyCommonHeader
        {
            request.Incaller = Assembly.GetExecutingAssembly().GetName().Name;
            request.IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return request;
        }

        /// <summary>
        /// Extension method for Task. Unlike Task.Result, this will re-throw the original exception
        /// instead of an aggregate exception. The authoring tool should only ever call Task.ResultOrException
        /// and never Task.Result (except for in this method).
        /// </summary>
        public static T ResultOrException<T>(this Task<T> task)
        {
            try
            {
                return task.Result; // will wait until completion or re-throw exception
            }
            catch (AggregateException e)
            {
                // Unwrap the AggregateException created by Task library back into the
                // original exception and re-throw it on this thread. Unfortunately we
                // lose the original stack trace, but we retain the type and therefore
                // the ability to programmatically handle the exception correctly.
                //
                // If we had a logging solution in place, this would be the place to
                // log the original stack trace of the exception because here is where
                // it is easiest to debug.
                if (e.InnerExceptions.Count == 1)
                {
                    throw e.InnerException;
                }
                else
                {
                    throw; // in this case we would lose information so we can't unwrap
                }
            }
        }
    }
}
