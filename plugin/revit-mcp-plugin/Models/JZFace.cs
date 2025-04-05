using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace revit_mcp_plugin.Models
{
    /// <summary>
    /// 三维面 
    /// </summary>
    public class JZFace
    {
        /// <summary>
        /// 外环（List<List<JZLine>> 类型）
        /// </summary>
        [JsonProperty("outerLoop")]
        public List<JZLine> OuterLoop { get; set; }

        /// <summary>
        /// 内环（List<JZLine> 类型，表示一个或多个内环）
        /// </summary>
        [JsonProperty("innerLoops")]
        public List<List<JZLine>> InnerLoops { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public JZFace()
        {
            InnerLoops = new List<List<JZLine>>();
            OuterLoop = new List<JZLine>();
        }
    }
}
