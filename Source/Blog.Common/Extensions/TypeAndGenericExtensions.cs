namespace Blog.Common.Extensions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public static class TypeAndGenericExtensions
    {
        public static string GetSplittedPascalCaseTypeName(this Type type)
        {
            return string.Join(" ", type.Name.GetSplittedPascalCaseWords());
        }

        public static string GetSplittedPascalCaseTypeName(this TypeInfo type)
        {
            return string.Join(" ", type.Name.GetSplittedPascalCaseWords());
        }

        public static string GetSplittedPascalCaseEnumTypeName<T>(this T @enum)
            where T : IComparable, IFormattable, IConvertible
        {
            return string.Join(
                " ",
                Enum.GetNames(
                    typeof(T))
                    .FirstOrDefault(t => t == @enum.ToString(CultureInfo.CurrentCulture))
                    .GetSplittedPascalCaseWords());
        }
    }
}