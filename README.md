# labs/ 资料库

Aether 的创作资产资料库：所有 agent、策略、技能、知识等资产以文件形式存放在这里，可版本管理（git）、可随 agent 按需加载。

> 各子目录内均有自己的 `readme.md`，说明该类资产的文件格式与创建入口。

## 目录结构

| 目录 | 存放内容 | 文件格式 | 说明 |
| --- | --- | --- | --- |
| `agents/` | **Agent（智能体）** 定义 | `.agent.md` | YAML front-matter（id/name/tools/model…）+ Markdown 正文（系统提示词） |
| `rules/` | **规则 agent**（无 LLM 的确定性规则大脑） | `.rule.yaml` | name/description/tools + steps（match→action）程序 |
| `skills/` | **技能**（可跨 agent 引用的提示词能力） | `<name>/SKILL.md` | front-matter（name/description/category）+ 正文，可带兄弟脚本 |
| `knowledge/` | **知识对象**（文档知识） | `.md` | kind + links 图边 + Markdown 正文，供 agent 查询引用 |
| `strategies/` | **声明式 YAML 策略** | `.strategy.yaml` | params + indicators + rules + default 配方，解析后供回测 |
| `codestrategies/` | **C# 代码策略** | `.cs` | `IStrategy` 实现类，Roslyn 编译注册进回测选择器 |
| `leanalgorithms/` | **Lean 算法策略** | `.lean.cs` | `LeanAlgorithmBase`（= QCAlgorithm）子类，跑 Lean 引擎 |
| `mcp-servers/` | **MCP server 配置**（外部工具接入） | `.json` | stdio（command+args）或 http（url），重连后注册进工具池 |
| `.github/` | GitHub Actions 工作流 | `.yml` | CI/发布流水线（如桌面应用三平台构建） |

## 补充说明

- `strategies/`、`codestrategies/`、`leanalgorithms/` 仅 Stockapp 租户（交易场景）使用，创建后即出现在回测/实盘的策略选择器中。
- 新建资产：在资料库页对应目录上右键「新建 → …」，或让 agent 通过相应工具创建。
- `mcp-servers/` 下的配置由宿主读取并重连；配置后其工具会自动勾选进所有 agent。
