
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSouecr;


    [Header("---------- Audio Clip ----------")]
    public AudioClip button;
    public AudioClip day_night;
    public AudioClip death;
    public AudioClip background;
    public AudioClip jump;
    public AudioClip mainmenu;
    public AudioClip warning;
    public AudioClip doublejump;


    private void Start()
    {

        musicSource.clip = background;
        musicSource.Play();

    
    
    }

    public void PlaySFX(AudioClip clip)
    {
    
     SFXSouecr.PlayOneShot(clip);
    
    }


}
