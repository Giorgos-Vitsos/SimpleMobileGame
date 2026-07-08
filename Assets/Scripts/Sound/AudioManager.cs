using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer & Sources")]
    public AudioMixer mainMixer;
    public AudioSource musicSource;
    public AudioSource sfxSource; 
    public AudioSource scaledSfxSource; 
    public AudioSource runLoopSource; 

    [Header("Audio Library (Sound Bank)")]
    public AudioClip mainMenuMusic;
    public List<SoundGroup> sfxLibrary = new List<SoundGroup>();

    private float _baseFootstepPitch = 1f;
    private bool _playerIsDead=false;
    private bool _gamePaused=false;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlaySFX += PlaySFX;
        GameEvents.OnPlayMusic += PlayMusic;
        GameEvents.OnSpeedChanged += HandleSpeedAudio;
        GameEvents.OnPlayerDeath += HandleDeath;
        GameEvents.OnRestartRequest += HandleRestart;
        GameEvents.OnPauseStateChanged += HandlePause; 
        SceneManager.sceneLoaded += HandleSceneChange;
    }

    private void OnDisable()
    {
        GameEvents.OnPlaySFX -= PlaySFX;
        GameEvents.OnPlayMusic -= PlayMusic;
        GameEvents.OnSpeedChanged -= HandleSpeedAudio;
        GameEvents.OnPlayerDeath -= HandleDeath;
        GameEvents.OnRestartRequest -= HandleRestart;
        GameEvents.OnPauseStateChanged -= HandlePause; 
        SceneManager.sceneLoaded -= HandleSceneChange;
    }

    private void Start()
    {
    
        PlayMusic(mainMenuMusic);
    }

    private void Update()
    {

        UpdatePitchSlowMo();
    }

    private void UpdatePitchSlowMo()
    {
        if (scaledSfxSource != null)
        {
            scaledSfxSource.pitch = Mathf.Clamp(Time.timeScale, 0.1f, 3f);
        }

        
        if (runLoopSource != null)
        {
            runLoopSource.pitch = Mathf.Clamp(_baseFootstepPitch * Time.timeScale, 0.1f, 3f);
        }
    }

    private void PlaySFX(SoundType requestedType)
    {
        SoundGroup? groupToPlay = null;
        
        foreach (SoundGroup group in sfxLibrary)
        {
            if (group.type == requestedType)
            {
                groupToPlay = group;
                break;
            }
        }

        if (groupToPlay != null && groupToPlay.Value.clips.Length > 0)
        {
            int randomIndex = Random.Range(0, groupToPlay.Value.clips.Length);
            AudioClip clip = groupToPlay.Value.clips[randomIndex];
            

            if (groupToPlay.Value.scalesWithTime)
            {
                scaledSfxSource.PlayOneShot(clip, groupToPlay.Value.volume); 
            }
            else
            {
                sfxSource.PlayOneShot(clip, groupToPlay.Value.volume);    
            }
        }
    }

    private void PlayMusic(AudioClip track)
    {
        if (musicSource.clip == track && musicSource.isPlaying) return;
        musicSource.clip = track;
        musicSource.Play();
    }

    private void HandleSpeedAudio(float currentSpeed, float maxSpeed)
{
    if (_playerIsDead) return;

    float speedPercent = currentSpeed / maxSpeed;

    // 1. MUSIC: Keeps the music locked to the game's progression
    float musicPitch = Mathf.Lerp(1.0f, 1.3f, speedPercent);
    mainMixer.SetFloat("MusicPitch", musicPitch);

    // 2. FOOTSTEPS: Using a power curve to handle slow speeds better
    // Raising speedPercent to 0.7f makes the pitch increase "slower" at the start,
    // which prevents the "chipmunk" effect at low speeds.
    float curve = Mathf.Pow(speedPercent, 1.2f);
    _baseFootstepPitch = Mathf.Lerp(0.5f, 4f, curve); 

    // Safety: ensure we don't play if speed is too low
    if (currentSpeed <= 0.05f)
    {
        if (runLoopSource.isPlaying) runLoopSource.Stop();
    }
    else if (!runLoopSource.isPlaying) 
    {
        runLoopSource.Play();
    }
}

    private void HandleDeath()
    {
        _playerIsDead=true;
        runLoopSource.Stop();
        musicSource.Stop();
        
        // Παίζουμε τον ήχο θανάτου
        PlaySFX(SoundType.PlayerDeath); 
        
        // Επαναφέρουμε το Mixer pitch στο κανονικό για το επόμενο Run
        mainMixer.SetFloat("MusicPitch", 1.0f);
    }

    private void HandleRestart()
    {
        _playerIsDead = false;
        PlayMusic(mainMenuMusic);
    }
    

    private void HandlePause(bool isPaused)
    {
        if (isPaused) runLoopSource.Pause();
        else runLoopSource.UnPause();
    }

    private void HandleSceneChange(Scene scene, LoadSceneMode mode)
    {
        if (runLoopSource != null) runLoopSource.Stop();
        if (scene.buildIndex == 0) // Assuming 0 is Main Menu
        {
            _playerIsDead = false;
            PlayMusic(mainMenuMusic);
        }
    }

}