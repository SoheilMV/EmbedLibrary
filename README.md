# Embed Library .NET
This program allows you to make your project portable after compilation. It is fully compatible with all .NET obfuscation tools and does not interfere with their processes.



## :hotsprings:Features :
| Feature                        | ILMerge            | Costura.Fody       | EmbedLibrary      |
|--------------------------------|-------------------|--------------------|-------------------|
| **Merging Method**             | Merges assemblies into a single file | Embeds assemblies in resources and loads at runtime | Embeds assemblies in resources and loads at runtime |
| **Supports All .NET Versions** | ❌ No             | ✅ Yes             | ✅ Yes             |
| **Loading Method**             | Merged into output file | Loads from resources | Loads from resources |
| **Requires External Build Tools** | ✅ Yes (Requires ILMerge.exe to be run separately after compilation) | ✅ Yes (Requires Costura.Fody NuGet package, runs automatically during compilation) | ✅ Yes (Requires EmbedLibrary.exe to be run separately after compilation) |
| **Memory-Only Loading**        | ❌ No             | ✅ Yes             | ✅ Yes             |
| **Build Process Modification** | ✅ No modification (Runs post-build, allows further processing like obfuscation) | ❌ Yes (Modifies build process, no flexibility after compilation) | ✅ No modification (Runs post-build, allows further processing like obfuscation) |
| **Compression Before Embedding** | ❌ No            | ❌ No              | ✅ Yes             |
| **Security Against Extraction** | ❌ Weak (Assemblies are in the output file) | ⚠️ Medium (Can be extracted from resources) | ⚠️ Harder (Loads from memory but can be dumped) |
| **Manual & Automatic Library Selection** | ❌ No (Requires all assemblies to be merged manually) | ❌ No (Automatically embeds all referenced assemblies) | ✅ Yes (Supports both manual selection & auto-detection of referenced libraries) |







# :pushpin:Credits
- [dnlib](https://github.com/0xd4d/dnlib) (Reads and writes .NET assemblies and modules)