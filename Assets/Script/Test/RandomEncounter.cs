using UnityEngine;

public class RandomEncounter : MonoBehaviour
{
    public float encounterRate = 0.5f; // 50% de chance de déclencher un combat

    void Update()
    {
        // Vérifier si la touche "C" est pressée
        if (Input.GetKeyDown(KeyCode.C))
        {
            CheckForEncounter();
        }
    }

    void CheckForEncounter()
    {
        // Générer un nombre aléatoire pour déterminer si un combat est déclenché
        if (Random.value < encounterRate)
        {
            TriggerCombat();
        }
        else
        {
            Debug.Log("Pas de combat cette fois-ci !");
        }
    }

    void TriggerCombat()
    {
        // Charger la scène de combat
        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
    }
}