// Copyright (c) Alexander Bocharov. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;
using System.Xml;

namespace ABForge.Xml.Tests;

public class SerializableDictionaryTests
{
    [Fact]
    public void ReadXml_ShouldDeserializeXmlToDictionary_AotSafe()
    {
        // Arrange
        var xml = "<Root><key1>value1</key1><key2>value2</key2></Root>";
        var bytes = Encoding.UTF8.GetBytes(xml);
        var dictionary = new SerializableDictionary();

        using var stream = new MemoryStream(bytes);
        using var reader = XmlReader.Create(stream);

        // Move to <Root> element
        reader.MoveToContent();

        // Act
        SerializableDictionary.ReadXml(reader, dictionary);

        // Assert
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("value1", dictionary["key1"]);
        Assert.Equal("value2", dictionary["key2"]);
    }

    [Fact]
    public void WriteXml_ShouldSerializeContent_InsideProvidedRoot()
    {
        // Arrange
        var dictionary = new SerializableDictionary
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = false,
            OmitXmlDeclaration = true
        });

        // Act
        // Caller (test) is responsible for the root element
        xmlWriter.WriteStartElement("Dictionary");
        SerializableDictionary.WriteXml(xmlWriter, dictionary);
        xmlWriter.WriteEndElement();
        xmlWriter.Flush();

        // Assert
        var result = stringWriter.ToString();
        Assert.Equal("<Dictionary><key1>value1</key1><key2>value2</key2></Dictionary>", result);
    }

    [Fact]
    public void RoundTrip_ThroughXmlConvert_WorksCorrectly()
    {
        // Arrange
        var original = new SerializableDictionary { { "test", "data" } };
        var serializer = new DictionarySerializerMock();

        // Act
        var bytes = XmlConvert.Serialize(original, serializer);
        Assert.NotNull(bytes);
        var restored = XmlConvert.Deserialize(bytes.AsSpan(), serializer);

        // Assert
        Assert.NotNull(restored);
        Assert.True(restored.ContainsKey("test"), "Restored dictionary should contain the key 'test'");
        Assert.Equal(original["test"], restored["test"]);
    }

    /// <summary>
    /// Mock of what the ABForge Source Generator would produce for a class property.
    /// It handles the property-named element and delegates content to the dictionary.
    /// </summary>
    private sealed class DictionarySerializerMock : IXmlSerializer<SerializableDictionary>
    {
        private const string RootName = "Data";

        public void WriteXml(XmlWriter writer, SerializableDictionary value)
        {
            writer.WriteStartElement(RootName);
            SerializableDictionary.WriteXml(writer, value);
            writer.WriteEndElement();
        }

        public SerializableDictionary? ReadXml(XmlReader reader)
        {
            var dict = new SerializableDictionary();

            if (reader.IsStartElement(RootName))
            {
                SerializableDictionary.ReadXml(reader, dict);
            }

            return dict;
        }
    }
}
