using System.Linq;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

namespace dnlib.Inject
{
    public class Injection
    {
        private readonly InjectMapper _mapper;
        private readonly HashSet<TypeDef> _processedTypes;

        public Injection(ModuleDef target)
        {
            _mapper = new InjectMapper(target);
            _processedTypes = new HashSet<TypeDef>();
        }

        public void Inject(TypeDef source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (_processedTypes.Contains(source)) return;

            try
            {

                var newTypeDef = Init(source);
                _mapper.TargetModule.Types.Add(newTypeDef);

                // Fix references after injection
                FixReferences(newTypeDef);
                FixGenericParameters(newTypeDef);

                _processedTypes.Add(source);

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to inject type {source.FullName}", ex);
            }
        }

        public bool TryInject(TypeDef source, out TypeDef target)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (_processedTypes.Contains(source))
            {
                target = null;
                return false;
            }
            else
            {
                try
                {
                    target = Init(source);
                    _mapper.TargetModule.Types.Add(target);

                    // Fix references after injection
                    //FixReferences(target);
                    //FixGenericParameters(target);

                    _processedTypes.Add(source);
                    return true;
                }
                catch
                {
                    target = null;
                    return false;
                }
            }
        }

        private TypeDef Init(TypeDef source)
        {
            TypeDef newTypeDef;
            IMemberRef existing;
            if (!_mapper.DnlibDefMaps.TryGetValue(source, out existing))
            {
                newTypeDef = Clone(source);
                _mapper.DnlibDefMaps[source] = newTypeDef;

                foreach (TypeDef nestedType in source.NestedTypes)
                {
                    if (!_processedTypes.Contains(nestedType))
                    {
                        var newNestedType = Init(nestedType);
                        newTypeDef.NestedTypes.Add(newNestedType);
                        _processedTypes.Add(nestedType);
                    }
                }

                foreach (var event_ in source.Events)
                {
                    var newEventDef = Clone(event_);
                    newTypeDef.Events.Add(newEventDef);
                    _mapper.DnlibDefMaps[event_] = newEventDef;
                }

                foreach (MethodDef method in source.Methods)
                {
                    var newMethodDef = Clone(method);
                    newTypeDef.Methods.Add(newMethodDef);
                    _mapper.DnlibDefMaps[method] = newMethodDef;
                }

                foreach (FieldDef field in source.Fields)
                {
                    var newFieldDef = Clone(field);
                    newTypeDef.Fields.Add(newFieldDef);
                    _mapper.DnlibDefMaps[field] = newFieldDef;
                }

                foreach (var property in source.Properties)
                {
                    var newPropertyDef = Clone(property);
                    newTypeDef.Properties.Add(newPropertyDef);
                    _mapper.DnlibDefMaps[property] = newPropertyDef;
                }

                

                Copy(source, true);
            }
            else
                newTypeDef = (TypeDef)existing;

            return newTypeDef;
        }

