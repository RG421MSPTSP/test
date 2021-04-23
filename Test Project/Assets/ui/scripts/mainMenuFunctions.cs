using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class mainMenuFunctions : MonoBehaviour
{
    //Inizia una nuova partita
    public void nuovaPartita() 
    {
        //carica la scena "Livello1"
        SceneManager.LoadScene("cacca");
        Debug.Log("Inizia il divertimento!");
    }

    //chiude gioco
    public void esci() 
    {
        Debug.Log("Hai chiuso il gioco!");
        Application.Quit();
    }
}
