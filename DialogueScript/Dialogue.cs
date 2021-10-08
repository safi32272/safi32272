 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue 
{
    public UnityEvent OnEnd,onStart;
    public Animator player;
    public bool isTwoWayConversation,simple_Talking,call_Talking;

    public string name;
    [TextArea(0, 10)]
    public string[] sentences;
   
}
