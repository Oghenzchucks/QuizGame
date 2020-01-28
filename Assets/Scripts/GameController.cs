using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Text questionText;
    public Text scoreDisplayText;
    public Text timeRemainingDisplayText;
    public SimpleObjectPool answerButtonObjectPool;
    public Transform answerButtonParent;
    public GameObject questionDisplay;
    public GameObject scoreDisplay;
    public GameObject roundEndDisplay;
    public GameObject previousButton;
    public GameObject UIPanel;
    public GameObject startPanel;
    public Text nextButton;

    private DataController dataController;
    private RoundData currentRoundData;
    private QuestionData[] questionPool;

    private bool isRoundActive;
    private float timeRemaining;
    private int questionIndex;
    private int playerScore;
    private List<GameObject> answerButtonGameObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        currentRoundData = dataController.GetCurrentRoundData();
        questionPool = currentRoundData.questions;
        timeRemaining = currentRoundData.timeLimitInSeconds;
        
        questionPool = Shuffle(questionPool);
        

        playerScore = 0;
        questionIndex = 0;

        
           
    }

    
    QuestionData[] Shuffle(QuestionData[] questionPool)
    {
        for (int t = 0; t < questionPool.Length; t++)
        {
            QuestionData tmp = questionPool[t];
            int r = Random.Range(t, questionPool.Length);
            questionPool[t] = questionPool[r];
            questionPool[r] = tmp;
        }

        return questionPool;
    }

    public void ShowQuestion()
    {
        isRoundActive = true;
        UIPanel.SetActive(true);
        RemoveAnswerButtons();
        QuestionData questionData = questionPool [questionIndex];
        questionText.text = questionData.questionText;
        
        for(int i = 0; i < questionData.answers.Length; i++)
        {
            GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();
            answerButtonGameObjects.Add(answerButtonGameObject);
            answerButtonGameObject.transform.SetParent(answerButtonParent);
           

            AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
            answerButton.Setup(questionData.answers[i]);
        }
    }

    private void RemoveAnswerButtons()
    {
        while (answerButtonGameObjects.Count > 0)
        {
            answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
            answerButtonGameObjects.RemoveAt(0);
        }
    }

    public void AnswerButtonClicked(bool isCorrect)
    {
        if (isCorrect)
        {
            playerScore += currentRoundData.pointsAddedForCorrectAnswers;
            scoreDisplayText.text = "Score: " + playerScore.ToString();
        }else
        {
            scoreDisplayText.text = "Score: " + playerScore.ToString();
        }
    }

    public void NextButton()
    {
        if (questionPool.Length > questionIndex + 1)
        {
            questionIndex++;
            ShowQuestion();
        } else
        {
            EndRound();
        }
    }

    public void PreviousButton()
    {
        if (questionIndex > 0)
        {
            questionIndex--;
            ShowQuestion();
        }
    }

    public void HandlePreviousButton(string value){
        if (value == "prev" && questionIndex == 1){
            previousButton.SetActive(false);
        }
        if (value == "next" && questionIndex == 0){
            previousButton.SetActive(true);
        }
    }

    public void HandleNextButton(string value){
        if (value == "prev" && questionIndex == questionPool.Length - 1){
            nextButton.text = "Next";
        }
        if (value == "next" && questionIndex == questionPool.Length - 2){
            nextButton.text = "Submit";
        }
    }

    public void EndRound()
    {
        isRoundActive = false;

        questionDisplay.SetActive(false);
        roundEndDisplay.SetActive(true);
        scoreDisplay.SetActive(true);
    }

    public void ReturnToMenu()
    {
        startPanel.SetActive(true);
        UIPanel.SetActive(false);
        roundEndDisplay.SetActive(false);
        questionDisplay.SetActive(false);
    }

    public void UpdateTimeRemainingDisplay()
    {
        timeRemainingDisplayText.text = "Time: " + Mathf.Round (timeRemaining).ToString();
    }

    public void Reset()
    {
        playerScore = 0;
        scoreDisplayText.text = "Score: " + playerScore.ToString();
        previousButton.SetActive(false);
        nextButton.text = "Next";
        questionIndex = 0;
        timeRemaining = currentRoundData.timeLimitInSeconds;
        scoreDisplay.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        if(isRoundActive)
        {
            UpdateTimeRemainingDisplay();
            timeRemaining -= Time.deltaTime;
            if(timeRemaining <= 0f)
            {
                EndRound();
            }
        }
    }
}
