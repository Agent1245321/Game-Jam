using UnityEngine;

public class launcher : MonoBehaviour
{


    public float xn;
    public float xx;
    public float yn;
    public float yx;

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<player>(out player p))
        {
            p.Launch(xn, xx, yn, yx);
        }
    }

}

