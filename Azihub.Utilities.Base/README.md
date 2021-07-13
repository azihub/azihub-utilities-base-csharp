# C# Utilities

Set of extensions and tools:

## Extensions
### `String`.`ToSnakeCase()`
Change any case to snake case.
```C#
[Theory]
[InlineData("To Snake CaseBest", "to_snake_case")]
[InlineData("second SnakeCase Try.", "second_snake_case_try")]
public void ToSnakeCase(string original, string expected)
{
    string calculated = original.ToSnakeCase();
    Output.WriteLine($"Original: {original}, calculated: {calculated} | expected: {expected}");
    Assert.Equal(expected, calculated);
}
```

### `String`.`ToConstantCase()`
Change any case to Constant Case : `CONSTANT_NAMING_CONVENTION`
```C#
[Theory]
[InlineData("BITCOIN_ETHEREUM_COINS", "Bitcoin Ethereum Coins")]
[InlineData("_1BITCOIN_ETHEREUM_COINS", "1Bitcoin Ethereum Coins")]
public void ToConstantCase(string expected, string original)
{
    string calculated = original.ToConstantCase(true);
    Output.WriteLine($"Original: {original}, calculated: {calculated} | expected: {expected}");
    Assert.Equal(expected, calculated);
}
```

### `String`.`GetSha256()`
Get Sha256 value of a string variable.
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

### `byte[]`.`GetSha256()`
Get Sha256 value of a string variable.
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

### `object`.`GetQueryString()`
turn object into a Web GET query string.

Usage:
```C#
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
```
## Path Utilities

### RemoveFolder
Remove X number of folder back up.
```C#
string input = "/first/second/third/forth/fifth/";
string expected = "/first/second/";
string result = PathUtilities.RemoveFolder(input, 3, "/");
Assert.Equal(expected, result);
```

## Environment Variables `.env`
Default dotnet `Environment.GetEnvironmentVariable` is missing loading data from .env,
below will load the file while skipping comments.
```C#
string varName1 = "TEST_VAR1";
string varValue1 = "TEST_VALUE1";
string varName2 = "TEST_VAR2";
string varValue2 = "TEST_VALUE2";
string testEnv = "# COMMENT TEST\n" +
                $"{varName1}={varValue1}\n"+
                $"{varName2}={varValue2}\n";

string tempEnvFilePath = Path.GetTempPath() + ".env";
File.WriteAllText(tempEnvFilePath, testEnv);
#endregion

// Conduct test
DotEnv.Load(tempEnvFilePath);
string testVar1 = Environment.GetEnvironmentVariable(varName1);
string testVar2 = Environment.GetEnvironmentVariable(varName2);

// evaluate result
Assert.Equal(varValue1, testVar1);
Assert.Equal(varValue2, testVar2);
```

### Get Environment Variables as Object with Values
Using `DotEnv`.`Load<T>()` you can get object of variables set in environment variables.

```C#

// Instance of Settings
public class WorkerSettings
{
    public string StringValue { get; set; }
    public int IntValue  { get; set; }
    public uint UIntValue  { get; set; }
    public long LongValue  { get; set; }
    public ulong ULongValue  { get; set; }
}

#region Test Env Vars
private const string STRING_VALUE   = "TEST";
private const int    INT_VALUE      = -2147483648;
private const uint   U_INT_VALUE    = 4294967295;
private const long   LONG_VALUE     = 9223372036854775807;
private const ulong  U_LONG_VALUE   = 18446744073709551615;
#endregion
[Fact]
public void GetEnvModelType()
{
    Environment.SetEnvironmentVariable(nameof(STRING_VALUE), STRING_VALUE);
    Environment.SetEnvironmentVariable(nameof(INT_VALUE)   , INT_VALUE.ToString() );
    Environment.SetEnvironmentVariable(nameof(U_INT_VALUE) , U_INT_VALUE.ToString() );
    Environment.SetEnvironmentVariable(nameof(LONG_VALUE)  , LONG_VALUE.ToString());
    Environment.SetEnvironmentVariable(nameof(U_LONG_VALUE), U_LONG_VALUE.ToString());

    WorkerSettings workerSettings = DotEnv.Load<WorkerSettings>();

    Assert.Equal(STRING_VALUE, workerSettings.StringValue);
    Assert.Equal(INT_VALUE, workerSettings.IntValue);
    Assert.Equal(U_INT_VALUE, workerSettings.UIntValue);
    Assert.Equal(LONG_VALUE, workerSettings.LongValue);
    Assert.Equal(U_LONG_VALUE, workerSettings.ULongValue);
}
```