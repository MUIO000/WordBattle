# Wordbattle Game

## 📖 Introduction

**Wordbattle Game** is a Unity game that combines **English vocabulary learning** with **strategy tower defense combat**. Players take on the role of a summoner, accumulating energy or triggering skills by answering English word questions, and summoning heroes and soldiers to battle enemies. The game aims to help players easily memorize and reinforce English words through intense and exciting combat.

## ✨ Core Features

### 1. ⚔️ Strategy Battle System
*   **Summoning Mechanism**: Summon different types of soldiers to assist in battle through `SummonerController`.
*   **Hero System**: Multiple heroes with unique skills and attributes to choose from (`HeroController`).
*   **Enemies & Bosses**: Challenge regular enemies and powerful Bosses with different attack patterns (`BossController`, `EnemyUnit`).
*   **Auto Pathfinding & Attack**: Units automatically find and attack the nearest enemies.

### 2. 📚 English Vocabulary Learning Module
*   **Question-Answer Interaction**: The core driving force of the game. Implemented through `VocabularyManager` and `QuestionPanel`.
*   **Question Bank Management**: `QuestionDatabase` supports question bank import, cyclic question generation, and review mechanisms for incorrectly answered questions.
*   **Performance Evaluation**: Rating based on answer accuracy and speed, with rewards given at settlement.

### 3. 📈 Progression & Advancement
*   **Hero/Soldier List**:
    *   View, unlock, and upgrade your units in the `HeroList` and `SoldierList` interfaces.
    *   The upgrade system includes level restrictions and gold consumption logic.
*   **Economy System**: Earn gold through battles to purchase more powerful units.

### 4. 🛠️ System Features
*   **Login System**: Complete login flow (`Login.cs`), including backend API interaction and Swagger definition support.
*   **Smooth Loading**: Scene loading system with progress bars and smooth animations (`LoadingSceneController`).
*   **UI Interaction**: Uses TextMeshPro to provide high-quality text display with rich animation effects.

## 📂 Project Structure

Main scripts are located in the `Assets/Scripts/` directory:

*   **Battle/**: Core logic for battle scenes
    *   `BattleManager.cs`: Battle flow control (start, victory, defeat).
    *   `SummonerController.cs`: Summoner logic.
    *   `Unit.cs`, `HeroController.cs`, `SoldierController.cs`: Unit base class and controllers.
    *   **Core/**: 
        *   `GameManager.cs`: Global game management.
        *   `VocabularyManager.cs`: Vocabulary and question logic.
    *   **Data/**: `QuestionDatabase.cs` (question bank), `UnitData.cs` (unit data).
*   **HeroList/**: Hero selection and progression interface logic.
*   **SoldierList/**: Soldier selection and progression interface logic.
*   **MainMenu/**: Main menu and login (`Login.cs`) logic.
*   **UI/** & **Panel/**: Generic UI components and panel logic.

## 🚀 Getting Started

### Requirements
*   **Unity Version**: Recommended to use 2021.3 LTS or higher.
*   **Dependencies**: 
    *   `TextMeshPro`: For UI text display.
    *   `Newtonsoft.Json`: For handling JSON data.

### Installation & Running
1.  Clone this project to your local machine.
2.  Use Unity Hub to add and open the project folder.
3.  If materials or fonts are missing, ensure that TextMeshPro essential resources have been imported (`Window -> TextMeshPro -> Import TMP Essential Resources`).
4.  Open the `Assets/Scenes/Login.unity` (or `MainMenu.unity`) scene to start the game.

## 🕹️ Gameplay Guide
1.  **Login**: Enter the game and log in to your account.
2.  **Prepare for Battle**: Enter the hero or soldier list from the main menu, adjust your battle lineup and upgrade units.
3.  **Combat**:
    *   Enter the battle scene.
    *   English word multiple-choice questions will appear at the bottom of the screen.
    *   **Answer Correctly**: Gain summoning resources or trigger hero attacks.
    *   **Summon Units**: Click the soldier icons at the bottom to consume resources and summon reinforcements.
    *   **Victory Condition**: Defeat the stage Boss.

---
*Created for English Learning & Gaming Fun.*
