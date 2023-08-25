﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace CompositeDirectorWithGeneratingComposites.CompositeDirector.CompositeGeneration
{
    public class CompositeCreator
    {
        public IPool<T> Create<T>() where T : class, IPoolItem
        {
            AssemblyName assemblyName = new AssemblyName
            {
                Name = "CompositeAssembly"
            };

            var poolType = typeof(IPool<>);
            var genericPoolType = poolType.MakeGenericType(typeof(T));

#if UNITY_EDITOR && ENABLE_CREATOR 
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.RunAndSave);
#else
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.RunAndSave);
#endif
            StringBuilder asmFileNameBldr = new StringBuilder();
            asmFileNameBldr.Append(assemblyName.Name);//"CompositeAssemblyModule" 
            // asmFileNameBldr.Append(".dll");
            string asmFileName = asmFileNameBldr.ToString();
#if UNITY_EDITOR && ENABLE_CREATOR 
            ModuleBuilder myModule = assemblyBuilder.DefineDynamicModule(asmFileName, asmFileName+".dll");
#else
            ModuleBuilder myModule = assemblyBuilder.DefineDynamicModule(asmFileName);
#endif
            
            var name = typeof(T).Name;
            if (name[0] == 'I')
                name = name.Substring(1, name.Length - 1);

            TypeBuilder typeBuilder = myModule.DefineType($"Composite{name}",
                TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(T));
            typeBuilder.AddInterfaceImplementation(genericPoolType);
            typeBuilder.AddInterfaceImplementation(typeof(IItemPool));

            FieldBuilder fieldBuilder = typeBuilder.DefineField("Items", typeof(List<T>), FieldAttributes.Public);
            var getItemsMethod = CreateGetItemsMethod(typeBuilder, fieldBuilder, typeof(T));
            
            FieldBuilder dictionaryBuilder = typeBuilder.DefineField("_removeActions", typeof(Dictionary<T, Action>), FieldAttributes.Private);
            
            var disposeField = typeBuilder.DefineField("Disposed", typeof(Action), FieldAttributes.Private);
            var disposeEvent = typeBuilder.DefineEvent("Disposed", EventAttributes.None, typeof(Action));
            var disposeMethod = typeof(IDisposable).GetMethods()[0];
            var dispose = CreateDisposeMethod(typeBuilder, getItemsMethod, disposeEvent, typeof(T), disposeField);
            typeBuilder.DefineMethodOverride(dispose, disposeMethod);
            var addDisposedMethod = CreateAddDisposedMethod(typeBuilder, disposeField, disposeEvent);
            var removeDisposedMethod = CreateRemoveDisposedMethod(typeBuilder, disposeField, disposeEvent);
            
            
            var removeMethodDeclaration = genericPoolType.GetMethod("Remove");
            var removeMethod = CreateRemoveMethod(typeBuilder, fieldBuilder, typeof(T),
                dictionaryBuilder, getItemsMethod);
            typeBuilder.DefineMethodOverride(removeMethod, removeMethodDeclaration!);
            
            var itemPoolRemoveMethodDeclaration = typeof(IItemPool).GetMethod("TryRemove");
            var itemPoolRemoveMethod = CreateItemPoolRemoveMethod(typeBuilder, typeof(T), removeMethod);
            typeBuilder.DefineMethodOverride(itemPoolRemoveMethod, itemPoolRemoveMethodDeclaration!);
            
            
            var addMethodDeclaration = genericPoolType.GetMethod("Add");
            var internalRemovingClassCreator = new InternalRemovingClassCreator();
            var internalRemoving = internalRemovingClassCreator
                .Create(myModule, typeBuilder, typeof(T),  removeMethod);
            var removingMethod = CreateRemovingMethod(typeBuilder, typeof(T), internalRemoving);
                    
            var addMethod = CreateAddMethod(typeBuilder, fieldBuilder, typeof(T),
                dictionaryBuilder, removingMethod);
            typeBuilder.DefineMethodOverride(addMethod, addMethodDeclaration);
            
            var itemPoolAddMethodDeclaration = typeof(IItemPool).GetMethod("TryAdd");
            var itemPoolAddMethod = CreateItemPoolAddMethod(typeBuilder, typeof(T), addMethod);
            typeBuilder.DefineMethodOverride(itemPoolAddMethod, itemPoolAddMethodDeclaration!);

            var methods = typeof(T).GetMethods();
            foreach (var method in methods)
            {
                var intern = CreateInternalCopy(typeBuilder, method, typeof(T));
                if(method.GetParameters().Length == 0)
                    CreateInterfaceMethod(typeBuilder, method, fieldBuilder, intern,
                        typeof(T));
                else
                {
                    List<KeyValuePair<Type, string>> args = new ();
                    foreach (var parameter in method.GetParameters())
                    {
                        args.Add(new KeyValuePair<Type, string>(parameter.ParameterType, parameter.Name));
                    }
                    
                    var internalCopyCreator = new InternalClassCreator();
                    var internalCopy = internalCopyCreator.Create(myModule, typeBuilder, intern, 
                        typeof(T), args.ToArray());
                    
                    CreateInterfaceMethodWithParams(typeBuilder, method, typeof(T), args.ToArray(), internalCopy);
                }
            }

            Type myType = typeBuilder.CreateType();
            object instance = Activator.CreateInstance(myType!);
            instance!.GetType().GetField("Items")!.SetValue(instance, new List<T>());
            instance!.GetType().GetField("_removeActions", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(instance, new Dictionary<T, Action>());
            
#if UNITY_EDITOR && ENABLE_CREATOR 
             assemblyBuilder.Save($"CompositeAssembly.dll");
#endif
            
            return instance as IPool<T>;
        }

        private MethodBuilder CreateItemPoolAddMethod(TypeBuilder typeBuilder, Type generic, MethodBuilder addMethod)
        {
            var builder = typeBuilder.DefineMethod("TryAdd", 
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual |
                MethodAttributes.NewSlot | MethodAttributes.Final,
                typeof(void),
                new [] {typeof(IPoolItem)});
            
            ILGenerator lout = builder.GetILGenerator();
            
            var internalClass = lout.DeclareLocal(generic);
            var v_1 = lout.DeclareLocal(typeof(bool));

            var jump = lout.DefineLabel();
            
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Isinst, generic);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldnull);
            lout.Emit(OpCodes.Cgt_Un);
            lout.Emit(OpCodes.Stloc_1);
            
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Brfalse_S, jump);
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Call, addMethod);
            
            lout.MarkLabel(jump);
            lout.Emit(OpCodes.Ret);

            return builder;
        }

        private MethodBuilder CreateItemPoolRemoveMethod(TypeBuilder typeBuilder, Type generic, MethodBuilder removeMethod)
        {
            var builder = typeBuilder.DefineMethod("TryRemove", 
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                typeof(void),
                new [] {typeof(IPoolItem)});
            
            ILGenerator lout = builder.GetILGenerator();
            
            var internalClass = lout.DeclareLocal(generic);
            var v_1 = lout.DeclareLocal(typeof(bool));

            var jump = lout.DefineLabel();
            
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Isinst, generic);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldnull);
            lout.Emit(OpCodes.Cgt_Un);
            lout.Emit(OpCodes.Stloc_1);
            
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Brfalse_S, jump);
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Call, removeMethod);
            
            lout.MarkLabel(jump);
            lout.Emit(OpCodes.Ret);

            return builder;
        }

        private MethodInfo CreateRemovingMethod(TypeBuilder typeBuilder, Type generic, Type internalClassBuilder)
        {
            var builder = typeBuilder.DefineMethod("Removing", MethodAttributes.Private | MethodAttributes.HideBySig,
                typeof(Action),
                new Type[] {generic});
            
            ILGenerator lout = builder.GetILGenerator();
            
            var internalClass = lout.DeclareLocal(internalClassBuilder);
            var v_1 = lout.DeclareLocal(typeof(Action));
            
            ConstructorInfo internalClassConstructor = internalClassBuilder.GetConstructors()[0];
            FieldInfo _this = internalClassBuilder.GetField("_this");
            FieldInfo item = internalClassBuilder.GetField("_item");
            MethodInfo removing = internalClassBuilder.GetMethod("Removing");
            
            var actionType = typeof(Action);
            var actionTypeConstructor = actionType.GetConstructors()[0];

            lout.Emit(OpCodes.Newobj, internalClassConstructor);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Stfld, _this);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Stfld, item);
            
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldftn, removing);
            lout.Emit(OpCodes.Newobj, actionTypeConstructor); 
            lout.Emit(OpCodes.Stloc_1);
            lout.Emit(OpCodes.Ldloc_1);
            
            lout.Emit(OpCodes.Ret);

            return builder;
        }

        private MethodInfo CreateRemoveDisposedMethod(TypeBuilder typeBuilder, FieldBuilder disposeField, EventBuilder disposeEvent)
        {
            var removeMethod = typeBuilder.DefineMethod("remove_Disposed",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                CallingConventions.Standard | CallingConventions.HasThis,
                typeof(void),
                new[] { typeof(Action) });
            
            var remove = typeof(Delegate).GetMethod("Remove", new[] { typeof(Delegate), typeof(Delegate) });
            var disposed = disposeField;
            var methods = typeof(Interlocked).GetMethods()
                .Where((info => info.Name == "CompareExchange" && info.GetParameters().Length == 3)).ToArray()[6];
            var compareExchange =
                methods.MakeGenericMethod(new Type[]{typeof(Action)});
                
            var lout = removeMethod.GetILGenerator();
            
            var jump = lout.DefineLabel();

            var v_0 = lout.DeclareLocal(typeof(Action));
            var v_1 = lout.DeclareLocal(typeof(Action));
            var v_2 = lout.DeclareLocal(typeof(Action));
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, disposeField);
            lout.Emit(OpCodes.Stloc_0);
            
            lout.MarkLabel(jump);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Stloc_1);
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Call, remove);
            lout.Emit(OpCodes.Castclass, typeof(Action));
            lout.Emit(OpCodes.Stloc_2);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldflda, disposed);
            lout.Emit(OpCodes.Ldloc_2);
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Call, compareExchange);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Bne_Un_S, jump);
            lout.Emit(OpCodes.Ret);
            
            disposeEvent.SetRemoveOnMethod(removeMethod);
            return removeMethod;
        }

        private MethodInfo CreateAddDisposedMethod(TypeBuilder typeBuilder, FieldBuilder disposeField, EventBuilder eventBuilder)
        {
            var addMethod = typeBuilder.DefineMethod("add_Disposed",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                CallingConventions.Standard | CallingConventions.HasThis,
                typeof(void),
                new[] { typeof(Action)});
            var generator = addMethod.GetILGenerator();
            var combine = typeof(Delegate).GetMethod("Combine", new[] { typeof(Delegate), typeof(Delegate) });
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, disposeField);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, combine);
            generator.Emit(OpCodes.Castclass, typeof(Action));
            generator.Emit(OpCodes.Stfld, disposeField);
            generator.Emit(OpCodes.Ret);
            eventBuilder.SetAddOnMethod(addMethod);

            return addMethod;
        }

        private MethodInfo CreateGetItemsMethod(TypeBuilder typeBuilder, FieldBuilder items, Type generic)
        {
            var poolType = typeof(List<>);
            var genericPoolType = poolType.MakeGenericType(generic);
            
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"get_Items",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName,
                genericPoolType,
                new Type[] {});
            FieldInfo field = items;
            
            ILGenerator lout = builder.GetILGenerator();
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, field);
            
            lout.Emit(OpCodes.Ret);

            return builder;
        }

        private MethodInfo CreateInternalCopy(TypeBuilder typeBuilder, MethodInfo method, Type generic)
        {
            var parameters = new List<Type>();
            parameters.Add(generic.MakeArrayType());
            foreach (var parameter in method.GetParameters())
            {
                parameters.Add(parameter.ParameterType);
            }
            
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"{method.Name}Internal",
                MethodAttributes.Public |  MethodAttributes.HideBySig,
                typeof(void),
                parameters.ToArray());

            ILGenerator lout = builder.GetILGenerator();
            var jump = lout.DefineLabel();
            var jump1 = lout.DefineLabel();
            
            var i = lout.DeclareLocal(typeof(Int32)); 
            var v1 = lout.DeclareLocal(typeof(bool));
            
            lout.Emit(OpCodes.Ldc_I4_0);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Br_S, jump);
            
            lout.MarkLabel(jump1);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldelem_Ref);
            for (int j = 1; j < parameters.Count; j++)
            {
                switch (j)
                {
                    case 1: 
                        lout.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2: 
                        lout.Emit(OpCodes.Ldarg_3);
                        break;
                    case 3:
                        lout.Emit(OpCodes.Ldarg_S, j);
                        break;
                }
            }
            
            lout.Emit(OpCodes.Callvirt, method);
            lout.Emit(OpCodes.Pop);
            
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldc_I4_1);
            lout.Emit(OpCodes.Add);
            lout.Emit(OpCodes.Stloc_0);
            
            lout.MarkLabel(jump);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Ldlen);
            lout.Emit(OpCodes.Conv_I4);
            lout.Emit(OpCodes.Clt);
            lout.Emit(OpCodes.Stloc_1);
            
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Brtrue_S, jump1);
            
            lout.Emit(OpCodes.Ret);
            
            return builder;
        }

        private MethodInfo CreateDisposeMethod(TypeBuilder typeBuilder, MethodInfo getItems,
            EventBuilder disposeEvent, Type generic, FieldBuilder disposeField)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"Dispose",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(void),
                Type.EmptyTypes);
            
            var poolType = typeof(List<>);
            var genericPoolType = poolType.MakeGenericType(generic);
            var getItem = genericPoolType.GetMethod("get_Item");
            var clear = genericPoolType.GetMethod("Clear");
            var getCount = genericPoolType.GetMethod("get_Count");
            var disposeMethod = typeof(IDisposable).GetMethod("Dispose");
            var invoke = typeof(Action).GetMethod("Invoke");

            ILGenerator lout = builder.GetILGenerator();
            
            var i = lout.DeclareLocal(typeof(Int32)); 
            var item = lout.DeclareLocal(generic);
            var v_2 = lout.DeclareLocal(typeof(bool));
            
            var jump = lout.DefineLabel();
            var jump1 = lout.DefineLabel();
            var jump2 = lout.DefineLabel();
            var jump3 = lout.DefineLabel();
            
            lout.Emit(OpCodes.Ldc_I4_0);
            lout.Emit(OpCodes.Stloc_0);
            
            lout.Emit(OpCodes.Br_S, jump);
            
            lout.MarkLabel(jump1);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Call, getItems);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Callvirt, getItem);
            lout.Emit(OpCodes.Stloc_1);
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Callvirt, disposeMethod);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldc_I4_1);
            lout.Emit(OpCodes.Add);
            lout.Emit(OpCodes.Stloc_0);
            
            lout.MarkLabel(jump);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Call, getItems);
            lout.Emit(OpCodes.Callvirt, getCount!);
            lout.Emit(OpCodes.Clt);
            lout.Emit(OpCodes.Stloc_2);
            
            lout.Emit(OpCodes.Ldloc_2);
            lout.Emit(OpCodes.Brtrue_S, jump1);
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Call, getItems);
            lout.Emit(OpCodes.Callvirt, clear!);
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, disposeField);
            lout.Emit(OpCodes.Dup);
            lout.Emit(OpCodes.Brtrue_S, jump2);
            lout.Emit(OpCodes.Pop);
            lout.Emit(OpCodes.Br_S, jump3);
            lout.MarkLabel(jump2);
            lout.Emit(OpCodes.Callvirt, invoke!);
            
            lout.MarkLabel(jump3);
            lout.Emit(OpCodes.Ret);

            return builder;
        }

        private MethodBuilder CreateAddMethod(TypeBuilder typeBuilder, FieldBuilder items, Type generic, 
            FieldInfo removeDictionary, MethodInfo removing)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"Add",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(void),
                new Type[] {generic});
            
            FieldInfo field = items;
            var add = field.FieldType.GetMethod("Add");

            var addDisposed = typeof(IPoolItem).GetMethod("add_Disposed");

            var dictionaryAdd = removeDictionary.FieldType.GetMethod("Add");
            
            ILGenerator lout = builder.GetILGenerator();

            var removingAction = lout.DeclareLocal(typeof(Action));
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, field);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Callvirt, add!);
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Call, removing!);
            lout.Emit(OpCodes.Stloc_0);
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, removeDictionary);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Callvirt, dictionaryAdd!);
            
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Callvirt, addDisposed);
            
            lout.Emit(OpCodes.Ret);

            return builder;
        }

        private MethodBuilder CreateRemoveMethod(TypeBuilder typeBuilder, FieldBuilder items, Type generic,
            FieldInfo removeDictionary, MethodInfo getItems)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"Remove",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(void),
                new [] {generic});

            FieldInfo field = items;
            var remove = field.FieldType.GetMethod("Remove");

            var getItem = removeDictionary.FieldType.GetMethod("get_Item");
            var removeFromDictionary = 
                removeDictionary.FieldType.GetMethods()
                    .Where(info => info.Name == "Remove" && info.GetParameters().Length == 1).ToArray()[0];
            
            var item_removeDisposed = typeof(IPoolItem).GetMethod("remove_Disposed");

            ILGenerator lout = builder.GetILGenerator();
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Call, getItems);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Callvirt, remove!);
            lout.Emit(OpCodes.Pop);
            
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, removeDictionary);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Callvirt, getItem!); 
            lout.Emit(OpCodes.Callvirt, item_removeDisposed);
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, removeDictionary);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Callvirt, removeFromDictionary!);
            lout.Emit(OpCodes.Pop);

            lout.Emit(OpCodes.Ret);
            
            return builder;
        }

        private MethodBuilder CreateInterfaceMethod(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder items, 
            MethodInfo internalCopy, Type generic)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"{method.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual |  MethodAttributes.HideBySig,
                typeof(Result),
                new Type[] {});

            ILGenerator lout = builder.GetILGenerator();
            
            var actionType = typeof(Action<>);
            var genericArray = generic.MakeArrayType();
            var genericActionType = actionType.MakeGenericType(genericArray);
            
            var compType = typeof(IPool<>);
            var genericCompType = compType.MakeGenericType(generic);
            
            var comp = lout.DeclareLocal(genericCompType);
            var func = lout.DeclareLocal(genericActionType);
            var v2 = lout.DeclareLocal(typeof(Result));
            
            var intern = internalCopy;

            var groupMethod = typeof(CompositeHelper).GetMethod("Group")!.MakeGenericMethod(generic);
            var intern1 = genericActionType.GetConstructors()[0];
            
            var jump = lout.DefineLabel();
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Stloc_0);
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldftn, intern);
            lout.Emit(OpCodes.Newobj, intern1);
            lout.Emit(OpCodes.Stloc_1);
            
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Call, groupMethod!);

            lout.Emit(OpCodes.Ldc_I4_0);
            lout.Emit(OpCodes.Stloc_2);
            lout.Emit(OpCodes.Br_S, jump);

            lout.MarkLabel(jump);
            lout.Emit(OpCodes.Ldloc_2);
            lout.Emit(OpCodes.Ret);

            return builder;
        }
        
        private MethodBuilder CreateInterfaceMethodWithParams(TypeBuilder typeBuilder, MethodInfo method, 
            Type generic, KeyValuePair<Type, string>[] args, Type internalClass)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"{method.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual |  MethodAttributes.HideBySig,
                typeof(Result),
                args.Select((pair => pair.Key)).ToArray());

            ILGenerator lout = builder.GetILGenerator();
            var composite = lout.DeclareLocal(internalClass);
            var v1 = lout.DeclareLocal(typeof(Result));

            var actionType = typeof(Action<>);
            var genericArray = generic.MakeArrayType();
            var genericActionType = actionType.MakeGenericType(genericArray);
            var genericActionTypeConstructor = genericActionType.GetConstructors()[0];

            var groupMethod = typeof(CompositeHelper).GetMethod("Group")!.MakeGenericMethod(generic);
            
            ConstructorInfo internalClassConstructor = internalClass.GetConstructors()[0];
            FieldInfo _this = internalClass.GetField("_this");
            MethodInfo b__0 = internalClass.GetMethod("b__0");

            lout.Emit(OpCodes.Newobj, internalClassConstructor);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Stfld, _this!);
            for (int i = 0; i < args.Length; i++)
            {
                FieldInfo field = internalClass.GetField(args[i].Value);
                lout.Emit(OpCodes.Ldloc_0);
                switch (i)
                {
                    case 0:
                        lout.Emit(OpCodes.Ldarg_1);
                        break;
                    case 1:
                        lout.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2:
                        lout.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        lout.Emit(OpCodes.Ldarg_S, (byte)i);
                        break;
                }
                lout.Emit(OpCodes.Stfld, field!);
            }
            
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldftn, b__0!);
            lout.Emit(OpCodes.Newobj, genericActionTypeConstructor);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Call, groupMethod!);
            
            lout.Emit(OpCodes.Ldc_I4_0);
            lout.Emit(OpCodes.Stloc_1);
            lout.Emit(OpCodes.Ldloc_1);
            
            lout.Emit(OpCodes.Ret);

            return builder;
        }
        
        private MethodBuilder CreateInterfaceMethodWithParams(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder items, 
            MethodInfo internalCopy, Type generic, Type[] args, string test)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"{method.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual |  MethodAttributes.HideBySig,
                typeof(Result),
                args);

            ILGenerator lout = builder.GetILGenerator();
            
            var composite = lout.DeclareLocal(generic);
            var v1 = lout.DeclareLocal(typeof(Result));
            var intern = internalCopy;

            var actionType = typeof(Action<>);
            var genericArray = generic.MakeArrayType();
            var genericActionType = actionType.MakeGenericType(genericArray);

            var groupMethod = typeof(CompositeHelper).GetMethod("Group");
            
            lout.Emit(OpCodes.Newobj, generic);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Stfld, generic);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Stfld, generic);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_2);
            lout.Emit(OpCodes.Stfld, generic);
            return builder;
        }
    }
}
#endif