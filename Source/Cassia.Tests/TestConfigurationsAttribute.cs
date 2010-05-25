using NUnit.Framework;

namespace Cassia.Tests
{
    public class TestConfigurationsAttribute : ValueSourceAttribute
    {
        public TestConfigurationsAttribute() : base(typeof(TestSettings), "Configurations") {}
    }
}