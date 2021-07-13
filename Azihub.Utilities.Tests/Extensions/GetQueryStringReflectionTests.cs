using Azihub.Utilities.Base.Extensions.Object;
using Azihub.Utilities.Base.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Azihub.Utilities.Tests.Extensions
{
    public class GetQueryStringReflectionTests : TestBase
    {
        public GetQueryStringReflectionTests(ITestOutputHelper outputHelper) : base(outputHelper) {}
        
        [Fact]
        public void GetQueryStringTest()
        {
            const string expected = "Name=John+Doe&Property=1&Value=Value+123!%40%23%24%25%5E%26*()";
            TestObject testObject = new TestObject()
            {
                Name = "John Doe",
                Property = 1,
                PropertyNull = null,
                Value = "Value 123!@#$%^&*()"
            };

            string result = testObject.GetQueryString();
            Output.WriteLine(@$"Input: ""{testObject.ToString()}"""+"\n"+
                @$"Result: ""{result}"""+"\n"+
                $@"Expected: ""{expected}""");

            Assert.Equal(expected, result);
        }
    }

    public class TestObject
    {
        public string Name { get; set; }
        public int Property { get; set; }
        public string PropertyNull { get; set; }
        public string Value{ get; set; }
        
        public new string ToString()
        {
            return $"Name: {Name} " +
                $"Property: {Property} " +
                $"PropertyNull: {PropertyNull} " +
                $"Value: {Value}";
        }
    }
}