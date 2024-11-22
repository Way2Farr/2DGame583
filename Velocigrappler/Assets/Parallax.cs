using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform player;
    private Vector2 startPos;
    private float startZ;

    private Vector2 Travel => (Vector2)cam.transform.position - startPos;

    private float DistanceFromSubject => transform.position.z - player.position.z;

    private float ClipPlane => (cam.transform.position.z + (DistanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));

    private float ParallaxFactor => Mathf.Abs(DistanceFromSubject) / ClipPlane;

    public void Start()
    {
        startPos = transform.position;
        startZ = transform.position.z;
    }

    public void Update()
    {
        Vector2 newPos = startPos + Travel * ParallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }
}