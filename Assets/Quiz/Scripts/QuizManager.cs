using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private QuizUI quizUI;
    [SerializeField] private List <QuizData> quizData;
    [SerializeField] private float timeLimit = 30f;

    private List<Question> questions;
    private Question selectedQuestion;
    private int scoreCount = 0;
    private float currentTime;
    private int lifeRemaining = 3;

    private GameStatus gameStatus = GameStatus.Next;

    public GameStatus GameStatus { get { return gameStatus; } }

    public void StartGame(int index)
    {
        quizUI.GameOverPanel.SetActive(false);
        scoreCount = 0;
        currentTime = timeLimit;
        lifeRemaining = 3;
        questions = new List<Question>();
        for (int i = 0; i < quizData[index].questions.Count; i++)
        {
            questions.Add(quizData[index].questions[i]);
        }

        

        SelectQuestion();
        gameStatus = GameStatus.Playing;
    }

    void SelectQuestion()
    {
        int val = UnityEngine.Random.Range(0, questions.Count);
        selectedQuestion = questions[val];

        quizUI.SetQuestion(selectedQuestion);

        questions.RemoveAt(val);
    }

    private void Update()
    {
        if (gameStatus == GameStatus.Playing)
        {
            currentTime -= Time.deltaTime;
            SetTimer(currentTime);
        }
    }

    private void SetTimer (float value)
    {
        TimeSpan time = TimeSpan.FromSeconds(value);
        quizUI.TimerText.text = "Time: " + time.ToString("mm' : 'ss");

        if(currentTime <= 0)
        {
            gameStatus = GameStatus.Next;
            quizUI.GameOverPanel.SetActive(true);
        }
    }


    public bool Answer(string answered)
    {
        bool correctAnswer = false;

        if (answered == selectedQuestion.correctAns)
        {
            correctAnswer = true;
            scoreCount += 50;
            quizUI.ScoreText.text = "Puntos: " + scoreCount;
        }
        else
        {
            lifeRemaining--;
            quizUI.ReduceLife(lifeRemaining);

            if(lifeRemaining <= 0)
            {
                gameStatus = GameStatus.Next;
                quizUI.GameOverPanel.SetActive(true);
            }

        }
        if (gameStatus == GameStatus.Playing)
        {
            if (questions.Count > 0)
            {
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                gameStatus = GameStatus.Next;
                quizUI.GameOverPanel.SetActive(true);
            }
            
        }
        

        return correctAnswer;
    }


}
[System.Serializable]
public class Question
{
    public string questioInfo;
    public QuestionType questionType;
    public Sprite questionImg;
    public AudioClip questionClip;
    public UnityEngine.Video.VideoClip questionVideo;
    public List<string> options;
    public string correctAns;
}

[System.Serializable]
public enum QuestionType
{
    TEXT,
    IMAGE,
    VIDEO,
    AUDIO
}

[System.Serializable]
public enum GameStatus
{
    Next,
    Playing
}
