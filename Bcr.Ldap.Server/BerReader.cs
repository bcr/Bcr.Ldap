using System.IO;

namespace Bcr.Ldap.Server;

class BerReader
{
    enum BerTagClass : byte
    {
        Application = 0x40,
        ContextSpecific = 0x80,
        Private = 0xC0,
        Constructed = 0x20,
        Universal = 0x00,
        Mask = 0xC0,
    }

    enum BerTagType : byte
    {
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

    public BerReader(Stream stream)
    {
        _stream = stream;
    }

    public byte ReadTag()
    {
        var tag = _stream.ReadByte();
        if (tag == -1)
        {
            throw new EndOfStreamException();
        }

        return (byte)tag;
    }

    public int ReadLength()
    {
        var length = _stream.ReadByte();
        if (length == -1)
        {
            throw new EndOfStreamException();
        }

        if (length < 0x80)
        {
            return length;
        }

        var count = length & 0x7F;
        if (count == 0)
        {
            throw new InvalidDataException("Indefinite length encoding not supported.");
        }

        var buffer = new byte[count];
        if (_stream.Read(buffer, 0, count) != count)
        {
            throw new EndOfStreamException();
        }

        return buffer.Aggregate(0, (total, next) => (total << 8) | next);
    }
}
