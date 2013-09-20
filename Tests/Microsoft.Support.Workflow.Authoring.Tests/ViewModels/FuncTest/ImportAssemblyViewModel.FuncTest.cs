using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Windows;
using AuthoringToolTests.Services;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel.Security;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class ImportAssemblyViewModelFunctionalTests
    {
        #region const string
        private TestContext testContextInstance;
        private const string TESTOWNER = "v-xtong";

        private Assembly assembly1 = TestInputs.Assemblies.TestInput_Lib1;
        private Assembly assembly2 = TestInputs.Assemblies.TestInput_Lib2;
        private Assembly assembly3 = TestInputs.Assemblies.TestInput_Lib3;

        // rename a text file extension to .dll
        private string InvalidDllPath;
        private const string InvalidDllFileName = @"TestData\InvalidLibrary.dll";

        // none standard dot net dll library
        private string NotStandardDotNetDllPath;
        private const string NotStandardDotNetDllFileName = @"TestData\DifProf32.dll";
        #endregion

        #region Properties
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #endregion

        #region test initiliaze
        [TestInitialize]
        public void TestInitialize()
        {
           // InvalidDllPath = Path.Combine(testContextInstance.DeploymentDirectory, InvalidDllFileName);
           // NotStandardDotNetDllPath = Path.Combine(testContextInstance.DeploymentDirectory, NotStandardDotNetDllFileName);
           InvalidDllPath= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Data\\"+InvalidDllFileName;
           NotStandardDotNetDllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\Data\\"+NotStandardDotNetDllFileName;
        }
        #endregion


        /*Failed Test*/
        [WorkItem(138818)]
        [Description("Test AddNewCategoryCommandExecute when NewCategoryName is valid and doesn't exist in server")]
        [TestCategory("Func-NoDif-Smoke")]
        [TestMethod()]
        [Ignore]
        public void VerifyAddNewCategoryCommandExecuteWithValidNewCategoryName()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                // Set
                string newCategoryName = TestUtilities.GetRandomStringOfLength(10);
                vmImportAssembly.NewCategoryName = newCategoryName;
                vmImportAssembly.AddNewCategoryCommandExecute();

                ObservableCollection<string> activityCategories = AssemblyHelper.GetCategoriesFromServer();

                // Verify
                Assert.IsTrue(activityCategories.Any(categoryName => string.Equals(categoryName, newCategoryName)));
            }
        }

        [WorkItem(138862)]
        [Description("Test AddNewCategoryCommandExecute when NewCategoryName already exists in server")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyAddNewCategoryCommandExecuteWithExistCategoryName()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                ObservableCollection<string> activityCategoriesBeforeAdd = AssemblyHelper.GetCategoriesFromServer();
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                if (activityCategoriesBeforeAdd.Count > 0)
                {
                    vmImportAssembly.NewCategoryName = activityCategoriesBeforeAdd.LastOrDefault();
                }

                vmImportAssembly.AddNewCategoryCommandExecute();

                ObservableCollection<string> activityCategoriesAfterAdd = AssemblyHelper.GetCategoriesFromServer();
                // Verify
                Assert.AreEqual(1, (activityCategoriesAfterAdd.Where(a => string.Equals(a, vmImportAssembly.NewCategoryName)).Select(a => a)).ToList().Count);
            }

        }

        [WorkItem(138819)]
        [Description("Test AddNewCategoryCommandExecute when NewCategoryName is null")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyAddNewCategoryCommandExecuteWhenNewCategoryNameIsNull()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                vmImportAssembly.NewCategoryName = null;

                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.AddNewCategoryCommandExecute();
                }).Throws<ArgumentNullException>();
            }
        }

        [WorkItem(138820)]
        [Description("Test AddNewCategoryCommandExecute when NewCategoryName is empty")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyAddNewCategoryCommandExecuteWhenNewCategoryNameIsEmpty()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                vmImportAssembly.NewCategoryName = string.Empty;

                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.AddNewCategoryCommandExecute();
                }).Throws<ArgumentNullException>();
            }
        }


        [WorkItem(138821)]
        [Description("Test AddNewCategoryCommandExecute with when NewCategoryName contains special characters")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyAddNewCategoryCommandExecuteWhenNewCategoryNameContainSpecailCharaters()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                vmImportAssembly.NewCategoryName = @"a b^d*}#$$$ = Like ][";

                // Set and Verify
                ExpectException.That(() =>
                {
                    vmImportAssembly.AddNewCategoryCommandExecute();
                }).Throws<UserFacingException>();
            }
        }


        [WorkItem(138823)]
        [Description("Test AddNewCategoryCommandExecute when the length of NewCategoryName is over max length (50)")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyAddNewCategoryCommandExecuteWhenNewCategoryNameOverMaxLength()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                vmImportAssembly.NewCategoryName = TestUtilities.GetRandomStringOfLength(100);

                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.AddNewCategoryCommandExecute();
                }).Throws<ArgumentOutOfRangeException>();
            }
        }

        [WorkItem(138825)]
        [Description("Test AddNewCategory button is enabled for valid NewCategoryName")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyCanAddNewCategoryCommandExecuteReturnTrueWithValidNewCategoryName()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                // Set
                vmImportAssembly.NewCategoryName = TestUtilities.GetRandomStringOfLength(15);

                // Verify
                Assert.IsTrue(vmImportAssembly.CanAddNewCategoryCommandExecute());
            }
        }

        [WorkItem(138826)]
        [Description("Test AddNewCategory button is disabled for null NewCategoryName")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyCanAddNewCategoryCommandExecuteReturnFalseWithNullNewCategoryName()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                // Set
                vmImportAssembly.NewCategoryName = null;
                // Verify
                Assert.IsFalse(vmImportAssembly.CanAddNewCategoryCommandExecute());
            }
        }

        [WorkItem(138827)]
        [Description("Test AddNewCategory button is disabled for empty NewCategoryName")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyCanAddNewCategoryCommandExecuteReturnFalseWithEmptyNewCategoryName()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                // Set
                vmImportAssembly.NewCategoryName = string.Empty;
                // Verify
                Assert.IsFalse(vmImportAssembly.CanAddNewCategoryCommandExecute());
            }
        }

        [WorkItem(138830)]
        [Description("Tests changing the default category for imports")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyChangeDefaultCategory()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            const string TEST_CATEGORY = "OAS";
            const string ASSERT_FAIL_MESSAGE = "Not all categories for imports equalled the new default.";
            using (new CachingIsolator())
            {
                var viewModel = new ImportAssemblyViewModel(assembly3.Location);

                Assert.IsTrue(viewModel.AssembliesToImport.Count > 0, "Setup for the test method failed: There must be one or more imports available");

                viewModel.ChangeDefaultCategory(TEST_CATEGORY);
                // the categories for all imports should all equal the test string now. Make sure of this.
                Assert.IsFalse(viewModel.AssembliesToImport.Any(import => import.Category != TEST_CATEGORY), ASSERT_FAIL_MESSAGE);

                viewModel.ChangeDefaultCategory(String.Empty);
                Assert.IsFalse(viewModel.AssembliesToImport.Any(import => !string.IsNullOrEmpty(import.Category)), ASSERT_FAIL_MESSAGE);
            }
        }

        [WorkItem(138833)]
        [Description("Test Initialize ImportAssemblyViewModel with an invalid assembly")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyInitializeImportAssemblyViewModelWithInvalidAssembly()
        {
            // Set and Verify
            ExpectException.That(() =>
            {
                // Set
                var vmImportAssembly = new ImportAssemblyViewModel(InvalidDllPath);
            }).Throws<AssemblyInspectionException>();

        }

        [WorkItem(138836)]
        [Description("Test Initialize ImportAssemblyViewModel withnone standard assembly")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyInitializeImportAssemblyViewModelWithNoneStandardAssembly()
        {
            // Set and Verify
            ExpectException.That(() =>
            {
                // Set
                var vmImportAssembly = new ImportAssemblyViewModel(NotStandardDotNetDllPath);
            }).Throws<AssemblyInspectionException>();
        }

        [WorkItem(138839)]
        [Description("Test Initialize ImportAssemblyViewModel when AssemblyFileName is empty")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyInitializeImportAssemblyViewModelWithEmptyAssemblyFileName()
        {
            // Set and Verify
            ExpectException.That(() =>
            {
                // Set
                var vmImportAssembly = new ImportAssemblyViewModel(string.Empty);
            }).Throws<ArgumentNullException>();
        }

        [WorkItem(138864)]
        [Description("Test CanImportCommandExecute function when itemToImport has location and reviewed")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyCanImportCommandExecuteReturnTrueWhenHasLocationAndReviewed()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                vmImportAssembly.AssembliesToImport
                    .ToList()
                    .ForEach(import =>
                    {
                        import.IsReviewed = true;
                    });

                // Set
                bool result = vmImportAssembly.CanImportCommandExecute();
                // Verify
                Assert.IsTrue(result);
            }
        }


        [WorkItem(138865)]
        [Description("Test CanImportCommandExecute function with empty assembliesToImport")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyCanImportCommandExecuteReturnFalseWhenAssembliesToImportIsEmpty()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                vmImportAssembly.AssembliesToImport.Clear();

                // Set
                bool result = vmImportAssembly.CanImportCommandExecute();
                // Verify
                Assert.IsFalse(result);
            }
        }

        [WorkItem(138866)]
        [Description("Test CanImportCommandExecute function when one of assembliesToImport does not have location")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyCanImportCommandExecuteReturnFalseWhenActivityAssemblyItemContainsNullLocation()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                vmImportAssembly.AssembliesToImport
                    .ToList()
                    .ForEach(import =>
                    {
                        import.IsReviewed = true;
                        import.Location = null;
                    });

                // Set
                bool result = vmImportAssembly.CanImportCommandExecute();
                // Verify
                Assert.IsFalse(result);
            }
        }

        [WorkItem(138868)]
        [Description("Test CanImportCommandExecute function when one of assembliesToImport does not has been reviewed")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyCanImportCommandExecuteReturnFalseWithoutReview()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                vmImportAssembly.AssembliesToImport
                    .ToList()
                    .ForEach(import =>
                    {
                        import.IsReviewed = false;
                    });

                // Set
                bool result = vmImportAssembly.CanImportCommandExecute();
                // Verify
                Assert.IsFalse(result);
            }
        }

        [WorkItem(138842)]
        [Description("Test the ImportAssembly function")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif-Smoke")]
        [TestMethod()]
        public void VerifyImportAssembly()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistCreateIntellisenseList();
            // Tests Import Assembly for Tests.dll
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);
                Assert.AreEqual(1, vmImportAssembly.AssembliesToImport.Count, "Only the test assembly is in the input list");

                //TestUtilities.MockCreateIntellisenseList(() =>
                //{
                // Act
                vmImportAssembly.ImportCommand.Execute();
                // Assert
                // Check for this assembly in Caching.ActivityAssemblyItems
                bool foundAssemblyInCache = false;
                foreach (ActivityAssemblyItem assemblyItem in Caching.ActivityAssemblyItems)
                {
                    if (assemblyItem.AssemblyName.Name == vmImportAssembly.AssembliesToImport[0].AssemblyName.Name)
                    {
                        foundAssemblyInCache = true;
                        System.Func<string, string, bool> IsSubdir = (string maindir, string subdir) =>
                        {
                            return subdir.StartsWith(maindir + Path.DirectorySeparatorChar);
                        };
                        Assert.IsTrue(IsSubdir(Utility.GetAssembliesDirectoryPath(), assemblyItem.Location), "After import, ActivityAssemblyItem's location should be in Assemblies folder so that Assembly.Load() can find it");
                    }
                }
                Assert.IsTrue(foundAssemblyInCache, "After import, ActivityAssemblyItem should be in Caching.ActivityAssemblyItems so that Assembly.Load() can find it");
                //});
            }
        }

        [WorkItem(138843)]
        [Description("Test the ImportAssembly function")]
        [Owner("v-maxw")]
        [TestCategory("Func-NoDif-Smoke")]
        [TestMethod()]
        public void VerifyImportAssemblyFunction()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistCreateIntellisenseList();
            // Arrange
            var stringBuilder = new StringBuilder();
            MessageBoxService.ShowFunc = ((msg, a, b, c, d) => { stringBuilder.Append(msg); return MessageBoxResult.OK; });

            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly3.Location);
                // Assert
                Assert.AreEqual(string.Empty, stringBuilder.ToString());
                Assert.AreEqual(3, vmImportAssembly.AssembliesToImport.Count);

                //TestUtilities.MockCreateIntellisenseList(() =>
                //{
                // Set
                foreach (var asm in vmImportAssembly.AssembliesToImport.Skip(1).ToList()) // only want test assembly in input list, to keep test clean
                    vmImportAssembly.AssembliesToImport.Remove(asm);
                vmImportAssembly.ImportCommand.Execute();
                bool result = TestUtilities.PreviewImportAssembly(assembly3.Location);
                // Assert
                Assert.IsTrue(result);
                //});
            }
        }

        [WorkItem(138844)]
        [Description("Tests the ImportAssembly with Dependencies functionality")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif-Smoke")]
        [TestMethod()]
        public void VerifyImportAssemblyWithDependenciesAndCategory()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistCreateIntellisenseList();
            using (new CachingIsolator())
            {
                //Making sure the assembly's doesn't exist in cache before import
                Assert.IsFalse(AssemblyHelper.IsAssemblyInLocalCaching(assembly1)
                                && AssemblyHelper.IsAssemblyInLocalCaching(assembly2)
                                && AssemblyHelper.IsAssemblyInLocalCaching(assembly3));

                var vmImportAssembly = new ImportAssemblyViewModel(assembly3.Location);
                Assert.AreEqual(3, vmImportAssembly.AssembliesToImport.Count);

                string newCategory = "Test";
                vmImportAssembly.AssembliesToImport
                    .ToList()
                    .ForEach(import =>
                    {
                        import.Category = newCategory;
                        import.ActivityItems
                              .ToList()
                              .ForEach(item => item.Category = newCategory);
                    });

                //TestUtilities.MockCreateIntellisenseList(() =>
                //{
                // Import
                vmImportAssembly.ImportCommand.Execute();
                // Check for the assemblies in Caching.ActivityAssemblyItems
                // Make sure's the dependencies are also imported along with imported assembly
                Assert.IsTrue(AssemblyHelper.IsAssemblyInLocalCaching(assembly1), "TestInput_Lib1 should be cached");
                Assert.IsTrue(AssemblyHelper.IsAssemblyInLocalCaching(assembly2), "TestInput_Lib2 should be cached");
                Assert.IsTrue(AssemblyHelper.IsAssemblyInLocalCaching(assembly3), "TestInput_Lib3 should be cached");

                //Making sure the all libraries displays under newCategory
                Assert.IsTrue(Caching.ActivityAssemblyItems.All(a => a.Category == newCategory));
                //});
            }
        }

        [WorkItem(138847)]
        [Description("Test ImportCommandExecute function when assembliesToImport is null")]
        [TestCategory("Func-Dif-Smoke")]
        [TestMethod()]
        [Owner("DiffRequired")]
        public void VerifyImportCommandExecuteWhenAssembliesToImportIsNull()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);
                using (var impl = new ImplementationOfType(typeof(ImportAssemblyViewModel)))
                {
                    impl.Register<ImportAssemblyViewModel>((inst) => inst.AssembliesToImport).Return(null);
                    // Set and Verify
                    ExpectException.That(() =>
                    {
                        // Set
                        vmImportAssembly.ImportCommandExecute();
                    }).Throws<ArgumentNullException>();
                }
            }
        }

        [WorkItem(138848)]
        [Description("Test ImportCommandExecute function when assembliesToImport is empty")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyImportCommandExecuteWhenAssembliesToImportIsEmpty()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);
                vmImportAssembly.AssembliesToImport.Clear();

                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.ImportCommandExecute();
                }).Throws<ArgumentException>();
            }
        }

        [WorkItem(138850)]
        [Description("Test ImportCommandExecute function when assembliesToImport contains a null item")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        [Ignore]
        public void VerifyImportCommandExecuteWhenAssembliesToImportContainsNullItem()
        {
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);
                vmImportAssembly.AssembliesToImport.Add(null);

                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.ImportCommandExecute();
                }).Throws<ArgumentException>();
            }
        }


        [WorkItem(138851)]
        [Description("Test ImportCommandExecute function when ActivityAssemblyItem does't have AssemblyName")]
        [TestCategory("Func-Dif-Smoke")]
        [TestMethod()]
        [Owner("DiffRequired")]
        [Ignore]
        public void VerifyImportCommandExecuteWhenActivityAssemblyItemWithoutAssemblyName()
        {
            TestUtilities.RegistCreateIntellisenseList();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                using (var impl = new ImplementationOfType(typeof(ActivityAssemblyItemViewModel)))
                {
                    impl.Register<ActivityAssemblyItemViewModel>((inst) => inst.AssemblyName).Return(null);

                    //TestUtilities.MockCreateIntellisenseList(() =>
                    //{
                    //Set and Verify
                    ExpectException.That(() =>
                    {
                        vmImportAssembly.ImportCommandExecute();
                    }).Throws<ArgumentException>();
                    //});
                }
            }
        }

        [WorkItem(138852)]
        [Description("Test ImportCommandExecute function when ActivityAssemblyItem does't have location")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyImportCommandExecuteWhenActivityAssemblyItemWithoutLocation()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);
                vmImportAssembly.AssembliesToImport
                    .ToList()
                    .ForEach(import =>
                    {
                        import.Location = null;
                    });

                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.ImportCommandExecute();
                }).Throws<ArgumentException>();
            }
        }

        [WorkItem(138854)]
        [Description("Test ImportCommandExecute function when ActivityAssemblyItem has a invalid location")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyImportCommandExecuteWhenActivityAssemblyItemWithInvalidLocation()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);

                vmImportAssembly.AssembliesToImport
                    .ToList()
                    .ForEach(import =>
                    {
                        import.Location = TestUtilities.GetRandomStringOfLength(10);
                    });

                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.ImportCommandExecute();
                }).Throws<ArgumentException>();
            }
        }

        [WorkItem(138870)]
        [Description("Test ImportCommandExecute function when ActivityAssemblyItem has been cached")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif-Smoke")]
        [TestMethod()]
        public void VerifyImportCommandExecuteWhenActivityAssemblyItemHasBeenInLocalCached()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            TestUtilities.RegistCreateIntellisenseList();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                using (new CachingIsolator())
                {
                    Assert.IsFalse(AssemblyHelper.IsAssemblyInLocalCaching(assembly1));
                    vmImportAssembly.AssembliesToImport
                        .ToList()
                        .ForEach(import =>
                        {
                            import.CachingStatus = CachingStatus.Latest;
                        });

                    //TestUtilities.MockCreateIntellisenseList(() =>
                    //{
                    // Set
                    vmImportAssembly.ImportCommandExecute();
                    Assert.IsFalse(AssemblyHelper.IsAssemblyInLocalCaching(assembly1));
                    //});
                }
            }
        }

        [WorkItem(138855)]
        [Description("Test InspectAssembly which will add all assemblies to AssembliesToImport")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyInspectAssemblyWillAddAllAssemblyiesToImportWithValidAssemblyPath()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                vmImportAssembly.AssembliesToImport.Clear();
                // Assert
                Assert.IsFalse(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly1.GetName().Name));
                Assert.IsFalse(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly2.GetName().Name));
                Assert.IsFalse(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly3.GetName().Name));

                // Set
                vmImportAssembly.InspectAssembly(assembly3.Location);
                // Assert
                Assert.AreEqual(3, vmImportAssembly.AssembliesToImport.Count);
                Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly1.GetName().Name));
                Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly2.GetName().Name));
                Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly3.GetName().Name));
            }
        }

        [WorkItem(138856)]
        [Description("Test InspectAssembly which will add all assemblies to AssembliesToImport wihtout Duplicate")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-Dif-Smoke")]
        [TestMethod()]
        public void VerifyInspectAssemblyAddAllAssembliesWithoutDuplicateToAssembliesToImport()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly2.Location);

                // Assert
                Assert.AreEqual(2, vmImportAssembly.AssembliesToImport.Count);
                Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly1.GetName().Name));
                Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly2.GetName().Name));

                using (var messageBox = new ImplementationOfType(typeof(MessageBoxService)))
                {
                    bool isAlreadyContains = false;
                    messageBox.Register(() => MessageBoxService.Show(Argument<string>.Any, Argument<string>.Any, MessageBoxButton.OK, MessageBoxImage.Information))
                      .Execute(() =>
                      {
                          isAlreadyContains = true;
                          return MessageBoxResult.None;
                      });
                    // Set
                    vmImportAssembly.InspectAssembly(assembly1.Location);

                    // Assert
                    Assert.IsTrue(isAlreadyContains);
                    Assert.AreEqual(2, vmImportAssembly.AssembliesToImport.Count);
                    Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly1.GetName().Name));
                    Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly2.GetName().Name));
                }
            }
        }

        [WorkItem(138858)]
        [Description("Test InspectAssembly when assemblyPath is null")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyInspectAssemblyWhenAssemblyPathIsNull()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);

                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.InspectAssembly(null);
                }).Throws<ArgumentNullException>();
            }
        }

        [WorkItem(138859)]
        [Description("Test InspectAssembly when assemblyPath is empty")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyInspectAssemblyWhenAssemblyPathIsEmpty()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);
                // Set and Verify
                ExpectException.That(() =>
                {
                    vmImportAssembly.InspectAssembly(string.Empty);
                }).Throws<ArgumentNullException>();
            }
        }


        [WorkItem(138860)]
        [Description("Test InspectAssembly when a valid assemblyPath is an invalid assembly")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyInspectAssemblyWhenAssemblyPathContainsInvalidAssembly()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                // Arrange
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);

                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.InspectAssembly(InvalidDllPath);
                }).Throws<AssemblyInspectionException>();
            }
        }

        [WorkItem(138861)]
        [Description("Test InspectAssembly when a valid assemblyPath is not a standard assembly")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyInspectAssemblyWhenAssemblyPathContainsNotStandardAssembly()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                // Set and Verify
                ExpectException.That(() =>
                {
                    // Set
                    vmImportAssembly.InspectAssembly(NotStandardDotNetDllPath);
                }).Throws<AssemblyInspectionException>();
            }
        }

        [WorkItem(139162)]
        [Description("Test ValidateLocation when passing valid ActivityAssemblyItemViewModel and location")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyValidateLocationReturnTrueWithValidParamaters()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                string assemblyNamePath = assembly1.Location;

                // set location to empty
                vmImportAssembly.AssembliesToImport
                        .ToList()
                        .ForEach(import =>
                        {
                            import.Location = string.Empty;
                        });

                // There is only on assembly in AssembliesToImport
                ActivityAssemblyItemViewModel currentActivityAssemblyVM = vmImportAssembly.AssembliesToImport.First();

                // Set
                bool result = ImportAssemblyViewModel_Accessor.ValidateLocation(currentActivityAssemblyVM, assemblyNamePath);

                // Verify
                Assert.IsTrue(result);
            }
        }

        [WorkItem(139164)]
        [Description("Test ValidateLocation when ActivityAssemblyItemViewModel is null")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyValidateLocationReturnFalseWithNullActivityAssemblyItemViewModel()
        {
            // Arrange
            string assemblyNamePath = assembly1.Location;

            // Set
            bool result = ImportAssemblyViewModel.ValidateLocation(null, assemblyNamePath, false);

            // Verify
            Assert.IsFalse(result);
        }

        [WorkItem(139165)]
        [Description("Test ValidateLocation when assemblyNamePath is invalid")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyValidateLocationReturnFalseWithInvalidActivityAssemblyName()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);
                string assemblyNamePath = assembly1.Location;

                // Set, Verify
                Assert.IsFalse(ImportAssemblyViewModel_Accessor.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), null, false));
                Assert.IsFalse(ImportAssemblyViewModel_Accessor.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), string.Empty, false));
                Assert.IsFalse(ImportAssemblyViewModel_Accessor.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), InvalidDllPath, false));
                Assert.IsFalse(ImportAssemblyViewModel_Accessor.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), NotStandardDotNetDllPath, false));
                Assert.IsFalse(ImportAssemblyViewModel_Accessor.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), "UnexistingFilePath", false));
            }
        }


        [WorkItem(139166)]
        [Description("Test ValidateLocation when assemblyNamePath contains wrong assembly")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyValidateLocationReturnFalseWhenAssemblyIsNotSame()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly1.Location);
                string differentAssemblyNamePath = assembly2.Location;
                string assembly1Name = vmImportAssembly.AssembliesToImport.First().Name;
                vmImportAssembly.AssembliesToImport.First().Name = "test1";

                // Set, Verify
                Assert.IsFalse(ImportAssemblyViewModel_Accessor.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), differentAssemblyNamePath));
                Assert.IsFalse(ImportAssemblyViewModel_Accessor.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), assembly1.Location));
                vmImportAssembly.AssembliesToImport.First().Name = assembly1Name;
                vmImportAssembly.AssembliesToImport.First().Version = "1.1.1.1";
                Assert.IsFalse(ImportAssemblyViewModel_Accessor.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), assembly1.Location));
            }
        }

        [WorkItem(139167)]
        [Description("Test ValidateLocation when passing invalid arguments")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        public void VerifyValidateLocationThrowExceptionWhenThrowOnFailureIsFalseWithInvalidArguments()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                string differentAssemblyNamePath = assembly2.Location;
                string assembly1Name = vmImportAssembly.AssembliesToImport.First().Name;
                vmImportAssembly.AssembliesToImport.First().Name = "test1";

                // Set, Verify
                ExpectException.That(() =>
                    {
                        ImportAssemblyViewModel.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), string.Empty);
                    }).Throws<UserFacingException>();

                ExpectException.That(() =>
                {
                    ImportAssemblyViewModel.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), null);
                }).Throws<UserFacingException>();

                ExpectException.That(() =>
                    {
                        ImportAssemblyViewModel.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), differentAssemblyNamePath);
                    }).Throws<UserFacingException>();

                ExpectException.That(() =>
                    {
                        ImportAssemblyViewModel.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), InvalidDllPath);
                    }).Throws<UserFacingException>();

                ExpectException.That(() =>
                    {
                        ImportAssemblyViewModel.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), NotStandardDotNetDllPath);
                    }).Throws<UserFacingException>();


                vmImportAssembly.AssembliesToImport.First().Name = assembly1Name;
                vmImportAssembly.AssembliesToImport.First().Version = "1.1.1.1";
                ExpectException.That(() =>
                    {
                        ImportAssemblyViewModel.ValidateLocation(vmImportAssembly.AssembliesToImport.First(), assembly1.Location);
                    }).Throws<UserFacingException>();
            }
        }

        [WorkItem(139168)]
        [Description("Test UpdateReferenceLocation when arguments are null or empty")]
        [Owner(TESTOWNER)]
        [TestCategory("Func-NoDif1-Smoke")]
        [TestMethod()]
        [Ignore]
        public void VerifyUpdateReferenceLocationThrowExceptionWhenActivityAssemblyItemIsNullOrAssemblyPathIsNUllOrEmpty()
        {
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly1.Location);
                string differentAssemblyNamePath = assembly2.Location;

                // Set, Verify
                ExpectException.That(() =>
                    {
                        vmImportAssembly.UpdateReferenceLocation(null, assembly1.Location);
                    }).Throws<ArgumentNullException>(err => Assert.AreEqual("activityAssemblyItem", err.ParamName));

                ExpectException.That(() =>
                    {
                        vmImportAssembly.UpdateReferenceLocation(vmImportAssembly.AssembliesToImport.First(), null);
                    }).Throws<ArgumentNullException>(err => Assert.AreEqual("assemblyPath", err.ParamName));
            }
        }


        [WorkItem(139169)]
        [Description("Test UpdateReferenceLocation when passing valid arguments")]
        [TestCategory("Func-Dif-Smoke")]
        [TestMethod()]
        [Owner("DiffRequired")]
        public void VerifyUpdateReferenceLocationForvalidActivityAssemblyItemAndAssemblyPath()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel_Accessor(assembly3.Location);
                ActivityAssemblyItemViewModel activityAssemblyItemVM = vmImportAssembly.AssembliesToImport.First();

                // Clear all items in AssembliesToImport
                vmImportAssembly.AssembliesToImport.Clear();
                using (var imp = new ImplementationOfType(typeof(ImportAssemblyViewModel)))
                {
                    imp.Register(() => ImportAssemblyViewModel.ValidateLocation(Argument<ActivityAssemblyItemViewModel>.Any, Argument<string>.Any, Argument<bool>.Any)).Return(true);
                    // Set
                    vmImportAssembly.UpdateReferenceLocation(activityAssemblyItemVM, assembly3.Location);

                    // Assert
                    Assert.AreEqual(3, vmImportAssembly.AssembliesToImport.Count);
                    Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly1.GetName().Name));
                    Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly2.GetName().Name));
                    Assert.IsTrue(vmImportAssembly.AssembliesToImport.Any(import => import.AssemblyName.Name == assembly3.GetName().Name));
                }
            }
        }


        [WorkItem(139170)]
        [Description("Test UpdateReferenceLocation when passing valid arguments for unsigned")]
        [TestCategory("Func-Dif-Smoke")]
        [TestMethod()]
        [Owner("DiffRequired")]
        public void VerifyUpdateReferenceLocationForvalidActivityAssemblyItemAndAssemblyPathForUnsigned()
        {
            TestUtilities.RegistUtilityGetCurrentWindow();
            using (new CachingIsolator())
            {
                var vmImportAssembly = new ImportAssemblyViewModel(assembly3.Location);
                ActivityAssemblyItemViewModel activityAssemblyItemVM = vmImportAssembly.AssembliesToImport.First();

                // Clear all items in AssembliesToImport
                vmImportAssembly.AssembliesToImport.Clear();

                using (var imp = new ImplementationOfType(typeof(ImportAssemblyViewModel)))
                {
                    imp.Register(() => ImportAssemblyViewModel.ValidateLocation(Argument<ActivityAssemblyItemViewModel>.Any, Argument<string>.Any, Argument<bool>.Any)).Throw(new ArgumentNullException());

                    // Set and Verify
                    ExpectException.That(() =>
                    {
                        // Set
                        vmImportAssembly.UpdateReferenceLocation(activityAssemblyItemVM, assembly3.Location);
                    }).Throws<ArgumentNullException>();
                }
            }
        }

    }
}
