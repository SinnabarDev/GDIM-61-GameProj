using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{ 
    
    [SerializeField] private GameObject _rhythmUI;
    [SerializeField] private Animator _npcState;
    [SerializeField] private AudioSource _backgroundMusic;
    [SerializeField] private AudioClip _beforeMusic;
    [SerializeField] private AudioClip _afterMusic;

    private bool _playMusic;
    private int _checkHypno; // 0 = no, 1 = yes


    void Start()
    {
        _backgroundMusic.clip = _beforeMusic;
        _playMusic = true;
        _checkHypno = 0;
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
            _playMusic = true;
        }
        
        if (_npcState.GetBool("isHypno") == false && _checkHypno == 0)
        {
            _backgroundMusic.clip = _afterMusic;
            _backgroundMusic.Play();
            _checkHypno++;
        }


    }

}