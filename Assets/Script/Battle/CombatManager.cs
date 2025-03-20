using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public List<Entity> playerTeam;
    public List<Entity> enemyTeam;
    private List<Entity> turnOrder;

    // Références UI
    public Text combatLogText;           // Text pour afficher les messages de combat
    public GameObject actionPanel;       // Panel pour les actions (attaque, défense, soin)
    public Button attackButton;
    public Button defendButton;
    public Button healButton;

    // Paramètres pour l'effet de défilement
    public float typewriterDelay = 0.05f; // Délai entre chaque lettre

    private Entity currentEntity;
    private bool isTurn = false; // Vérifie si c'est le tour du personnage
    private bool isTyping = false; // Booléen pour vérifier si une coroutine est en cours

    // Sons
    public AudioClip attackSound;
    public AudioClip blockSound;
    public AudioClip healSound;

    void Start()
    {
        // Initialiser l'UI
        InitializeUI();
        StartCombat();
    }

    void InitializeUI()
    {
        // Cacher le panneau d'actions au début
        actionPanel.SetActive(false);

        // Ajouter des listeners aux boutons
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        defendButton.onClick.AddListener(OnDefendButtonClicked);
        healButton.onClick.AddListener(OnHealButtonClicked);
    }

    void StartCombat()
    {
        turnOrder = new List<Entity>();
        turnOrder.AddRange(playerTeam);
        turnOrder.AddRange(enemyTeam);
        StartCoroutine(CombatLoop());
    }

    IEnumerator CombatLoop()
    {
        while (!IsCombatOver())
        {
            foreach (var entity in turnOrder)
            {
                if (entity == null || entity.IsDead()) continue;

                currentEntity = entity;

                if (playerTeam.Contains(entity))
                {
                    // Afficher le panneau d'actions pour le joueur
                    actionPanel.SetActive(true);

                    // Lancer l'animation du joueur (tour du joueur)
                    if (entity.animator != null)
                    {
                        entity.animator.SetBool("isTurn", true);
                    }
                    yield return StartCoroutine(PlayerTurn(entity));
                    if (entity.animator != null)
                    {
                        entity.animator.SetBool("isTurn", false);
                    }
                }
                else
                {
                    // Cacher le panneau d'actions pour l'ennemi
                    actionPanel.SetActive(false);

                    // Lancer l'animation de l'ennemi (tour de l'ennemi)
                    if (entity.animator != null)
                    {
                        entity.animator.SetBool("isTurn", true);
                    }
                    yield return StartCoroutine(EnemyTurn(entity));
                    if (entity.animator != null)
                    {
                        entity.animator.SetBool("isTurn", false);
                    }
                }

                if (IsCombatOver()) break;
            }
        }

        EndCombat();
    }

    IEnumerator PlayerTurn(Entity player)
    {
        // Indiquer que c'est le tour du joueur
        isTurn = true;

        // Effacer le texte avant d'afficher un nouveau message
        ClearCombatLog();
        yield return StartCoroutine(TypewriterEffect(string.Format("C'est au tour de {0} !", player.entityName)));

        // Attendre que le joueur choisisse une action
        yield return new WaitUntil(() => currentEntity == null);

        yield return new WaitForSeconds(1); // Attendre 1 seconde avant de passer au tour suivant

        // Fin du tour du joueur
        isTurn = false;
    }

    IEnumerator EnemyTurn(Entity enemy)
    {
        // Indiquer que c'est le tour de l'ennemi
        isTurn = true;

        // Effacer le texte avant d'afficher un nouveau message
        ClearCombatLog();
        yield return StartCoroutine(TypewriterEffect(string.Format("C'est au tour de {0} !", enemy.entityName)));

        yield return new WaitForSeconds(1); // Attendre 1 seconde pour simuler un délai
        Entity target = playerTeam[0]; // Toujours attaquer le premier joueur pour l'instant
        int damage = enemy.attack - target.defense;
        target.TakeDamage(damage);

        // Effacer le texte avant d'afficher un nouveau message
        ClearCombatLog();
        yield return StartCoroutine(TypewriterEffect(string.Format("{0} attaque {1} pour {2} dégâts !", enemy.entityName, target.entityName, damage)));

        yield return new WaitForSeconds(1); // Attendre 1 seconde avant de passer au tour suivant

        // Fin du tour de l'ennemi
        isTurn = false;
    }

    bool IsCombatOver()
    {
        return playerTeam.TrueForAll(entity => entity.IsDead()) || enemyTeam.TrueForAll(entity => entity.IsDead());
    }

    void EndCombat()
    {
        if (playerTeam.TrueForAll(entity => entity.IsDead()))
        {
            StartCoroutine(TypewriterEffect("Vous avez perdu !"));
        }
        else
        {
            StartCoroutine(TypewriterEffect("Vous avez gagné !"));
        }
    }

    void OnAttackButtonClicked()
    {
        if (!isTurn) return; // Ne pas permettre d'attaque si ce n'est pas le tour du joueur

        Entity target = enemyTeam[0]; // Toujours attaquer le premier ennemi pour l'instant
        int damage = currentEntity.attack - target.defense;
        target.TakeDamage(damage);

        // Jouer le son d'attaque
        PlaySound(attackSound);

        // Mettre à jour le combat log
        ClearCombatLog();
        StartCoroutine(TypewriterEffect(string.Format("{0} attaque {1} pour {2} dégâts !", currentEntity.entityName, target.entityName, damage)));

        currentEntity = null; // Terminer le tour
        isTurn = false; // Fin du tour
    }

    void OnDefendButtonClicked()
    {
        if (!isTurn) return; // Ne pas permettre de défense si ce n'est pas le tour du joueur

        // Jouer le son de défense
        PlaySound(blockSound);

        // Implémenter la défense
        ClearCombatLog();
        StartCoroutine(TypewriterEffect(string.Format("{0} se défend !", currentEntity.entityName)));
        currentEntity = null; // Terminer le tour
        isTurn = false; // Fin du tour
    }

    void OnHealButtonClicked()
    {
        if (!isTurn) return; // Ne pas permettre de soin si ce n'est pas le tour du joueur

        // Jouer le son de soin
        PlaySound(healSound);

        // Implémenter le soin
        int healAmount = 20;
        currentEntity.currentHP = Mathf.Min(currentEntity.currentHP + healAmount, currentEntity.maxHP);

        // Mettre à jour le combat log
        ClearCombatLog();
        StartCoroutine(TypewriterEffect(string.Format("{0} se soigne de {1} HP !", currentEntity.entityName, healAmount)));

        currentEntity = null; // Terminer le tour
        isTurn = false; // Fin du tour
    }

    void PlaySound(AudioClip clip)
    {
        if (currentEntity != null && currentEntity.GetComponent<AudioSource>() != null)
        {
            currentEntity.GetComponent<AudioSource>().PlayOneShot(clip);
        }
    }

    void ClearCombatLog()
    {
        // Effacer le texte du journal de combat
        combatLogText.text = "";
    }

    IEnumerator TypewriterEffect(string message)
    {
        // Attendre que la coroutine précédente se termine
        while (isTyping)
        {
            yield return null;
        }

        // Définir isTyping à true pour bloquer les autres appels
        isTyping = true;

        // Afficher le message lettre par lettre
        for (int i = 0; i < message.Length; i++)
        {
            combatLogText.text += message[i];
            yield return new WaitForSeconds(typewriterDelay);
        }

        // Définir isTyping à false pour autoriser de nouveaux appels
        isTyping = false;
    }
}
