using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using revit_mcp_sdk.API.Interfaces;
using SampleCommandSet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleCommandSet.Commands.Create
{
    /// <summary>
    /// 创建墙的外部事件处理器
    /// </summary>
    public class CreateWallEventHandler : IExternalEventHandler, IWaitableExternalEventHandler
    {
        // 创建墙的参数
        private double _startX;
        private double _startY;
        private double _endX;
        private double _endY;
        private double _height;
        private double _thickness;

        // 创建的墙体信息
        private Wall _createdWall;
        public WallInfo CreatedWallInfo { get; private set; }

        // 标记操作是否完成
        private bool _taskCompleted;

        // 事件等待对象
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);

        /// <summary>
        /// 设置创建墙的参数
        /// </summary>
        public void SetWallParameters(double startX, double startY, double endX, double endY, double height, double thickness)
        {
            _startX = startX;
            _startY = startY;
            _endX = endX;
            _endY = endY;
            _height = height;
            _thickness = thickness;

            _taskCompleted = false;
            _resetEvent.Reset();
        }

        /// <summary>
        /// 等待墙创建完成
        /// </summary>
        /// <param name="timeoutMilliseconds">超时时间（毫秒）</param>
        /// <returns>操作是否在超时前完成</returns>
        public bool WaitForCompletion(int timeoutMilliseconds = 10000)
        {
            return _resetEvent.WaitOne(timeoutMilliseconds);
        }

        /// <summary>
        /// IExternalEventHandler.Execute 实现
        /// </summary>
        public void Execute(UIApplication app)
        {
            try
            {
                Document doc = app.ActiveUIDocument.Document;

                using (Transaction trans = new Transaction(doc, "创建墙体"))
                {
                    trans.Start();

                    // 创建墙的起点和终点
                    XYZ startPoint = new XYZ(_startX, _startY, 0);
                    XYZ endPoint = new XYZ(_endX, _endY, 0);

                    // 创建墙的曲线
                    Line curve = Line.CreateBound(startPoint, endPoint);

                    // 获取当前文档中的墙类型
                    FilteredElementCollector collector = new FilteredElementCollector(doc);
                    collector.OfClass(typeof(WallType));
                    WallType wallType = collector.FirstOrDefault(w => w.Name.Contains("常规")) as WallType;

                    // 创建墙
                    _createdWall = Wall.Create(
                        doc,
                        curve,
                        wallType.Id,
                        doc.ActiveView.GenLevel.Id,
                        _height,
                        0.0,  // 墙基点偏移
                        false,  // 不翻转
                        false); // 不是结构墙

                    trans.Commit();

                    // 获取墙的详细信息
                    CreatedWallInfo = new WallInfo
                    {
                        ElementId = _createdWall.Id.IntegerValue,
                        StartPoint = new Models.Point { X = startPoint.X, Y = startPoint.Y, Z = 0 },
                        EndPoint = new Models.Point { X = endPoint.X, Y = endPoint.Y, Z = 0 },
                        Height = _height,
                        Thickness = _thickness,
                    };
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("错误", $"创建墙体时出错: {ex.Message}");

            }
            finally
            {
                _taskCompleted = true;
                _resetEvent.Set(); // 通知等待线程操作已完成
            }
        }

        /// <summary>
        /// IExternalEventHandler.GetName 实现
        /// </summary>
        public string GetName()
        {
            return "创建墙体";
        }
    }
}
