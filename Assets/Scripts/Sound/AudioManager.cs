using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

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
    private bool _playerIsDead = false;

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
        GameEvents.OnLoadRequest += ResetAudioState;
    }

    private void OnDisable()
    {
        GameEvents.OnPlaySFX -= PlaySFX;
        GameEvents.OnPlayMusic -= PlayMusic;
        GameEvents.OnSpeedChanged -= HandleSpeedAudio;
        GameEvents.OnPlayerDeath -= HandleDeath;
        GameEvents.OnRestartRequest -= HandleRestart;
        GameEvents.OnPauseStateChanged -= HandlePause;
        GameEvents.OnLoadRequest -= ResetAudioState;
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
        float musicPitch = Mathf.Lerp(1.0f, 1.3f, speedPercent);
        mainMixer.SetFloat("MusicPitch", musicPitch);

        float curve = Mathf.Pow(speedPercent, 1.2f);
        _baseFootstepPitch = Mathf.Lerp(0.5f, 4f, curve);

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
        _playerIsDead = true;
        runLoopSource.Stop();
        musicSource.Stop();


        PlaySFX(SoundType.PlayerDeath);
        StartCoroutine(PlayDelayedRoutine(SoundType.PlayersBodyHit, 1.5f));

        mainMixer.SetFloat("MusicPitch", 1.0f);
    }

    private IEnumerator PlayDelayedRoutine(SoundType type, float delay)
    {
        
        yield return new WaitForSeconds(delay);
        PlaySFX(type);
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
        if (scene.buildIndex == 0)
        {
            _playerIsDead = false;
            PlayMusic(mainMenuMusic);
        }
    }

    public void ResetAudioState()
    {
        if (runLoopSource != null) runLoopSource.Stop();
        _playerIsDead = false;

        mainMixer.SetFloat("MusicPitch", 1.0f);
    }

}