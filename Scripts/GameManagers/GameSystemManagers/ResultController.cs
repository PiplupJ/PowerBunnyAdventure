using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum GameResult
{
    Victory,
    Defeat
}
public class ResultController : MonoBehaviour
{
    [SerializeField] private GameObject   resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Image  overlayImage;

    [SerializeField] private InputActionAsset _inputSystem;

    private InputActionMap ActionMap;

    public InputAction Move;
    public InputAction Select;

    public string nextScene;

    private void Awake()
    {
        resultPanel.SetActive(false);

        ActionMap = _inputSystem.FindActionMap("PlayerControls");
        Move = ActionMap.FindAction("Move");
        Select = ActionMap.FindAction("Select");

        ActionMap.Enable();
    }

    public void Activate(GameResult result)
    {
        StartCoroutine(ResultRoutine(result));
    }

    private IEnumerator ResultRoutine(GameResult result)
    {
        yield return StartCoroutine(ShowResult(result));

        while(true)
        {
            if(Select.WasPressedThisFrame())
            {
                ConfirmSelection();
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator ShowResult(GameResult result)
    {
        //resultText.transform.localScale = Vector3.zero; 
        resultText.text = result == GameResult.Victory ? "VICTORY" : "DEFEAT";
        resultText.color = result == GameResult.Victory ? Color.yellow : Color.red;

        float elapsedTime = 0;
        float duration = 0.5f;
        Color overlayColor = new Color(0f, 0f, 0f, 0f);
        overlayImage.color = overlayColor;

        resultPanel.SetActive(true);

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            overlayColor.a = Mathf.Lerp(0f, 0.6f, elapsedTime/duration);
            overlayImage.color = overlayColor;

            yield return null;
        }

        //yield return StartCoroutine(PopInText());
    }


    private void ConfirmSelection()
    {
        SceneManager.LoadScene(nextScene);
    }

}
