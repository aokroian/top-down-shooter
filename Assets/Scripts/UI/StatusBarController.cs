using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatusBarController : MonoBehaviour
{
    public UIDocument statusBarDoc;
    public UIDocument reloadBarDoc;
    //public UIDocument pauseButtonDoc;

    public PlayerController playerController;
    public PlayerAmmoController ammoController;
    public Target target;
    public ProgressionHolder progressionHolder;
    public Camera mainCamera;
    //public GameLoopController gameLoopController;

    private VisualElement statusContainer;
    private StatusBarVisualElement statusBar;

    private VisualElement reloadContainer;
    private VisualElement reloadInnerContainer;
    private ReloadBarVisualElement reloadBar;

    //private VisualElement pauseButtonContainer;
    //private Button pauseButton;

    // Start is called before the first frame update
    void OnEnable()
    {
        statusContainer = statusBarDoc.rootVisualElement;
        statusBar = statusContainer.Q<StatusBarVisualElement>();
        statusBar.Init();

        reloadContainer = reloadBarDoc.rootVisualElement;
        reloadBar = reloadContainer.Q<ReloadBarVisualElement>();
        reloadInnerContainer = reloadContainer.Q("Container");
        reloadBar.Init();
        reloadBar.SetReloadProgress(0f);

        //pauseButtonContainer = pauseButtonDoc.rootVisualElement;
        //pauseButton = pauseButtonContainer.Q<Button>();
        //pauseButton.RegisterCallback<ClickEvent>(e => gameLoopController.Pause());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatusBar();
        UpdateReloadBar();
    }

    private void UpdateStatusBar()
    {
        statusBar.SetHealth(target.GetHPPercent());
        statusBar.SetStamina(playerController.GetStaminaPercent());

        AmmoType ammoType = ammoController.GetCurrentAmmoType();
        statusBar.SetAmmo(ammoController.GetCurrentAmmoInClip(), ammoController.GetAmmoLeft(ammoType));

        statusBar.SetCreditCount(progressionHolder.moneyCount);
    }

    private void UpdateReloadBar()
    {
        float progress = playerController.GetReloadTimerPercent();
        if (progress != 0)
        {
            var playerPos = playerController.transform.position;
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(reloadContainer.panel, playerPos, mainCamera);
            reloadInnerContainer.transform.position = new Vector2(newPosition.x - reloadInnerContainer.layout.width / 2, newPosition.y + 20f);
        }

        reloadBar.SetReloadProgress(progress);
    }
}
