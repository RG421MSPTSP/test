using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatEsseri : MonoBehaviour
{
    public int maxVita = 100;
    public int vitaOra { get; private set; } //imposta il valore in modo che siano get pubblico , set privato.
    public Stat danno;
    public Stat armatura;//forse

    void Awake()
    {
        vitaOra = maxVita;
    }
    
    public void Ferito (int danno)
    {
        danno = armatura.GetValore() - danno;//forse 
        danno = Mathf.Clamp(danno, 0, int.MaxValue); //danno mai negativo quindi non recupera vita anche se armatura alta (toglibile)
        vitaOra = vitaOra - danno;
        Debug.Log(transform.name + "si fa" + danno + "danni"); //per leggere i danni volendo segnarlo nel debug log

        if(vitaOra <= 0)
        {
            Muori();
        }
    }
    public virtual void Muori()
    {
    //animazione di morte,loot per i nemici. game over, checkpoint per il giocatore
    Debug.Log(transform.name + " morto "); //per leggere che è morto volendo
    }

}
