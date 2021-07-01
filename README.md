# Azihub Utilities (Base) with C#
Tools:
- Extension to get Sha256 of a string variable: `Extension.String.Sha256`
```C#
[Theory]
[InlineData("test", "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08")]
[InlineData("ComplexTestString!@#$5^7891234", "bfccc4e552d7641c39978cbad557f5035973082116a015b130181185f32d96ee")]
public void TestStringToSha256(string text, string expectedHash)
{
    string sha256 = text.GetSha256();
    OutputHelper.WriteLine($"Input: {text} hash rate is {sha256}");
    Assert.Equal(expectedHash, sha256);
}
```
- Extension to get Sha256 of a ByteArray variable: `Extensions.ByteArray.Sha256`
```C#
[Theory]
[InlineData("test", "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08")]
[InlineData("ComplexTestString!@#$5^7891234", "bfccc4e552d7641c39978cbad557f5035973082116a015b130181185f32d96ee")]
public void TestByteArrayToSha256(string text, string expectedHash)
{
    byte[] byteArray = Encoding.UTF8.GetBytes(text);
    string sha256 = byteArray.GetSha256();
    OutputHelper.WriteLine($"Input: {text} hash rate is {sha256}");
    Assert.Equal(expectedHash, sha256);
}
```
- `PathUtilities`
```C#
[Fact]
public void PathTest()
{
    string input = "/first/second/third/forth/fifth/";
    string expected = "/first/second/";
    string result = PathUtilities.RemoveFolder(input, 3, "/");
    Assert.Equal(expected, result);
}
```
- Validate Mobile number (to send SMS):
```C#
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
```
