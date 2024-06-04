using Meta.Voice.Samples.Dictation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
       obj.GetComponent<DictationActivation>().ToggleActivation();
    }




}
