using Autodesk.Revit.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_mcp_plugin.Models
{
    /// <summary>
    /// 线状构件
    /// </summary>
    public class LineElement
    {
        /// <summary>
        /// 构件类型
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; } = "INVALID";
        /// <summary>
        /// 类型Id
        /// </summary>
        [JsonProperty("typeId")]
        public int TypeId { get; set; } = -1;
        /// <summary>
        /// 路径曲线
        /// </summary>
        [JsonProperty("locationLine")]
        public JZLine LocationLine { get; set; }
        /// <summary>
        /// 厚度
        /// </summary>
        [JsonProperty("thickness")]
        public double Thickness { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        [JsonProperty("height")]
        public double Height { get; set; }
        /// <summary>
        /// 底部标高
        /// </summary>
        [JsonProperty("baseLevel")]
        public double BaseLevel { get; set; }
        /// <summary>
        /// 底部偏移/基于面的偏移
        /// </summary>
        [JsonProperty("baseOffset")]
        public double BaseOffset { get; set; }
        /// <summary>
        /// 参数化属性
        /// </summary>
        [JsonProperty("parameters")]
        public Dictionary<string, double> Parameters { get; set; }

        public LineElement()
        {
            Parameters = new Dictionary<string, double>();
        }
    }
}
