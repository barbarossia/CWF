//-----------------------------------------------------------------------
// <copyright file="TestController.cs" company="Microsoft">
// Copyright
// This is the controller for an mvc website. 
// The purpose is to call test cases with a url that has a querystring specifying which test case.
// These urls are then used in webtest to measure performance of that testcase.
// </copyright>
//-----------------------------------------------------------------------

namespace QueryServicePerfTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// The controller class for the website, which will run the testcase based on the querystring.
    /// </summary>
    public class TestController : Controller
    {
        #region Constants

        protected const string DROP_PATH = @"\\pqotfspvh02\Drops\staging\CommonWorkflowSystem\CWS Dev CI\";
        protected const string QUERY_SERVICE_DLL = "Query_Service.Tests.dll";
        protected const string PATH_NOT_FOUND_RESULT = "Path of dll not found";
        protected const string FAIL_RESULT = "Fail";
        protected const string PASS_RESULT = "Pass";
        protected const string NOT_FOUND_RESULT = "Test not found";
        protected const string TESTBASE_CLASSNAME = "QueryServiceTestBase";
        protected const string QUERYSTRING_ARG1 = "table";
        protected const string QUERYSTRING_ARG2 = "testcase";
        protected const string TESTINIT_METHODNAME = "TestInit";
        protected const string CLEANUP_METHODNAME = "CleanUp";
        protected const string ASSEMBLY_PATH = @"\\PQOOASSWS01\QueryServiceTests";

        #endregion

        /// <summary>
        /// This method will parse the querystring of the url and then run the resulting testcase
        /// </summary>
        /// <returns>The result of calling the test case</returns>
        public string Index()
        {

            string table = string.Empty;
            string testcase = string.Empty;
            string result = NOT_FOUND_RESULT;
            //string path = DROP_PATH;

            try
            {
                NameValueCollection queryString = this.HttpContext.Request.QueryString;

                table = queryString[QUERYSTRING_ARG1];
                testcase = queryString[QUERYSTRING_ARG2];

                // Commented code kept for future use
                // This will get the latest drop of the dll from which we'll call the testcase
                //DirectoryInfo buildDir = new DirectoryInfo(path);
                //DirectoryInfo[] dirsUnder = buildDir.GetDirectories();
                //if (dirsUnder.Count() == 0)
                //{
                //    return PATH_NOT_FOUND_RESULT;
                //}
                //DirectoryInfo latestDir = dirsUnder[0];
                //foreach (DirectoryInfo dirInfo in dirsUnder)
                //{
                //    if (latestDir.CreationTime < dirInfo.CreationTime)
                //    {
                //        string actualPathTemp = Path.Combine(dirInfo.FullName, QUERY_SERVICE_DLL);
                //        if (System.IO.File.Exists(actualPathTemp))
                //        {
                //            latestDir = dirInfo;
                //        }
                //    }
                //}

                //string actualPath = Path.Combine(latestDir.FullName, QUERY_SERVICE_DLL);

                string actualPath = Path.Combine(ASSEMBLY_PATH, QUERY_SERVICE_DLL);

                if (!System.IO.File.Exists(actualPath))
                {
                    return PATH_NOT_FOUND_RESULT;
                }

                Assembly testAssembly = Assembly.LoadFrom(actualPath);
                
                // Using reflection, we need to find the init and cleanup methods that will be called with the testcase
                MethodInfo testInit = null;
                MethodInfo cleanup = null;
                // We also need to load the UnitTestFramework.dll
                MethodInfo loadAssembly = null;

                Type testBase = (from type in testAssembly.GetTypes() where type.Name.ToLower().Equals(TESTBASE_CLASSNAME.ToLower()) select type).First();

                testInit = (from methodinfo in testBase.GetMethods() where methodinfo.Name.Equals(TESTINIT_METHODNAME) select methodinfo).First();
                cleanup = (from methodinfo in testBase.GetMethods() where methodinfo.Name.Equals(CLEANUP_METHODNAME) select methodinfo).First();
                loadAssembly = (from methodinfo in testBase.GetMethods() where methodinfo.Name.Equals("LoadUnitTestFrameworkAssembly") select methodinfo).First();
                

                // Using reflection, we call the testcase based on the values in the querystring
                var functionalTestClasses = from type in testAssembly.GetTypes()
                                            where (type.Name.EndsWith("Test") && !type.Name.Equals("DALUnitTest")) 
                                            select type;
                
                foreach (Type t in functionalTestClasses)
                {
                    if (t.Name.ToLower().Contains(table.ToLower()))
                    {
                        MethodInfo[] testMethods = t.GetMethods();
                        foreach (MethodInfo testMethod in testMethods)
                        {
                            if (testMethod.Name.ToLower().Equals(testcase.ToLower()))
                            {
                                try
                                {
                                    object instance = Activator.CreateInstance(t);
                                    loadAssembly.Invoke(instance, null);
                                    testInit.Invoke(instance, null);
                                    testMethod.Invoke(instance, null);
                                    cleanup.Invoke(instance, null);
                                    result = PASS_RESULT;
                                }
                                catch (Exception ex)
                                {
                                    return FAIL_RESULT + "1 " + ex.Message + " " + ex.StackTrace;
                                }

                               break;
                            }
                        }

                       return result;
                    }
                }

                return FAIL_RESULT + "2";
            }
            catch (Exception ex)
            {
                return FAIL_RESULT + "3 " + ex.Message + " " + ex.StackTrace;
            }
        }
    }
}
