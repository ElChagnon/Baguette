using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCombat : MonoBehaviour
{
    public void ReturnToWorld()
    {
        SceneManager.LoadScene("World");
    }
}