using Bcr.Ldap.Server;

namespace Bcr.Ldap.Server.Tests;

public class BerReaderTests
{
    [Fact]
    public void ReadTag()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 0x30 });
        var reader = new BerReader(stream);

        // Act
        var tag = reader.ReadTag();

        // Assert
        Assert.Equal(0x30, tag);
    }

    [Fact]
    public void ReadLength()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 0x30, 0x84, 0x01, 0x02, 0x03, 0x04 });
        var reader = new BerReader(stream);

        // Act
        reader.ReadTag();
        var length = reader.ReadLength();

        // Assert
        Assert.Equal(0x01020304, length);
    }

    [Fact]
    public void ReadShortLength()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 0x30, 0x02 });
        var reader = new BerReader(stream);

        // Act
        reader.ReadTag();
        var length = reader.ReadLength();

        // Assert
        Assert.Equal(0x02, length);
    }
}
