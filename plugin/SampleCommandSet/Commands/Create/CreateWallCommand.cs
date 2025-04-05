using Autodesk.Revit.UI;
using revit_mcp_sdk.API.Base;
using System;
using Newtonsoft.Json.Linq;

namespace SampleCommandSet.Commands.Create
{
    /// <summary>
    /// 创建墙命令
    /// </summary>
    public class CreateWallCommand : ExternalEventCommandBase
    {
        private CreateWallEventHandler _handler => (CreateWallEventHandler)Handler;

        /// <summary>
        /// 命令名称
        /// </summary>
        public override string CommandName => "create_Wall";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uiApp">Revit UIApplication</param>
        public CreateWallCommand(UIApplication uiApp)
            : base(new CreateWallEventHandler(), uiApp)
        {
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameters">JSON参数</param>
        /// <param name="requestId">请求ID</param>
        /// <returns>命令执行结果</returns>
        public override object Execute(JObject parameters, string requestId)
        {
            try
            {
                // 解析墙参数
                double startX = parameters["startX"].Value<double>();
                double startY = parameters["startY"].Value<double>();
                double endX = parameters["endX"].Value<double>();
                double endY = parameters["endY"].Value<double>();
                double height = parameters["height"].Value<double>();
                double thickness = parameters["thickness"].Value<double>();

                // 设置墙体参数
                _handler.SetWallParameters(startX, startY, endX, endY, height, thickness);

                // 触发外部事件并等待完成
                if (RaiseAndWaitForCompletion(10000))
                {
                    return _handler.CreatedWallInfo;
                }
                else
                {
                    throw new TimeoutException("创建墙体操作超时");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"创建墙体失败: {ex.Message}");
            }
        }
    }
}
