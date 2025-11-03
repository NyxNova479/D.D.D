using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    //Il me faut un payer qui puissent se déplacer:
    // en avant avec Z
    // en arrière avec S
    // vers sa gauche avec Q
    // vers sa droite avec D
    // J'aimerais également qu'il puisse sauter uniquement si il se trouve au sol



    public Transform player_transform;


    private float player_speed = 10;
    
    


    public bool isGrounded ;
    private float verticalVelocity;
    public float gravity;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

        
        

        


    // Update is called once per frame
    void Update()
    {
        Move();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
            
        }
        if (!isGrounded)
        {
            gravity -= 10 * Time.deltaTime;
        }
        else
        {
            gravity = 0;
        }
        transform.position += new Vector3(0, gravity * Time.deltaTime, 0);
        
    }

    private void Move()
    {
        
        float vert_translation = Input.GetAxisRaw("Vertical") ;
        float hor_translation = Input.GetAxisRaw("Horizontal") ;


        transform.Translate(new Vector3 (hor_translation, 0, vert_translation).normalized * Time.deltaTime * player_speed) ;

        
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        bool canJump = isGrounded;
        if(collision.gameObject.tag == "Ground")
        {
            canJump = true;
            isGrounded = canJump;
        }

    }


    void Jump()
    {
        Vector3 newPlayerPosition = player_transform.position;
        gravity = 5.5f;


        isGrounded = false;
    }


    



}
