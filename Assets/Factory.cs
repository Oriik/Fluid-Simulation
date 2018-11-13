using UnityEngine;

public class Factory : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    public int startNumber = 100;
    public float reloadTime = 2f;
    public float reloadProgress = 0f;

    public GameObject prefab;

    [Space]
    
    public float alpha = 1000;
    public float h =1;
    public float k = 1f;
    public float rho0 = 50f;
    public float kNear = 60f;
}