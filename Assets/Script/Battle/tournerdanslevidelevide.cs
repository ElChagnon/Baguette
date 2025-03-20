using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tournerdanslevidelevide : MonoBehaviour {
    public float tournerdanslevide = 1.0f; // Vitesse de rotation de la Skybox

    void Update()
    {
        // Modifie la rotation de la Skybox en fonction du temps
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * tournerdanslevide);
    }
}
