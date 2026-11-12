using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Victory celebration (feedback only).
/// When the score reaches the full-board total (all bricks cleared), a short "VICTORY" banner
/// video plays over the game, with the green screen removed by a shader, so the player can
/// tell they won. It does NOT pause, end or change the game in any way: the ball keeps bouncing
/// exactly like in the starter. It can play again after a game-over reset brings the score back down.
///
/// Added after a playtester cleared the board and could not tell that they had won.
/// </summary>
public class VictoryCelebration : MonoBehaviour
{
    [Header("Trigger")]
    [Tooltip("Score that starts the celebration. 28 bricks x 10 points = 280 (board cleared).")]
    [SerializeField] private int triggerScore = 280;

    [Header("References")]
    [SerializeField] private VideoPlayer player;
    [Tooltip("RawImage showing the video's RenderTexture, using the 'SWE402/UI Chroma Key' material. Raycast Target OFF.")]
    [SerializeField] private RawImage image;

    [Header("Layout")]
    [Tooltip("Which part of the video frame to show (the banner sits at the top of the frame).")]
    [SerializeField] private Rect visibleUV = new Rect(0f, 0.62f, 1f, 0.38f);
    [Tooltip("Banner width as a fraction of the screen width (the video has empty green sides).")]
    [SerializeField, Range(0.3f, 2f)] private float widthFraction = 1.25f;
    [Tooltip("World X of the middle of the play area (the arena, not the screen: the starter's camera is off-center).")]
    [SerializeField] private float arenaCenterX = 0f;
    [Tooltip("World Y where the top edge of the banner image sits. The arena's inner top is about 5.")]
    [SerializeField] private float topWorldY = 5.1f;

    [Header("Audio")]
    [SerializeField, Range(0f, 1f)] private float volume = 0.7f;

    private const float VideoAspect = 16f / 9f;
    private bool played;
    private bool showPending;

    private void Awake()
    {
        if (image != null) image.enabled = false;
        if (player != null)
        {
            player.playOnAwake = false;
            player.isLooping = false;
            player.sendFrameReadyEvents = true;
        }
    }

    private void OnEnable()
    {
        GameEvents.ScoreChanged += HandleScoreChanged;
        if (player != null)
        {
            player.frameReady += OnFrameReady;
            player.loopPointReached += OnFinished;
            player.Prepare();   // get ready now, so the banner appears instantly later
        }
    }

    private void OnDisable()
    {
        GameEvents.ScoreChanged -= HandleScoreChanged;
        if (player != null)
        {
            player.frameReady -= OnFrameReady;
            player.loopPointReached -= OnFinished;
            player.Stop();
        }
        if (image != null) image.enabled = false;
        showPending = false;
    }

    private void HandleScoreChanged(int score)
    {
        if (score < triggerScore) { played = false; return; }   // reset (game over) allows a replay
        if (played) return;
        played = true;
        Show();
    }

    private void Show()
    {
        if (player == null || image == null) return;

        // size and place the banner for the current screen
        var canvas = image.canvas;
        var canvasRt = canvas.GetComponent<RectTransform>();
        float width = canvasRt.rect.width * widthFraction;
        float height = width * (visibleUV.height / visibleUV.width) / VideoAspect;
        var rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 1f);          // place by the banner's top-center
        rt.sizeDelta = new Vector2(width, height);

        // Convert the arena's world position (not the screen center) into canvas coordinates.
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 screen = cam.WorldToScreenPoint(new Vector3(arenaCenterX, topWorldY, 0f));
            Camera uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRt, screen, uiCam, out Vector2 local))
                rt.anchoredPosition = local;
        }
        image.uvRect = visibleUV;

        player.SetDirectAudioVolume(0, volume);
        showPending = true;       // the image appears with the first video frame (no flash of an empty texture)
        player.Stop();
        player.Play();
    }

    private void OnFrameReady(VideoPlayer vp, long frame)
    {
        if (showPending && image != null)
        {
            image.enabled = true;
            showPending = false;
        }
    }

    private void OnFinished(VideoPlayer vp)
    {
        if (image != null) image.enabled = false;
        vp.Stop();
        vp.Prepare();   // ready for a replay
    }
}
