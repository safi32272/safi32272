
using UnityEngine;
using UnityEngine.Events;

public class Timeline : MonoBehaviour
{
    public UnityEvent onStartup, onEnd;
   
    void Start()
    {
        
    }
    private void OnDisable()
    {
        onEnd?. Invoke();
       PlayerPrefs.SetInt("Timeline", 1);
    }

}
