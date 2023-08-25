#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration
{
    public class InternalClassCreator
    {
        public Type Create(ModuleBuilder myModule, TypeBuilder compositeType, MethodInfo internalMethod, 
            Type generic, params KeyValuePair<Type, string>[] parameters)
        {
            var name = $"{compositeType.FullName}__Internal_{internalMethod.Name}";
            foreach (var parameter in parameters)
            {
                name += $"_{parameter.Key.ToString()!.Replace('.', '0').ToLower()}";
            }
            
            TypeBuilder typeBuilder = myModule.DefineType(name,
                TypeAttributes.Public);
            FieldBuilder composite = typeBuilder.DefineField($"_this", 
                compositeType, FieldAttributes.Public);

            List<FieldBuilder> fields = new();
            foreach (var parameter in parameters)
            {
                FieldBuilder fieldBuilder = typeBuilder.DefineField($"{parameter.Value}", 
                    parameter.Key, FieldAttributes.Public);
                fields.Add(fieldBuilder);
            }
            
            // ReSharper disable once CoVariantArrayConversion
            CreateMethod(typeBuilder, composite, internalMethod, generic, fields.ToArray());

            Type myType = typeBuilder.CreateType();
            
            return myType;
        }

        private void CreateMethod(TypeBuilder typeBuilder, FieldInfo composite, MethodInfo internalMethod, 
            Type generic, params FieldInfo[] parameters)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"b__0",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                typeof(void),
                new [] {generic.MakeArrayType()});
            
            ILGenerator lout = builder.GetILGenerator();
            
            var items = lout.DeclareLocal(generic.MakeArrayType()); 
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, composite);
            lout.Emit(OpCodes.Ldarg_1);
            foreach (FieldInfo parameter in parameters)
            {
                lout.Emit(OpCodes.Ldarg_0);
                lout.Emit(OpCodes.Ldfld, parameter);
            }
            lout.Emit(OpCodes.Call, internalMethod);
            lout.Emit(OpCodes.Ret);
        }
    }
}
#endif