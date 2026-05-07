// Copyright (c) Alexander Bocharov. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace ABForge.Xml;

/// <summary>
/// Instructs the ABForge Source Generator to create an AOT-safe XML serializer for this class.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public sealed class GenerateXmlSerializerAttribute : Attribute { }
