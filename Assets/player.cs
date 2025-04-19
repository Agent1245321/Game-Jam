using System.Security.Claims;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    /*
     * Reminder to make falling its own state so we can diasble movement
     * 
     * Maybe look at switch statements, prob can be used more.
     * 
     * Splat Animation???
     * 
     * Actually parrying the wall and ground would be cool lmao
     * 
     * 
     * 
     * 
     * 
     */
    public Animator animator;

    public States state;

    public LayerMask theGround;

    private Vector3 dir;
    private Vector3 xdir;
    private Vector2 moveInput;
    public float crouchInput;

    [Header("References")]
    public GameObject tur;
    public Rigidbody rb;
    public Material material0;
    public Material material1;
    public Material material2;
    public Material material3;


    [Header("References2")]

    [Header("Tweaks")]
    private float p;
    public float parryWindow;
    public float parryCooldown;
    private float pc;

    public float timefreezeamount;
    public float timeslow;
    public float tf;
    public float moveSpeed;

    public RaycastHit hit;
    public bool grounded;

    public PlayerInput input;
   



    //stats



    public enum States
    {
        crawling,
        parry,
        rolling
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
           
            grounded = true;
            
            
        }
        else
        {
            grounded = false;
           
        }


            animator.SetFloat("Crouch", crouchInput);
        animator.SetFloat("WhoreInput", Mathf.Abs(moveInput.x));
        animator.SetBool("Grounded", grounded);
        if (p <= 0f && pc <= 0f && crouchInput > 0 && state == States.crawling)
        {
            p = parryWindow;
            pc = parryCooldown;
        }

        if (tf > 0)
        {
            Time.timeScale = timeslow;
            Time.fixedDeltaTime = timeslow * 0.02f;
            tf -= Time.fixedDeltaTime;
        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
        }

        if (p >0f)
        {
            state = States.parry;
            p -= Time.deltaTime;
        }

       
        else
        {
            if(crouchInput == 0)
            {
                state = States.crawling;
            }
            else
            {
                state = States.rolling;
            }
           

        }


        switch (state) 
        { 
            
                case States.crawling:
                tur.GetComponent<MeshRenderer>().material = material0;
                input.actions.FindAction("Move").Enable();

                break;

                case States.parry:
                tur.GetComponent<MeshRenderer>().material = material1;
                input.actions.FindAction("Move").Disable();
                break;

                case States.rolling:
                tur.GetComponent<MeshRenderer>().material = material2;
                input.actions.FindAction("Move").Disable();
                break;

                default:
                tur.GetComponent<MeshRenderer>().material = material3;
                break;


        }

        if (pc > 0f)
        {
            pc -= Time.deltaTime;
        }



    }



    public void OnCrouch(InputValue value)
    {
        crouchInput = value.Get<float>();
       
     
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputValue action)   
    {
        moveInput = action.Get<Vector2>();
    }

    public void Move()
    {
        tur.transform.position += new Vector3(moveInput.x * moveSpeed, 0, 0);
        if(moveInput.x > 0)
        {
            tur.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if(moveInput.x < 0)
        {
            tur.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void Launch(float xmin, float xmax, float ymin, float ymax)
    {
        dir = new Vector3(Random.Range(xmin, xmax), Random.Range(ymin, ymax), 0);
        xdir = new Vector3(Random.Range(xmin, xmax), 0, 0);

        switch (state)
        {

            case States.crawling:
                rb.AddForce(dir * .1f, ForceMode.VelocityChange);

                break;

            case States.parry:
                tf = timefreezeamount;
                rb.AddForce(dir, ForceMode.VelocityChange);
                break;

            case States.rolling:
                rb.AddForce(xdir, ForceMode.VelocityChange);
                break;

            default:
                tur.GetComponent<MeshRenderer>().material = material3;
                break;


        }


       

       
    }

}
