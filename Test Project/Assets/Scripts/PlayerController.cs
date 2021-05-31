using System.Collections;         //Viene importato lo spazio dei nomi per gestire le raccolte di oggetti
using System.Collections.Generic; //Viene importato lo spazio dei nomi per l'utilizzo dei tipi generici
using UnityEngine;                //Viene importato lo spazio dei nomi per gli oggetti principali di Unity

/**
 * <summary>Contiene i metodi per il movimento e le azioni del player.</summary>
 */
[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    /*Dichiarazione delle proprietà pubbliche*/
    public float gravity = -9.81f;             ///<value>Valore della forza di gravità del mondo.</value>
    public float walkingSpeed = 0.25f;         ///<value>Velocità durante lo spostamento.</value>
    public float runningSpeed = 5.0f;          ///<value>Velocità durante la corsa.</value>
    public float rotationSpeed = 720.0f;       ///<value>Velocità di rotazione.</value>
    public int totalJump = 2;                  ///<value>Numero di salti consecutivi.</value>
    public float jumpStrength = 3.5f;          ///<value>Forza del salto.</value>
    public PowerUp powerup = PowerUp.Disabled; ///<value>Power-up posseduto.</value>
    public GameObject shockWavePrefab;         ///<value>Riferimento al prefab dell'onda d'urto per l'attacco speciale.</value>

    /*Dichiarazione costanti pubbliche*/
    public const float playerHeight = 1.05f; ///<value>Altezza del collider del Character Controller.</value>

    /*Dichiarazione costanti*/
    private const int firstJump = 1; ///<value>Definizione costante per l'indice di partenza del conteggio dei salti consecutivi.</value>

    /*Dichiarazione delle variabili d'istanza*/
    private CharacterController controller; ///<value>Dichiarazione componente CharacterController.</value>
    private Animator animator;              ///<value>Componente Animator per gestire le animazioni.</value>
    private float speed = -1.0f;            ///<value>Dichiarazione e inizializzazione velocità.</value>
    private float previousX = -1.0f;        ///<value>Dichiarazione e inizializzazione posizione a terra.</value>
    private float previousY = -1.0f;        ///<value>Dichiarazione e inizializzazione posizione in aria.</value>
    private float previousZ = -1.0f;        ///<value>Dichiarazione e inizializzazione posizione a terra.</value>
    private int currentJump = firstJump;    ///<value>Dichiarazione e inizializzazione numero di salti consecutivi.</value>

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>(); //Si recupera il componente Character Controller
        animator = GetComponent<Animator>();              //Si recupera il componente Animator
    }

    // Update is called once per frame
    void Update()
    {
        if (powerup == PowerUp.ForceField) //Si controlla che il player sia all'interno del campo di forza
        {
            Move(); //Viene chiamato il metodo per il movimento
        }
        else if (!Defend()) //Viene chiamato il metodo per la difesa
        {
            Move();   //Viene chiamato il metodo per il movimento
            Attack(); //Viene chiamato il metodo per selezionare l'attacco
        }
    }

    // LateUpdate is called every frame
    private void LateUpdate()
    {
        AdjustCollider(); //Correzione della posizione del collider
    }

    /**
     * <summary>Movimento del player.</summary>
     */
    private void Move()
    {
        const float animationStart = 0.0f,      //Definizione costante per l'istante iniziale dell'animazione
                    animationEnd = 0.5f,        //Definizione costante per la durata totale dell'animazione
                    animationIncrement = 0.1f,  //Definizione costante per l'incremento della velocità dell'animazione
                    otherJumpMinHeight = 60.0f, //Definizione costante per l'altezza minima in percentuale per i successivi salti
                    totalJumpHeight = 100.0f,   //Definizione costante per l'altezza totale in percentuale del salto
                    jumpConstant = -2.0f;       //Definizione costante per la formula del salto

        bool isGrounded = controller.isGrounded, //Dichiarazione e inizializzazione variabile di controllo per la posizione del giocatore
             jump = Input.GetButtonDown("Jump"); //Dichiarazione e inizializzazione variabile di controllo per il tasto di salto premuto 

        float x = Input.GetAxis("Horizontal"),          //Dichiarazione e inizializzazione posizione sull'asse x
              z = Input.GetAxis("Vertical"),            //Dichiarazione e inizializzazione posizione sull'asse z
              velocity = animator.GetFloat("velocity"); //Dichiarazione e inizializzazione velocità dell'animazione

        /*Viene calcolata l'altezza minima per poter effettuare il salto corrente*/
        float otherJumpCurrentMinHeight = jumpStrength * currentJump * otherJumpMinHeight / totalJumpHeight;

        Vector3 movement = new Vector3(x, 0.0f, z); //Modifica della posizione

        if (isGrounded && previousY < 0.0f) //Si controlla che il personaggio sia fermo
        {
            currentJump = firstJump;  //Reinizializzazione del conteggio dei salti effettuati
            previousY = jumpConstant; //Inizializzazione della posizione sull'asse y
        }

        if (x != 0.0f || z != 0.0f) //Si controlla che il personaggio sia in movimento
        {
            velocity += animationIncrement * Time.deltaTime; //Incremento della velocità dell'animazione
            velocity = Mathf.Abs(velocity);                  //Si considera la velocità in valore assoluto
        }
        else //Il personaggio sta rallentando sino a fermarsi
        {
            velocity -= (animationEnd - animationIncrement) * Time.deltaTime; //Decremento della velocità dell'animazione
        }

        velocity = Mathf.Clamp(velocity, animationStart, animationEnd); //Si limitano i valori massimi e minimi della velocità

        if (velocity > animationIncrement) //Si controlla che il personaggio stia correndo
        {
            /*Aggiornamento della velocità di spostamento*/
            speed = speed < runningSpeed ? speed + (animationIncrement / 2.0f) : runningSpeed;
        }
        else //Il personaggio sta rallentando o è fermo
        {
            /*Aggiornamento della velocità di spostamento*/
            speed = speed > walkingSpeed ? speed/2.0f : walkingSpeed;
            speed = speed <= walkingSpeed ? walkingSpeed : speed;
        }

        if (velocity != 0.0f && movement == Vector3.zero) //Si controlla che l'animazione sia ancora attiva
        {
            if (previousX != 0.0f) //Si controlla che la posizione non sia nulla
            {
                previousX = previousX > 0.0f ? walkingSpeed : -walkingSpeed; //Aggiornamento della posizione precedente
            }

            if (previousZ != 0.0f) //Si controlla che la posizione non sia nulla
            {
                previousZ = previousZ > 0.0f ? walkingSpeed : -walkingSpeed; //Aggiornamento della posizione precedente
            }

            movement = new Vector3(previousX, 0.0f, previousZ); //Modifica della posizione
        }

        animator.SetFloat("velocity", velocity);            //Animazione della camminata
        controller.Move(movement * speed * Time.deltaTime); //Movimento del personaggio

        if (movement != Vector3.zero) //Si controlla che il personaggio sia in movimento
        {
            previousX = x; //Salvataggio della posizione corrente del personaggio per futuro spostamento
            previousZ = z; //Salvataggio della posizione corrente del personaggio per futuro spostamento

            /*Rotazione del personaggio*/
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(movement, Vector3.up),
                rotationSpeed * Time.deltaTime
            );
        }

        /*Si controlla che il personaggio sia sul terreno e che l'utente abbia premuto il tasto per saltare*/
        if (currentJump <= totalJump && jump)
        {
            /*Si controlla che il personaggio possa effettuare ulteriori salti*/
            if (currentJump == firstJump || previousY >= otherJumpCurrentMinHeight)
            {
                /*Calcolo della posizione in ascesa*/
                previousY = Mathf.Sqrt(
                    jumpStrength * currentJump * jumpConstant * gravity
                );

                ++currentJump; //Incremento del numero di salti consecutivi
            }
        }

        previousY += gravity * Time.deltaTime;                                //Calcolo della posizione di caduta
        animator.SetFloat("height", Mathf.Clamp01(previousY));                //Animazione del salto
        controller.Move(new Vector3(0.0f, previousY, 0.0f) * Time.deltaTime); //Movimento del personaggio
    }

    /**
     * <summary>Questo metodo gestisce la difesa e l'animazione a essa associata.</summary>
     * <returns>Restituisce true se il player si sta difendendo, false altrimenti.</returns>
     */
    private bool Defend()
    {
        /*Dichiarazione e inizializzazione variabile per l'input dell'utente*/
        bool userInput = Input.GetButton("Fire2");

        /*Si controlla che il personaggio sia fermo*/
        bool notMoving = animator.GetFloat("velocity") == 0.0f && animator.GetFloat("height") == 0.0f;

        animator.SetBool("defend", userInput && notMoving); //Animazione del personaggio

        return userInput && notMoving; //Viene restituito il valore che indica che il personaggio si sta proteggendo
    }

    /**
     * <summary>Questo metodo si occupa di selezionare l'attacco da effettuare.</summary>
     */
    private void Attack()
    {
        bool userInput = Input.GetButtonDown("Fire1"); //Dichiarazione e inizializzazione variabile per l'input dell'utente

        if (userInput) //Si controlla che l'utente abbia premuto il tasto per attaccare
        {
            switch(powerup) //Si seleziona il tipo di azione da eseguire
            {
                case PowerUp.ShockWave:         //Si seleziona di attaccare col power-up
                    powerup = PowerUp.Disabled; //Il power-up viene utilizzato
                    SpecialAttack();            //Viene chiamato il metodo per il primo power-up
                    break;                      //Interruzione del costrutto di selezione
                default:                        //Si seleziona l'attacco semplice
                    powerup = PowerUp.Disabled; //Qualsiasi altro power-up viene scartato
                    SimpleAttack();             //Viene chiamato il metodo per l'attacco
                    break;                      //Interruzione del costrutto di selezione
            }
        }
    }

    /**
     * <summary>Questo metodo gestisce l'attacco e le animazioni a esso associate.</summary>
     */
    private void SimpleAttack()
    {
        animator.SetTrigger("attack"); //Animazione del personaggio
    }

    /**
     * <summary>Questo metodo gestisce l'attacco speciale e le animazioni a esso associate.</summary>
     */
    private void SpecialAttack()
    {
        animator.SetTrigger("specialAttack"); //Animazione del personaggio
        StartCoroutine("shockWaveEffect");    //Creazione dell'onda d'urto
    }

    /**
     * <summary>Questo metodo si occupa di correggere la posizione del collider del Character Controller.</summary>
     */
    private void AdjustCollider()
    {
        const int animationsLayer = 0;  //Definizione costante per il livello in cui si trovano le animazioni
        const int firstInformation = 0; //Definizione costante per ottenere il nome dell'animazione

        /*Dichiarazione e inizializzazione variabile per contenere le informazioni sull'animazione corrente*/
        AnimatorClipInfo[] currentAnimation = animator.GetCurrentAnimatorClipInfo(animationsLayer);

        if (currentAnimation.Length > firstInformation) //Controllo validità informazioni
        {
            /*Selezione dei valori del collider in base all'animazione*/
            switch (currentAnimation[firstInformation].clip.name)
            {
                /*Si selezionano i valori del collider per l'animazione dell'attacco semplice*/
                case "Attack01":
                    controller.radius = 0.68f; //Aggiornamento raggio collider

                    /*Aggiornamento posizione collider*/
                    controller.center = new Vector3(
                        -0.15f,
                        controller.radius + controller.skinWidth,
                        0.12f
                    );

                    break; //Interruzione del costrutto di selezione

                /*Si selezionano i valori del collider per l'animazione dell'attacco speciale*/
                case "Attack02":
                    controller.radius = 0.74f; //Aggiornamento raggio collider

                    /*Aggiornamento posizione collider*/
                    controller.center = new Vector3(
                        -0.07f,
                        controller.radius + controller.skinWidth,
                        0.08f
                    );

                    break; //Interruzione del costrutto di selezione

                /*Si selezionano i valori del collider per tutte le altre animazioni*/
                default:
                    controller.radius = 0.65f; //Aggiornamento raggio collider

                    /*Aggiornamento posizione collider*/
                    controller.center = new Vector3(
                        0.0f,
                        controller.radius + controller.skinWidth,
                        0.0f
                    );

                    break; //Interruzione del costrutto di selezione
            }
        }
    }

    /**
     * <summary>Questa coroutine mostra l'onda d'urto che segue l'attacco speciale.</summary>
     */
    IEnumerator shockWaveEffect()
    {
        const float waitingTime = 0.3f,  //Definizione costante per il tempo di attesa prima di mostrare l'onda d'urto
                    wavePosition = 0.5f; //Definizione costante per la posizione in aria dell'onda d'urto

        yield return new WaitForSeconds(waitingTime); //L'onda d'urto non viene mostrata subito

        if (shockWavePrefab != null) //Si controlla il collegamento al prefab
        {
            /*Viene istanziato il prefab dell'onda d'urto*/
            GameObject specialEffect = Instantiate(

                shockWavePrefab,
                new Vector3(transform.position.x, transform.position.y + wavePosition, transform.position.z),
                Quaternion.identity

            ) as GameObject;
        }
    }
}