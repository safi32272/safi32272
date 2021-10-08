using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> sentences;
    public TextMeshProUGUI npc_name, dialogue_text;
    public UnityEvent onStartDialogue, onEndDialgoue;
    public Button continue_button;
    public Animator player,dialogue_boxAnim;
    public int textcounter=0;


    public enum AnimState { Talking,idle}
    void Start()
    {
        sentences=new Queue<string>(); 
    }

  public void StartConversation(Dialogue dialogue)
    {
        dialogue.onStart?.Invoke();
        dialogue_boxAnim.SetBool("isOpen", true);
        player=dialogue.player;
        sentences.Clear();
        npc_name.text=dialogue.name;
        foreach ( string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextDialogue(dialogue);
      
    
        continue_button.onClick.RemoveAllListeners();
        continue_button.onClick.AddListener(() => DisplayNextDialogue(dialogue));
        AnimatorController(dialogue, "Talking");
    }
 
    public void DisplayNextDialogue(Dialogue dialogue)
    {
        AnimatorController(dialogue,"Talking");

       
        if (sentences.Count==0)
        {
            EndOfDialogue(dialogue);
            AnimatorController(dialogue, "OnGround");
            
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeOfSentence(sentence, dialogue));

    }

   public void AnimatorController(Dialogue dialogue, string anim)
    {
        //if (player.gameObject.tag=="Player")
        //{

        //}
        
        if (dialogue.simple_Talking==true)
        {
            if (anim == "Talking")
            {
                dialogue.player.SetBool("Talking", true);
                //dialogue.player.SetBool("OnGround", false);
            }
            else if (anim == "OnGround")
            {
                dialogue.player.SetBool("Talking", false);
                //dialogue.player.SetBool("OnGround", true);
            }
        }
        else if (dialogue.call_Talking == true)
        {
            if (anim == "Talking")
            {
                dialogue.player.SetBool("CallTalking", true);
                //dialogue.player.SetBool("OnGround", false);
            }
            else if (anim == "OnGround")
            {
                dialogue.player.SetBool("CallTalking", false);
                //dialogue.player.SetBool("OnGround", true);
            }
        }
      
       

      
    }

    IEnumerator TypeOfSentence(string sentence,Dialogue dialogue)
    {
        dialogue_text.text="";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogue_text.text+=letter;
        
            if (dialogue_text.text.Length==sentence.Length)
            {
                AnimatorController(dialogue, "OnGround");
            }
            yield return null;
        }
    }

    public void EndOfDialogue(Dialogue dialogue)
    {

        dialogue.OnEnd?.Invoke();
        if (dialogue.isTwoWayConversation==true)
        {
              dialogue_boxAnim.SetBool("isOpen", true);
         
            
        }
        else
        {
            dialogue_boxAnim.SetBool("isOpen", false);
        }

    }
}
