using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputSystem;
    private InputActionMap ActionMap;
    public InputAction Select;

    private bool isOnTransition;
    [SerializeField] private string nextScene;

    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private BlinkingText blinkingText;
    void Start()
    {
        BGMController.Instance.PlayByBGMType(BGMType.Title);
        ActionMap = _inputSystem.FindActionMap("PlayerControls");
        Select = ActionMap.FindAction("AnyKey");
        ActionMap.Enable();
    }

    void Update()
    {
        if(isOnTransition) { return;}

        if(Select.WasPressedThisFrame()){
            isOnTransition = true;
            blinkingText.Hide();
            StartCoroutine(SceneTransitionRoutine());
        }
    }

    public IEnumerator SceneTransitionRoutine()
    {
        animationController.PlayAction("Ready");

        float timeout = 10f;

        while(animationController.GetNormalizedTime("Ready")< 1.0f && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null; 
        }

        SceneManager.LoadScene(nextScene);
    }
}
