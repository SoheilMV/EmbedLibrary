using dnlib.DotNet;
using System;
using System.Collections.Generic;

namespace dnlib.Inject
{
    /// <summary>
    ///     Context of the injection process.
    /// </summary>
    internal class InjectContext : ImportMapper
    {
        /// <summary>
        ///     The mapping of origin definitions to injected definitions.
        /// </summary>
        public readonly Dictionary<IDnlibDef, IDnlibDef> Maps = new Dictionary<IDnlibDef, IDnlibDef>();

        /// <summary>
        ///     The module which source type originated from.
        /// </summary>
        public readonly ModuleDef OriginModule;

        /// <summary>
        ///     The module which source type is being injected to.
        /// </summary>
        public readonly ModuleDef TargetModule;

        /// <summary>
        ///     The importer.
        /// </summary>
        readonly Importer importer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InjectContext" /> class.
        /// </summary>
        /// <param name="module">The origin module.</param>
        /// <param name="target">The target module.</param>
        public InjectContext(ModuleDef module, ModuleDef target)
        {
            OriginModule = module;
            TargetModule = target;
            importer = new Importer(target, ImporterOptions.TryToUseTypeDefs, default(GenericParamContext), this);
        }

        /// <summary>
        ///     Gets the importer.
        /// </summary>
        /// <value>The importer.</value>
        public Importer Importer
        {
            get { return importer; }
        }

        /// <inheritdoc />
        public override ITypeDefOrRef Map(ITypeDefOrRef source)
        {
            return null;
        }

        /// <inheritdoc />
        public override IField Map(FieldDef source)
        {
            if (Maps.ContainsKey(source))
                return (FieldDef)Maps[source];
            return null;
        }

        /// <inheritdoc />
        public override IMethod Map(MethodDef source)
        {
            if (Maps.ContainsKey(source))
                return (MethodDef)Maps[source];
            return null;
        }

        /// <inheritdoc />
        public TypeDef Map(TypeDef typeDef)
        {
            if (Maps.ContainsKey(typeDef))
                return (TypeDef)Maps[typeDef];
            return null;
        }

        /// <inheritdoc />
        public override MemberRef Map(MemberRef source)
        {
            return null;
        }

        /// <inheritdoc />
        public override TypeRef Map(Type source)
        {
            return null;
        }
    }
}
