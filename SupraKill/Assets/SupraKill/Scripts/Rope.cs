using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private int resolution, waveCount, wobbleCount;
    [SerializeField] private float waveSize, animSpeed;

    private LineRenderer line;

    private void Start()

    {
        line = GetComponent<LineRenderer>();
    }

    //Update is called once per frame

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine( routine: AnimateRope(target.position));
        }
    }

    private IEnumerator AnimateRope(Vector3 targetPos)
    {
        line.positionCount = resolution;
        float angle = LookAtAngle(target: targetPos - transform.position);

        float percent = 0;
        while (percent <= 1f)

        {
            percent += Time.deltaTime * animSpeed;
            SetPoints(targetPos, percent, angle);
            yield return null;
        }
        SetPoints(targetPos, percent: 1, angle);
    }

    private void SetPoints(Vector3 targetPos, float percent, float angle)
    {
        Vector3 ropeEnd = Vector3.Lerp(a: transform.position, b: targetPos, percent);
        float lenght = Vector2.Distance(a: transform.position, b: ropeEnd);

        for (int i = 0; i < resolution; i++)
        {
            float xPos = (float)i / resolution * lenght;
            float reversePercent = (1 - percent);

            float amplitude = Mathf.Sin(f: reversePercent * wobbleCount * Mathf.PI);

            float yPos = Mathf.Sin(f: (float) waveCount * i / resolution * 2 * Mathf.PI * reversePercent) * amplitude;

            Vector2 pos = RotatePoint(new Vector2(x: xPos + transform.position.x, y: yPos + transform.position.y), pivot: transform.position, angle);
            line.SetPosition(i, pos);

        }
    }

    Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle)
    {
        Vector2 dir = point - pivot;
        dir = Quaternion.Euler(x: 0, y: 0, z: angle) * dir;
        point = dir + pivot;
        return point;
    }

    private float LookAtAngle(Vector2 target)
    {
        return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
    }

}
