using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float visuale = 10f;
    public float distanzaAttaco;
    public LayerMask whatIsGround;//convenzione o nome cambiabile?

    public Vector3 zonaCaccia;
    public bool inCaccia;
    public float distanzaCaccia;
    Transform preda;
    NavMeshAgent agent; //ho lasciato agent per convenzione per capire che usa i metodi etc dell'agent NavMesh
    // Start is called before the first frame update
    void Start()
    {
        preda = PlayerManager.instance.giocatore.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanza = Vector3.Distance(preda.position, transform.position);
        if (distanza > visuale)
        {
            CacciaPreda();
        }
        if (distanza <= visuale) //settare diversamente per nemici a distanza e da corpo a corpo
        {
            agent.SetDestination(preda.position);
            if (distanza <= agent.stoppingDistance)
            {
                //attacca
                GuardaPreda();//ruotaverso il bersaglio
            }
            
        }
        
    }

    void GuardaPreda()
    {
        Vector3 direzione = (preda.position - transform.position).normalized;
        Quaternion rotazioneVisuale = Quaternion.LookRotation(new Vector3(direzione.x, 0, direzione.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotazioneVisuale, Time.deltaTime * 5f);//dovrebbe ruotare in modo più fluido rispetto a passare la semplice rotazioneVisuale
    }

    void OnDrawGizmosSelected() //mostra in rosso l'area di visuale del nemico
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visuale);
    }
    private void CacciaPreda()
    {
        if (!inCaccia)   //se non sta cercando trova un punto dove cercare
        {
            CercaZonaCaccia();
        }
        if(inCaccia)     //va al punto da cercare
        {
            agent.SetDestination(zonaCaccia);
        }

        Vector3 distanzaZonaCaccia = transform.position - zonaCaccia;

        if(distanzaZonaCaccia.magnitude <2f)  //ha raggiunto il punto e quindi è pronto per cercare altrove
        {
            inCaccia = false;
        }
    }
    private void CercaZonaCaccia()
    {
        float randomZ = Random.Range(-distanzaCaccia, distanzaCaccia);
        float randomX = Random.Range(-distanzaCaccia, distanzaCaccia);

        zonaCaccia = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(zonaCaccia, -transform.up, 2f, whatIsGround))
        {
            inCaccia = true;
        }
    }
}
