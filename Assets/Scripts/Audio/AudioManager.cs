using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX Clips (assign in Inspector — falls back to procedural)")]
    [SerializeField] private AudioClip collectClipAsset;
    [SerializeField] private AudioClip damageClipAsset;
    [SerializeField] private AudioClip victoryClipAsset;
    [SerializeField] private AudioClip gameOverClipAsset;

    [Header("Combat SFX (assign in Inspector — falls back to procedural)")]
    [SerializeField] private AudioClip shootClipAsset;
    [SerializeField] private AudioClip enemyShootClipAsset;
    [SerializeField] private AudioClip explosionClipAsset;
    [SerializeField] private AudioClip powerUpClipAsset;
    [SerializeField] private AudioClip shieldBreakClipAsset;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private AudioClip collectClip;
    private AudioClip damageClip;
    private AudioClip victoryClip;
    private AudioClip gameOverClip;

    private AudioClip shootClip;
    private AudioClip enemyShootClip;
    private AudioClip explosionClip;
    private AudioClip powerUpClip;
    private AudioClip shieldBreakClip;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // Use assigned clips if available, otherwise generate procedurally
        collectClip  = collectClipAsset  != null ? collectClipAsset  : GenerateTone(880f, 0.12f, envelope: true);
        damageClip   = damageClipAsset   != null ? damageClipAsset   : GenerateTone(120f, 0.35f, envelope: false);
        victoryClip  = victoryClipAsset  != null ? victoryClipAsset  : GenerateJingle(ascending: true);
        gameOverClip = gameOverClipAsset != null ? gameOverClipAsset : GenerateJingle(ascending: false);

        shootClip       = shootClipAsset       != null ? shootClipAsset       : GenerateTone(660f, 0.08f, envelope: true);
        enemyShootClip  = enemyShootClipAsset  != null ? enemyShootClipAsset  : GenerateTone(300f, 0.10f, envelope: true);
        explosionClip   = explosionClipAsset   != null ? explosionClipAsset   : GenerateTone(90f,  0.25f, envelope: false);
        powerUpClip     = powerUpClipAsset     != null ? powerUpClipAsset     : GenerateJingle(ascending: true);
        shieldBreakClip = shieldBreakClipAsset != null ? shieldBreakClipAsset : GenerateTone(200f, 0.20f, envelope: false);

        var ambientClip = GenerateAmbient();
        musicSource.clip = ambientClip;
        musicSource.loop = true;
        musicSource.volume = 0.45f;
        musicSource.playOnAwake = false;
        musicSource.Play();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnVictory += PlayVictory;
            GameManager.Instance.OnGameOver += PlayGameOver;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnVictory -= PlayVictory;
            GameManager.Instance.OnGameOver -= PlayGameOver;
        }
    }

    public void PlayCollect() => sfxSource.PlayOneShot(collectClip, 0.7f);
    public void PlayDamage() => sfxSource.PlayOneShot(damageClip, 0.8f);
    public void PlayVictory() => sfxSource.PlayOneShot(victoryClip, 1f);
    public void PlayGameOver() => sfxSource.PlayOneShot(gameOverClip, 1f);

    public void PlayShoot() => sfxSource.PlayOneShot(shootClip, 0.4f);
    public void PlayEnemyShoot() => sfxSource.PlayOneShot(enemyShootClip, 0.3f);
    public void PlayExplosion() => sfxSource.PlayOneShot(explosionClip, 0.6f);
    public void PlayPowerUp() => sfxSource.PlayOneShot(powerUpClip, 0.7f);
    public void PlayShieldBreak() => sfxSource.PlayOneShot(shieldBreakClip, 0.6f);

    private AudioClip GenerateTone(float freq, float duration, bool envelope)
    {
        const int sr = 44100;
        int n = (int)(sr * duration);
        float[] data = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / sr;
            float s = Mathf.Sin(2f * Mathf.PI * freq * t);
            if (envelope) s *= 1f - (float)i / n;
            else s *= Mathf.Min(1f, (float)i / (sr * 0.01f)) * (1f - (float)i / n);
            data[i] = s * 0.5f;
        }
        var clip = AudioClip.Create("tone_" + freq, n, 1, sr, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip GenerateJingle(bool ascending)
    {
        float[] freqs = ascending
            ? new[] { 440f, 554f, 659f, 880f }
            : new[] { 660f, 554f, 440f, 330f };

        const int sr = 44100;
        int noteLen = (int)(sr * 0.18f);
        int total = freqs.Length * noteLen;
        float[] data = new float[total];

        for (int f = 0; f < freqs.Length; f++)
        {
            for (int i = 0; i < noteLen; i++)
            {
                float t = (float)i / sr;
                float env = 1f - (float)i / noteLen;
                data[f * noteLen + i] = Mathf.Sin(2f * Mathf.PI * freqs[f] * t) * env * 0.55f;
            }
        }

        var clip = AudioClip.Create(ascending ? "victory" : "gameover", total, 1, sr, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip GenerateAmbient()
    {
        const int sr = 44100;

        // Synthwave-style minor arpeggio over a 4-chord progression (Am - F - C - G),
        // in an audible mid range so it's clearly heard on any speaker.
        float[] melody =
        {
            440.00f, 523.25f, 659.25f, 523.25f, // Am: A4 C5 E5 C5
            349.23f, 440.00f, 523.25f, 440.00f, // F:  F4 A4 C5 A4
            261.63f, 329.63f, 392.00f, 329.63f, // C:  C4 E4 G4 E4
            392.00f, 493.88f, 587.33f, 493.88f  // G:  G4 B4 D5 B4
        };
        float[] bass = { 110.00f, 87.31f, 65.41f, 98.00f }; // A2 F2 C2 G2 (one per chord)

        const float noteDur = 0.30f;
        int noteSamples = (int)(sr * noteDur);
        int total = melody.Length * noteSamples;
        float[] data = new float[total];

        for (int note = 0; note < melody.Length; note++)
        {
            float freq = melody[note];
            float bassFreq = bass[note / 4];
            for (int i = 0; i < noteSamples; i++)
            {
                float t = (float)i / sr;
                float env = Mathf.Sin(Mathf.PI * (float)i / noteSamples); // smooth attack/release
                float lead = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.22f * env;
                float bassWave = Mathf.Sin(2f * Mathf.PI * bassFreq * t) * 0.16f;
                data[note * noteSamples + i] = lead + bassWave;
            }
        }

        var clip = AudioClip.Create("ambient", total, 1, sr, false);
        clip.SetData(data, 0);
        return clip;
    }
}
