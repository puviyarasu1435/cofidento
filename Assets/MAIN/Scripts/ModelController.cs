using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    public Animator animator;
    public API_Manager manager;
    private string ModelState;
    public string Gender;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindObjectOfType<API_Manager>();
        ModelState = manager.STUDENT_STATE;
        animator.SetTrigger(manager.STUDENT_STATE);
    }

    // Update is called once per frame
    void Update()
    {
        if(ModelState!=manager.STUDENT_STATE && manager.STUDENT_STATE != "HAND_RAISE")
        {
            animator.SetTrigger(manager.STUDENT_STATE);
            ModelState = manager.STUDENT_STATE;
        }
    }
}
