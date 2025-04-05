﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SampleCommandSet.Extensions;
using revit_mcp_sdk.API.Interfaces;

namespace SampleCommandSet.Commands.Access
{
    /// <summary>
    /// 获取当前视图元素的事件处理器
    /// </summary>
    public class GetCurrentViewElementsEventHandler : IExternalEventHandler, IWaitableExternalEventHandler
    {
        // 默认模型类别列表
        private readonly List<string> _defaultModelCategories = new List<string>
        {
            "OST_Walls",
            "OST_Doors",
            "OST_Windows",
            "OST_Furniture",
            "OST_Columns",
            "OST_Floors",
            "OST_Roofs",
            "OST_Stairs",
            "OST_StructuralFraming",
            "OST_Ceilings",
            "OST_MEPSpaces",
            "OST_Rooms"
        };
        // 默认注释类别列表
        private readonly List<string> _defaultAnnotationCategories = new List<string>
        {
            "OST_Dimensions",
            "OST_TextNotes",
            "OST_GenericAnnotation",
            "OST_WallTags",
            "OST_DoorTags",
            "OST_WindowTags",
            "OST_RoomTags",
            "OST_AreaTags",
            "OST_SpaceTags",
            "OST_ViewportLabels",
            "OST_TitleBlocks"
        };

        // 查询参数
        private List<string> _modelCategoryList;
        private List<string> _annotationCategoryList;
        private bool _includeHidden;
        private int _limit;

        // 执行结果
        public ViewElementsResult ResultInfo { get; private set; }

        // 状态同步对象
        public bool TaskCompleted { get; private set; }
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);

        // 设置查询参数
        public void SetQueryParameters(List<string> modelCategoryList, List<string> annotationCategoryList, bool includeHidden, int limit)
        {
            _modelCategoryList = modelCategoryList;
            _annotationCategoryList = annotationCategoryList;
            _includeHidden = includeHidden;
            _limit = limit;
            TaskCompleted = false;
            _resetEvent.Reset();
        }

        // 实现IWaitableExternalEventHandler接口
        public bool WaitForCompletion(int timeoutMilliseconds = 10000)
        {
            return _resetEvent.WaitOne(timeoutMilliseconds);
        }

        public void Execute(UIApplication app)
        {
            try
            {
                var uiDoc = app.ActiveUIDocument;
                var doc = uiDoc.Document;
                var activeView = doc.ActiveView;

                // 如果传入的类别列表为空，则使用默认列表
                List<string> modelCategories = (_modelCategoryList == null || _modelCategoryList.Count == 0)
                    ? _defaultModelCategories
                    : _modelCategoryList;

                List<string> annotationCategories = (_annotationCategoryList == null || _annotationCategoryList.Count == 0)
                    ? _defaultAnnotationCategories
                    : _annotationCategoryList;

                // 合并所有类别
                List<string> allCategories = new List<string>();
                allCategories.AddRange(modelCategories);
                allCategories.AddRange(annotationCategories);

                // 获取当前视图中的所有元素
                var collector = new FilteredElementCollector(doc, activeView.Id)
                    .WhereElementIsNotElementType();

                // 获取所有元素
                IList<Element> elements = collector.ToElements();

                // 按类别筛选
                if (allCategories.Count > 0)
                {
                    // 转换字符串类别为枚举
                    List<BuiltInCategory> builtInCategories = new List<BuiltInCategory>();
                    foreach (string categoryName in allCategories)
                    {
                        if (Enum.TryParse(categoryName, out BuiltInCategory category))
                        {
                            builtInCategories.Add(category);
                        }
                    }
                    // 如果成功解析了类别，则使用类别过滤器
                    if (builtInCategories.Count > 0)
                    {
                        ElementMulticategoryFilter categoryFilter = new ElementMulticategoryFilter(builtInCategories);
                        elements = new FilteredElementCollector(doc, activeView.Id)
                            .WhereElementIsNotElementType()
                            .WherePasses(categoryFilter)
                            .ToElements();
                    }
                }

                // 过滤隐藏的元素
                if (!_includeHidden)
                {
                    elements = elements.Where(e => !e.IsHidden(activeView)).ToList();
                }

                // 限制返回数量
                if (_limit > 0 && elements.Count > _limit)
                {
                    elements = elements.Take(_limit).ToList();
                }

                // 构建结果
                var elementInfos = elements.Select(e => new ElementInfo
                {
                    Id = e.Id.GetIdValue(),
                    UniqueId = e.UniqueId,
                    Name = e.Name,
                    Category = e.Category?.Name ?? "unknow",
                    Properties = GetElementProperties(e)
                }).ToList();

                ResultInfo = new ViewElementsResult
                {
                    ViewId = activeView.Id.GetIdValue(),
                    ViewName = activeView.Name,
                    TotalElementsInView = new FilteredElementCollector(doc, activeView.Id).GetElementCount(),
                    FilteredElementCount = elementInfos.Count,
                    Elements = elementInfos
                };
            }
            catch (Exception ex)
            {
                TaskDialog.Show("error", ex.Message);
            }
            finally
            {
                TaskCompleted = true;
                _resetEvent.Set();
            }
        }

        private Dictionary<string, string> GetElementProperties(Element element)
        {
            var properties = new Dictionary<string, string>();

            // 添加通用属性
            properties.Add("ElementId", element.Id.GetIdValue().ToString());

            if (element.Location != null)
            {
                if (element.Location is LocationPoint locationPoint)
                {
                    var point = locationPoint.Point;
                    properties.Add("LocationX", point.X.ToString("F2"));
                    properties.Add("LocationY", point.Y.ToString("F2"));
                    properties.Add("LocationZ", point.Z.ToString("F2"));
                }
                else if (element.Location is LocationCurve locationCurve)
                {
                    var curve = locationCurve.Curve;
                    properties.Add("Start", $"{curve.GetEndPoint(0).X:F2}, {curve.GetEndPoint(0).Y:F2}, {curve.GetEndPoint(0).Z:F2}");
                    properties.Add("End", $"{curve.GetEndPoint(1).X:F2}, {curve.GetEndPoint(1).Y:F2}, {curve.GetEndPoint(1).Z:F2}");
                    properties.Add("Length", curve.Length.ToString("F2"));
                }
            }

            // 获取常用参数值
            var commonParams = new[] { "Comments", "Mark", "Level", "Family", "Type" };
            foreach (var paramName in commonParams)
            {
                Parameter param = element.LookupParameter(paramName);
                if (param != null && !param.IsReadOnly)
                {
                    if (param.StorageType == StorageType.String)
                        properties.Add(paramName, param.AsString() ?? "");
                    else if (param.StorageType == StorageType.Double)
                        properties.Add(paramName, param.AsDouble().ToString("F2"));
                    else if (param.StorageType == StorageType.Integer)
                        properties.Add(paramName, param.AsInteger().ToString());
                    else if (param.StorageType == StorageType.ElementId)
                        properties.Add(paramName, param.AsElementId().GetIdValue().ToString());
                }
            }

            return properties;
        }

        public string GetName()
        {
            return "获取当前视图元素";
        }
    }

    /// <summary>
    /// 元素信息数据结构
    /// </summary>
    public class ElementInfo
    {
        public long Id { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// 视图元素结果数据结构
    /// </summary>
    public class ViewElementsResult
    {
        public long ViewId { get; set; }
        public string ViewName { get; set; }
        public int TotalElementsInView { get; set; }
        public int FilteredElementCount { get; set; }
        public List<ElementInfo> Elements { get; set; } = new List<ElementInfo>();
    }
}
