using UnityEngine;

/// <summary>
/// Improvement #3 — Event-based audio.
/// Plays a random clip from a per-event list:
///   BallHitPaddle  -> paddle clips
///   BrickDestroyed -> brick clips (pitch climbs with the combo streak, capped)
///   BallLost       -> ball-lost clips
/// Never repeats the same clip twice in a row, adds a small random pitch/volume variation,
/// and uses a fixed pool of AudioSources (oldest voice is reused) so rapid hits cannot pile
/// up into a wall of sound or allocate anything at runtime.
/// </summary>
public class FeedbackAudio : MonoBehaviour
{
    [Header("Clips (random pick, no immediate repeat)")]
    [SerializeField] private AudioClip[] paddleClips;
    [SerializeField] private AudioClip[] brickClips;
    [SerializeField] private AudioClip[] ballLostClips;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float paddleVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float brickVolume = 0.7f;
    [SerializeField, Range(0f, 1f)] private float ballLostVolume = 0.9f;
    [Tooltip("Random +/- volume change per play.")]
    [SerializeField, Range(0f, 0.3f)] private float volumeJitter = 0.08f;

    [Header("Pitch")]
    [Tooltip("Random +/- pitch change per play (1 = normal).")]
    [SerializeField, Range(0f, 0.3f)] private float pitchJitter = 0.06f;
    [Tooltip("Brick pitch rise per combo step (bricks broken without touching the paddle).")]
    [SerializeField, Range(0f, 0.1f)] private float brickPitchPerCombo = 0.03f;
    [Tooltip("Highest pitch bonus the combo can add.")]
    [SerializeField, Range(0f, 0.6f)] private float maxComboPitchBonus = 0.24f;

    [Header("Timing")]
    [Tooltip("Delay before the brick sound, so the pop lines up with the end of the brick implode (0 = instant).")]
    [SerializeField, Range(0f, 0.3f)] private float brickSoundDelay = 0.12f;

    [Header("Voices")]
    [Tooltip("Most sounds that can play at once. When full, the oldest is replaced.")]
    [SerializeField, Range(2, 16)] private int maxVoices = 8;

    private AudioSource[] voices;
    private int nextVoice;
    private int lastPaddle = -1, lastBrick = -1, lastLost = -1;

    private void Awake()
    {
        voices = new AudioSource[maxVoices];
        for (int i = 0; i < maxVoices; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;   // plain 2D sound
            voices[i] = src;
        }
    }

    private void OnEnable()
    {
        GameEvents.BallHitPaddle += HandleBallHitPaddle;
        GameEvents.BrickDestroyed += HandleBrickDestroyed;
        GameEvents.BallLost += HandleBallLost;
    }

    private void OnDisable()
    {
        GameEvents.BallHitPaddle -= HandleBallHitPaddle;
        GameEvents.BrickDestroyed -= HandleBrickDestroyed;
        GameEvents.BallLost -= HandleBallLost;
    }

    private void HandleBallHitPaddle(ImpactInfo info)
    {
        Play(paddleClips, ref lastPaddle, paddleVolume, 1f, 0f);
    }

    private void HandleBrickDestroyed(ImpactInfo info)
    {
        float bonus = Mathf.Min(Mathf.Max(0, info.Combo - 1) * brickPitchPerCombo, maxComboPitchBonus);
        Play(brickClips, ref lastBrick, brickVolume, 1f + bonus, brickSoundDelay);
    }

    private void HandleBallLost()
    {
        Play(ballLostClips, ref lastLost, ballLostVolume, 1f, 0f);
    }

    private void Play(AudioClip[] clips, ref int last, float volume, float basePitch, float delay)
    {
        if (clips == null || clips.Length == 0) return;

        // Random pick that never repeats the previous clip (when there is a choice).
        int index = Random.Range(0, clips.Length);
        if (clips.Length > 1 && index == last) index = (index + 1 + Random.Range(0, clips.Length - 1)) % clips.Length;
        last = index;
        AudioClip clip = clips[index];
        if (clip == null) return;

        AudioSource src = voices[nextVoice];
        nextVoice = (nextVoice + 1) % voices.Length;   // oldest voice gets reused

        src.Stop();
        src.clip = clip;
        src.pitch = Mathf.Max(0.1f, basePitch + Random.Range(-pitchJitter, pitchJitter));
        src.volume = Mathf.Clamp01(masterVolume * volume * (1f + Random.Range(-volumeJitter, volumeJitter)));
        if (delay > 0f) src.PlayDelayed(delay); else src.Play();
    }
}
