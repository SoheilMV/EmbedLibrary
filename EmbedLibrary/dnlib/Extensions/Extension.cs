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

        public static MethodDef CopyMethod(this MethodDef originMethod, ModuleDefMD module)
        {
            InjectContext ctx = new InjectContext(module, module);

            MethodDefUser newMethodDef = new MethodDefUser
            {
                Signature = ctx.Importer.Import(originMethod.Signature)
            };

            newMethodDef.Name = Guid.NewGuid().ToString().Replace("-", string.Empty);

            newMethodDef.Parameters.UpdateParameterTypes();

            if (originMethod.ImplMap != null)
                newMethodDef.ImplMap = new ImplMapUser(new ModuleRefUser(ctx.TargetModule, originMethod.ImplMap.Module.Name), originMethod.ImplMap.Name, originMethod.ImplMap.Attributes);

            foreach (CustomAttribute ca in originMethod.CustomAttributes)
                newMethodDef.CustomAttributes.Add(new CustomAttribute((ICustomAttributeType)ctx.Importer.Import(ca.Constructor)));

            if (originMethod.HasBody)
            {
                newMethodDef.Body = new CilBody(originMethod.Body.InitLocals, new List<Instruction>(), new List<ExceptionHandler>(), new List<Local>());
                newMethodDef.Body.MaxStack = originMethod.Body.MaxStack;

                var bodyMap = new Dictionary<object, object>();

                foreach (Local local in originMethod.Body.Variables)
                {
                    var newLocal = new Local(ctx.Importer.Import(local.Type));
                    newMethodDef.Body.Variables.Add(newLocal);
                    newLocal.Name = local.Name;

                    bodyMap[local] = newLocal;
                }

                foreach (Instruction instr in originMethod.Body.Instructions)
                {
                    var newInstr = new Instruction(instr.OpCode, instr.Operand);
                    newInstr.SequencePoint = instr.SequencePoint;

                    if (newInstr.Operand is IType)
                        newInstr.Operand = ctx.Importer.Import((IType)newInstr.Operand);

                    else if (newInstr.Operand is IMethod)
                        newInstr.Operand = ctx.Importer.Import((IMethod)newInstr.Operand);

                    else if (newInstr.Operand is IField)
                        newInstr.Operand = ctx.Importer.Import((IField)newInstr.Operand);

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

                foreach (ExceptionHandler eh in originMethod.Body.ExceptionHandlers)
                    newMethodDef.Body.ExceptionHandlers.Add(new ExceptionHandler(eh.HandlerType)
                    {
                        CatchType = eh.CatchType == null ? null : (ITypeDefOrRef)ctx.Importer.Import(eh.CatchType),
                        TryStart = (Instruction)bodyMap[eh.TryStart],
                        TryEnd = (Instruction)bodyMap[eh.TryEnd],
                        HandlerStart = (Instruction)bodyMap[eh.HandlerStart],
                        HandlerEnd = (Instruction)bodyMap[eh.HandlerEnd],
                        FilterStart = eh.FilterStart == null ? null : (Instruction)bodyMap[eh.FilterStart]
                    });

                newMethodDef.Body.SimplifyMacros(newMethodDef.Parameters);
            }

            return newMethodDef;
        }
    }
}
