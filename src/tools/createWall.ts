// import { z } from "zod";
// import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
// import { RevitClientConnection } from "../utils/SocketClient.js";

// export function registerCreateWallTool(server: McpServer) {
//   server.tool(
//     "createWall",
//     "create wall",
//     {
//       startX: z.number(),
//       startY: z.number(),
//       endX: z.number(),
//       endY: z.number(),
//       height: z.number(),
//       thickness: z.number(),
//     },
//     async (args, extra) => {
//       // 创建参数对象
//       const params = {
//         startX: args.startX,
//         startY: args.startY,
//         endX: args.endX,
//         endY: args.endY,
//         height: args.height,
//         thickness: args.thickness,
//       };

//       const revitClient = new RevitClientConnection("localhost", 8080);

//       try {
//         // 等待连接建立
//         await new Promise<void>((resolve, reject) => {
//           // 如果已连接，直接解析
//           if (revitClient.isConnected) {
//             resolve();
//             return;
//           }

//           // 设置临时事件监听器
//           const onConnect = () => {
//             revitClient.socket.removeListener("connect", onConnect);
//             revitClient.socket.removeListener("error", onError);
//             resolve();
//           };

//           const onError = (error: any) => {
//             revitClient.socket.removeListener("connect", onConnect);
//             revitClient.socket.removeListener("error", onError);
//             reject(new Error("连接到Revit客户端失败"));
//           };

//           // 添加事件监听器
//           revitClient.socket.on("connect", onConnect);
//           revitClient.socket.on("error", onError);

//           // 连接到服务器
//           revitClient.connect();

//           // 设置连接超时
//           setTimeout(() => {
//             revitClient.socket.removeListener("connect", onConnect);
//             revitClient.socket.removeListener("error", onError);
//             reject(new Error("连接到Revit客户端超时"));
//           }, 5000);
//         });

//         // 使用新的sendCommand方法，直接传递命令名称和参数
//         const response = await revitClient.sendCommand("createWall", params);

//         // 检查是否有错误信息
//         if (response.errorMessage && response.errorMessage.trim() !== "") {
//           return {
//             content: [
//               {
//                 type: "text",
//                 text: `创建墙体失败: ${response.errorMessage}`,
//               },
//             ],
//             isError: true,
//           };
//         }

//         // 成功创建墙体，返回详细信息
//         return {
//           content: [
//             {
//               type: "text",
//               text: `墙体创建成功！\n墙体ID: ${response.elementId}\n起点: (${response.startPoint.x}, ${response.startPoint.y})\n终点: (${response.endPoint.x}, ${response.endPoint.y})\n高度: ${response.height}\n厚度: ${response.thickness}`,
//             },
//           ],
//           isError: false,
//         };
//       } catch (error) {
//         return {
//           content: [
//             {
//               type: "text",
//               text: `处理Revit响应时出错: ${
//                 error instanceof Error ? error.message : String(error)
//               }`,
//             },
//           ],
//           isError: true,
//         };
//       } finally {
//         // 操作完成后关闭连接
//         revitClient.disconnect();
//       }
//     }
//   );
// }
