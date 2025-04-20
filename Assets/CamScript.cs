using Unity.VisualScripting;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    public Transform player;
    public player playScrip;

    public Vector3 camPos;
    public Vector3 playerPos;
    public Rigidbody rb;
    public Vector3 velocity;

    public Vector3 offset;

    public float maxLead;
    public float leadPerc;

    public float xtarget;
    public float ytarget;
    public Vector3 targetPos;
    public Vector3 fluidPos;

    public float zoomFactor;
    public float snapDistance;
    public float lerpFactor;

    private void Update()
    {
        camPos = new Vector3(transform.position.x, transform.position.y - offset.y, 0);
        playerPos = player.transform.position;

        velocity = rb.linearVelocity;

        xtarget = playerPos.x + Mathf.Clamp(velocity.x, -maxLead, maxLead) * leadPerc;

        ytarget = playerPos.y + Mathf.Clamp(velocity.y, -maxLead, maxLead) * leadPerc;

        ytarget = Mathf.Clamp(ytarget, offset.y, Mathf.Infinity);


        targetPos = new Vector3(xtarget, ytarget, 0);



        


    }

    private void LateUpdate()
    {
        Camera.main.fieldOfView = 60f - (playScrip.tf * zoomFactor);
        flyTo(targetPos);
    }

    private void flyTo(Vector3 target)
    {
        Debug.Log(Mathf.Abs((targetPos - camPos).magnitude));   
        if (playScrip.tf <= 0)
        {
            Debug.Log("Trying to Fly");
            if (Mathf.Abs((targetPos - camPos).magnitude) > snapDistance * 4)
            {
                Debug.Log("Flying");
                   
                fluidPos = Vector3.Lerp(camPos, target, lerpFactor);
                transform.position = fluidPos + offset;

            }
            else if (Mathf.Abs((targetPos - camPos).magnitude) > snapDistance)
            {
                Debug.Log("Speeding");
                fluidPos = Vector3.Lerp(camPos, target, .85f);
                transform.position = fluidPos + offset;
            }

            else
            {
                Debug.Log("Snapping" + Time.time);
                transform.position = target + offset;
            }
        }

    }
}
