using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WorldMapController : MonoBehaviour
{
    public WorldMapManager worldMapManager;
    public WorldMapPlayer player;
    [SerializeField] private float inputDelay = 0.2f;

    [SerializeField] private InputActionAsset _inputSystem;

    private InputActionMap ActionMap;

    public InputAction Move;
    public InputAction Select;

    private Vector2 _directionInput;

    private bool isMoving;
    private float _inputTimer = 0f;

    private void Awake()
    {
        ActionMap = _inputSystem.FindActionMap("PlayerControls");
        Move = ActionMap.FindAction("Move");
        Select = ActionMap.FindAction("Select");

        ActionMap.Enable();

        isMoving = false;
    }

    private void Start()
    {
        player.transform.position = worldMapManager.CurrentNode.WorldPos;
        player.RotateToDefault();
        BGMController.Instance.PlayByBGMType(BGMType.Title);
    }

    private void Update()
    {
        if(isMoving) { return; }

        if(_inputTimer > 0){
            _inputTimer -= Time.deltaTime;
            return;
        }

        _directionInput = Move.ReadValue<Vector2>();
        bool selectInput = Select.WasPressedThisFrame();

        if(selectInput)
        {
            isMoving = true;
            StartCoroutine(player.StartRoutine(LoadWorld));
        }
        
        if(_directionInput.x * _directionInput.x + _directionInput.y * _directionInput.y < 0.0001)
        {
            return;
        } 

        WorldMoveAngle angle = _directionInput.x > 0 ? WorldMoveAngle.Right : WorldMoveAngle.Left;

        if(!worldMapManager.HasWorldToMove(angle))
        {
            return;
        }

        isMoving = true;
        _inputTimer = inputDelay;

        Vector3 destination = worldMapManager.GetDestination(angle);

        worldMapManager.DisableNodeDescription();
        StartCoroutine(player.MoveToNextWorld(destination, () => OnArrival(angle)));
    }

    private void OnArrival(WorldMoveAngle angle)
    {
        worldMapManager.UpdateWorldIndex(angle);
        worldMapManager.EnableNodeDescription();
        isMoving = false;
    }

    private void LoadWorld()
    {
        isMoving = false;
        worldMapManager.EnterCurrentWorld();
    }

}
