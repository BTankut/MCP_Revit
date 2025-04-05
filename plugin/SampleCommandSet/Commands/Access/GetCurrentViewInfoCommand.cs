using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using revit_mcp_sdk.API.Base;
using System;

namespace SampleCommandSet.Commands.Access
{
    public class GetCurrentViewInfoCommand : ExternalEventCommandBase
    {
        private GetCurrentViewInfoEventHandler _handler => (GetCurrentViewInfoEventHandler)Handler;

        public override string CommandName => "get_current_view_info";

        public GetCurrentViewInfoCommand(UIApplication uiApp)
            : base(new GetCurrentViewInfoEventHandler(), uiApp)
        {
        }

        public override object Execute(JObject parameters, string requestId)
        {
            // 触发外部事件并等待完成
            if (RaiseAndWaitForCompletion(10000)) // 10秒超时
            {
                return _handler.ResultInfo;
            }
            else
            {
                throw new TimeoutException("获取信息超时");
            }
        }
    }

}
