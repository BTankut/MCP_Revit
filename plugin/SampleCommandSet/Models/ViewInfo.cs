using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommandSet.Models
{
    /// <summary>
    /// 视图信息数据结构
    /// </summary>
    public class ViewInfo
    {
        public long Id { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string ViewType { get; set; }
        public bool IsTemplate { get; set; }
        public int Scale { get; set; }
        public string DetailLevel { get; set; }
    }
}
