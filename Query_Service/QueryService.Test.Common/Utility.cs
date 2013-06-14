using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Microsoft.Support.Workflow.Service.Test.Common
{
    public static class Utility
    {
        #region Config files contents
        public static string TempLocation = Environment.GetEnvironmentVariable("TEMP");

        static string userError = @"<userErrorConfiguration>
                                      <errors>   
                                        <add errorCode=""50001"" userMessage=""Unable to update the specified activity category."" faultType=""Fault""></add>
                                        <add errorCode=""50002"" userMessage=""Invalid AuthGroupName."" faultType=""ServiceFault""></add>

                                        <add errorCode=""52000"" userMessage=""Workflow not found."" faultType=""PublishingFault""></add>
                                        <add errorCode=""52001"" userMessage=""Workflow XAML not found."" faultType=""PublishingFault""></add>
                                        <add errorCode=""52002"" userMessage=""Publishing workflow not found."" faultType=""PublishingFault""></add>

                                        <add errorCode=""52500"" userMessage=""Invalid caller."" faultType=""ValidationFault""></add>
                                        <add errorCode=""52501"" userMessage=""Invalid caller version."" faultType=""ValidationFault""></add>
                                        <add errorCode=""52520"" userMessage=""Invalid activity category ID."" faultType=""ValidationFault""></add>
                                        <add errorCode=""52521"" userMessage=""Invalid activity category GUID."" faultType=""ValidationFault""></add>
                                        <add errorCode=""52522"" userMessage=""IInvalid created user."" faultType=""ValidationFault""></add>
                                        <add errorCode=""52523"" userMessage=""Invalid description."" faultType=""ValidationFault""></add>
                                        <add errorCode=""52524"" userMessage=""Invalid activity category name."" faultType=""ValidationFault""></add>
                                        <add errorCode=""52525"" userMessage=""Invalid meta tags."" faultType=""ValidationFault""></add>
                                        <add errorCode=""52526"" userMessage=""Invalid auth group name."" faultType=""ValidationFault""></add>
    
                                        <add errorCode=""52527"" userMessage=""Activity category updated by user is required."" faultType=""validationfault""></add>

                                        <add errorCode=""53000"" userMessage=""An error occurred.  Contact system administrator for assistance."" faultType=""ServiceFault""></add>
                                      </errors>
                                    </userErrorConfiguration>";

        static string logSetting = @"<loggingConfiguration>
                          <settings>
                            <add key=""loglevel.default"" value=""2""></add>
                            <add key=""loglevel.databaseError"" value=""2""></add>
                            <add key=""loglevel.dataAccessLayerError"" value=""2""></add>
                            <add key=""loglevel.businessLayererror"" value=""2""></add>
                            <add key=""loglevel.webservicelayererror"" value=""2""></add>    
                            <add key=""loglevel.logwritererror"" value=""2""></add>
                            <add key=""loglevel.businesslayervalidation"" value=""2""></add>
                            <add key=""LogName"" value=""WorkflowQueryService""></add>
                            <add key=""LogWriter"" value=""Microsoft.Support.Workflow.Service.Common.Logging.EventLogWriter,QueryService.Common""></add>
                          </settings>  
                       </loggingConfiguration>";


        static string errorCodeMapping = @"<errorCodeEventCategoryConfiguration>
                                <errorCodes>
                                    <!--Possible values for severity=error|warning|information|successaudit|failureaudit-->
                                    <!--Possible values for eventcategory=Administrative|Operational|Analytic|Debug|ApiUsage|None-->
                                    <add errorCode=""52500"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52501"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52502"" severity=""error"" eventCategory=""Analytic"" />

                                    <!--Business Layer Validations - Activity Category Input Validations-->
                                    <add errorCode=""52520"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52521"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52522"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52523"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52524"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52525"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52526"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52527"" severity=""error"" eventCategory=""Analytic"" />

                                    <!--Business Layer Validations - Store Activity Input Validations-->
                                    <add errorCode=""52540"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52541"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52542"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52543"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52544"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52545"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52546"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52547"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52548"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52549"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52550"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52551"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52552"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52553"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52554"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52555"" severity=""error"" eventCategory=""Analytic"" />

                                    <!--Business Layer Validations - Activity Library Input Validations-->
                                    <add errorCode=""52560"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52561"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52562"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52563"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52564"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52565"" severity=""error"" eventCategory=""Analytic"" />

                                    <!--Business Layer Validations - Activity Library Dependency Input Validations-->
                                    <add errorCode=""52580"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52581"" severity=""error"" eventCategory=""Analytic"" />
                                    <add errorCode=""52582"" severity=""error"" eventCategory=""Analytic"" />

                                    <!--Business Layer Validations - Workflow Type Input Validations-->
                                    <add errorCode=""52600"" severity=""error"" eventCategory=""Analytic"" />
                                </errorCodes>
                         </errorCodeEventCategoryConfiguration>";
        #endregion

        /// <summary>
        /// Generates a random string.
        /// </summary>
        /// <param name="size">Length of the random value to be generated.</param>
        /// <returns>Random string.</returns>
        public static string GenerateRandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static int GenerateRandomNumber(int max)
        {
            Random random = new Random();
            return random.Next(max);
        }

        public static int GetRandomStatusCode
        {
            get
            {
                int statusCode = 1000;
                int ran = new Random().Next(0, 3);

                switch (ran)
                {
                    case 0:
                        statusCode = 1000;
                        break;
                    case 1:

                        statusCode = 1010;
                        break;
                    case 2:
                        statusCode = 1020;
                        break;
                }

                return statusCode;
            }
        }

        public static void CopyTestConfigs(string copyDirectory)
        {
            CopyFile(string.Format(@"{0}\userError.config", copyDirectory), userError);
            CopyFile(string.Format(@"{0}\logSetting.config", copyDirectory), logSetting);
            CopyFile(string.Format(@"{0}\errorCodeMapping.config", copyDirectory), errorCodeMapping);           
        }

        private static void CopyFile(string path, string content)
        {
            if(!File.Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(content);
                    writer.Close();
                }
            }
        }       
    }
}
