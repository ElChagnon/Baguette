using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public string entityName;
    public int maxHP = 100;
    public int currentHP;
    public int maxMP = 50;
    public int currentMP;
    public int attack = 10;
    public int defense = 5;

    // Références à la barre de vie et au texte dans l'UI
    public Slider healthSlider;   // Barre de vie
    public Text healthText;       // Texte affichant le nom et la vie de l'entité
    public Animator animator;     // L'Animator pour l'animation

    void Start()
    {
        currentHP = maxHP;
        currentMP = maxMP;

        // Initialiser la barre de vie avec la valeur maximale
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
        }

        // Afficher le texte avec le nom et les HP
        if (healthText != null)
        {
            healthText.text = entityName + " - " + currentHP + " HP";
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        // Mettre à jour la barre de vie
        if (healthSlider != null)
        {
            healthSlider.value = currentHP;
        }

        // Mettre à jour le texte affiché
        if (healthText != null)
        {
            healthText.text = entityName + " - " + currentHP + " HP";
        }

        Debug.Log(string.Format("{0} a pris {1} dégâts ! HP restant : {2}", entityName, damage, currentHP));
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }

    public void PlayTurnAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsTurn", true);  // Joue l'animation pendant le tour
        }
    }

    public void StopTurnAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsTurn", false); // Arrête l'animation après le tour
        }
    }
}
