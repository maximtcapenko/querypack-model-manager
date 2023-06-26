namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl
{
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class Extensions
    {
        public static PropertyBuilder AddField(this TypeBuilder self, string fieldName, Type fielType)
        {
            //build field
            var field = self.DefineField(fieldName, fielType, FieldAttributes.Private);

            //define property
            var property = self.DefineProperty(fieldName, PropertyAttributes.None, fielType, null);
            //build setter
            var setter = self.DefineMethod("set_" + fieldName, MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { fielType });
            var setterILG = setter.GetILGenerator();
            setterILG.Emit(OpCodes.Ldarg_0);
            setterILG.Emit(OpCodes.Ldarg_1);
            setterILG.Emit(OpCodes.Stfld, field);
            setterILG.Emit(OpCodes.Ret);
            property.SetSetMethod(setter);


            //build getter
            var getter = self.DefineMethod("get_" + fieldName, MethodAttributes.Public | MethodAttributes.Virtual, fielType, Type.EmptyTypes);
            var getterILG = getter.GetILGenerator();
            getterILG.Emit(OpCodes.Ldarg_0);
            getterILG.Emit(OpCodes.Ldfld, field);
            getterILG.Emit(OpCodes.Ret);
            property.SetGetMethod(getter);

            return property;
        }
    }
}