// Copyright (c) Alexander Bocharov. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Xml;

namespace ABForge.Xml;

/// <summary>
/// Represents a high-performance serializable dictionary for XML operations.
/// </summary>
/// <remarks>
/// Optimized for Native AOT and low memory allocation.
/// </remarks>
public class SerializableDictionary : Dictionary<string, string?>
{
    /// <summary>
    /// Static helper for manual or generated serialization.
    /// </summary>
    public static void WriteXml(XmlWriter writer, SerializableDictionary dictionary)
    {
        ArgumentNullException.ThrowIfNull(writer);

        foreach (var (key, value) in dictionary)
        {
            if (value is not null)
            {
                writer.WriteElementString(key, value);
            }
        }
    }

    /// <summary>
    /// Static helper for manual or generated deserialization using Span-based access.
    /// </summary>
    public static void ReadXml(XmlReader reader, SerializableDictionary dictionary)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader.IsEmptyElement)
        {
            reader.Read();
            return;
        }

        reader.ReadStartElement();

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                string key = reader.LocalName;
                // Using modern ReadElementContentAsString for dictionary keys
                // but you can optimize further with ValueSpan if keys are known
                string value = reader.ReadElementContentAsString();
                dictionary[key] = value;
            }
            else
            {
                reader.Read();
            }
        }

        reader.ReadEndElement();
    }
}
