using System;
using System.Reflection;

namespace DvsSapLink2.Helper
{
    public static class EnumExtension
    {
        /// <summary>
        /// Get the value of the description attribute for a enum value
        /// </summary>
        public static string GetDescription(this Enum genericEnum)
        {
            var genericEnumType = genericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(genericEnum.ToString());
            if (memberInfo.Length <= 0)
                return genericEnum.ToString();

            var eloAttribute = memberInfo[0].GetCustomAttribute<EloAttribute>(false);
            return eloAttribute?.Description ?? genericEnum.ToString();
        }

        /// <summary>
        /// Get the value of the description attribute for a enum value
        /// </summary>
        public static int GetOrder(this Enum genericEnum)
        {
            var genericEnumType = genericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(genericEnum.ToString());
            if (memberInfo.Length <= 0)
                return int.MaxValue;

            var eloAttribute = memberInfo[0].GetCustomAttribute<EloAttribute>(false);
            return eloAttribute?.Order ?? int.MaxValue;
        }
    }
}