using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using revit_mcp_sdk.API.Base;
using System;

namespace SampleCommandSet.Commands.Access
{
    /// <summary>
    /// 获取当前选中元素的命令
    /// </summary>
    public class GetSelectedElementsCommand : ExternalEventCommandBase
    {
        private GetSelectedElementsEventHandler _handler => (GetSelectedElementsEventHandler)Handler;

        public override string CommandName => "get_selected_elements";

        public GetSelectedElementsCommand(UIApplication uiApp)
            : base(new GetSelectedElementsEventHandler(), uiApp)
        {
        }

        public override object Execute(JObject parameters, string requestId)
        {
            try
            {
                // 解析参数
                int? limit = parameters?["limit"]?.Value<int>();

                // 设置数量限制
                _handler.Limit = limit;

                // 触发外部事件并等待完成
                if (RaiseAndWaitForCompletion(15000))
                {
                    return _handler.ResultElements;
                }
                else
                {
                    throw new TimeoutException("获取选中元素超时");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"获取选中元素失败: {ex.Message}");
            }
        }
    }
}