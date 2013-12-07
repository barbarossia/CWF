using Microsoft.Support.Workflow.Authoring.CDS;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Tests.CDS
{
    [TestClass]
    public class QueryExtensionsTest
    {
        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void SortBySortsOnOneItem()
        {
            // Arrange
            var list = (new[] { 
                new MockQueryClass { Id = "B" }, new MockQueryClass { Id = "A" }, new MockQueryClass { Id = "C" }
            }).AsQueryable();

            // Act
            var result = list.SortBy(new[] { "Id" }, ListSortDirection.Ascending);

            // Assert
            Assert.AreEqual(result.ElementAt(0).Id, "A");
            Assert.AreEqual(result.ElementAt(1).Id, "B");
            Assert.AreEqual(result.ElementAt(2).Id, "C");
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void SortBySortsOnOnePropertyDescending()
        {
            // Arrange
            var list = (new[] { 
                new MockQueryClass { Id = "A" }, new MockQueryClass { Id = "B" }, new MockQueryClass { Id = "C" }
            }).AsQueryable();

            // Act
            var result = list.SortBy(new[] { "Id" }, ListSortDirection.Descending);

            // Assert
            Assert.AreEqual(result.ElementAt(0).Id, "C");
            Assert.AreEqual(result.ElementAt(1).Id, "B");
            Assert.AreEqual(result.ElementAt(2).Id, "A");
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void SortBySortsOnMultiplePropertyAscending()
        {
            // Arrange
            var list = (new[] { 
                new MockQueryClass { Id = "X", Name = "A" }, 
                new MockQueryClass { Id = "Z", Name = "" }, 
                new MockQueryClass { Id = "P", Name = null },
                new MockQueryClass { Id = "Q", Name = "R" }
            }).AsQueryable();

            // Act
            var result = list.SortBy(new[] { "Name", "Id" }, ListSortDirection.Ascending);

            // Assert
            Assert.AreEqual(result.ElementAt(0).Id, "X");
            Assert.AreEqual(result.ElementAt(1).Id, "P");
            Assert.AreEqual(result.ElementAt(2).Id, "Q");
            Assert.AreEqual(result.ElementAt(3).Id, "Z");
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void SortBySortsOnMultiplePropertyDescending()
        {
            // Arrange
            var list = (new[] { 
                new MockQueryClass { Id = "X", Name = "A" }, 
                new MockQueryClass { Id = "Z", Name = "" }, 
                new MockQueryClass { Id = "P", Name = null },
                new MockQueryClass { Id = "Q", Name = "R" }
            }).AsQueryable();

            // Act
            var result = list.SortBy(new[] { "Name", "Id" }, ListSortDirection.Descending);

            // Assert
            Assert.AreEqual(result.ElementAt(0).Id, "Z");
            Assert.AreEqual(result.ElementAt(1).Id, "Q");
            Assert.AreEqual(result.ElementAt(2).Id, "P");
            Assert.AreEqual(result.ElementAt(3).Id, "X");
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void SortBySortsOnMoreThanTwoProperties()
        {
            // Arrange
            var list = (new[] { 
                new MockQueryClass { Id = "X", Name = "A", Description = "D0" }, 
                new MockQueryClass { Id = "Z", Name = "" , Description = null }, 
                new MockQueryClass { Id = "P", Name = null, Description = "" },
                new MockQueryClass { Id = "Q", Name = "R", Description = "D1" }
            }).AsQueryable();

            // Act
            var result = list.SortBy(new[] { "Description", "Name", "Id" }, ListSortDirection.Ascending);

            // Assert
            Assert.AreEqual(result.ElementAt(0).Id, "X");
            Assert.AreEqual(result.ElementAt(1).Id, "Q");
            Assert.AreEqual(result.ElementAt(2).Id, "P");
            Assert.AreEqual(result.ElementAt(3).Id, "Z");
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void GetSortExpressionForSingleParameter()
        {
            // Arrange
            var source = new[] { new MockQueryClass() }.AsQueryable();
            var expected = source.OrderBy(p => p.Id).Expression as MethodCallExpression;

            // Act
            var expression = QueryExtensions.GetSortExpression(source, new[] { "Id" }, ListSortDirection.Ascending);

            AreExpressionsEqual(expected, expression);
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void GetSortExpressionForChainedParameter()
        {
            // Arrange
            var source = new[] { new MockQueryClass() }.AsQueryable();
            var expected = source.OrderBy(p => String.Concat(p.Id, p.Name)).Expression as MethodCallExpression;

            // Act
            var expression = QueryExtensions.GetSortExpression(source, new[] { "Id", "Name" }, ListSortDirection.Ascending);

            AreExpressionsEqual(expected, expression);
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void GetSortExpressionDescendingForChainedParameter()
        {
            // Arrange
            var source = new[] { new MockQueryClass() }.AsQueryable();
            var expected = source.OrderByDescending(p => String.Concat(p.Name, p.Id)).Expression as MethodCallExpression;

            // Act
            var expression = QueryExtensions.GetSortExpression(source, new[] { "Name", "Id" }, ListSortDirection.Descending);

            AreExpressionsEqual(expected, expression);
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void GetSortExpressionForIPackage()
        {
            // Arrange
            var PackageA = PackageUtility.CreatePackage("A");
            var source = new[] { PackageA }.AsQueryable();
            var expected = source.OrderBy(p => p.Id).Expression as MethodCallExpression;

            // Act
            var expression = QueryExtensions.GetSortExpression<IPackage>(source, new[] { "Id" }, ListSortDirection.Ascending);

            AreExpressionsEqual(expected, expression);
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetSortExpressionForKnownType()
        {
            // Arrange
            var knowTypes = new Type[] { typeof(int), typeof(string) };
            var source = new[] { "test" }.AsQueryable();
            var expected = source.OrderBy(p => p.Length).Expression as MethodCallExpression;

            // Act
            var expression = QueryExtensions.GetSortExpression(source, new[] { "Id" }, ListSortDirection.Ascending, knowTypes);

            AreExpressionsEqual(expected, expression);
        }

        private static void AreExpressionsEqual(MethodCallExpression a, MethodCallExpression b)
        {
            // An expression visitor should be the way to do this, but keeping it simple.

            Assert.AreEqual(a.Method, b.Method);

            var aLambda = (a.Arguments[1] as UnaryExpression).Operand as LambdaExpression;
            var bLambda = (b.Arguments[1] as UnaryExpression).Operand as LambdaExpression;


            if (aLambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                Assert.AreEqual((aLambda.Body as MemberExpression).Member, (bLambda.Body as MemberExpression).Member);
            }
            else
            {
                var aConcatCall = aLambda.Body as MethodCallExpression;
                var bConcatCall = bLambda.Body as MethodCallExpression;

                Assert.AreEqual((aConcatCall.Arguments[0] as MemberExpression).Member, (bConcatCall.Arguments[0] as MemberExpression).Member);
                Assert.AreEqual((aConcatCall.Arguments[1] as MemberExpression).Member, (bConcatCall.Arguments[1] as MemberExpression).Member);
            }
        }

        public class MockQueryClass
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }
    }
}
