using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggerarea : MonoBehaviour
{
   public int id;
    public GameObject attachObject;
    public Color[] colors;

    [SerializeField] AudioCollection audioCollection;
    private void OnEnable()
    {
        GameEvents.onDoorTriggerEnter += OnDoorWayOpen;
        //GameEvents.onTriggerExited += OnDoorClose;
        GameEvents.onToolsChanger += ToolChangers;
    }
    private void OnDisable()
    {
        GameEvents.onDoorTriggerEnter -= OnDoorWayOpen;
        //GameEvents.onTriggerExited -= OnDoorClose;
        GameEvents.onToolsChanger -= ToolChangers;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            attachObject = other.gameObject;
            attachObject.GetComponent<tools>().id = id;
            attachObject.transform.parent = this.transform;
            attachObject.transform.position = new Vector3(0, 0, 0);
            //GameEvents.onDoorTriggerEnter += OnDoorWayOpen;
            GameEvents.OnDoorTriggerEvent(id);
            GameEvents.OnToolChanger(id);
            //PlayTriggersound();
        }
        PlayTriggersound();
    }
    public void OnDoorWayOpen(int _id)
    {
        print(_id);
        if (id == _id && attachObject)
        {
            //this.GetComponent<MeshRenderer>().materials[0].color = Color.red;
            attachObject.GetComponent<tools>().id = 0;
            attachObject.transform.parent = null;
            attachObject = null;
            //GameEvents.current.click.onClick.RemoveListener(() = GameEvents.onDoorTriggerEnter(id));
            //GameEvents.onDoorTriggerEnter -= OnDoorWayOpen;
            //PlayTriggersound();
        }
       
    }
    public void ToolChangers(int _id)
    {
        print(id + "" + _id);
        if(id==_id && attachObject)
        {

            attachObject.GetComponent<MeshRenderer>().materials[0].color=colors[Random.Range(0,colors.Length)];
        }
        //PlayTriggersound();
    }
    public void OnDoorClose(int _id)
    {
        if (id == _id)
        {
            this.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
        }
    
    }
  public void PlayTriggersound()
    {
        if(audioCollection!=null && AudioManager.instance!=null)
        {
            AudioClip soundToPlay;
            soundToPlay = audioCollection[1];
            AudioManager.instance.PlayOneShotSound("Player", soundToPlay, this.transform.position, audioCollection.volume, audioCollection.spatialBlend, audioCollection.priority);
        }
    }
}
