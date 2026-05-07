# ABForge.Xml

[![Build Main](https://github.com/alexbocharov/ABForge.Xml/actions/workflows/build-main.yml/badge.svg)](https://github.com/alexbocharov/ABForge.Xml/actions/workflows/build-main.yml)
[![NuGet Version](https://img.shields.io/nuget/v/ABForge.Xml?color=blue&label=nuget)](https://www.nuget.org/packages/ABForge.Xml/)
[![Target](https://img.shields.io/badge/target-.NET%2010-blue)](https://dotnet.microsoft.com/)

**ABForge.Xml** is a high-performance XML serialization and deserialization library for modern .NET. It is designed from the ground up for **Native AOT** compatibility and cloud-native workloads.

## Features

- 🚀 **Native AOT Ready**: No reflection-based serialization at runtime. 
- 🛠️ **Source Generation**: Leverages Incremental Source Generators for compile-time safety and speed.
- 📉 **Low Allocation**: Optimized memory usage with `Span<T>` and `IBufferWriter<byte>` support.
- 📦 **.NET Aspire Ready**: Designed to work seamlessly in distributed environments.

## Getting Started

### Installation

Install the package via NuGet CLI:

```dotnetcli
dotnet add package ABForge.Xml
```

### Basic Usage (AOT-Safe)

1. Decorate your model with the `[GenerateXmlSerializer]` attribute:

```csharp
using ABForge.Xml;

[GenerateXmlSerializer]
public partial class UserProfile
{
    public required string Name { get; set; }
    public int Age { get; set; }
}
```

2. Serialize using the generated serializer:

```csharp
var profile = new UserProfile { Name = "Alexander", Age = 30 };

// Using the automatically generated UserProfileXmlSerializer
byte[] data = XmlConvert.Serialize(profile, new UserProfileXmlSerializer());
```

## Advanced Scenarios

The library supports `ReadOnlySpan<byte>` for ultra-fast deserialization:

```csharp
ReadOnlySpan<byte> xmlData = ...;
var profile = XmlConvert.Deserialize(xmlData, new UserProfileXmlSerializer());
```

## Feedback & Contributing

We welcome contributions! Please visit our GitHub repository:
[https://github.com/alexbocharov/ABForge.Xml](https://github.com/alexbocharov/ABForge.Xml)

## License

All code in this repository is licensed under the [MIT License](LICENSE).