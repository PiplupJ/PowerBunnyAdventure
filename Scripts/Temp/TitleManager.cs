using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputSystem;
    private InputActionMap ActionMap;
    public InputAction Select;

    [SerializeField] private string nextScene;

    void Start()
    {
        BGMController.Instance.PlayByBGMType(BGMType.Title);
        ActionMap = _inputSystem.FindActionMap("PlayerControls");
        Select = ActionMap.FindAction("AnyKey");
        ActionMap.Enable();
    }

    void Update()
    {
        bool selectInput = Select.WasPressedThisFrame();
        if(selectInput){
            SceneManager.LoadScene(nextScene);
        }
    }
}
