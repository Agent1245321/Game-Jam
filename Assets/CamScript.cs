using UnityEngine;

public class CamScript : MonoBehaviour
{
    public GameObject player;
    public player playScrip;

    public Vector3 offset;

    public float zoomFactor;

    private void LateUpdate()
    {
        Camera.main.fieldOfView = 60f - (playScrip.tf * zoomFactor);
      

        this.transform.position = player.transform.position + offset;
    }
}
