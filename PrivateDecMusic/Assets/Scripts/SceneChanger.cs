using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public float changetime;
    public string scenename;
   
    void Update()
    {
        changetime -= Time.deltaTime;
        if (changetime <= 0 )
        { SceneManager.LoadScene(scenename); }
    }
}
