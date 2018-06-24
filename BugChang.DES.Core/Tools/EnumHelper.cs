using System;
using System.ComponentModel;

namespace BugChang.DES.Core.Tools
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum enumValue)
        {
            var value = enumValue.ToString();
            var field = enumValue.GetType().GetField(value);
            var objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
            if (objs.Length == 0)    //当描述属性没有时，直接返回名称
                return value;
            var descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }
    }
}
