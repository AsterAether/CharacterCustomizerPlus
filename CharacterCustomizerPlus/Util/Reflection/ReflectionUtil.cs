using System;
using System.Linq;
using System.Reflection;

namespace CharacterCustomizer.Util.Reflection
{
    public static class ReflectionUtil
    {
        #region Assembly

        public static Type GetClass(this Assembly assembly, string namesp, string name)
        {
            return assembly.GetTypes().First(t => t.IsClass && t.Namespace == namesp &&
                                                  t.Name == name);
        }

        #endregion
    }
}