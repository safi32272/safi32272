using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public Button call_button;
    public bool is_call = false;

    private void Start()
    {
        if (is_call == true)
        {
            if (call_button)
            {
                call_button.onClick.RemoveAllListeners();
                call_button.onClick.AddListener(TriggerDialgoue);
            }
        }
    }

    //public Animator player, Npc;
    public void TriggerDialgoue()
    {
        FindObjectOfType<DialogueManager>().StartConversation(dialogue);

    }
}
