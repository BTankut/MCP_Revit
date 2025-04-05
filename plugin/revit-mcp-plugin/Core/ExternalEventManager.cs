using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using revit_mcp_sdk.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_mcp_plugin.Core
{
    /// <summary>
    /// 管理外部事件的创建和生命周期
    /// </summary>
    public class ExternalEventManager
    {
        private static ExternalEventManager _instance;
        private Dictionary<string, ExternalEventWrapper> _events = new Dictionary<string, ExternalEventWrapper>();
        private bool _isInitialized = false;
        private UIApplication _uiApp;
        private ILogger _logger;

        public static ExternalEventManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ExternalEventManager();
                return _instance;
            }
        }

        private ExternalEventManager() { }

        public void Initialize(UIApplication uiApp, ILogger logger)
        {
            _uiApp = uiApp;
            _logger = logger;
            _isInitialized = true;
        }

        /// <summary>
        /// 获取或创建外部事件
        /// </summary>
        public ExternalEvent GetOrCreateEvent(IWaitableExternalEventHandler handler, string key)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("ExternalEventManager 尚未初始化");

            // 如果存在且处理器匹配，直接返回
            if (_events.TryGetValue(key, out var wrapper) &&
                wrapper.Handler == handler)
            {
                return wrapper.Event;
            }

            // 需要在UI线程中创建事件
            ExternalEvent externalEvent = null;

            // 使用活动文档的上下文执行创建事件的操作
            _uiApp.ActiveUIDocument.Document.Application.ExecuteCommand(
                (uiApp) => {
                    externalEvent = ExternalEvent.Create(handler);
                }
            );

            if (externalEvent == null)
                throw new InvalidOperationException("无法创建外部事件");

            // 存储事件
            _events[key] = new ExternalEventWrapper
            {
                Event = externalEvent,
                Handler = handler
            };

            _logger.Info($"为 {key} 创建了新的外部事件");

            return externalEvent;
        }

        /// <summary>
        /// 清除事件缓存
        /// </summary>
        public void ClearEvents()
        {
            _events.Clear();
        }

        private class ExternalEventWrapper
        {
            public ExternalEvent Event { get; set; }
            public IWaitableExternalEventHandler Handler { get; set; }
        }
    }
}

namespace Autodesk.Revit.DB
{
    public static class ApplicationExtensions
    {
        public delegate void CommandDelegate(UIApplication uiApp);

        /// <summary>
        /// 在 Revit 上下文中执行命令
        /// </summary>
        public static void ExecuteCommand(this Autodesk.Revit.ApplicationServices.Application app, CommandDelegate command)
        {
            // 这个方法在 Revit 上下文中调用，可以安全地创建 ExternalEvent
            command?.Invoke(new UIApplication(app));
        }
    }
}