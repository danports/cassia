using System.Collections;
using System.Reflection;
using NUnit.Framework;

namespace Cassia.Tests
{
    public class TestConfigurationsAttribute : ParameterDataAttribute
    {
        public override IEnumerable GetData(ParameterInfo parameter)
        {
            return TestSettings.Configurations;
        }
    }
}