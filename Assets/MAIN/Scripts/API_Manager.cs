using Meta.WitAi.TTS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.Windows;

public class API_Manager : MonoBehaviour
{
    [SerializeField] private TTSSpeaker _speaker;
    public string apiUrl;
    public string apiUrlAI;
    public float pollingInterval = 15f; // Time interval for polling the API (in seconds)
    public TMP_Text TranscriptsData;
    public List<GameObject> StudentList = new List<GameObject>();
    public string STUDENT_STATE = "IDEL";
    private string Pre_State = "";
    public AudioSource elspeker;
    private GameObject currentstudent;
    private bool test = false;
    private bool IsReq = false;

    [SerializeField] private string _dateId = "[DATE]";

    [System.Serializable]
    public class ParagraphData
    {
        public string paragraph;
    }
    void Start()
    {


        StartCoroutine(PollApiResponse());
    }

    private void Update()
    {
        if (test)
        {
            Debug.Log(elspeker.isPlaying);
            if (!elspeker.isPlaying)
            {
                _speaker.Stop();
                _speaker.CancelInvoke();
                test = false;
                IsReq = false;
                currentstudent.GetComponent<ModelController>().animator.SetTrigger("IDEL");
                STUDENT_STATE = "IDEL";
            }
        }
    }


    public void AskQuestion()
    {
        if(TranscriptsData.text != "" || TranscriptsData.text != null)
        {
            ParagraphData data = new ParagraphData
            {
                paragraph = TranscriptsData.text
            };

            // Start the coroutine to send data
            StartCoroutine(SendData(data));
        }

    }

    IEnumerator PollApiResponse()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(apiUrl);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error fetching data: " + www.error);
            }
            else
            {
                if (Pre_State!=www.downloadHandler.text)
                {
                    STUDENT_STATE = www.downloadHandler.text;
                    Pre_State = www.downloadHandler.text;
                }
            }

            yield return new WaitForSeconds(pollingInterval);
        }
    }

    IEnumerator SendData(ParagraphData data)
    {
        // Prepare JSON data from ParagraphData
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrlAI, jsonData, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                if (!IsReq)
                {
                    currentstudent = StudentList[UnityEngine.Random.Range(0, StudentList.Count)];
                    if (currentstudent.GetComponent<ModelController>().Gender == "F")
                    {
                        _speaker.VoiceID = "ROSIE";
                    }
                    string response = www.downloadHandler.text;
                    Debug.Log("Generated Question: " + response);
                    string phrase = FormatText(www.downloadHandler.text);
                    _speaker.Speak(phrase);
                    IsReq = true;
                }

            }
        }
    }

    // Process the response

    public void SudentAnimation(string TriggerName)
    {
        
        currentstudent.GetComponent<ModelController>().animator.SetTrigger(TriggerName);
        test = true;
    }

    private string FormatText(string text)
    {
        string result = text;
        if (result.Contains(_dateId))
        {
            DateTime now = DateTime.UtcNow;
            string dateString = $"{now.ToLongDateString()} at {now.ToLongTimeString()}";
            result = text.Replace(_dateId, dateString);
        }
        return result;
    }
}
