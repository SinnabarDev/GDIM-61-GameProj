using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{ 
    
    [SerializeField] private GameObject _rhythmUI;
    [SerializeField] private AudioSource _backgroundMusic;

    private bool _playMusic;

    void Start()
    {
        _playMusic = true;
    }


    void Update()
    {
        if (_rhythmUI.activeInHierarchy == false && _playMusic == true)
        {
            _backgroundMusic.Play();
            _playMusic = false;
        }

        else if (_rhythmUI.activeInHierarchy == true)
        {
            _backgroundMusic.Stop();
        }


    }

}