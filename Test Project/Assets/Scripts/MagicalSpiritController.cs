using System.Collections;         //Viene importato lo spazio dei nomi per gestire le raccolte di oggetti
using System.Collections.Generic; //Viene importato lo spazio dei nomi per l'utilizzo dei tipi generici
using UnityEngine;                //Viene importato lo spazio dei nomi per gli oggetti principali di Unity
using UnityEngine.AI;             //Viene importato lo spazio dei nomi per l'intelligenza artificiale di Unity

///<summary>Questa classe si occupa del comportamento dello spiritello.</summary>
[RequireComponent(typeof(NavMeshAgent))]
public class MagicalSpiritController : MonoBehaviour
{
    /*Dichiarazione delle variabili pubbliche*/
    public PowerUp powerup = PowerUp.Disabled; ///<value>Power-up posseduto.</value>
    public float attackTime = 15.0f;           ///<value>Durata del power-up delle sfere di luce.</value>
    public float nextSphere = 5.0f;            ///<value>Tempo in secondi prima del lancio della prossima sfera di luce.</value>
    public float sphereSpeed = 500.0f;         ///<value>Velocità con cui lanciare le sfere di luce.</value>
    public GameObject forceFieldPrefab;        ///<value>Riferimento al prefab per il campo protettivo.</value>
    public GameObject magicSpherePrefab;       ///<value>Riferimento al prefab per le sfere di luce.</value>

    /*Dichiarazione delle costanti*/
    private const float distanceFromPlayer = 2.0f; ///<value>Costante per la distanza dal player.</value>
    private const float distanceFromEnemy = 4.0f;  ///<value>Costante per la distanza dai nemici.</value>

    /*Dichiarazione delle variabili d'istanza*/
    private NavMeshAgent navigationMeshAgent; ///<value>Componente Navigation Mesh Agent dell'oggetto.</value>
    private GameObject player;                ///<value>Riferimento al player.</value>
    private Transform playerTransform;        ///<value>Riferimento al componente Transform del player.</value>
    private Transform target;                 ///<value>Componente Transform dell'oggetto da seguire.</value>
    private bool deactivation = false;        ///<value>Variabile di controllo per la coroutine per terminare il power-up.</value>
    private bool sphereThrow = true;          ///<value>Variabile di controllo per il prossimo lancio della sfera di luce.</value>

    // Start is called before the first frame update
    void Start()
    {
        navigationMeshAgent = GetComponent<NavMeshAgent>();  //Inizializzazione Navigation Mesh Agent
        player = GameObject.FindGameObjectWithTag("Player"); //Aggiornamento del riferimento al player

        if (player != null) //Controllo validità riferimento
        {
            playerTransform = player.transform; //Si ottiene la componente Transform del player
        }
    }

    // Update is called once per frame
    void Update()
    {
        PowerUpActivation(); //Attivazione dei power-up
        TargetCorrection();  //Correzione dell'oggetto da seguire

        if (target != null) //Si controlla la validità del riferimento all'oggetto da seguire
        {
            navigationMeshAgent.SetDestination(target.position); //Viene seguito l'oggetto selezionato
        }
    }

    // FixedUpdate is called every physics timestep
    private void FixedUpdate()
    {
        if (target != null) //Si controlla la validità del riferimento all'oggetto da seguire
        {
            transform.LookAt(target); //Posizionamento verso l'oggetto da seguire
        }
    }

    // OnTriggerEnter is called when triggers occur
    private void OnTriggerEnter(Collider other)
    {
        /*if (powerup == PowerUp.MagicSpheres && other.tag.Equals())
        {
            navigationMeshAgent.stoppingDistance = distanceFromEnemy;
            target = other.transform;
        }*/
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        /*if (powerup == PowerUp.MagicSpheres && other.tag.Equals())
        {
            target = null;
        }*/
    }

    /**
     * <summary>Questo metodo si occupa di attivare il power-up selezionato.</summary>
     */
    private void PowerUpActivation()
    {
        switch (powerup) //Si seleziona l'azione da eseguire in base al power-up disponibile
        {
            case PowerUp.ForceField: //Secondo power-up
                if (!deactivation)   //Si controlla la validità dell'azione
                {
                    target = null;        //Si smette di seguire il player
                    ForceFieldCreation(); //Creazione del campo protettivo attorno al player
                    deactivation = true;  //Si aggiorna il valore della variabile di controllo
                }

                break; //Interruzione del costrutto di selezione

            case PowerUp.MagicSpheres: //Terzo power-up
                MagicSpheresAttack();  //Viene eseguito il power-up

                if (!deactivation) //Si controlla la validità dell'azione
                {
                    /*Si avvia la coroutine per la terminazione del power-up*/
                    StartCoroutine("StopMagicSpheresAttack");
                }

                break; //Interruzione del costrutto di selezione

            default:                        //Power-up sconosciuto
                powerup = PowerUp.Disabled; //Viene disabilitato qualunque power-up
                deactivation = false;       //Si ripristina il valore della variabile di controllo
                break;                      //Interruzione del costrutto di selezione
        }
    }

