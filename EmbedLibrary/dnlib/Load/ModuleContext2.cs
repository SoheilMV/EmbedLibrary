using System;
using System.Linq;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.Inject;

namespace dnlib.Load
{
	public class ModuleContext2
    {
        private ResourceManagement _resources = null;

        public AssemblyDef Assembly { get; private set; }
        public ModuleDef Module { get; private set; }
        public ModuleDefMD ModuleMD { get; private set; }
        public Importer Importer { get; private set; }
        public TypeDef EntryPointType { get; private set; }
        public MethodDef EntryPointMethod { get; private set; }
        public TypeDef GlobalType { get; private set; }
        public MethodDef GlobalTypeStaticConstructor { get; private set; }
        public ResourceManagement Resources { get { return _resources; } }

        public ModuleContext2(ModuleDefMD module)
		{
            ModuleMD = module;

            Assembly = module.Assembly;

            Module = Assembly.ManifestModule;

            Importer = new Importer(Module);

            EntryPointMethod = Module.EntryPoint;
            if(EntryPointMethod != null)
                EntryPointType = Module.EntryPoint.DeclaringType;

            GlobalType = Module.GlobalType;
            GlobalTypeStaticConstructor = GlobalType.FindOrCreateStaticConstructor();

            _resources = new ResourceManagement(this);
		}

        public TypeDef GetTypeDef(Type type)
        {
            var typeModule = ModuleDefMD.Load(type.Module);
            var typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(type.MetadataToken));
            return typeDef;
        }

        public MethodDef GetMethodDef(Type type, string name)
        {
            var typeDef = GetTypeDef(type);
            var method = typeDef.Methods.Single(m => m.Name == name);
            return method;
        }

        public MethodDef GetMethodDef(TypeDef typeDef, string name)
        {
            var method = typeDef.Methods.Single(m => m.Name == name);
            return method;
        }

        public MethodDef GetMethodDef(IEnumerable<IDnlibDef> members, string name)
        {
            var method = (MethodDef)members.Single(m => m.Name == name);
            return method;
        }

        public AssemblyRefUser GetAssemblyRefUser(string path)
        {
            var assembly = System.Reflection.Assembly.LoadFile(path);
            var assemblyName = assembly.GetName();
            var assemblyRefUser = new AssemblyRefUser(assemblyName);
            return assemblyRefUser;
        }

        public IEnumerable<AssemblyRef> GetAssemblyRefs()
        {
            return Module.GetAssemblyRefs();
        }

        public TypeDef CopyType(Type type)
        {
            var typeDef = GetTypeDef(type);
            var newTypeDef = InjectHelper.Inject(typeDef, Module);
            return newTypeDef;
        }

        public TypeDef CopyTypeDef(TypeDef typeDef)
        {
            var newTypeDef = InjectHelper.Inject(typeDef, Module);
            return newTypeDef;
        }

        public MethodDef CopyMethodDef(MethodDef methodDef)
        {
            var newMethodDef = InjectHelper.Inject(methodDef, Module);
            return newMethodDef;
        }

        public IEnumerable<IDnlibDef> Merge(Type type)
        {
            var typeDef = GetTypeDef(type);
            var newTypeDef = InjectHelper.Clone(typeDef);
            Module.Types.Add(newTypeDef);
            var members = InjectHelper.Inject(typeDef, newTypeDef, Module);
            return members;
        }

        public IEnumerable<IDnlibDef> Merge(TypeDef typeDef)
        {
            var newTypeDef = InjectHelper.Clone(typeDef);
            Module.Types.Add(newTypeDef);
            var members = InjectHelper.Inject(typeDef, newTypeDef, Module);
            return members;
        }
    }
}
