# ABForge.Xml

[![Build Main](https://github.com/alexbocharov/ABForge.Xml/actions/workflows/build-main.yml/badge.svg)](https://github.com/alexbocharov/ABForge.Xml/actions/workflows/build-main.yml)
![NuGet Version](https://img.shields.io/nuget/vpre/ABForge.Xml?color=blue&label=nuget%20(preview))
![Target](https://img.shields.io/badge/target-.NET%2010-blue)

**ABForge.Xml** is a high-performance XML serialization library for modern .NET, designed with **Native AOT** and **Low-Allocation** patterns in mind. It provides a developer-friendly API similar to Newtonsoft.Json while maintaining the performance requirements of 2026's cloud-native applications.

## Key Features

- **Native AOT Ready**: No runtime Reflection.Emit. Uses source generation and static analysis.
- **Performance**: Optimized for `Span<byte>` and `IBufferWriter<byte>` to minimize GC pressure.
- **Aspire Integration**: Seamlessly connects with .NET Aspire orchestration for configuration and logging.
- **Modern API**: Simple `XmlConvert.Serialize` / `Deserialize` methods with flexible settings.

## Installation

```bash
dotnet add package ABForge.Xml
```

## Quick Start

```csharp
using ABForge.Xml;

var myObject = new MyData { Name = "ABForge", Value = 2026 };

// Serialize to byte array (AOT-safe)
byte[] xmlData = XmlConvert.Serialize(myObject);

// Deserialize back
var restored = XmlConvert.Deserialize<MyData>(xmlData);
```

## Roadmap

- [ ] Source Generator for XML schemas.
- [ ] Async streaming for large XML documents.
- [ ] .NET Aspire Component wrapper.

## License

All code in this repository is licensed under the [MIT License](LICENSE).