    /**
     * <summary>Questo metodo si occupa di selezionare l'oggetto da seguire.</summary>
     */
    private void TargetCorrection()
    {
        /*Si controlla che non ci sia alcun target da seguire*/
        if (target == null && playerTransform != null && powerup != PowerUp.ForceField)
        {
            /*Ripristino della distanza tra lo spirito e il player*/
            navigationMeshAgent.stoppingDistance = distanceFromPlayer;

            target = playerTransform; //Ripristino del target
        }
    }

    /**
     * <summary>Questo metodo si occupa di creare il campo protettivo attorno al player.</summary>
     */
    private void ForceFieldCreation() //Si controlla la correttezza del riferimento al prefab
    {
        GameObject instantiatedForceField; //Dichiarazione variabile per contenere il campo protettivo istanziato

        if (forceFieldPrefab != null) //Si controlla la correttezza del riferimento al prefab
        {
            /*Viene istanziato il prefab per il campo protettivo*/
            instantiatedForceField = Instantiate(forceFieldPrefab) as GameObject;
        }
        else //Il power-up non può essere attivato
        {
            powerup = PowerUp.Disabled; //Viene disabilitato qualunque power-up
            deactivation = false;       //Si ripristina il valore della variabile di controllo
        }
    }

    /**
     * <summary>Questo metodo si occupa di attaccare il nemico con sfere di luce.</summary>
     */
    private void MagicSpheresAttack()
    {
        GameObject instantiatedSphere; //Dichiarazione variabile per contenere la sfera istanziata
        Rigidbody sphereRigidbody;     //Dichiarazione variabile per contenere il componente Rigidbody della sfera istanziata

        /*Si controllano la correttezza del riferimento al prefab, la possibilità di lanciare una
          nuova sfera di luce e che l'obiettivo non sia il player*/
        if (magicSpherePrefab != null && sphereThrow && target != playerTransform)
        {
            /*Viene istanziato il prefab per la sfera di luce*/
            instantiatedSphere = Instantiate(

                magicSpherePrefab,
                transform.position + transform.forward,
                Quaternion.identity

            ) as GameObject;

            /*Si recupera il componente Rigidbody della sfera di luce istanziata*/
            sphereRigidbody = instantiatedSphere.GetComponent<Rigidbody>();

            /*La sfera di luce viene lanciata contro il bersaglio*/
            sphereRigidbody.AddForce(transform.forward * sphereSpeed);

            sphereThrow = false;               //Aggiornamento della variabile di controllo per il prossimo lancio
            StartCoroutine("ThrowNextSphere"); //Viene avviata la coroutine per il prossimo lancio
        }
        else if (magicSpherePrefab == null) //Il power-up non può essere attivato
        {
            StopCoroutine("StopMagicSpheresAttack"); //Si interrompe l'esecuzione della coroutine

            powerup = PowerUp.Disabled; //Viene disabilitato qualunque power-up
            deactivation = false;       //Si ripristina il valore della variabile di controllo
        }
    }

    /**
     * <summary>Questa coroutine si occupa di disabilitare il power-up una volta
     * terminato il tempo.</summary>
     */
    IEnumerator StopMagicSpheresAttack()
    {
        deactivation = true; //Si aggiorna il valore della variabile di controllo

        yield return new WaitForSeconds(attackTime); //Si attende che il tempo del power-up arrivi al termine

        powerup = PowerUp.Disabled; //Il power-up viene disabilitato
        deactivation = false;       //Si ripristina il valore della variabile di controllo
    }

    /**
     * <summary>Questa coroutine si occupa di abilitare la creazione di sfere di
     * luce a intervalli regolari.</summary>
     */
    IEnumerator ThrowNextSphere()
    {
        yield return new WaitForSeconds(nextSphere); //Si attende prima del prossimo lancio

        sphereThrow = true; //Aggiornamento della variabile di controllo per il prossimo lancio
    }
}