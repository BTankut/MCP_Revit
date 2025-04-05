﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using revit_mcp_sdk.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SampleCommandSet.Extensions;

namespace SampleCommandSet.Commands.Access
{
    public class GetSelectedElementsEventHandler : IExternalEventHandler, IWaitableExternalEventHandler
    {
        // 执行结果
        public List<ElementInfo> ResultElements { get; private set; }

        // 状态同步对象
        public bool TaskCompleted { get; private set; }
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);

        // 限制返回的元素数量
        public int? Limit { get; set; }

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

                // 获取当前选中的元素
                var selectedIds = uiDoc.Selection.GetElementIds();
                var selectedElements = selectedIds.Select(id => doc.GetElement(id)).ToList();

                // 应用数量限制
                if (Limit.HasValue && Limit.Value > 0)
                {
                    selectedElements = selectedElements.Take(Limit.Value).ToList();
                }

                // 转换为ElementInfo列表
                ResultElements = selectedElements.Select(element => new ElementInfo
                {
                    Id = element.Id.GetIdValue(),
                    UniqueId = element.UniqueId,
                    Name = element.Name,
                    Category = element.Category?.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", "获取选中元素失败: " + ex.Message);
                ResultElements = new List<ElementInfo>();
            }
            finally
            {
                TaskCompleted = true;
                _resetEvent.Set();
            }
        }

        public string GetName()
        {
            return "获取选中元素";
        }
    }
}