using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Inject;
using dnlib.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnlib.Extensions
{
    public static class Extension
    {
        public static ModuleContext2 GetModuleContext2(this ModuleDefMD module)
        {
            return new ModuleContext2(module);
        }
    }
}
