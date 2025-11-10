using UnityEngine;
//using UnityEditor.UIElements;

public class FirstPersonCharacterController : MonoBehaviour
{
    // Les blocs de codes contenant les algorythmes interressant sont marque avec ca:
    // §§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

    [Header("Objets generaux")]
    public GameObject enemyHandler;

    [Header("Combat")]
    public GameObject closestEnemy;
    //public Rigidbody rb;

    // mouvement X, Z
    [Header("mouvement X, Z")]
    public bool oldOrNew; // choisir le nouveau ou l'ancien systeme de mouvement obsolete
    public float zMovementInput;
    public float xMovementInput;
    public float playerMaxMovementSpeed;
    public Vector3 directionMovement;
    public Vector3 nextPositionMovement;

    // Saut et gravite
    [Header("Saut et gravite")]
    public float puissanceSaut;
    public float vitesseDeChuteMax;
    public float vitesseVerticaleActuelle;
    public float gravityAcceleration;
    bool jumped;
    public Vector3 jumpV3;
    public bool canJump;

    // detection du sol
    [Header("detection du sol")]
    public float sphereCastDistance;
    public float sphereCastRadius;
    public bool onGround;
    public float collisionY;
    bool justHitGround;

    // mouvement camera
    [Header("mouvement camera")]
    public GameObject playerFPSCamera;
    public float mouseSensitivity;
    float cameraVerticalRotation;
    bool lockedCursor = true;

    [Header("Barre d'item")]
    public int currentIndexItemScrollBar;

    // pointeur
    [Header("pointeur")]
    public Vector3 playerPointerDirection; // la direction dans laquelle la camera pointe  

    // tirer
    [Header("tirer")]
    public GameObject projectile;

    [Header("Tests et modes")]
    public GameObject directionArrow;
    public GameObject pointVisualizer;
    void Start()
    {
        Cursor.visible = false; // Rendre la sourirs invisible
        Cursor.lockState = CursorLockMode.Locked;  // caller la souris au centre de l'écran
    }
    void Update()
    {
        //directionArrow.transform.rotation = Quaternion.Euler(directionMovement);
        //PlayerJump();        
        PlayerCameraMovement();
        MouseWheelScroll();
        Gravity();            
        GroundDetection();
        PlayerJump();    
        PlayerXZMovementNew();
        //Toolbar();
    }

