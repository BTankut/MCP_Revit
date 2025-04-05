using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommandSet.Extensions
{
    /// <summary>
    /// 提供 Revit API 跨版本兼容性的扩展方法
    /// </summary>
    public static class RevitApiCompatibilityExtensions
    {
        // 缓存反射结果以提高性能
        private static readonly Lazy<PropertyInfo> ElementIdValueProperty =
            new Lazy<PropertyInfo>(() => typeof(ElementId).GetProperty("Value"));

        private static readonly Lazy<PropertyInfo> ElementIdIntegerValueProperty =
            new Lazy<PropertyInfo>(() => typeof(ElementId).GetProperty("IntegerValue"));

        /// <summary>
        /// 以版本兼容的方式获取 ElementId 的整数值
        /// </summary>
        public static int GetIdValue(this ElementId id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            // 先检查是否有 Value 属性 (Revit 2022+)
            if (ElementIdValueProperty.Value != null)
            {
                try
                {
                    return (int)ElementIdValueProperty.Value.GetValue(id);
                }
                catch
                {
                    // 失败时回退到 IntegerValue
                }
            }

            // 使用 IntegerValue (旧版本 Revit)
            return id.IntegerValue;
        }

        /// <summary>
        /// 获取文档当前 Revit 版本号
        /// </summary>
        public static int GetRevitVersionNumber(this Document doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            string versionString = doc.Application.VersionNumber;

            if (int.TryParse(versionString, out int versionNumber))
            {
                return versionNumber;
            }
            return 0;
        }

    }
}