        private void FixReferences(TypeDef target)
        {
            foreach (var method in target.Methods)
            {
                if (method.HasBody)
                {
                    foreach (var instruction in method.Body.Instructions)
                    {
                        if (instruction.Operand is ITypeDefOrRef typeRef)
                        {
                            instruction.Operand = _mapper.ImportType(typeRef);
                        }
                        else if (instruction.Operand is IMemberRef memberRef)
                        {
                            IMemberRef importedMemberRef;
                            try
                            {
                                importedMemberRef = _mapper.ImportMember(memberRef);
                            }
                            catch (Exception)
                            {
                                // If import fails, try to preserve the original reference
                                importedMemberRef = memberRef;
                            }
                            instruction.Operand = importedMemberRef;
                        }
                    }
                }

                // Fix method references
                if (method.HasOverrides)
                {
                    var newOverrides = new List<MethodOverride>();
                    foreach (var methodOverride in method.Overrides)
                    {
                        try
                        {
                            var importedMethodDeclaration = _mapper.ImportMethod(methodOverride.MethodDeclaration);
                            if (importedMethodDeclaration != null)
                            {
                                newOverrides.Add(new MethodOverride(method, importedMethodDeclaration));
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    method.Overrides.Clear();
                    foreach (var methodOverride in newOverrides)
                    {
                        method.Overrides.Add(methodOverride);
                    }
                }
            }

            foreach (var nestedType in target.NestedTypes)
            {
                FixReferences(nestedType);
            }
        }

        private void FixGenericParameters(TypeDef target)
        {
            foreach (var method in target.Methods)
            {
                if (method.HasGenericParameters)
                {
                    foreach (var genericParam in method.GenericParameters)
                    {
                        foreach (var constraint in genericParam.GenericParamConstraints)
                        {
                            constraint.Constraint = _mapper.ImportType(constraint.Constraint);
                        }
                    }
                }
            }

            if (target.HasGenericParameters)
            {
                foreach (var genericParam in target.GenericParameters)
                {
                    foreach (var constraint in genericParam.GenericParamConstraints)
                    {
                        constraint.Constraint = _mapper.ImportType(constraint.Constraint);
                    }
                }
            }

            foreach (var nestedType in target.NestedTypes)
            {
                FixGenericParameters(nestedType);
            }
        }

        #region Clone

        /// <summary>
        ///     Clones the specified origin TypeDef.
        /// </summary>
        /// <param name="source">The origin TypeDef.</param>
        /// <returns>The cloned TypeDef.</returns>
        private TypeDefUser Clone(TypeDef source)
        {
            var typeDef = new TypeDefUser(source.Namespace, source.Name, _mapper.ImportType(source.BaseType))
            {
                Attributes = source.Attributes,
                ClassLayout = source.ClassLayout != null
                    ? new ClassLayoutUser(source.ClassLayout.PackingSize, source.ClassSize)
                    : null
            };

            foreach (GenericParam genericParam in source.GenericParameters)
                typeDef.GenericParameters.Add(CloneGenericParam(genericParam));

            return typeDef;
        }

        /// <summary>
        ///     Clones the specified origin MethodDef.
        /// </summary>
        /// <param name="source">The origin MethodDef.</param>
        /// <returns>The cloned MethodDef.</returns>
        private MethodDefUser Clone(MethodDef source)
        {
            var methodDef = new MethodDefUser(source.Name, _mapper.ImportMethodSig(source.MethodSig), source.ImplAttributes, source.Attributes);

            foreach (GenericParam genericParam in source.GenericParameters)
                methodDef.GenericParameters.Add(CloneGenericParam(genericParam));

            return methodDef;
        }

        /// <summary>
        ///     Clones the specified origin MethodDef.
        /// </summary>
        /// <param name="source">The origin MethodDef.</param>
        /// <returns>The cloned MethodDef.</returns>
        private PropertyDefUser Clone(PropertyDef source)
            => new PropertyDefUser(source.Name, _mapper.ImportPropertySig(source.PropertySig), source.Attributes);

        /// <summary>
        ///     Clones the specified origin FieldDef.
        /// </summary>
        /// <param name="origin">The origin FieldDef.</param>
        /// <returns>The cloned FieldDef.</returns>
        private FieldDefUser Clone(FieldDef source)
            => new FieldDefUser(source.Name, _mapper.ImportFieldSig(source.FieldSig), source.Attributes);

        private EventDefUser Clone(EventDef source)
            => new EventDefUser(source.Name, _mapper.ImportType(source.EventType), source.Attributes);

        private GenericParamUser CloneGenericParam(GenericParam source)
            => new GenericParamUser(source.Number, source.Flags, source.Name);

        #endregion

        #region Copy

        /// <summary>
        ///     Copies the information to the injected definitions.
        /// </summary>
        /// <param name="source">The origin TypeDef.</param>
        /// <param name="copySelf">if set to <c>true</c>, copy information of <paramref name="typeDef" />.</param>
        private void Copy(TypeDef source, bool copySelf)
        {
            if (copySelf)
                CopyTypeAttributes(source);

            foreach (TypeDef nestedType in source.NestedTypes)
                Copy(nestedType, true);

            foreach (MethodDef method in source.Methods)
                CopyMethodDef(method);

            foreach (FieldDef field in source.Fields)
                CopyFieldDef(field);

            foreach (PropertyDef propertyDef in source.Properties)
                CopyPropertyDef(propertyDef);

            foreach (EventDef eventDef in source.Events)
                    CopyEventDef(eventDef);
        }

        /// <summary>
        ///     Copies the information from the origin type to injected type.
        /// </summary>
        /// <param name="typeDef">The origin TypeDef.</param>
        private void CopyTypeAttributes(TypeDef source)
        {
            var newTypeDef = (TypeDef)_mapper.DnlibDefMaps[source];

            newTypeDef.BaseType = _mapper.Importer.Import(source.BaseType);

            foreach (InterfaceImpl iface in source.Interfaces)
                newTypeDef.Interfaces.Add(new InterfaceImplUser(_mapper.Importer.Import(iface.Interface)));
        }

        /// <summary>
        ///     Copies the information from the origin method to injected method.
        /// </summary>
        /// <param name="methodDef">The origin MethodDef.</param>
        private void CopyMethodDef(MethodDef methodDef)
        {
            var newMethodDef = (MethodDef)_mapper.DnlibDefMaps[methodDef];

            newMethodDef.Signature = _mapper.ImportCallingConventionSig(methodDef.Signature);
            newMethodDef.Parameters.UpdateParameterTypes();

            if (methodDef.ImplMap != null)
                newMethodDef.ImplMap = new ImplMapUser(new ModuleRefUser(_mapper.TargetModule, methodDef.ImplMap.Module.Name), methodDef.ImplMap.Name, methodDef.ImplMap.Attributes);

            foreach (CustomAttribute ca in methodDef.CustomAttributes)
                newMethodDef.CustomAttributes.Add(new CustomAttribute(_mapper.ImportCustomAttributeType(ca.Constructor)));

            if (methodDef.HasBody)
            {
                newMethodDef.Body = new CilBody(methodDef.Body.InitLocals, new List<Instruction>(), new List<ExceptionHandler>(), new List<Local>());
                newMethodDef.Body.MaxStack = methodDef.Body.MaxStack;

                var bodyMap = new Dictionary<object, object>();

                foreach (Local local in methodDef.Body.Variables)
                {
                    var newLocal = new Local(_mapper.ImportTypeSig(local.Type));
                    newMethodDef.Body.Variables.Add(newLocal);
                    bodyMap[local] = newLocal;
                }

                foreach (Instruction instr in methodDef.Body.Instructions)
                {
                    var newInstr = new Instruction(instr.OpCode, instr.Operand);
                    if (newInstr.Operand is IType)
                        newInstr.Operand = _mapper.ImportType((IType)newInstr.Operand);

                    else if (newInstr.Operand is IMethod)
                        newInstr.Operand = _mapper.ImportMethod((IMethod)newInstr.Operand);

                    else if (newInstr.Operand is IField)
                        newInstr.Operand = _mapper.ImportField((IField)newInstr.Operand);

                    newMethodDef.Body.Instructions.Add(newInstr);
                    bodyMap[instr] = newInstr;
                }

                foreach (Instruction instr in newMethodDef.Body.Instructions)
                {
                    if (instr.Operand != null && bodyMap.ContainsKey(instr.Operand))
                        instr.Operand = bodyMap[instr.Operand];

                    else if (instr.Operand is Instruction[])
                        instr.Operand = ((Instruction[])instr.Operand).Select(target => (Instruction)bodyMap[target]).ToArray();
                }

                foreach (ExceptionHandler eh in methodDef.Body.ExceptionHandlers)
                    newMethodDef.Body.ExceptionHandlers.Add(new ExceptionHandler(eh.HandlerType)
                    {
                        CatchType = eh.CatchType == null ? null : _mapper.ImportType(eh.CatchType),
                        TryStart = (Instruction)bodyMap[eh.TryStart],
                        TryEnd = (Instruction)bodyMap[eh.TryEnd],
                        HandlerStart = (Instruction)bodyMap[eh.HandlerStart],
                        HandlerEnd = (Instruction)bodyMap[eh.HandlerEnd],
                        FilterStart = eh.FilterStart == null ? null : (Instruction)bodyMap[eh.FilterStart]
                    });

                newMethodDef.Body.SimplifyMacros(newMethodDef.Parameters);
            }
        }

        /// <summary>
        ///     Copies the information from the origin field to injected field.
        /// </summary>
        /// <param name="fieldDef">The origin FieldDef.</param>
        private void CopyFieldDef(FieldDef fieldDef)
        {
            var newFieldDef = (FieldDef)_mapper.DnlibDefMaps[fieldDef];
            newFieldDef.Signature = _mapper.Importer.Import(fieldDef.Signature);
        }

        /// <summary>
        ///     Copies the information from the origin property to injected property.
        /// </summary>
        /// <param name="propertyDef">The origin PropertyDef.</param>
        private void CopyPropertyDef(PropertyDef propertyDef)
        {
            var newPropertyDef = (PropertyDef)_mapper.DnlibDefMaps[propertyDef];
            if (propertyDef.SetMethod != null)
                if (propertyDef.SetMethod.HasBody)
                    newPropertyDef.SetMethod = newPropertyDef.DeclaringType.Methods.Where(m => m.Name == $"set_{propertyDef.Name}").First();
            if (propertyDef.GetMethod != null)
                if (propertyDef.GetMethod.HasBody)
                    newPropertyDef.GetMethod = newPropertyDef.DeclaringType.Methods.Where(m => m.Name == $"get_{propertyDef.Name}").First();
        }

        /// <summary>
        ///     Copies the information from the origin event to injected event.
        /// </summary>
        /// <param name="eventDef">The origin EventDef.</param>
        private void CopyEventDef(EventDef eventDef)
        {
            var newEventDef = (EventDef)_mapper.DnlibDefMaps[eventDef];

            newEventDef.EventType = _mapper.ImportType(eventDef.EventType);

            if (eventDef.AddMethod != null)
            {
                newEventDef.AddMethod = newEventDef.DeclaringType.Methods.FirstOrDefault(
                    m => m.Name == eventDef.AddMethod.Name && m.HasBody);
            }

            if (eventDef.RemoveMethod != null)
            {
                newEventDef.RemoveMethod = newEventDef.DeclaringType.Methods.FirstOrDefault(
                    m => m.Name == eventDef.RemoveMethod.Name && m.HasBody);
            }

            if (eventDef.InvokeMethod != null)
            {
                newEventDef.InvokeMethod = newEventDef.DeclaringType.Methods.FirstOrDefault(
                    m => m.Name == eventDef.InvokeMethod.Name && m.HasBody);
            }

            foreach (var method in eventDef.OtherMethods)
            {
                var copiedMethod = newEventDef.DeclaringType.Methods.FirstOrDefault(
                    m => m.Name == method.Name && m.HasBody);
                if (copiedMethod != null)
                    newEventDef.OtherMethods.Add(copiedMethod);
            }

            foreach (var ca in eventDef.CustomAttributes)
            {
                newEventDef.CustomAttributes.Add(
                    new CustomAttribute((ICustomAttributeType)_mapper.Importer.Import(ca.Constructor)));
            }
        }


        #endregion
    }
}
