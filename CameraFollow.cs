using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float follow_height = 8f;
    public float follow_Distance = 6f;
    private Transform player;

    private float target_Height;
    private float current_Height;
    private float current_Rotation;
    // Start is called before the first frame update
    void Awake()
    {
        player=GameObject.FindGameObjectWithTag("Player").transform;
        //StartCoroutine(CameraFollowByco());
    }

    // Update is called once per frame
    void Update()
    {
        target_Height=player.position.y+follow_height;
        current_Rotation=transform.eulerAngles.y;
        current_Height=Mathf.Lerp(transform.position.y, target_Height, 0.9f*Time.deltaTime);
        Quaternion euler = Quaternion.Euler(0f, current_Rotation, 0f);
        Vector3 target_position = player.position-(euler*Vector3.forward)*follow_Distance;
        target_position.y=current_Height;
        transform.position=target_position;
        transform.LookAt(player);
    }

    IEnumerator CameraFollowByco()
    {
        yield return new WaitForSeconds(0.05f) ;
        target_Height=player.position.y+follow_height;
        current_Rotation=transform.eulerAngles.y;
        current_Height=Mathf.Lerp(transform.position.y, target_Height, 0.9f*Time.deltaTime);
        Quaternion euler = Quaternion.Euler(0f, current_Rotation, 0f);
        Vector3 target_position = player.position-(euler*Vector3.forward)*follow_Distance*Time.deltaTime;
        target_position.y=current_Height;
        transform.position=target_position;
        transform.LookAt(player);
        yield return StartCoroutine(CameraFollowByco());

    }
}
