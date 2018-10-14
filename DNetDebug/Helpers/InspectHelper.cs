using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using Discord;

namespace DNetDebug.Helpers
{
    public static class InspectHelper
    {
        private static string GetValue(object prop, object obj)
        {
            object value;

            /* PropertyInfo and FieldInfo both derive from MemberInfo, but that does not have a GetValue method, so the only
                supported ancestor is object */
            switch (prop)
            {
                case PropertyInfo propertyInfo:
                    value = propertyInfo.GetValue(obj);
                    break;
                case FieldInfo fieldInfo:
                    value = fieldInfo.GetValue(obj);
                    break;
                default:
                    throw new InvalidOperationException(
                        "GetValue(object, object): first parameter prop must be PropertyInfo or FieldInfo");
            }

            if (value == null) return "Null";

            string HandleEnumerable(IEnumerable enumerable)
            {
                var enu = enumerable.Cast<object>().ToList();
                return $"{enu.Count} [{enu.GetType().Name}]";
            }

            string HandleNormal()
            {
                return value + $" [{value.GetType().Name}]";
            }

            switch (value)
            {
                case IEnumerable enumerable:
                    if (value is string) return HandleNormal();
                    return HandleEnumerable(enumerable);
                default:
                    return HandleNormal();
            }
        }

        public static string Inspect(this object obj)
        {
            var type = obj.GetType();

            var inspection = new StringBuilder();
            inspection.AppendLine($"<< Inspecting type [{type.Name}] >>");
            inspection.AppendLine($"<< String Representation: [{obj}] >>");
            inspection.AppendLine();

            /* Get list of properties, with no index parameters (to avoid exceptions) */
            var props = type.GetProperties().Where(a => a.GetIndexParameters().Length == 0)
                .OrderBy(a => a.Name).ToList();

            /* Get list of fields */
            var fields = type.GetFields().OrderBy(a => a.Name).ToList();

            /* Handle properties in type */
            if (props.Count != 0)
            {
                /* Add header if we have fields as well */
                if (fields.Count != 0) inspection.AppendLine("<< Properties >>");

                /* Get the longest named property in the list, so we can make the column width that + 5 */
                var columnWidth = props.Max(a => a.Name.Length) + 5;
                foreach (var prop in props)
                {
                    /* Crude skip to avoid request errors */
                    if (inspection.Length > 1800) continue;

                    /* Create a blank string gap of the remaining space to the end of the column */
                    var sep = new string(' ', columnWidth - prop.Name.Length);

                    /* Add the property name, then the separator, then the value */
                    inspection.AppendLine($"{prop.Name}{sep}{(prop.CanRead ? GetValue(prop, obj) : "Unreadable")}");
                }
            }

            /* Repeat the same with fields */
            if (fields.Count != 0)
            {
                if (props.Count != 0)
                {
                    inspection.AppendLine();
                    inspection.AppendLine("<< Fields >>");
                }

                var columnWidth = fields.Max(ab => ab.Name.Length) + 5;
                foreach (var prop in fields)
                {
                    if (inspection.Length > 1800) continue;

                    var sep = new string(' ', columnWidth - prop.Name.Length);
                    inspection.AppendLine($"{prop.Name}:{sep}{GetValue(prop, obj)}");
                }
            }

            /* If the object is an enumerable type, add a list of it's items */
            if (obj is IEnumerable objEnumerable)
            {
                inspection.AppendLine();
                inspection.AppendLine("<< Items >>");
                foreach (var prop in objEnumerable) inspection.AppendLine($" - {prop}");
            }

            return Format.Code(inspection.ToString(), "ini");
        }
    }
}