using Newtonsoft.Json;

namespace SampleCommandSet.Models
{
    /// <summary>
    /// 墙体信息结构，用于返回创建的墙的详细信息
    /// </summary>
    public class WallInfo
    {
        [JsonProperty("elementId")]
        public int ElementId { get; set; }
        [JsonProperty("startPoint")]
        public Point StartPoint { get; set; } = new Point();
        [JsonProperty("endPoint")]
        public Point EndPoint { get; set; } = new Point();
        [JsonProperty("height")]
        public double Height { get; set; }
        [JsonProperty("thickness")]
        public double Thickness { get; set; }
    }
}
