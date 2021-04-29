using System.Collections;         //Viene importato lo spazio dei nomi per gestire le raccolte di oggetti
using System.Collections.Generic; //Viene importato lo spazio dei nomi per l'utilizzo dei tipi generici
using UnityEngine;                //Viene importato lo spazio dei nomi per gli oggetti principali di Unity
using UnityEngine.AI;             //Viene importato lo spazio dei nomi per l'intelligenza artificiale di Unity

public class MagicalSpiritController : MonoBehaviour
{
    /*Dichiarazione delle variabili pubbliche*/
    public PowerUp powerup = PowerUp.Disabled; ///<value>Power-up posseduto.</value>
    public float sphereSpeed = 100.0f;         ///<value>Velocità con cui lanciare le sfere di luce.</value>
    public GameObject forceFieldPrefab;        ///<value>Riferimento al prefab per il campo protettivo.</value>
    public GameObject magicSpherePrefab;       ///<value>Riferimento al prefab per le sfere di luce.</value>

    /*Dichiarazione delle variabili d'istanza*/
    private NavMeshAgent navigationMeshAgent; ///<value>Componente Navigation Mesh Agent dell'oggetto.</value>
    private GameObject player;                ///<value>Riferimento al player.</value>
    private Transform target;                 ///<value>Componente Transform dell'oggetto da seguire.</value>

    // Start is called before the first frame update
    void Start()
    {
        navigationMeshAgent = GetComponent<NavMeshAgent>();  //Inizializzazione Navigation Mesh Agent
        player = GameObject.FindGameObjectWithTag("Player"); //Aggiornamento del riferimento al player
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            navigationMeshAgent.SetDestination(target.position);
        }
    }

    /**
     * <summary>Questo metodo si occupa di trovare il nemico da attaccare.</summary>
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            target = player.transform;
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
    }

    /**
     * <summary>Questo metodo si occupa di attaccare il nemico con sfere di luce.</summary>
     */
    private void MagicSpheresAttack()
    {
        GameObject instantiatedSphere; //Dichiarazione variabile per contenere la sfera istanziata
        Rigidbody sphereRigidbody;     //Dichiarazione variabile per contenere il componente Rigidbody della sfera istanziata

        if (magicSpherePrefab != null) //Si controlla la correttezza del riferimento al prefab
        {
            /*Viene istanziato il prefab per la sfera di luce*/
            instantiatedSphere = Instantiate(

                magicSpherePrefab,
                transform.position,
                Quaternion.identity

            ) as GameObject;

            /*Si recupera il componente Rigidbody della sfera di luce istanziata*/
            sphereRigidbody = instantiatedSphere.GetComponent<Rigidbody>();

            /*La sfera di luce viene lanciata contro il bersaglio*/
            sphereRigidbody.AddForce(Vector3.forward * sphereSpeed);
        }
    }
}
