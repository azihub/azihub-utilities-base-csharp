using Azihub.Utilities.Base.PathUtilities;
using Azihub.Utilities.Base.PathUtilities.Exceptions;
using System.IO;
using Xunit;

namespace Azihub.Utilities.Tests.PathUtility
{
    public class PathUtilitiesTests
    {
        [Fact]
        public void PathInvalidDirectorySeparatorTest()
        {
            Assert.Throws<InvalidDirectorySeperatorCharException>(
                () => PathUtilities.RemoveFolder("/", 1, "\\\\")
                );
        }
        [Fact]
        public void PathTest()
        {
            string input = "/first/second/third/forth/fifth/";
            string expected = "/first/second/";
            string result = PathUtilities.RemoveFolder(input, 3, "/");
            Assert.Equal(expected, result);
        }
    }
}
