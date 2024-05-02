using System.Text;

namespace Bcr.Ldap.Server;

class DerEncoder
{
    private Stack<MemoryStream> _streamStack = new();
    private MemoryStream _stream = new();

    public void AddTag(int tag)
    {
        _stream.WriteByte((byte)tag);
        if ((tag & (int) BerReader.BerTag.Constructed) != 0)
        {
            _streamStack.Push(_stream);
            _stream = new MemoryStream();
        }
    }

    public void AddLength(int length)
    {
        if (length < 0x80)
        {
            _stream.WriteByte((byte)length);
        }
        else if (length < 0x100)
        {
            _stream.WriteByte(0x81);
            _stream.WriteByte((byte)length);
        }
        else if (length < 0x10000)
        {
            _stream.WriteByte(0x82);
            _stream.WriteByte((byte)(length >> 8));
            _stream.WriteByte((byte)length);
        }
        else if (length < 0x1000000)
        {
            _stream.WriteByte(0x83);
            _stream.WriteByte((byte)(length >> 16));
            _stream.WriteByte((byte)(length >> 8));
            _stream.WriteByte((byte)length);
        }
        else
        {
            _stream.WriteByte(0x84);
            _stream.WriteByte((byte)(length >> 24));
            _stream.WriteByte((byte)(length >> 16));
            _stream.WriteByte((byte)(length >> 8));
            _stream.WriteByte((byte)length);
        }
    }

    public void EndConstructed()
    {
        var constructed = _stream.ToArray();
        _stream = _streamStack.Pop();
        AddLength(constructed.Length);
        _stream.Write(constructed);
    }

    public void AddInteger(int value, int tag = (int) (BerReader.BerTag.Universal | BerReader.BerTag.Primitive | BerReader.BerTag.Integer))
    {
        AddTag(tag);
        if (value < 0x100)
        {
            AddLength(1);
            _stream.WriteByte((byte)value);
        }
        else if (value < 0x10000)
        {
            AddLength(2);
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)value);
        }
        else if (value < 0x1000000)
        {
            AddLength(3);
            _stream.WriteByte((byte)(value >> 16));
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)value);
        }
        else
        {
            AddLength(4);
            _stream.WriteByte((byte)(value >> 24));
            _stream.WriteByte((byte)(value >> 16));
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)value);
        }
    }

    public void AddEnumerated(int value)
    {
        AddInteger(value, (int) (BerReader.BerTag.Universal | BerReader.BerTag.Primitive | BerReader.BerTag.Enumerated));
    }

    public void AddLdapString(string value)
    {
        var encoded = Encoding.UTF8.GetBytes(value);
        AddTag((int) (BerReader.BerTag.Universal | BerReader.BerTag.Primitive | BerReader.BerTag.OctetString));
        AddLength(encoded.Length);
        _stream.Write(encoded);
    }

    internal async Task WriteToStreamAsync(Stream stream, CancellationToken stoppingToken)
    {
        while (_streamStack.Count > 0)
        {
            EndConstructed();
        }
        await stream.WriteAsync(_stream.ToArray(), stoppingToken);
    }
}
