using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cui.Desktop.Models;

namespace Cui.Desktop.Services;

public sealed class SampleDataService : IConversationDataSource
{
    public Task<IEnumerable<ChatMessage>> GetConversationAsync()
    {
        var now = DateTimeOffset.Now;
        var messages = new List<ChatMessage>
        {
            new()
            {
                Type = ChatMessageType.User,
                Timestamp = now,
                Text = "请帮我查看项目简介，并总结核心能力。"
            },
            new()
            {
                Type = ChatMessageType.Assistant,
                Timestamp = now.AddSeconds(2),
                Blocks =
                {
                    new ThinkingContentBlock("正在查找 docs/overview.md 以了解项目背景。"),
                    new ToolUseContentBlock(new ToolInvocation
                    {
                        ToolName = "read",
                        DisplayName = "读取文件",
                        Arguments = new Dictionary<string, object?>
                        {
                            { "path", "docs/overview.md" }
                        },
                        Result = ToolResult.Pending()
                    })
                }
            },
            new()
            {
                Type = ChatMessageType.Assistant,
                Timestamp = now.AddSeconds(5),
                Blocks =
                {
                    new ToolUseContentBlock(new ToolInvocation
                    {
                        ToolName = "read",
                        DisplayName = "读取文件",
                        Arguments = new Dictionary<string, object?>
                        {
                            { "path", "docs/overview.md" }
                        },
                        Result = ToolResult.Completed(output: "# Common Agent UI\n- 支持多模型\n- 浏览器实时监控")
                    }),
                    new TextContentBlock("我已读取项目概览，接下来会给出总结。")
                }
            },
            new()
            {
                Type = ChatMessageType.Assistant,
                Timestamp = now.AddSeconds(8),
                Blocks =
                {
                    new TextContentBlock("**核心能力**\n1. 多任务并行\n2. 多模型路由\n3. 推送与语音辅助")
                }
            },
            new()
            {
                Type = ChatMessageType.User,
                Timestamp = now.AddSeconds(12),
                Text = "继续梳理一下任务执行流程。"
            },
            new()
            {
                Type = ChatMessageType.Assistant,
                Timestamp = now.AddSeconds(14),
                Blocks =
                {
                    new ToolUseContentBlock(new ToolInvocation
                    {
                        ToolName = "task",
                        DisplayName = "执行子任务",
                        Arguments = new Dictionary<string, object?>
                        {
                            { "title", "梳理任务流程" }
                        },
                        Result = ToolResult.Completed(children: new[]
                        {
                            new ChatMessage
                            {
                                Type = ChatMessageType.Assistant,
                                Timestamp = now.AddSeconds(15),
                                Blocks =
                                {
                                    new TextContentBlock("调度器启动后台任务")
                                }
                            },
                            new ChatMessage
                            {
                                Type = ChatMessageType.Assistant,
                                Timestamp = now.AddSeconds(16),
                                Blocks =
                                {
                                    new TextContentBlock("实时推送状态到前端时间线")
                                }
                            }
                        })
                    }),
                    new ThinkingContentBlock("整理子任务结果，准备总结。")
                }
            },
            new()
            {
                Type = ChatMessageType.Assistant,
                Timestamp = now.AddSeconds(18),
                Blocks =
                {
                    new TextContentBlock("任务流程包括调度、执行与通知三个阶段。")
                }
            },
            new()
            {
                Type = ChatMessageType.Assistant,
                Timestamp = now.AddSeconds(20),
                Blocks =
                {
                    new ToolUseContentBlock(new ToolInvocation
                    {
                        ToolName = "edit",
                        DisplayName = "更新文档",
                        Arguments = new Dictionary<string, object?>
                        {
                            { "path", "docs/overview.md" }
                        },
                        Result = ToolResult.Completed(isError: true, errorMessage: "文件被锁定，无法写入。")
                    })
                }
            },
            new()
            {
                Type = ChatMessageType.Error,
                Timestamp = now.AddSeconds(21),
                Text = "更新文档失败：文件被其他进程占用。"
            }
        };

        return Task.FromResult<IEnumerable<ChatMessage>>(messages);
    }
}
