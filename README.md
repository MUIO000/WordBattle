    # Wordbattle Game (单词大作战)

    ## 📖 项目简介 (Introduction)
    **Wordbattle Game** 是一款结合了**英语词汇学习**与**策略塔防战斗**的 Unity 游戏。玩家扮演召唤师，通过回答英语单词问题来积累能量或触发技能，召唤英雄和士兵与敌人进行对战。游戏旨在让玩家在紧张刺激的战斗中轻松记忆和巩固英语单词。

    ## ✨ 核心功能 (Features)

    ### 1. ⚔️ 策略战斗系统 (Battle System)
    *   **召唤机制**：通过 `SummonerController` 召唤不同类型的士兵协助战斗。
    *   **英雄系统**：多位拥有独特技能和属性的英雄可供选择 (`HeroController`)。
    *   **敌人与Boss**：挑战具有不同攻击模式的普通敌人和强力 Boss (`BossController`, `EnemyUnit`)。
    *   **自动寻路与攻击**：单位会自动寻找最近的敌人并进行攻击。

    ### 2. 📚 英语问答模块 (Vocabulary Learning)
    *   **答题互动**：游戏的核心驱动力。通过 `VocabularyManager` 和 `QuestionPanel` 实现。
    *   **题库管理**：`QuestionDatabase` 支持题库的导入、循环出题以及答错题目的复习机制。
    *   **表现评估**：根据答题准确率和速度进行评级，并在结算时给予奖励。

    ### 3. 📈 养成与进阶 (Progression)
    *   **英雄/士兵列表**：
        *   在 `HeroList` 和 `SoldierList` 界面查看、解锁和升级你的单位。
        *   升级系统包含等级限制和金币消耗逻辑。
    *   **经济系统**：通过战斗获取金币，用于购买更强大的单位。

    ### 4. 🛠️ 系统功能 (System Features)
    *   **登录系统**：完整的登录流程 (`Login.cs`)，包含后端 API 交互及 Swagger 定义支持。
    *   **平滑加载**：实现了带有进度条和平滑动画的场景加载系统 (`LoadingSceneController`)。
    *   **UI 交互**：使用 TextMeshPro 提供高质量文本显示，包含丰富的动画效果。

    ## 📂 项目结构 (Project Structure)

    主要脚本位于 `Assets/Scripts/` 目录下：

    *   **Battle/**: 战斗场景核心逻辑
        *   `BattleManager.cs`: 战斗流程控制（开始、胜利、失败）。
        *   `SummonerController.cs`: 召唤师逻辑。
        *   `Unit.cs`, `HeroController.cs`, `SoldierController.cs`: 单位基类与控制器。
        *   **Core/**: 
            *   `GameManager.cs`: 全局游戏管理。
            *   `VocabularyManager.cs`: 词汇与题目逻辑。
        *   **Data/**: `QuestionDatabase.cs` (题库), `UnitData.cs` (单位数据)。
    *   **HeroList/**: 英雄选择与养成界面逻辑。
    *   **SoldierList/**: 士兵选择与养成界面逻辑。
    *   **MainMenu/**: 主菜单与登录 (`Login.cs`) 逻辑。
    *   **UI/** & **Panel/**: 通用 UI 组件与面板逻辑。

    ## 🚀 快速开始 (Getting Started)

    ### 环境要求
    *   **Unity 版本**: 建议使用 2021.3 LTS 或更高版本。
    *   **依赖包**: 
        *   `TextMeshPro`: 用于 UI 文本显示。
        *   `Newtonsoft.Json`: 用于处理 JSON 数据。

    ### 安装与运行
    1.  克隆本项目到本地。
    2.  使用 Unity Hub 添加并打开项目文件夹。
    3.  如果出现材质或字体丢失，请确保已导入 TextMeshPro 基本资源 (`Window -> TextMeshPro -> Import TMP Essential Resources`)。
    4.  打开 `Assets/Scenes/Login.unity` (或 `MainMenu.unity`) 场景开始游戏。

    ## 🕹️ 玩法说明
    1.  **登录**：进入游戏，登录账号。
    2.  **备战**：在主菜单进入英雄或士兵列表，调整出战阵容并升级单位。
    3.  **战斗**：
        *   进入战斗场景。
        *   屏幕下方会出现英语单词选择题。
        *   **正确答题**：获得召唤资源或触发英雄攻击。
        *   **召唤单位**：点击底部士兵图标消耗资源召唤援军。
        *   **胜利条件**：击败关卡 Boss。

    ---
    *Created for English Learning & Gaming Fun.*

