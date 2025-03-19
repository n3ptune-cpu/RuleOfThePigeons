using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player;

    public void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10);     
    }
}
