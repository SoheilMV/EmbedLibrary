﻿using System.Linq;
using System.IO;
using System.IO.Compression;
using dnlib.Load;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace EmbedLibrary.Core.Library
{
    public class Embed
    {
        public static void Execute(AssemblyContext context, string[] librarys)
        {
            //Embedding libraries into resources for dynamic usage
            foreach (var path in librarys)
            {
                string name = Path.GetFileName(path);
                byte[] file = File.ReadAllBytes(path);
                byte[] zip = Compress(file);
                context.Resources.Add($"Embedded.{name}", zip);
            }

            //Class injection
            var typeDef = context.GetTypeDef(typeof(Loader));
            var copyTypeDef = context.Merge(typeDef, "AssemblyLoader", "EmbedLibrary");
            if (copyTypeDef != null)
            {
                //Call the load method in the <Module> class
                var method = copyTypeDef.Methods.Single(m => m.Name == "Load");
                context.GlobalTypeStaticConstructor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, method));
            }
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
