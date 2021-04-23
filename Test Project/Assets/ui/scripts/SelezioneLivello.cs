using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelezioneLivello : MonoBehaviour
{
    //mettere le funzioni per caricare i vari livelli ricordarsi che va segnato in qualche modo che bisogna caricare dati di default, non necessario per lv1
    public void nuovaPartita()
    {
        //carica la scena "Livello1"
        SceneManager.LoadScene("cacca");
        Debug.Log("Inizia il divertimento!");
    }
}
