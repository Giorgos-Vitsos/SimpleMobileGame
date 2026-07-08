using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer & Sources")]
    public AudioMixer mainMixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;         // Ηχείο για Νομίσματα, UI (Κανονικός χρόνος)
    public AudioSource scaledSfxSource;   // Ηχείο για Dodge, Crashes (Επηρεάζεται από Time.timeScale)
    public AudioSource runLoopSource;     // Ηχείο για τα βήματα (Επηρεάζεται από Ταχύτητα & Time.timeScale)

    [Header("Audio Library (Sound Bank)")]
    public AudioClip mainMenuMusic;
    public List<SoundGroup> sfxLibrary = new List<SoundGroup>();

    // Αποθηκεύει το pitch των βημάτων με βάση την ταχύτητα, πριν μπει ο υπολογισμός του slow-motion
    private float _baseFootstepPitch = 1f;
    private bool _playerIsDead=false;

    private void Awake()
    {
        // Εξασφαλίζουμε ότι υπάρχει μόνο ένας AudioManager σε όλο το παιχνίδι
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
    }

    private void OnDisable()
    {
        GameEvents.OnPlaySFX -= PlaySFX;
        GameEvents.OnPlayMusic -= PlayMusic;
        GameEvents.OnSpeedChanged -= HandleSpeedAudio;
        GameEvents.OnPlayerDeath -= HandleDeath;
        GameEvents.OnRestartRequest -= HandleRestart;
    }

    private void Start()
    {
    
        PlayMusic(mainMenuMusic);
    }

    private void Update()
    {

        if (scaledSfxSource != null)
        {
            scaledSfxSource.pitch = Mathf.Clamp(Time.timeScale, 0.1f, 3f);
        }

        // 2. Εφαρμογή του Time Scale ΣΥΝ την ταχύτητα τρεξίματος για τα βήματα
        if (runLoopSource != null)
        {
            runLoopSource.pitch = Mathf.Clamp(_baseFootstepPitch * Time.timeScale, 0.1f, 3f);
        }
    }

    // --- ΒΑΣΙΚΕΣ ΜΕΘΟΔΟΙ ΑΝΑΠΑΡΑΓΩΓΗΣ ---

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
        else
        {
            Debug.LogWarning("Δεν βρέθηκε ήχος για το SoundType: " + requestedType);
        }
    }

    private void PlayMusic(AudioClip track)
    {
        // 1. Check if we are loading the main menu FIRST!
        if (track == mainMenuMusic)
        {
            if (runLoopSource != null) runLoopSource.Stop();
            
            // Safety reset just in case you quit to the menu while the death screen was active
            _playerIsDead = false; 
        }

        // 2. NOW check if the track is already playing to avoid restarting the song
        if (musicSource.clip == track) return;
        
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

    private void HandleRestart()=>_playerIsDead = false;

}