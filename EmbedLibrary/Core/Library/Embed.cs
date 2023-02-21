using System.IO;
using System.IO.Compression;
using dnlib.Load;
using dnlib.Inject;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace EmbedLibrary.Core.Library
{
    public class Embed
    {
        public static void Execute(ModuleContext2 context, string[] librarys)
        {
            //Inject librarys to resource
            foreach (var path in librarys)
            {
                string name = Path.GetFileName(path);
                byte[] file = File.ReadAllBytes(path);
                byte[] zip = Compress(file);
                context.Resources.Add($"Embedded.{name}", zip);
            }

            //Add a new class
            TypeDef loadClass = new TypeDefUser("EmbedLibrary", "AssemblyLoader", context.Module.CorLibTypes.Object.TypeDefOrRef);
            context.Module.Types.Add(loadClass);

            //Inject functions into class
            var typeDef = context.GetTypeDef(typeof(Loader));
            var members = InjectHelper.Inject(typeDef, loadClass, context.Module);

            //Call the load method in the <Module> class
            var method = context.GetMethodDef(members, "Load"); //(MethodDef)members.Single(method => method.Name == "Load");
            context.GlobalTypeStaticConstructor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, method));
        }

        //https://stackoverflow.com/questions/10599596/compress-and-decompress-a-stream-with-compression-deflatestream
        private static byte[] Compress(byte[] data)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                    {
                        deflateStream.Write(data, 0, data.Length);
                    }
                    return memoryStream.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
