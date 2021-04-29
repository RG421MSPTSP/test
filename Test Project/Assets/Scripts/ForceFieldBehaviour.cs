using System.Collections;         //Viene importato lo spazio dei nomi per gestire le raccolte di oggetti
using System.Collections.Generic; //Viene importato lo spazio dei nomi per l'utilizzo dei tipi generici
using UnityEngine;                //Viene importato lo spazio dei nomi per gli oggetti principali di Unity

/**
 * <summary>Contiene i metodi per la gestione del secondo power-up.</summary>
 */
public class ForceFieldBehaviour : MonoBehaviour
{
    /*Dichiarazione delle variabili pubbliche*/
    public float diameter = 4.0f;        ///<value>Diametro del campo protettivo.</value>
    public float scaleFactor = 5.0f;     ///<value>Fattore di scala per l'espansione del campo.</value>
    public float protectionTime = 10.0f; ///<value>Tempo di vita del campo protettivo.</value>

    /*Dichiarazione delle variabili d'istanza*/
    private GameObject player;         ///<value>Collegamento al player.</value>
    private Transform playerTransform; ///<value>Collegamento alla posizione del player.</value>

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //Collegamento al player
        playerTransform = player.transform;                  //Collegamento al player

        transform.localScale = Vector3.zero; //Inizializzazione dimensione del campo protettivo

        /*Si comunica al player che il campo di forza è stato attivato*/
        player.GetComponent<PlayerController>().powerup = PowerUp.ForceField;

        StartCoroutine("Deactivate"); //Viene avviata la coroutine per la futura distruzione del campo
    }

    // Update is called once per frame
    void Update()
    {
        /*Calcolo del diametro per eventuale espansione del campo protettivo*/
        float tmpDiameter = transform.localScale.x + scaleFactor * Time.deltaTime;

        /*Dichiarazione e inizializzazione vettore per contenere le nuove dimensioni del campo*/
        Vector3 tmpScale = new Vector3(tmpDiameter, tmpDiameter, tmpDiameter);

        /*Dichiarazione e inizializzazione vettore per contenere le dimensioni massime*/
        Vector3 defaultScale = new Vector3(diameter, diameter, diameter);

        /*Dichiarazione e inizializzazione vettore per le dimensioni per l'aggiornamento*/
        Vector3 newScale = transform.localScale.x < diameter ? tmpScale : defaultScale;

        /*Si controlla che il campo sia svanito*/
        if (scaleFactor < 0.0f && transform.localScale.x <= 0.0f)
        {
            /*Si comunica al player che il campo di forza è stato disattivato*/
            player.GetComponent<PlayerController>().powerup = PowerUp.Disabled;

            Destroy(gameObject); //Distruzione del campo protettivo
        }

        transform.localScale = newScale; //Aggiornamento delle dimensioni del campo protettivo
    }

    // LateUpdate is called every frame
    private void LateUpdate()
    {
        /*Dichiarazione costante dell'altezza del player per corretto posizionamento del campo protettivo*/
        const float heightOffset = PlayerController.playerHeight / 2.0f;

        /*La posizione del campo protettivo viene aggiornata in base a quella del player*/
        transform.position = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y + heightOffset,
            playerTransform.position.z
        );
    }

    /**
     * <summary>Questa coroutine si occupa della distruzione del campo protettivo.</summary>
     */
    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(protectionTime); //Si attende la fine del periodo di vita del campo

        scaleFactor *= -1.0f; //Aggiornamento del fattore di scala

        /*Calcolo del diametro per eventuale espansione del campo protettivo*/
        float tmpDiameter = transform.localScale.x + scaleFactor * Time.deltaTime;

        /*Dichiarazione e inizializzazione vettore per contenere le nuove dimensioni del campo*/
        Vector3 tmpScale = new Vector3(tmpDiameter, tmpDiameter, tmpDiameter);

        transform.localScale = tmpScale; //Aggiornamento delle dimensioni del campo protettivo
    }
}