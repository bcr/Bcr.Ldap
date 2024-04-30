using System.IO;
using System.Text;

namespace Bcr.Ldap.Server;

class BerReader
{
    public enum BerTag : byte
    {
        Application = 0x40,
        ContextSpecific = 0x80,
        Private = 0xC0,
        Constructed = 0x20,
        Primitive = 0x00,
        Universal = 0x00,

        EndOfContent = 0x00,
        Boolean = 0x01,
        Integer = 0x02,
        BitString = 0x03,
        OctetString = 0x04,
        Null = 0x05,
        ObjectIdentifier = 0x06,
        ObjectDescriptor = 0x07,
        External = 0x08,
        Real = 0x09,
        Enumerated = 0x0A,
        EmbeddedPdv = 0x0B,
        UTF8String = 0x0C,
        RelativeOid = 0x0D,
        Sequence = 0x10,
        Set = 0x11,
        NumericString = 0x12,
        PrintableString = 0x13,
        T61String = 0x14,
        VideotexString = 0x15,
        IA5String = 0x16,
        UtcTime = 0x17,
        GeneralizedTime = 0x18,
        GraphicString = 0x19,
        VisibleString = 0x1A,
        GeneralString = 0x1B,
        UniversalString = 0x1C,
        CharacterString = 0x1D,
        BMPString = 0x1E,
    }

    private readonly Stream _stream;
    private readonly CancellationToken _stoppingToken;
    private int _length;

    public BerReader(Stream stream, CancellationToken stoppingToken = default)
    {
        _stream = stream;
        _stoppingToken = stoppingToken;
    }

    private async Task<byte[]> ReadFully(int count)
    {
        var buffer = new byte[count];

        await _stream.ReadExactlyAsync(buffer, _stoppingToken);

        return buffer;
    }

    private async Task<byte[]> ReadElement()
    {
        return await ReadFully(_length);
    }

    private async Task<byte> ReadByte()
    {
        var buffer = await ReadFully(1);
        return buffer[0];
    }

    public async Task<byte> ReadTag()
    {
        var tag = await ReadByte();
        _length = await ReadLength();
        return tag;
    }

    public async Task ExpectTag(BerTag expected)
    {
        var tag = await ReadTag();
        var byteExpected = (byte) expected;
        if (tag != byteExpected)
        {
            throw new InvalidDataException($"Expected tag 0x{byteExpected:X2}, but got 0x{tag:X2}.");
        }
    }

    private async Task<int> ReadLength()
    {
        var length = await ReadByte();

        if (length < 0x80)
        {
            return length;
        }

        var count = length & 0x7F;
        if (count == 0)
        {
            throw new InvalidDataException("Indefinite length encoding not supported.");
        }

        var buffer = await ReadFully(count);

        return buffer.Aggregate(0, (total, next) => (total << 8) | next);
    }

    public async Task<int> ReadInteger()
    {
        var buffer = await ReadElement();

        return buffer.Aggregate(0, (total, next) => (total << 8) | next);
    }

    public async Task<string> ReadExpectedLdapString()
    {
        await ExpectTag(BerTag.Universal | BerTag.Primitive | BerTag.OctetString);
        var buffer = await ReadElement();

        return Encoding.UTF8.GetString(buffer);
    }

    public async Task<string> ReadExpectedLdapDN()
    {
        return await ReadExpectedLdapString();
    }

    public async Task SkipElement()
    {
        await ReadElement();
    }
}
