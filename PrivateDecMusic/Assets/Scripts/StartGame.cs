using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string sceneName;

    public void LoadByName()
    {
        SceneManager.LoadScene(sceneName);
    }
}
