using MonoUtilities.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UtilsTests
{
    public class VersionInfoTests
    {
        [Fact]
        public void RC2_ShouldBeNewerThanRC1()
        {
            VersionInfo a = new("2022.01.24-RC2");
            VersionInfo b = new("2022.01.24-RC1");
            Assert.True(a > b);
        }

        [Fact]
        public void RC1_ShouldBeOlderThanRC2()
        {
            VersionInfo a = new("2022.01.24-RC1");
            VersionInfo b = new("2022.01.24-RC2");
            Assert.True(a < b);
        }

        [Fact]
        public void VersionPatch_ShouldBeNewer()
        {
            VersionInfo a = new("2022.01.24");
            VersionInfo b = new("2022.01.21");
            Assert.True(a > b);
        }

        [Fact]
        public void VersionMinor_ShouldBeNewer()
        {
            VersionInfo a = new("2022.02.24");
            VersionInfo b = new("2022.01.24");
            Assert.True(a > b);
        }

        [Fact]
        public void VersionMajor_ShouldBeNewer()
        {
            VersionInfo a = new("2023.01.24");
            VersionInfo b = new("2022.01.24");
            Assert.True(a > b);
        }

        [Fact]
        public void Version_ShouldBeSame()
        {
            VersionInfo a = new("2022.01.24");
            VersionInfo b = new("2022.01.24");
            Assert.True(a == b);
        }

        [Fact]
        public void SameVersion_ShouldBeEqualOrGreater()
        {
            VersionInfo a = new("2022.01.24");
            VersionInfo b = new("2022.01.24");
            Assert.True(a >= b);
        }

        [Fact]
        public void SameVersion_ShouldBeEqualOrLesser()
        {
            VersionInfo a = new("2022.01.24");
            VersionInfo b = new("2022.01.24");
            Assert.True(a <= b);
        }

        [Fact]
        public void VersionString_ShouldHaveCorrectFormat()
        {
            VersionInfo a = new("2022.01.24");            
            Assert.Equal("2022.01.24", a.ToString());
        }

        [Fact]
        public void VersionStringSuffix_ShouldHaveCorrectFormat()
        {
            VersionInfo a = new("2022.01.24-alpha");
            Assert.Equal("2022.01.24-alpha", a.ToString());
        }
    }
}
