using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common
{
    public static class ReflectionExtensions
    {

        public static string FormatIntegralType(this Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
            {
                return underlying.FormatIntegralType() + "?";
            }

            if (_integralTypeNames.TryGetValue(type, out var result))
            {
                return result;
            }
            return type.Name;
        }

        private static Dictionary<Type, string> _integralTypeNames = new Dictionary<Type, string>()
        {
            {typeof(int), "int"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(decimal), "decimal"},
            {typeof(bool), "bool"},
            {typeof(string), "string"},
            {typeof(byte), "byte"},
            {typeof(char), "char"},
        };


        public static bool IsFloatingPoint(this Type type)
        {
            return type == typeof(double)
                || type == typeof(float)
                || type == typeof(decimal);
        }

        public static bool IsNullable(this Type type) => Nullable.GetUnderlyingType(type) != null;

        public static bool IsNumeric(this TypeCode code)
        {
            switch (code)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;

                default:
                    return false;
            }
        }



        public static IEnumerable<Assembly> GetAllDependencies(this Assembly assembly)
        {
            var names = GetAllDependencyNames(assembly)
                .Distinct()
                .ToList();
            foreach (var name in names)
            {
                yield return Assembly.Load(name);
            }
        }

        private static IEnumerable<AssemblyName> GetAllDependencyNames(this Assembly assembly)
        {
            foreach (var dependency in assembly.GetReferencedAssemblies())
            {
                yield return dependency;
            }
        }

    }
}
