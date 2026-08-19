using System.ComponentModel;

namespace EBVL.Shared.Statics.Common.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this object field)
    {
        var fieldName = field.ToString();

        if (fieldName == null)
        {
            return string.Empty;
        }

        var fieldInfo = field.GetType().GetField(fieldName);

        if (fieldInfo == null)
        {
            return fieldName;
        }

        var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : fieldName;
    }
}
