using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionData
{
    public string question;      // 题干
    public string[] options;     // 选项
    public int correctIndex;     // 正确答案的索引

    // 添加验证方法
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(question) 
            && options != null 
            && options.Length > 0 
            && correctIndex >= 0 
            && correctIndex < options.Length;
    }
}

public class QuestionDatabase : MonoBehaviour
{
    [SerializeField] private TextAsset questionDataFile; // 可以从JSON文件加载
    private List<QuestionData> questions = new List<QuestionData>();
    private HashSet<string> questionSet = new HashSet<string>(); // 用于检查重复题目

    private void Awake()
    {
        LoadQuestions();
    }

    private void LoadQuestions()
    {
        // 这里先用硬编码，之后可以改为从文件加载
        AddQuestion("wants to", new string[] { "多个", "向前的", "很少的", "想要" }, 3);
        AddQuestion("apple", new string[] { "苹果", "香蕉", "橙子", "梨" }, 0);
        AddQuestion("fast", new string[] { "慢的", "快的", "高的", "矮的" }, 1);
        // 可以继续添加更多题目...
    }

    private void AddQuestion(string question, string[] options, int correctIndex)
    {
        // 检查题目是否重复
        if (questionSet.Contains(question))
        {
            Debug.LogWarning($"重复的题目: {question}");
            return;
        }

        var questionData = new QuestionData
        {
            question = question,
            options = options,
            correctIndex = correctIndex
        };

        // 验证题目数据
        if (!questionData.IsValid())
        {
            Debug.LogError($"无效的题目数据: {question}");
            return;
        }

        questions.Add(questionData);
        questionSet.Add(question);
    }

    public QuestionData GetQuestion(int index)
    {
        if (index >= 0 && index < questions.Count)
            return questions[index];
        Debug.LogWarning($"试图访问无效的题目索引: {index}");
        return null;
    }

    public int QuestionCount => questions.Count;
}