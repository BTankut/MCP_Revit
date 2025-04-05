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
    /// Revit构件类型
    /// </summary>
    public enum ComponentType
    {
        // 主体结构构件
        /// <summary>
        /// 墙
        /// 线状构件
        /// OST_Walls
        /// </summary>
        Wall,
        /// <summary>
        /// 楼板
        /// 面状构件
        /// OST_Floors
        /// </summary>
        Floor,
        /// <summary>
        /// 天花板
        /// 面状构件
        /// OST_Ceilings
        /// </summary>
        Ceiling,
        /// <summary>
        /// 柱
        /// 点状构件
        /// OST_Columns, OST_StructuralColumns
        /// </summary>
        Column,
        /// <summary>
        /// 梁
        /// 线状构件
        /// OST_StructuralFraming
        /// </summary>
        Beam,
        /// <summary>
        /// 屋顶
        /// 面状构件
        /// OST_Roofs
        /// </summary>
        Roof,
        /// <summary>
        /// 基础
        /// 面状构件
        /// OST_StructuralFoundation
        /// </summary>
        Foundation,
        /// <summary>
        /// 幕墙
        /// 线状构件
        /// OST_Curtain_Systems, OST_CurtainWallPanels, OST_CurtainWallMullions
        /// </summary>
        CurtainWall,

        // 开口构件
        /// <summary>
        /// 门
        /// 点状构件
        /// OST_Doors
        /// </summary>
        Door,
        /// <summary>
        /// 窗
        /// 点状构件
        /// OST_Windows
        /// </summary>
        Window,
        /// <summary>
        /// 其他开口
        /// 面状构件
        /// OST_ShaftOpening, OST_FloorOpening, OST_WallOpening, OST_RoofOpening
        /// </summary>
        Opening,

        // 垂直交通
        /// <summary>
        /// 楼梯
        /// a面状构件
        /// OST_Stairs
        /// </summary>
        Stair,
        /// <summary>
        /// 栏杆/扶手
        /// 线状构件
        /// OST_Railings
        /// </summary>
        Railing,
        /// <summary>
        /// 坡道
        /// 面状构件
        /// OST_Ramps
        /// </summary>
        Ramp,

        // MEP构件
        /// <summary>
        /// 管道
        /// 线状构件
        /// OST_PipeCurves
        /// </summary>
        Pipe,
        /// <summary>
        /// 风管
        /// 线状构件
        /// OST_DuctCurves
        /// </summary>
        Duct,
        /// <summary>
        /// 机电设备
        /// 点状构件
        /// OST_MechanicalEquipment
        /// </summary>
        MechanicalEquipment,
        /// <summary>
        /// 电气设备
        /// 点状构件
        /// OST_ElectricalEquipment
        /// </summary>
        ElectricalEquipment,
        /// <summary>
        /// 卫浴设备
        /// 点状构件
        /// OST_PlumbingFixtures
        /// </summary>
        PlumbingFixture,

        // 家具与装饰
        /// <summary>
        /// 家具
        /// 点状构件
        /// OST_Furniture
        /// </summary>
        Furniture,

        // 注释与参照元素
        /// <summary>
        /// 房间
        /// 面状构件
        /// OST_Rooms
        /// </summary>
        Room,
        /// <summary>
        /// 轴网
        /// 线状构件
        /// OST_Grids
        /// </summary>
        Grid,
        /// <summary>
        /// 标高
        /// 线状构件
        /// OST_Levels
        /// </summary>
        Level,
        /// <summary>
        /// 图纸(视图)
        /// 不适用于点、线、面分类
        /// OST_Sheets
        /// </summary>
        Sheet,
        /// <summary>
        /// 标记
        /// 点状构件
        /// OST_MultiCategoryTags, OST_DoorTags, OST_WindowTags, 等多种标记类型
        /// </summary>
        Tag,
        /// <summary>
        /// 尺寸标注
        /// 线状构件
        /// OST_Dimensions
        /// </summary>
        Dimension,
        /// <summary>
        /// 文本注释
        /// 点状构件
        /// OST_TextNotes
        /// </summary>
        TextNote,
        /// <summary>
        /// 视口
        /// 点状构件
        /// OST_Viewports
        /// </summary>
        Viewport,

        // 通用/其他
        /// <summary>
        /// 通用模型
        /// 无法判断
        /// OST_GenericModel
        /// </summary>
        GenericModel,

        /// <summary>
        /// 未定义
        /// </summary>
        None = -1,
    }

    /// <summary>
    /// 点状构件
    /// </summary>
    public class PointElement
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
        /// 定位点坐标
        /// </summary>
        [JsonProperty("locationPoint")]
        public JZPoint LocationPoint { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        [JsonProperty("width")]
        public double Width { get; set; } = -1;
        /// <summary>
        /// 深度
        /// </summary>
        [JsonProperty("depth")]
        public double Depth { get; set; }
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
        /// 底部偏移
        /// </summary>
        [JsonProperty("baseOffset")]
        public double BaseOffset { get; set; }
        /// <summary>
        /// 参数化属性
        /// </summary>
        [JsonProperty("parameters")]
        public Dictionary<string, double> Parameters { get; set; }

        public PointElement()
        {
            Parameters = new Dictionary<string, double>();
        }
    }
}
