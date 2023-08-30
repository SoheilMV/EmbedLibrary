using dnlib.DotNet;
using dnlib.DotNet.Writer;
using dnlib.Extensions;
using System;
using System.IO;

namespace dnlib.Load
{
    public class AssemblyLoader : IDisposable
    {
        public ModuleDefMD ModuleDefMD { get; private set; }

        public AssemblyLoader(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Missing file: {path}");
            }
            try
            {
                ModuleContext modCtx = ModuleDef.CreateModuleContext();
                ModuleDefMD = ModuleDefMD.Load(path, modCtx);
            }
            catch 
            {
                throw new FileNotFoundException($"Error for load module");
            }
        }

        public void Write(string path, bool writePdb = true)
        {
            try
            {
                if (ModuleDefMD.IsILOnly)
                {
                    var options = new ModuleWriterOptions(ModuleDefMD)
                    {
                        MetadataOptions = { Flags = MetadataFlags.PreserveAll },
                        PEHeadersOptions = { NumberOfRvaAndSizes = 0x10 },
                        Logger = DummyLogger.NoThrowInstance,
                        WritePdb = writePdb
                    };

                    ModuleDefMD.Write(path, options);
                }
                else
                {
                    var options = new NativeModuleWriterOptions(ModuleDefMD, true)
                    {
                        MetadataOptions = { Flags = MetadataFlags.PreserveAll },
                        Logger = DummyLogger.NoThrowInstance,
                        WritePdb = writePdb
                    };

                    ModuleDefMD.NativeWrite(path, options);
                }
            }
            catch
            {
                throw new FileNotFoundException($"Fail to save current module");
            }
        }

        public AssemblyContext GetAssemblyContext()
        {
            return ModuleDefMD.GetAssemblyContext();
        }

        public void Dispose()
        {
            ModuleDefMD.Dispose();
        }
    }
}
