using MonoUtilities.Validation;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace UtilsTests
{
    public class ChecksumTests
    {
        [Fact]
        public void GetChecksum_ShouldReturnKnown()
        {
            using FileStream file = File.Create("checksum_temp");
            try
            {
                byte[] fileContents = new byte[] { 12, 16, 32, 15, 123, 12, 254, 216, 10, 2, 2, 8 };
                byte[] checksum = new byte[] { 210, 169, 152, 14, 138, 233, 2, 65, 240, 8, 94, 86, 215, 40, 74, 167, 23, 196, 23, 199, 203, 110, 90, 174, 241, 159, 155, 77, 51, 190, 185, 118 };
                file.Write(fileContents, 0, fileContents.Length);
                file.Close();
                Assert.True(ChecksumUtil.ValidateChecksum("checksum_temp", checksum));
            }
            finally 
            {
                File.Delete("checksum_temp");
            }
        }

        [Fact]
        public void CheckSumString_ShouldReturnKnown()
        {
            byte[] checksum = new byte[] { 210, 169, 152, 14, 138, 233, 2, 65, 240, 8, 94, 86, 215, 40, 74, 167, 23, 196, 23, 199, 203, 110, 90, 174, 241, 159, 155, 77, 51, 190, 185, 118 };
            Assert.Equal("D2A9980E8AE90241F0085E56D7284AA717C417C7CB6E5AAEF19F9B4D33BEB976", ChecksumUtil.ChecksumToString(checksum));
        }
    }
}
