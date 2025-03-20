using UnityEngine;
using System.Collections.Generic;

public class BruitPas : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip defaultFootstep;
    public LayerMask groundLayer;
    public float stepInterval = 0.5f; // Temps entre deux bruits de pas
    private float stepTimer;
    private PhysicMaterial lastMaterial; // Stocke le dernier matériau détecté

    // Dictionnaire des sons en fonction du matériau
    public List<MaterialFootstep> materialFootsteps = new List<MaterialFootstep>();

    private void Update()
    {
        if (IsMoving())
        {
            PhysicMaterial currentMaterial = GetCurrentMaterial();

            if (currentMaterial != lastMaterial)
            {
                // Change immédiatement le son si le matériau est différent
                lastMaterial = currentMaterial;
                PlayFootstepSound(currentMaterial);
            }

            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayFootstepSound(currentMaterial);
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0; // Reset si le joueur ne bouge pas
        }
    }

    private bool IsMoving()
    {
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }

    private void PlayFootstepSound(PhysicMaterial material)
    {
        AudioClip clip = GetFootstepSound(material);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private AudioClip GetFootstepSound(PhysicMaterial material)
    {
        if (material != null)
        {
            foreach (var matStep in materialFootsteps)
            {
                if (matStep.material == material)
                    return matStep.footstepSound;
            }
        }
        return defaultFootstep;
    }

    private PhysicMaterial GetCurrentMaterial()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, groundLayer))
        {
            return hit.collider.sharedMaterial;
        }
        return null;
    }
}

[System.Serializable]
public class MaterialFootstep
{
    public PhysicMaterial material;
    public AudioClip footstepSound;
}
