using MonoUtilities.Validation;
using System;
using Xunit;

namespace UtilsTests
{
    public class VersionUtilTests
    {
        [Fact]
        public void RC2_ShouldBeNewerThanRC1()
        {
            Assert.True(VersionUtil.IsVersionNewer("2022.01.24-RC2", "2022.01.24-RC1"));
        }

        [Fact]
        public void RC1_ShouldBeOlderThanRC2()
        {
            Assert.False(VersionUtil.IsVersionNewer("2022.01.24-RC1", "2022.01.24-RC2"));
        }

        [Fact]
        public void VersionPatch_ShouldBeNewer()
        {
            Assert.True(VersionUtil.IsVersionNewer("2022.01.24", "2022.01.21"));
        }

        [Fact]
        public void VersionMinor_ShouldBeNewer()
        {
            Assert.True(VersionUtil.IsVersionNewer("2022.02.24", "2022.01.24"));
        }

        [Fact]
        public void VersionMajor_ShouldBeNewer()
        {
            Assert.True(VersionUtil.IsVersionNewer("2023.01.24", "2022.01.24"));
        }

        [Fact]
        public void Version_ShouldBeSame()
        {
            Assert.False(VersionUtil.IsVersionNewer("2022.01.24", "2022.01.24"));
        }

        [Fact]
        public void VersionEmpty_ShouldBeOlder()
        {
            Assert.False(VersionUtil.IsVersionNewer("", "2022.01.24"));
        }

        [Fact]
        public void CompareEmpty_ShouldBeOlder()
        {
            Assert.True(VersionUtil.IsVersionNewer("2022.01.24", ""));
        }

        [Fact]
        public void ExtractMajor_ShouldReturnMajorPartOnly()
        {
            Assert.Equal("2022", VersionUtil.ExtractMajor("2022.01.24"));
        }

        [Fact]
        public void ExtractMinor_ShouldReturnMinorPartOnly()
        {
            Assert.Equal("01", VersionUtil.ExtractMinor("2022.01.24"));
        }

        [Fact]
        public void ExtractPatch_ShouldReturnPatchPartOnly()
        {
            Assert.Equal("24", VersionUtil.ExtractPatch("2022.01.24"));
        }

        [Fact]
        public void ExtractSuffix_ShouldReturnSuffixPartOnly()
        {
            Assert.Equal("alpha", VersionUtil.ExtractSuffix("2022.01.24-alpha"));
        }

        [Fact]
        public void ExtractEmptySuffix_ShouldReturnEmptyString()
        {
            Assert.Equal("", VersionUtil.ExtractSuffix("2022.01.24"));
        }

        [Fact]
        public void LeadingZeroes_ShouldBeIgnored()
        {
            Assert.True(VersionUtil.IsVersionNewer("2022.02.24", "2022.01.24"));
        }
    }
}
