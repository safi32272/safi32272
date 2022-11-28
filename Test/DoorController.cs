using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    int id;
    void OnEnable()
    {
        GameEvents.onDoorTriggerEnter += OnDoorWayOpen;
    }
    private void OnDisable()
    {
        GameEvents.onDoorTriggerEnter -= OnDoorWayOpen;
    }
    void OnDoorWayOpen(int _id)
    {
        print("dooropen");
        StartCoroutine(DoorOpen());
    }
    IEnumerator DoorOpen()
    {
        print("dooropencalled");
        while (this.transform.position.y<5)
        {
            float x = this.transform.position.y;

            this.transform.position = new Vector3(this.transform.position.x, x, this.transform.position.z);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
