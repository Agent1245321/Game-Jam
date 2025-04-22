using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    /*
     * Camera Fluidity
     * 
     * Saving Location
     * 
     * Menu
     * 
     * Hats
     * 
     * Other Launchers?
     * 
     * Rig that model
     * 
     * Achievments
     * 
     * stats
     * 
     * The Level
     * 
     * 
     * 
     * 
     * Importsmn:
     * 
     * Parry Roof
     * Reset Cooldown on Wall Parry
     * 
     * 
     * 
     */
    public Animator animator;

    public States state;
    public Surface surface;

    public LayerMask theGround;

    private Vector3 dir;
    private Vector3 xdir;
    private Vector2 moveInput;
    public float crouchInput;

    [Header("References")]
    public GameObject tur;
    public Rigidbody rb;
    public ParticleSystem effect;
    public Material material0;
    public Material material1;
    public Material material2;
    public Material material3;
    public Material material4;
    public Material material5;

    public PhysicsMaterial friction;
    public PhysicsMaterial frictionless;



    private float p;
    private float pc;
    public float g;
    public float gc;

    [Header("Tweaks")]
    public float parryWindow;
    public float parryCooldown;
    public float gripWindow;
    public float gripCooldown;


    public float timefreezeamount;
    public float timeslow;
    public float tf;
    public float moveSpeed;

    public RaycastHit hit;
    public bool grounded;
    public bool stuck;

    public PlayerInput input;








    public enum States
    {
        crawling,
        parry,
        grip,
        rolling,
        falling

    }

    public enum Surface
    { 
        down,
        right,
        up,
        left
    }



    void Update()
    {

        animator.SetFloat("Crouch", crouchInput);
        animator.SetFloat("WhoreInput", Mathf.Abs(moveInput.x));
        animator.SetBool("Grounded", grounded);

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            grounded = true;
        }
        else if(stuck == true)
        {
            grounded = true;
        }
        else
        {
            
            grounded = false;
        }



        if (p <= 0f && pc <= 0f && crouchInput > 0 && state != States.rolling)
        {
           
            p = parryWindow;
            pc = parryCooldown;
        }
        else if (g <= 0f && gc <= 0f && crouchInput <= 0 && state == States.rolling && !grounded)
        {
            g = gripWindow;
            gc = gripCooldown;
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

        if (g > 0f)
        {
            state = States.grip;
            g -= Time.deltaTime;
        }

        else if (p > 0f)
        {
           
            state = States.parry;
            p -= Time.deltaTime;
            UnStick();
        }


        else if (crouchInput > 0)
        {
            state = States.rolling;
            UnStick();
        }
        else if (grounded)
        {
            state = States.crawling;
        }
        else
        {
            state = States.falling;
        }





        switch (state)
        {

            case States.crawling:
                tur.GetComponent<MeshRenderer>().material = material0;
                input.actions.FindAction("Move").Enable();
                this.GetComponent<Collider>().material = friction;

                break;

            case States.parry:

                tur.GetComponent<MeshRenderer>().material = material1;
                this.GetComponent<Collider>().material = frictionless;
                input.actions.FindAction("Move").Disable();
                break;

            case States.rolling:
                tur.GetComponent<MeshRenderer>().material = material2;
                input.actions.FindAction("Move").Disable();
                this.GetComponent<Collider>().material = frictionless;
                break;

            case States.falling:
                tur.GetComponent<MeshRenderer>().material = material5;
                input.actions.FindAction("Move").Disable();
                this.GetComponent<Collider>().material = frictionless;
                break;

            case States.grip:
                tur.GetComponent<MeshRenderer>().material = material4;
                input.actions.FindAction("Move").Disable();
                this.GetComponent<Collider>().material = friction;
                break;

            default:
                tur.GetComponent<MeshRenderer>().material = material3;
                this.GetComponent<Collider>().material = friction;
                break;


        }

        if (pc > 0f)
        {
            pc -= Time.deltaTime;
        }

        if (gc > 0f)
        {
            gc -= Time.deltaTime;
        }



    }

    public void Stick()
    {
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
        stuck = true;

        switch (surface)
        {
            case Surface.down:
                
                    tur.transform.eulerAngles = new Vector3(0, 0, 0);

                break;

            case Surface.up:
               
                    tur.transform.eulerAngles = new Vector3(0, 180, 180);
              

                break;

            case Surface.left:
                
                    tur.transform.eulerAngles = new Vector3(180, 0, 270);
                
                break;

            case Surface.right:
               
                    tur.transform.eulerAngles = new Vector3(0, 0, 90);
                
                break;

        }


    }

    public void UnStick()
    {

        rb.useGravity = true;
        stuck = false;

        tur.transform.eulerAngles = new Vector3(0, 0, 0);
        surface = Surface.down;
    }

    public void OnCollisionEnter(Collision collision)
    {
     
        switch (state)
        {
            case States.parry:
                if (collision.gameObject.tag == "ground")
                {
                    
                    rb.AddForce(new Vector3(0, collision.relativeVelocity.y, 0), ForceMode.VelocityChange);
                }
                else if (collision.gameObject.tag == "wall")
                {
                   
                    rb.AddForce(new Vector3(collision.relativeVelocity.x, 0, 0), ForceMode.VelocityChange);
                }
                break;

            case States.grip:
                Debug.Log("Gripppppp");
                switch (collision.gameObject.tag)
                {

                    case "floor":
                        surface = Surface.down;
                        break;

                    case "ceiling":
                        surface = Surface.up;
                        break;

                    case "wall":
                        if (Physics.Raycast(transform.position, Vector3.right, out hit, 1f))
                        {
                            Debug.Log("Hit Right Wall");
                            surface = Surface.right;
                        }
                        else
                        {
                            Debug.Log("Hit Left Wall");
                            surface = Surface.left;
                        }
                        break;

                    default:

                        break;
                
                }

                Stick();


                break;

            case States.rolling:
                
                break;

            case States.falling:

                break;

            default:

                break;
           
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
      

        switch (surface)
        {
            case Surface.down:
                tur.transform.position += new Vector3(moveInput.x * moveSpeed, 0, 0);

                if (moveInput.x > 0)
                {
                    tur.transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else if (moveInput.x < 0)
                {
                    tur.transform.eulerAngles = new Vector3(0, 180, 0);
                }

                break;

                case Surface.up:
                tur.transform.position += new Vector3(moveInput.x * moveSpeed, 0, 0);

                if (moveInput.x > 0)
                {
                    tur.transform.eulerAngles = new Vector3(0, 180, 180);
                }
                else if (moveInput.x < 0)
                {
                    tur.transform.eulerAngles = new Vector3(0, 0, 180);
                }

                break;

            case Surface.left:
                tur.transform.position += new Vector3(0, moveInput.y * moveSpeed, 0);

                if (moveInput.y > 0)
                {
                    tur.transform.eulerAngles = new Vector3(180, 0, 270);
                }
                else if (moveInput.y < 0)
                {
                    tur.transform.eulerAngles = new Vector3(0, 0, 270);
                }
                break;
            
            case Surface.right:
                tur.transform.position += new Vector3(0, moveInput.y * moveSpeed, 0);

                if (moveInput.y > 0)
                {
                    tur.transform.eulerAngles = new Vector3(0, 0, 90);
                }
                else if (moveInput.y < 0)
                {
                    tur.transform.eulerAngles = new Vector3(180, 0, 90);
                }
                break;

        }


       
    }

    public void Launch(float xmin, float xmax, float ymin, float ymax)
    {
        UnStick();
       
        dir = new Vector3(Random.Range(xmin, xmax), Random.Range(ymin, ymax), 0);
        xdir = new Vector3(Random.Range(xmin, xmax), 0, 0);

        switch (state)
        {

            case States.crawling:
                rb.AddForce(dir * .1f, ForceMode.VelocityChange);

                break;

            case States.parry:
                Parry();
                tf = timefreezeamount;
                rb.AddForce(dir, ForceMode.VelocityChange);
                pc = 0;
                break;

            case States.rolling:
                rb.AddForce(xdir, ForceMode.VelocityChange);
                break;

            default:
                tur.GetComponent<MeshRenderer>().material = material3;
                break;


        }

       





    }

     public void Parry()
    {
        
        effect.Play();
    }

}
