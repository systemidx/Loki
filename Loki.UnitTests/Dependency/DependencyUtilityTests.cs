using System;
using Loki.Interfaces.Dependency;
using Loki.Server.Dependency;
using Xunit;

namespace Loki.UnitTests.Dependency
{
    public class DependencyUtilityTests
    {
        #region Fake Object Definitions

        private interface ITestInterface
        {
            Guid Id { get; }
        }

        private class TestClass : ITestInterface
        {
            public Guid Id { get; }

            public TestClass(Guid id)
            {
                Id = id;
            }
        }

        private struct TestStructure
        {
            public readonly Guid Id;

            public TestStructure(Guid id)
            {
                Id = id;
            }
        }

        #endregion

        /// <summary>
        /// Registers and resolves structs.
        /// </summary>
        [Fact]
        public void DependencyUtilityRegistersAndResolvesStructs()
        {
            Guid id = Guid.NewGuid();
            IDependencyUtility utility = new DependencyUtility();

            utility.Register<TestStructure>(new TestStructure(id));

            TestStructure resolvedObject = utility.Resolve<TestStructure>();

            Assert.Equal(id, resolvedObject.Id);
        }

        /// <summary>
        /// Registers and resolves classes.
        /// </summary>
        [Fact]
        public void DependencyUtilityRegistersAndResolvesClasses()
        {
            Guid id = Guid.NewGuid();
            IDependencyUtility utility = new DependencyUtility();

            utility.Register<TestClass>(new TestClass(id));

            TestClass resolvedObject = utility.Resolve<TestClass>();

            Assert.Equal(id, resolvedObject.Id);
        }

        /// <summary>
        /// Registers and resolves interfaces.
        /// </summary>
        [Fact]
        public void DependencyUtilityRegistersAndResolvesInterfaces()
        {
            Guid id = Guid.NewGuid();
            IDependencyUtility utility = new DependencyUtility();

            utility.Register<ITestInterface>(new TestClass(id));

            ITestInterface resolvedObject = utility.Resolve<ITestInterface>();

            Assert.Equal(id, resolvedObject.Id);
        }
    }
}
