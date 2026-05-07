// Copyright (c) Alexander Bocharov. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ABForge.Xml;

/// <summary>
/// Provides ultra-high-performance, AOT-compliant methods for XML serialization using modern .NET memory patterns.
/// </summary>
public static class XmlConvert
{
    private static readonly UTF8Encoding Utf8NoBom = new(false);

    private static readonly XmlWriterSettings DefaultWriterSettings = new()
    {
        Encoding = Utf8NoBom,
        Indent = true,
        OmitXmlDeclaration = false,
        CloseOutput = false
    };

    /// <summary>
    /// Serializes an object directly into a buffer writer to avoid extra byte array copies.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="writer">The buffer writer to write to.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="serializer">The AOT-generated serializer implementation.</param>
    public static void Serialize<T>(IBufferWriter<byte> writer, T value, IXmlSerializer<T> serializer) where T : class
    {
        ArgumentNullException.ThrowIfNull(writer);
        if (value is null)
        {
            return;
        }

        // Wrap the IBufferWriter in a stream adapter to write directly to the target buffer
        using var stream = new BufferWriterStream(writer);
        using var xmlWriter = XmlWriter.Create(stream, DefaultWriterSettings);

        serializer.WriteXml(xmlWriter, value);
        xmlWriter.Flush();
    }

    /// <summary>
    /// Serializes an object to a byte array using pooled memory where possible.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="serializer">The AOT-generated serializer implementation.</param>
    /// <returns>A byte array containing the XML document.</returns>
    public static byte[]? Serialize<T>(T value, IXmlSerializer<T> serializer) where T : class
    {
        if (value is null)
        {
            return null;
        }

        var writer = new ArrayBufferWriter<byte>(1024);
        Serialize(writer, value, serializer);

        return writer.WrittenSpan.ToArray();
    }

    /// <summary>
    /// Deserializes the XML document from a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="value">The byte array span containing the XML document.</param>
    /// <param name="serializer">The AOT-generated serializer implementation.</param>
    /// <returns>The deserialized object.</returns>
    public static unsafe T? Deserialize<T>(ReadOnlySpan<byte> value, IXmlSerializer<T> serializer) where T : class
    {
        if (value.IsEmpty)
        {
            return default;
        }

        // Use UnmanagedMemoryStream to wrap the Span without copying
        fixed (byte* pValue = value)
        {
            using var stream = new UnmanagedMemoryStream(pValue, value.Length);
            using var xmlReader = XmlReader.Create(stream);
            return serializer.ReadXml(xmlReader);
        }
    }

    /// <summary>
    /// Serializes the specified object using classic reflection-based serialization.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A byte array containing the XML document.</returns>
    [RequiresDynamicCode("Uses reflection-based XmlSerializer. Use IXmlSerializer overload for Native AOT.")]
    [RequiresUnreferencedCode("Reflection-based serialization is not trim-safe.")]
    public static byte[]? Serialize<T>(T value)
    {
        if (value is null)
        {
            return null;
        }

        var xmlSerializer = new XmlSerializer(typeof(T));
        var writer = new ArrayBufferWriter<byte>(1024);

        using (var stream = new BufferWriterStream(writer))
        using (var xmlWriter = XmlWriter.Create(stream, DefaultWriterSettings))
        {
            xmlSerializer.Serialize(xmlWriter, value);
        }

        return writer.WrittenSpan.ToArray();
    }

    /// <summary>
    /// Internal adapter to expose <see cref="IBufferWriter{T}"/> as a <see cref="Stream"/>.
    /// </summary>
    private sealed class BufferWriterStream(IBufferWriter<byte> writer) : Stream
    {
        public override void Write(byte[] buffer, int offset, int count) => writer.Write(buffer.AsSpan(offset, count));
        public override void Flush() { }
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
