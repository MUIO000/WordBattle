using UnityEngine;
using System.Collections.Generic;

public class VocabularyManager : MonoBehaviour
{
    private QuestionDatabase questionDatabase;
    [SerializeField] private int optionsCount = 4;  // 保留这个在Inspector中设置，因为这是配置项

    private int currentQuestionIndex = -1;
    private List<int> usedIndices = new List<int>();
    private List<int> wrongIndices = new List<int>(); // 记录上一轮答错的题目索引
    private bool isReviewingWrong = false;            // 是否在复习错题
    private int wrongReviewPointer = 0;               // 当前复习到第几个错题

    // 改为private，只允许通过方法访问
    private QuestionData currentQuestion;
    private string[] currentOptions;
    private int correctAnswerIndex;

    // 添加属性访问器
    public QuestionData CurrentQuestion => currentQuestion;
    public string[] CurrentOptions => currentOptions;
    public int CorrectAnswerIndex => correctAnswerIndex;

    private void Awake()
    {
        questionDatabase = FindObjectOfType<QuestionDatabase>();
        if (questionDatabase == null)
        {
            Debug.LogError("场景中未找到 QuestionDatabase!");
        }
        else
        {
            Debug.Log($"VocabularyManager Awake: 找到QuestionDatabase，题目数量={questionDatabase.QuestionCount}");
            // 在Awake中就准备第一道题
            usedIndices.Clear();
            if (questionDatabase.QuestionCount > 0)
            {
                NextQuestion();
                Debug.Log($"VocabularyManager Awake: 第一题准备完成，题目={currentQuestion?.question}");
            }
        }
    }

    // 初始化 - 现在只需要确保数据已准备好
    private void Start()
    {
        // Start方法现在主要用于确认初始化完成
        if (currentQuestion == null && questionDatabase != null && questionDatabase.QuestionCount > 0)
        {
            NextQuestion();
        }
    }

    // 生成下一道题
    public void NextQuestion()
    {
        if (questionDatabase == null || questionDatabase.QuestionCount == 0)
        {
            Debug.LogError("题库未初始化或为空");
            return;
        }

        // 第一轮题库用完，进入错题复习
        if (usedIndices.Count >= questionDatabase.QuestionCount)
        {
            Debug.Log("本轮题目已完成，准备下一轮");
            usedIndices.Clear();
            
            // 如果有错题，进入错题复习模式
            if (wrongIndices.Count > 0)
            {
                isReviewingWrong = true;
                wrongReviewPointer = 0;
                Debug.Log($"开始错题复习，共{wrongIndices.Count}道错题");
            }
            else
            {
                isReviewingWrong = false;
                Debug.Log("没有错题，开始新一轮");
            }
        }

        if (isReviewingWrong && wrongReviewPointer < wrongIndices.Count)
        {
            // 错题复习模式
            currentQuestionIndex = wrongIndices[wrongReviewPointer];
            wrongReviewPointer++;
            
            // 如果是最后一道错题
            if (wrongReviewPointer >= wrongIndices.Count)
            {
                wrongIndices.Clear();
                isReviewingWrong = false;
                Debug.Log("错题复习完成");
            }
        }
        else
        {
            // 正常出题模式
            do
            {
                currentQuestionIndex = Random.Range(0, questionDatabase.QuestionCount);
            } while (usedIndices.Contains(currentQuestionIndex));
            
            usedIndices.Add(currentQuestionIndex);
        }

        // 获取题目并检查
        currentQuestion = questionDatabase.GetQuestion(currentQuestionIndex);
        if (currentQuestion == null)
        {
            Debug.LogError($"无法获取题目: {currentQuestionIndex}");
            return;
        }

        GenerateOptions();
    }

    // 生成选项（含正确答案和干扰项）
    private void GenerateOptions()
    {
        if (currentQuestion == null)
        {
            Debug.LogError("当前题目为空，无法生成选项");
            return;
        }

        // 直接使用题目的选项
        currentOptions = currentQuestion.options;
        correctAnswerIndex = currentQuestion.correctIndex;
    }

    // 检查答案
    public bool CheckAnswer(int selectedIndex)
    {
        bool isCorrect = selectedIndex == correctAnswerIndex;
        if (!isCorrect && !wrongIndices.Contains(currentQuestionIndex))
        {
            wrongIndices.Add(currentQuestionIndex);
        }
        return isCorrect;
    }
}
