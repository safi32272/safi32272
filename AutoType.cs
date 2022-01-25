using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoType : MonoBehaviour
{
    public float letterPause = 0.02f;
    public AudioClip sound;
    public AudioSource audio;
    public string message;
    public GameObject okBt;
    private void Start()
    {
        if (okBt)
        {
            okBt.SetActive(false);
        }

        message = GetComponent<Text>().text;
        this.GetComponent<Text>().text = "";
        //GetComponent<AudioSource>().Play();

        StartCoroutine(TypeText());
    }
    // Use this for initialization
    void OnEnable()
    {
        
        //if (sound)
    }
     
    IEnumerator TypeText()
    {
        foreach (char letter in message.ToCharArray())
        {
            GetComponent<Text>().text += letter;
            if (sound)
                audio.PlayOneShot(sound);
            // yield return 0;
            yield return new WaitForSeconds(letterPause);
        }
        if(okBt)
        {
            okBt.SetActive(true);
        }
        
    }
}
