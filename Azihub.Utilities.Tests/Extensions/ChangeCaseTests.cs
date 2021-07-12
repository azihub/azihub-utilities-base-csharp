using Azihub.Utilities.Base.Extensions.String;
using Azihub.Utilities.Base.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Azihub.Utilities.Tests.Extensions
{
    public class StringUtilityTests : TestBase
    {
        public StringUtilityTests(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData("To Snake CaseBest", "to_snake_case")]
        [InlineData("second SnakeCase Try.", "second_snake_case_try")]
        public void ToSnakeCase(string original, string expected)
        {
            string calculated = original.ToSnakeCase();
            Output.WriteLine($"Original: {original}, calculated: {calculated} | expected: {expected}");
            Assert.Equal(expected, calculated);
        }
        
        [Theory]
        [InlineData("BITCOIN_ETHEREUM_COINS", "Bitcoin Ethereum Coins")]
        [InlineData("_1BITCOIN_ETHEREUM_COINS", "1Bitcoin Ethereum Coins")]
        public void ToConstantCase(string expected, string original)
        {
            string calculated = original.ToConstantCase(true);
            Output.WriteLine($"Original: {original}, calculated: {calculated} | expected: {expected}");
            Assert.Equal(expected, calculated);
        }
    }
}