    void PlayerXZMovementNew() // mouvement normalise qui ne derive pas
    {
        directionMovement = (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized;
        nextPositionMovement = directionMovement; //+ jumpV3; // Input.GetAxis() donne un float adoucis entre 1 et -1
        //rb.position += directionMovement.normalized * playerMaxMovementSpeed * Time.fixedDeltaTime;

        RaycastHit hit;
        if (Physics.CapsuleCast(transform.position + new Vector3(0, 0.5f, 0), transform.position + new Vector3(0, -0.5f, 0), 0.5f, directionMovement, out hit, 0.1f)) // on verifie si il y a un obstacle dans la direction du mouvement
        {
            RaycastHit[] hits;
            hits = Physics.CapsuleCastAll(transform.position + new Vector3(0, 0.5f, 0), transform.position + new Vector3(0, -0.5f, 0), 0.45f, directionMovement, 0.1f);
            //Debug.Log(hits.Length);
            if (hits.Length > 1)
            {
                nextPositionMovement = new Vector3(0, 0, 0);
            }
                Vector3 hitDir = new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z);// + directionMovement;
                nextPositionMovement += -hitDir.normalized;
                // si oui on prends la position de la collision et on trouve la direction opposee par rapport au mouvement pour "glisser" contre les murs
                //Debug.Log("Hit: " + hit.collider.name + " at " + hit.point);
                Instantiate(pointVisualizer, hit.point, Quaternion.identity);
                directionArrow.transform.LookAt(new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z) + nextPositionMovement);
                // Debug.Break();
                //transform.position += new Vector3(0, hit.point.y, 0);
            //}
            //else
            //{
            //    nextPositionMovement = new Vector3(0, 0, 0);
            //}
            
        }
       transform.position += (nextPositionMovement * playerMaxMovementSpeed * Time.fixedDeltaTime);
    }
    void GroundDetection() // le sphereCast à un rayon plus petit que le rigidBody pour éviter de confondre les murs comme des sols;
    {       
        LayerMask coucheGround = LayerMask.GetMask("Ground"); // on crée une variable couche
        RaycastHit gHit; // on déclare une variable RaycastHit qui permet d'avoir des informations sur se qu'a touché un Raycast
        if (Physics.SphereCast(transform.position, sphereCastRadius, -transform.up, out gHit, sphereCastDistance, coucheGround)) // le SphereCast ne voit que la couche "Ground" et ignore le reste
        {
            float pTY = transform.position.y;
            onGround = true;
            collisionY = gHit.transform.position.y; 
                  
            if (justHitGround)
            {
                //Debug.Log("hit = " + gHit.point.y + " transform = " + gHit.transform.position.y);               
                vitesseVerticaleActuelle = 0;

                // pTY = transform y du joueur au moment du hit
                //Instantiate(pointVisualizer, gHit.transform.position, Quaternion.identity);
                //transform.position = new Vector3(transform.position.x, pTY + (collisionY), transform.position.z);     //gHit.point.y + (gHit.transform.position.y - gHit.point.y)
            }            
        }
        else
        {
            justHitGround = true;
            onGround = false;
        }
    }
    void PlayerJump() // on fait juste monter la vitesse verticale et la gravite fait le reste
    {
        if (Input.GetKey(KeyCode.Space) && onGround)
        {
            vitesseVerticaleActuelle = puissanceSaut;
            //Debug.Log("hitspace");            
        }
    }

    void Gravity() // on fait juste baisser la vitesse verticale
    {
        if (!onGround && vitesseVerticaleActuelle > -vitesseDeChuteMax)
        {           
            vitesseVerticaleActuelle -= gravityAcceleration * Time.deltaTime; // la gravité fait baisser la vitesse verticale à chaque frame                       
        }
        transform.position += new Vector3(0, vitesseVerticaleActuelle, 0) * Time.deltaTime;
    }

   

    void OnDrawGizmos() // se lance tout seul dans l'éditeur, sert uniquement à voir les trucs invisibles 
    {
        Gizmos.color = Color.green; // la couleur du gizmo
        Gizmos.DrawSphere(transform.position - transform.up * sphereCastDistance, sphereCastRadius); // on crée une representation visuelle du spherecast !!! la syntaxe est differente du vrai SphereCast !!!
        //Gizmos.DrawSphere((transform.position + directionMovement), sphereCastRadius);
    }

    void PlayerCameraMovement() // on crée une fonction
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity; // on récupère l'axe x de la souris et on la met dans la variable "inputX"
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity; // on récupère l'axe y de la souris et on la met dans la variable "inputY"
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f); // on force la rotation verticale à rester entre -90 et 90
        playerFPSCamera.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        transform.Rotate(Vector3.up * inputX);
        //rb.MoveRotation(rb.rotation * Vector3.up * inputX);
        playerPointerDirection = playerFPSCamera.transform.eulerAngles; // le pointeur = la rotation de la camera
    }

    void MouseWheelScroll()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
        {
            if (currentIndexItemScrollBar == 8)
            {
                currentIndexItemScrollBar = 0;
            }
            else
            {
                currentIndexItemScrollBar++;
            }
        }

        if (scrollInput < 0f)
        {
            if (currentIndexItemScrollBar == 0)
            {
                currentIndexItemScrollBar = 8;
            }
            else
            {
                currentIndexItemScrollBar--;
            }
        }
        //Debug.Log(currentIndexItemScrollBar);
    }

    void Toolbar()
    {
        // bricolage
        if (currentIndexItemScrollBar == 0)
        {
            ShootMode();
        }
        else
        {
            AutomaticShootMode();
        }
    }

    void ShootMode() // on instancie l'object projectile dans la rotation du pointeur
    {
        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            RaycastHit shotHit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out shotHit))
                {
                var instantiated = Instantiate(projectile, transform.position, transform.rotation);
                instantiated.transform.LookAt(shotHit.point);
                //transform.LookAt(target, Vector3.left);
                
                    /*if (shotHit.transform.gameObject.GetComponent<TargetScript>())
                    {
                        Destroy(shotHit.transform.gameObject);
                        targetManager.GetComponent<TargetManagerScript>().InstantiateNewTarget();
                        Debug.Log("joueur HIT");
                    }*/
                }
            else
            {
                Instantiate(projectile, transform.position, transform.rotation);
            }
        }
    }
    void AutomaticShootMode() // on instancie l'object projectile dans la rotation du pointeur
    {
        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            GameObject closestEnemy = FindClosestObject(transform.position, 400f, 8);
            if (closestEnemy != null)
            {
                var instantiated = Instantiate(projectile, transform.position, Quaternion.Euler(playerPointerDirection));
                instantiated.transform.LookAt(closestEnemy.transform.position);
                RaycastHit shotHit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out shotHit))
                {
                    /*if (shotHit.transform.gameObject.GetComponent<TargetScript>())
                    {
                        Destroy(shotHit.transform.gameObject);
                        targetManager.GetComponent<TargetManagerScript>().InstantiateNewTarget();
                        Debug.Log("joueur HIT");
                    }*/
                }
            }
            else
            {
                var instantiated = Instantiate(projectile, transform.position, transform.rotation);
            }
        }
    }

    GameObject FindClosestObject(Vector3 center, float radius, int layer)
    {
        float smallestDistance = radius; // = la plus grande variable
        GameObject closestObject = gameObject;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius, 1 << layer); // on cherche tout les gameobjects dans un rayon et on les met dans l'array "hitColliders"

        foreach (var hitCollider in hitColliders) // pour chaques gameobjects trouves
        {           
            float currentDiastance = (center - hitCollider.transform.position).sqrMagnitude; // on calcul la distance entre le centre et l'objet actuel
            // on cherche la distance la plus petite
            if (smallestDistance > currentDiastance) // si la distance actuelle est plus petite que la precedente
            {
                smallestDistance = currentDiastance; // la distance actuelle devient la distance la plus courte
                closestObject = hitCollider.transform.gameObject; // le resultat final = l'objet actuel
            }
        } // apres avoir compare toutes les distances de tout les gameobject trouves on se retrouve avec la distance la plus petite

        if (closestObject != gameObject) // si le resultat n'est pas le gameobject etabli par defaut
        {
            return (closestObject); // le resultat de la fonction = closestObject;
        }
        else
        {
            return (null); // le resultat de la fonction est rien
        }
    }
}