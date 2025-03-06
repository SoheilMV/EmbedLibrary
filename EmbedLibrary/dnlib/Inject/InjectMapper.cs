using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace dnlib.Inject
{
    internal class InjectMapper : ImportMapper
    {
        private readonly Dictionary<ITypeDefOrRef, ITypeDefOrRef> _typeRefCache;
        private readonly Dictionary<Type, ITypeDefOrRef> _typeCache;
        private readonly Dictionary<IMemberRef, IMemberRef> _memberRefCache;
        private readonly Dictionary<IMethodDefOrRef, IMethodDefOrRef> _methodRefCache;
        private readonly Dictionary<TypeSig, TypeSig> _typeSigCache;
        private readonly Dictionary<MethodSig, MethodSig> _methodSigCache;
        private readonly Dictionary<PropertySig, PropertySig> _propertySigCache;
        private readonly Dictionary<FieldSig, FieldSig> _fieldSigCache;
        private readonly Dictionary<CallingConventionSig, CallingConventionSig> _callingConventionSigCache;
        private readonly Dictionary<ICustomAttributeType, ICustomAttributeType> _customAttributeTypeCache;
        private readonly Dictionary<IType, IType> _itypeCache;
        private readonly Dictionary<IMethod, IMethod> _methodCache;
        private readonly Dictionary<IField, IField> _fieldCache;

        public Dictionary<IMemberRef, IMemberRef> DnlibDefMaps { get; private set; }
        public ModuleDef TargetModule { get; private set; }
        public Importer Importer { get; private set; }

        public InjectMapper(ModuleDef target)
        {
            _typeRefCache = new Dictionary<ITypeDefOrRef, ITypeDefOrRef>();
            _typeCache = new Dictionary<Type, ITypeDefOrRef>();
            _memberRefCache = new Dictionary<IMemberRef, IMemberRef>();
            _methodRefCache = new Dictionary<IMethodDefOrRef, IMethodDefOrRef>();
            _typeSigCache = new Dictionary<TypeSig, TypeSig>();
            _methodSigCache = new Dictionary<MethodSig, MethodSig>();
            _propertySigCache = new Dictionary<PropertySig, PropertySig>();
            _fieldSigCache = new Dictionary<FieldSig, FieldSig>();
            _callingConventionSigCache = new Dictionary<CallingConventionSig, CallingConventionSig>();
            _customAttributeTypeCache = new Dictionary<ICustomAttributeType, ICustomAttributeType>();
            _itypeCache = new Dictionary<IType, IType>();
            _methodCache = new Dictionary<IMethod, IMethod>();
            _fieldCache = new Dictionary<IField, IField>();
            DnlibDefMaps = new Dictionary<IMemberRef, IMemberRef>();
            TargetModule = target ?? throw new ArgumentNullException(nameof(target));
            Importer = new Importer(target, ImporterOptions.TryToUseTypeDefs, default, this);
        }

        public override ITypeDefOrRef Map(ITypeDefOrRef source)
        {
            if (source == null) return null;

            if (_typeRefCache.TryGetValue(source, out var cachedType))
                return cachedType;

            var key = DnlibDefMaps.Keys.FirstOrDefault(k => k.MDToken == source.MDToken);
            return key != null ? DnlibDefMaps[key] as ITypeDefOrRef : null;
        }

        public override IMethod Map(MethodDef source)
        {
            if (source == null) return null;

            if (_methodRefCache.TryGetValue(source, out var cachedMethod))
                return cachedMethod;

            if (_memberRefCache.TryGetValue(source, out var cachedMember))
                return cachedMember as IMethod;

            var key = DnlibDefMaps.Keys.FirstOrDefault(k => k.MDToken == source.MDToken);
            return key != null ? DnlibDefMaps[key] as IMethod : null;
        }

        public override IField Map(FieldDef source)
        {
            if (source == null) return null;

            if (_memberRefCache.TryGetValue(source, out var cachedMember))
                return cachedMember as IField;

            var key = DnlibDefMaps.Keys.FirstOrDefault(k => k.MDToken == source.MDToken);
            return key != null ? DnlibDefMaps[key] as IField : null;
        }

        public override MemberRef Map(MemberRef source)
        {
            if (source == null) return null;

            if (_memberRefCache.TryGetValue(source, out var cachedMember))
                return cachedMember as MemberRef;

            var key = DnlibDefMaps.Keys.FirstOrDefault(k => k.MDToken == source.MDToken);
            return key != null ? DnlibDefMaps[key] as MemberRef : null;
        }

        public override ITypeDefOrRef Map(Type source)
        {
            if (source == null) return null;
            if (_typeCache.TryGetValue(source, out var cachedType))
                return cachedType;

            var key = DnlibDefMaps.Keys.FirstOrDefault(k => k.MDToken.ToInt32() == source.MetadataToken);
            var result = key != null ? DnlibDefMaps[key] as ITypeDefOrRef : null;
            if (result != null)
                _typeCache[source] = result;

            return result;
        }

        public ITypeDefOrRef ImportType(ITypeDefOrRef type)
        {
            if (type == null) return null;
            if (_typeRefCache.TryGetValue(type, out var cachedType))
                return cachedType;

            var importedType = Importer.Import(type);
            _typeRefCache[type] = importedType;
            return importedType;
        }

        public IMemberRef ImportMember(IMemberRef member)
        {
            if (member == null) return null;
            if (_memberRefCache.TryGetValue(member, out var cachedMember))
                return cachedMember;

            IMemberRef importedMember;
            switch (member)
            {
                case MemberRef memberRef:
                    importedMember = Importer.Import(memberRef);
                    break;
                case MethodDef methodDef:
                    importedMember = Importer.Import(methodDef);
                    break;
                case FieldDef fieldDef:
                    importedMember = Importer.Import(fieldDef);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported member type: {member.GetType().FullName}");
            }

            _memberRefCache[member] = importedMember;
            return importedMember;
        }

        public IMethodDefOrRef ImportMethod(IMethodDefOrRef method)
        {
            if (method == null) return null;
            if (_methodRefCache.TryGetValue(method, out var cachedMethod))
                return cachedMethod;

            var importedMethod = Importer.Import(method) as IMethodDefOrRef;
            _methodRefCache[method] = importedMethod;
            return importedMethod;
        }

        public TypeSig ImportTypeSig(TypeSig sig)
        {
            if (sig == null) return null;
            if (_typeSigCache.TryGetValue(sig, out var cachedSig))
                return cachedSig;

            var importedSig = Importer.Import(sig);
            _typeSigCache[sig] = importedSig;
            return importedSig;
        }

        public MethodSig ImportMethodSig(MethodSig sig)
        {
            if (sig == null) return null;
            if (_methodSigCache.TryGetValue(sig, out var cachedSig))
                return cachedSig;

            var importedSig = Importer.Import(sig);
            _methodSigCache[sig] = importedSig;
            return importedSig;
        }

        public PropertySig ImportPropertySig(PropertySig sig)
        {
            if (sig == null) return null;
            if (_propertySigCache.TryGetValue(sig, out var cachedSig))
                return cachedSig;

            var importedSig = Importer.Import(sig);
            _propertySigCache[sig] = importedSig;
            return importedSig;
        }

        public FieldSig ImportFieldSig(FieldSig sig)
        {
            if (sig == null) return null;
            if (_fieldSigCache.TryGetValue(sig, out var cachedSig))
                return cachedSig;

            var importedSig = Importer.Import(sig);
            _fieldSigCache[sig] = importedSig;
            return importedSig;
        }

        public CallingConventionSig ImportCallingConventionSig(CallingConventionSig sig)
        {
            if (sig == null) return null;
            if (_callingConventionSigCache.TryGetValue(sig, out var cachedSig))
                return cachedSig;

            var importedSig = Importer.Import(sig);
            _callingConventionSigCache[sig] = importedSig;
            return importedSig;
        }

        public ICustomAttributeType ImportCustomAttributeType(ICustomAttributeType attributeType)
        {
            if (attributeType == null) return null;
            if (_customAttributeTypeCache.TryGetValue(attributeType, out var cachedSig))
                return cachedSig;

            var importedSig = (ICustomAttributeType)Importer.Import(attributeType);
            _customAttributeTypeCache[attributeType] = importedSig;
            return importedSig;
        }

        public IType ImportType(IType type)
        {
            if (type == null) return null;
            if (_itypeCache.TryGetValue(type, out var cachedType))
                return cachedType;

            var importedType = Importer.Import(type);
            _itypeCache[type] = importedType;
            return importedType;
        }

        public IMethod ImportMethod(IMethod method)
        {
            if (method == null) return null;
            if (_methodCache.TryGetValue(method, out var cachedMethod))
                return cachedMethod;

            var importedMethod = Importer.Import(method);
            _methodCache[method] = importedMethod;
            return importedMethod;
        }

        public IField ImportField(IField field)
        {
            if (field == null) return null;
            if (_fieldCache.TryGetValue(field, out var cachedField))
                return cachedField;

            var importedField = Importer.Import(field);
            _fieldCache[field] = importedField;
            return importedField;
        }
    }
}
