// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Tests
{
    [TestFixture]
    public class ServiceProviderTests
    {
        private interface ITestSingleton1
        {
            public string PublicSigma { get; }
        }

        private class TestSingleton1 : ITestSingleton1
        {
            public string PublicSigma => "TestSingleton1";
        }

        private class TestSingleton2
        {
            private readonly TestSingleton1 testSingleton1;

            public TestSingleton2(TestSingleton1 testSingleton1)
            {
                this.testSingleton1 = testSingleton1;
            }

            public string PublicSigma => testSingleton1.PublicSigma + "qqq";
        }

        private class TestModule1
        {
            public TestModule1(TestSingleton1 testSingleton1)
            {
            }
        }

        [Test]
        public void SimpleFactory()
        {
            ServiceProvider services = new ServiceProvider();

            services.AddSingleton(services => new TestSingleton1());
            services.AddSingleton(services => new TestSingleton2(services.GetService<TestSingleton1>()));

            var service1 = services.GetService<TestSingleton1>();
            var service2 = services.GetService<TestSingleton2>();

            Assert.AreEqual("TestSingleton1", service1.PublicSigma);
            Assert.AreEqual("TestSingleton1qqq", service2.PublicSigma);
        }

        [Test]
        public void DependencyInjection()
        {
            ServiceProvider services = new ServiceProvider();

            services.AddSingleton<TestSingleton1>();
            services.AddSingleton<TestSingleton2>();

            var service1 = services.GetService<TestSingleton1>();
            var service2 = services.GetService<TestSingleton2>();

            Assert.AreEqual("TestSingleton1", service1.PublicSigma);
            Assert.AreEqual("TestSingleton1qqq", service2.PublicSigma);
        }

        [Test]
        public void WrongConstructorTest()
        {
            ServiceProvider services = new ServiceProvider();

            services.AddSingleton<TestSingleton2>();

            Assert.Throws<Exception>(() => services.GetService<TestSingleton2>());
        }

        [Test]
        public void AddServiceProviderTests()
        {
            ServiceProvider p1 = new ServiceProvider();
            p1.AddSingleton<TestSingleton1>();

            ServiceProvider p2 = new ServiceProvider();
            p2.AddServiceProvider(p1);

            var service = p2.GetService<TestSingleton1>();

            Assert.AreEqual("TestSingleton1", service.PublicSigma);
        }
    }
}
