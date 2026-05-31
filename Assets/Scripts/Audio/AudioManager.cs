using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private AudioClip collectClip;
    private AudioClip damageClip;
    private AudioClip victoryClip;
    private AudioClip gameOverClip;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        collectClip = GenerateTone(880f, 0.12f, envelope: true);
        damageClip = GenerateTone(120f, 0.35f, envelope: false);
        victoryClip = GenerateJingle(ascending: true);
        gameOverClip = GenerateJingle(ascending: false);

        var ambientClip = GenerateAmbient();
        musicSource.clip = ambientClip;
        musicSource.loop = true;
        musicSource.volume = 0.35f;
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
        const int duration = 10;
        int n = sr * duration;
        float[] data = new float[n];
        float fadeLen = sr * 0.25f;

        for (int i = 0; i < n; i++)
        {
            float t = (float)i / sr;
            float s = 0.50f * Mathf.Sin(2f * Mathf.PI * 55f * t)
                    + 0.25f * Mathf.Sin(2f * Mathf.PI * 110f * t)
                    + 0.12f * Mathf.Sin(2f * Mathf.PI * 82.5f * t)
                    + 0.06f * Mathf.Sin(2f * Mathf.PI * 165f * t);
            s *= 0.65f + 0.35f * Mathf.Sin(2f * Mathf.PI * 0.25f * t);

            float fade = 1f;
            if (i < fadeLen) fade = i / fadeLen;
            if (i > n - fadeLen) fade = (n - i) / fadeLen;

            data[i] = s * fade * 0.28f;
        }

        var clip = AudioClip.Create("ambient", n, 1, sr, false);
        clip.SetData(data, 0);
        return clip;
    }
}
