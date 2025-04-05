using Autodesk.Revit.UI;
using revit_mcp_sdk.API.Interfaces;
using revit_mcp_sdk.API.Versioning;
using revit_mcp_plugin.Configuration;
using revit_mcp_plugin.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace revit_mcp_plugin.Core
{
    /// <summary>
    /// 命令管理器，负责加载和管理命令
    /// </summary>
    public class CommandManager
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly ILogger _logger;
        private readonly ConfigurationManager _configManager;
        private readonly UIApplication _uiApplication;
        private readonly RevitVersionAdapter _versionAdapter;

        public CommandManager(
            ICommandRegistry commandRegistry,
            ILogger logger,
            ConfigurationManager configManager,
            UIApplication uiApplication)
        {
            _commandRegistry = commandRegistry;
            _logger = logger;
            _configManager = configManager;
            _uiApplication = uiApplication;
            _versionAdapter = new RevitVersionAdapter(_uiApplication.Application);
        }

        /// <summary>
        /// 加载配置文件中指定的所有命令
        /// </summary>
        public void LoadCommands()
        {
            _logger.Info("开始加载命令");
            string currentVersion = _versionAdapter.GetRevitVersion();
            _logger.Info("当前 Revit 版本: {0}", currentVersion);

            // 从配置加载外部命令
            foreach (var commandConfig in _configManager.Config.Commands)
            {
                try
                {
                    if (!commandConfig.Enabled)
                    {
                        _logger.Info("跳过禁用的命令: {0}", commandConfig.CommandName);
                        continue;
                    }

                    // 检查版本兼容性
                    if (commandConfig.SupportedRevitVersions != null &&
                        commandConfig.SupportedRevitVersions.Length > 0 &&
                        !_versionAdapter.IsVersionSupported(commandConfig.SupportedRevitVersions))
                    {
                        _logger.Warning("命令 {0} 不支持当前 Revit 版本 {1}，已跳过",
                            commandConfig.CommandName, currentVersion);
                        continue;
                    }

                    // 替换路径中的版本占位符
                    commandConfig.AssemblyPath = commandConfig.AssemblyPath.Contains("{VERSION}")
                        ? commandConfig.AssemblyPath.Replace("{VERSION}", currentVersion)
                        : commandConfig.AssemblyPath;

                    // 加载外部命令程序集
                    LoadCommandFromAssembly(commandConfig);
                }
                catch (Exception ex)
                {
                    _logger.Error("加载命令 {0} 失败: {1}", commandConfig.CommandName, ex.Message);
                }
            }

            _logger.Info("命令加载完成");
        }

        /// <summary>
        /// 加载特定程序集中的特定命令
        /// </summary>
        /// <param name="commandName">命令名称</param>
        /// <param name="assemblyPath">程序集路径</param>
        private void LoadCommandFromAssembly(CommandConfig config)
        {
            try
            {
                // 确定程序集路径
                string assemblyPath = config.AssemblyPath;
                if (!Path.IsPathRooted(assemblyPath))
                {
                    // 如果不是绝对路径，则相对于Commands目录
                    string baseDir = PathManager.GetCommandsDirectoryPath();
                    assemblyPath = Path.Combine(baseDir, assemblyPath);
                }

                if (!File.Exists(assemblyPath))
                {
                    _logger.Error("命令程序集不存在: {0}", assemblyPath);
                    return;
                }

                // 加载程序集
                Assembly assembly = Assembly.LoadFrom(assemblyPath);

                // 查找实现 IRevitCommand 接口的类型
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(revit_mcp_sdk.API.Interfaces.IRevitCommand).IsAssignableFrom(type) &&
                        !type.IsInterface &&
                        !type.IsAbstract)
                    {
                        try
                        {
                            // 创建命令实例
                            revit_mcp_sdk.API.Interfaces.IRevitCommand command;

                            // 检查命令是否实现了可初始化接口
                            if (typeof(IRevitCommandInitializable).IsAssignableFrom(type))
                            {
                                // 创建实例并初始化
                                command = (IRevitCommand)Activator.CreateInstance(type);
                                ((IRevitCommandInitializable)command).Initialize(_uiApplication);
                            }
                            else
                            {
                                // 尝试查找接受 UIApplication 的构造函数
                                var constructor = type.GetConstructor(new[] { typeof(UIApplication) });
                                if (constructor != null)
                                {
                                    command = (IRevitCommand)constructor.Invoke(new object[] { _uiApplication });
                                }
                                else
                                {
                                    // 使用无参构造函数
                                    command = (IRevitCommand)Activator.CreateInstance(type);
                                }
                            }

                            // 检查命令名称是否与配置匹配
                            if (command.CommandName == config.CommandName)
                            {
                                _commandRegistry.RegisterCommand(command);
                                _logger.Info("已注册外部命令: {0} (来自 {1})",
                                    command.CommandName, Path.GetFileName(assemblyPath));
                                break; // 找到匹配的命令后退出循环
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("创建命令实例失败 [{0}]: {1}", type.FullName, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("加载命令程序集失败: {0}", ex.Message);
            }
        }
    }
}
