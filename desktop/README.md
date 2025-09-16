# CUI WPF Desktop Client

该目录包含基于 WPF + Telerik UI 的示例桌面前端，实现了 Common Agent UI 的时间线会话视图。项目核心特性：

- **消息分组与过滤**：自动合并相邻类型的消息，过滤掉仅包含 `tool_result` 的用户消息。
- **时间线式助手渲染**：助手消息按内容块逐个渲染，支持 Markdown、思考态、工具调用与 JSON 回退。
- **工具渲染管线**：`ToolContentControl` 按工具类型切换模板，展示文件读取、编辑、任务树及通用参数，并在 pending/error 状态下给出提示。
- **流式状态指示**：`MainViewModel` 跟踪工具执行与流式输出，底部提供动态指示点。
- **示例数据服务**：`SampleDataService` 构建示例会话，便于调试。

> ⚠️ 运行该项目需要安装 Telerik UI for WPF，并在本地开发环境中配置对应的 NuGet 源或引用。当前仓库不包含商业依赖。

## 结构

```
desktop/
├── Cui.Desktop/              # WPF 项目
│   ├── Models/               # 消息、工具与内容块模型
│   ├── ViewModels/           # MVVM 视图模型
│   ├── Views/Controls/       # 自定义控件（如 ToolContentControl）
│   ├── Services/             # 示例数据源等服务
│   ├── Selectors/            # 模板选择器
│   ├── App.xaml              # 应用入口
│   └── MainWindow.xaml       # 主界面
└── README.md
```

## 运行

1. 在 Windows 环境下安装 .NET 8 SDK，并确保可用 Telerik UI for WPF。
2. 在 `desktop/Cui.Desktop` 目录执行：
   ```bash
   dotnet restore
   dotnet build
   ```
3. 启动应用：
   ```bash
   dotnet run
   ```

应用启动后会自动加载示例会话，也可以通过右上角按钮重新加载数据。
