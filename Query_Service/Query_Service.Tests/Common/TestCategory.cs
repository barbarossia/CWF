using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Query_Service.Tests.Common
{
    /// <summary>
    /// A class that contains the different test categories.
    /// </summary>
    class TestCategory
    {
        /// <summary>
        /// A constant that marks full test cases.
        /// </summary>
        public const string Full = "Full";

        /// <summary>
        /// A constant that marks build verification tests (BVTs).
        /// </summary>
        public const string BVT = "BVT";

        /// <summary>
        /// A constant that marks Smoke test cases.
        /// </summary>
        public const string Smoke = "Smoke";

        /// <summary>
        /// A constant that marks Unit test cases.
        /// </summary>
        public const string Unit = "Unit";
    }

    /// <summary>
    /// QualityGates definition.
    /// </summary>
    public static class QualityGates
    {
        /// <summary>
        /// this is a definition for functionality.
        /// </summary>
        public const string Functionality = "Functionality";


        /// <summary>
        /// this is a definition for Scalability tests.
        /// </summary>
        public const string Scalability = "Scalability";

    }
}
