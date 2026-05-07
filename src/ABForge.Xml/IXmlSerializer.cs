// Copyright (c) Alexander Bocharov. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Xml;

namespace ABForge.Xml;

/// <summary>
/// Defines the contract for high-performance, AOT-safe XML serialization.
/// </summary>
/// <typeparam name="T">The type of the object to serialize or deserialize.</typeparam>
public interface IXmlSerializer<T> where T : class
{
    /// <summary>
    /// Writes the object state to the specified <see cref="XmlWriter"/>.
    /// </summary>
    /// <param name="writer">The target XML writer.</param>
    /// <param name="value">The object instance to serialize.</param>
    void WriteXml(XmlWriter writer, T value);

    /// <summary>
    /// Reads the XML document and restores the object state.
    /// </summary>
    /// <param name="reader">The source XML reader.</param>
    /// <returns>A restored instance of <typeparamref name="T"/>, or <see langword="null"/>.</returns>
    T? ReadXml(XmlReader reader);
}
