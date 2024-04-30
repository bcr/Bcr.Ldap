using Bcr.Ldap.Server;

namespace Bcr.Ldap.Server.Tests;

public class BerReaderTests
{
    [Fact]
    public async void ReadTag()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 0x30, 0x01 });
        var reader = new BerReader(stream);

        // Act
        var tag = await reader.ReadTag();

        // Assert
        Assert.Equal((byte) (BerReader.BerTag.Universal | BerReader.BerTag.Constructed | BerReader.BerTag.Sequence), tag);
    }

    [Fact]
    public async void ExpectTag_WhenTagIsExpected_ShouldNotThrowException()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 0x30, 0x01 });
        var reader = new BerReader(stream);

        // Act
        await reader.ExpectTag(BerReader.BerTag.Universal | BerReader.BerTag.Constructed | BerReader.BerTag.Sequence);

        // Assert
        Assert.True(true);
    }

    [Fact]
    public void ExpectTag_WhenTagIsNotExpected_ShouldThrowException()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 0x30, 0x01 });
        var reader = new BerReader(stream);

        // Act
        // Assert
        Assert.ThrowsAsync<InvalidDataException>(async () => await reader.ExpectTag(BerReader.BerTag.Universal | BerReader.BerTag.Primitive | BerReader.BerTag.Integer));
    }
}
