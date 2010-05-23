using NUnit.Framework;

namespace Cassia.Tests
{
    public class TestServersAttribute : ValueSourceAttribute
    {
        public TestServersAttribute() : base(typeof(TestSettings), "Servers") {}
    }
}