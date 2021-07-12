using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatusBarController : MonoBehaviour
{
    public UIDocument statusBarDoc;
    public UIDocument reloadBarDoc;
    public UIDocument pauseButtonDoc;

    public PlayerController playerController;
    public PlayerAmmoController ammoController;
    public Target target;
    public ProgressionHolder progressionHolder;
    public Camera mainCamera;

    private VisualElement statusContainer;
    private StatusBarVisualElement statusBar;

    private VisualElement reloadContainer;
    private VisualElement reloadInnerContainer;
    private ReloadBarVisualElement reloadBar;

    private VisualElement pauseButtonContainer;
    private Button pauseButton;

    // Start is called before the first frame update
    void Start()
    {
        statusContainer = statusBarDoc.rootVisualElement;
        statusBar = statusContainer.Q<StatusBarVisualElement>();
        statusBar.Init();
        statusContainer.transform.position = new Vector2(10f, 10f);

        reloadContainer = reloadBarDoc.rootVisualElement;
        reloadBar = reloadContainer.Q<ReloadBarVisualElement>();
        reloadInnerContainer = reloadContainer.Q("Container");
        reloadBar.Init();
        reloadBar.SetReloadProgress(0f);

        pauseButtonContainer = pauseButtonDoc.rootVisualElement;
        pauseButton = pauseButtonContainer.Q<Button>();

        //pauseButtonContainer.transform.position =
        //    new Vector2(mainCamera.pixelWidth - pauseButtonContainer.layout.width - 10f, mainCamera.pixelHeight - pauseButtonContainer.layout.height - 10f);
        pauseButtonContainer.transform.position = RuntimePanelUtils.ScreenToPanel(pauseButtonContainer.panel, new Vector2(mainCamera.pixelWidth - 64f - 10f, 10));
        Debug.Log("pos " + new Vector2(mainCamera.pixelWidth - pauseButtonContainer.layout.width - 10f, 10) + " - " + pauseButton.layout.width + " - " + pauseButton);
        Debug.Log("pos2 " + RuntimePanelUtils.ScreenToPanel(pauseButtonContainer.panel, new Vector2(Screen.width - pauseButton.layout.width - 10f, Screen.height - pauseButtonContainer.layout.height - 10f)));
        Debug.Log("pos3 " + mainCamera.pixelWidth + " " + mainCamera.pixelHeight);
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
            //Vector3 pos3 = new Vector3(playerPos.x, 4f, playerPos.z);
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(reloadContainer.panel, playerPos, mainCamera);
            reloadInnerContainer.transform.position = new Vector2(newPosition.x - reloadInnerContainer.layout.width / 2, newPosition.y + 20f);
            Debug.Log("width: " + reloadInnerContainer.layout.width);
        }

        reloadBar.SetReloadProgress(progress);
    }
}
