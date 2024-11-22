using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using Player.Grapple;


public class grapplingRope : MonoBehaviour
{
    [SerializeField] Transform target; 

    [SerializeField] private int resolution, waveCount, wobbleCount;
    [SerializeField] private float waveSize, animSpeed;

    private LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public IEnumerator AnimRope(Vector3 targetPos) {

        line.positionCount = resolution;
        float angle = LookAtAngle( targetPos - transform.position);

        float percent = 0;
        while (percent <= 1f) {
            percent += Time.deltaTime * animSpeed;
            SetPoints(targetPos, percent,angle);
            yield return null;
        }
    SetPoints(targetPos,1, angle);
    }

    public void StopRope() {
        StopAllCoroutines();
        line.positionCount = 0;
    }

    private void SetPoints(Vector3 targetPos, float percent, float angle) {
        Vector3 ropeEnd = Vector3.Lerp(transform.position, targetPos, percent);
        float length = Vector2.Distance(transform.position, ropeEnd);

        for(int i =0; i < resolution; i++) {
            float xPos = (float) i /resolution * length;
            float reversePercent = (1 - percent);

            float amplitutde = Mathf.Sin(reversePercent * wobbleCount * Mathf.PI);

            float yPos = Mathf.Sin( (float) waveCount * i / resolution * 2 * Mathf.PI * reversePercent) * amplitutde;

            Vector2 pos = RotatePoint(new Vector2(xPos + transform.position.x, yPos + transform.position.y), transform.position, angle);
            line.SetPosition(i, pos);
        }
    }
    
    Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle) {

        Vector2 dir = point - pivot;
        dir = Quaternion.Euler (0,0, angle) * dir;
        point = dir + pivot;
        return point;
    }

    private float LookAtAngle (Vector2 target) {
        return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
    }
}
