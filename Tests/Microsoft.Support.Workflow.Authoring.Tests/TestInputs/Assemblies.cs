using System.Reflection;

namespace Microsoft.Support.Workflow.Authoring.Tests.TestInputs
{
    public static partial class TestInputs
    {
        public static class Assemblies
        {
            // not mutable, so we can cache them instead of regenerating
            static Assemblies()
            {
                TestInput_LibraryWithLongNamespace =
                    typeof (
                        CodeActivityLibraryaaaaaaaaaaaaaaaaaaaaaaaaaaaaa50aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa100aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa150aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa200aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa240
                            .CodeActivityWithLongNamespace).Assembly;
                //TestInput_NoActivityLibrary = Assembly.LoadFrom(@"TestInput_NoActivityLibrary.dll");
                TestInput_LibraryA = typeof (LibraryA.Activity1).Assembly;
                TestInput_LibraryB = typeof (LibraryB.Activity1).Assembly;
                TestInput_LibraryC = typeof (LibraryC.Activity1).Assembly;
                TestInput_LibraryD = typeof (ActivityLibrary1.Activity1).Assembly;

                TestInput_Lib1 = typeof(Testinput_Lib1.Activity1).Assembly;
                TestInput_Lib2 = typeof(TestInput_Lib2.Activity1).Assembly;
                TestInput_Lib3 = typeof(Testinput_Lib3.Activity1).Assembly;
            }

            //public static Assembly TestInput_Library1 { get; private set; }
            //public static Assembly TestInput_Library2 { get; private set; }
            //public static Assembly TestInput_Library3 { get; private set; }
            public static Assembly TestInput_LibraryWithLongNamespace { get; private set; }
            public static Assembly TestInput_NoActivityLibrary { get; private set; }
            public static Assembly TestInput_LibraryA { get; private set; }
            public static Assembly TestInput_LibraryB { get; private set; }
            public static Assembly TestInput_LibraryC { get; private set; }
            // C depends on A, B, D. None of them are signed.
            public static Assembly TestInput_LibraryD { get; private set; }

            // New library with valid version
            public static Assembly TestInput_Lib1 { get; private set; }
            public static Assembly TestInput_Lib2 { get; private set; }
            public static Assembly TestInput_Lib3 { get; private set; }
        }
    }
}
