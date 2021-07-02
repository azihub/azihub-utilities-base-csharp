using Azihub.Utilities.Base;
using Xunit;

namespace Azihub.Utilities.Tests.ValidatorsTests
{
    public class MobileTests
    {
        [Theory]
        [InlineData(16156182211, true)]   // US Mobile or Landline
        [InlineData(161561822, false)]   // US Mobile or Landline (Invalid)
        [InlineData(60121234567, true)]  // Malaysia Mobile
        [InlineData(601212345678, false)]  // Malaysia Mobile (Invalid)
        [InlineData(989121234567, true)]  // Iran Mobile
        [InlineData(98912123, false)]  // Iran Mobile (Invalid)
        [InlineData(31612345678, true)]  // Netherlands Mobile
        [InlineData(31203690388, false)]  // Netherlands Landline
        public void MobileValidationTest(ulong mobile, bool expectedResult)
        {
            Assert.Equal(expectedResult,
                    ValidationBase.IsValidMobileNumber(mobile)
                );
        }
    }
}
