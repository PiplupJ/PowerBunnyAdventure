using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum ControlMode
{        
    GameMode,CardDrawMode, PauseMode
}    
public class PlayerController : MonoBehaviour
{
    //スタックでモード管理
    private Stack<ControlMode> _controlModeStack = new Stack<ControlMode>();
    public ControlMode _controlMode => _controlModeStack.Peek();

    [SerializeField] private InputActionAsset _inputSystem;

    private InputActionMap ActionMap;

    public InputAction Move;
    public InputAction Select;
    public InputAction Pause;

    private Vector2 _directionInput;

    //自動操作モード
    public bool IsAuto;

    //クラスの操作用interfaceを持っている
    public ICardDrawControl cardDrawControl; 
    public IPlayerControl _currentPlayer;

    [SerializeField] private PlayerAction _playerAction;
    public PlayerAction Action => _playerAction;

    private void Awake()
    {
        ActionMap = _inputSystem.FindActionMap("PlayerControls");
        Move = ActionMap.FindAction("Move");
        Select = ActionMap.FindAction("Select");
        Pause = ActionMap.FindAction("Pause");
        ActionMap.Enable();

        IsAuto = false;
        
        PushControlMode(ControlMode.GameMode);
        Debug.Log("PlayerController初期化完了");
    }
    
    public void SetPlayer(IPlayerControl selectedPlayer)
    {
        _currentPlayer = selectedPlayer;
    }
   
    private void Update()
    {
        if(IsAuto) { return; }
        _directionInput = Move.ReadValue<Vector2>();
        bool selectInput = Select.WasPressedThisFrame();
        switch(_controlMode)
        {
            case ControlMode.GameMode :
                _currentPlayer.HandlePlayerInput(_directionInput);
                break;
            case ControlMode.CardDrawMode :
                cardDrawControl.HandleCardSelection( _directionInput, selectInput);
                break;
        }
    }

    private void FixedUpdate() 
    {
        if(_controlMode != ControlMode.GameMode || _currentPlayer == null) { return; }    
        
        float dt = Time.fixedDeltaTime;

        _currentPlayer.HandlePlayerAction(dt);
    }

    public void PushControlMode(ControlMode newMode)
    {
        _controlModeStack.Push(newMode);
    }

    public void PopControlMode()
    {
        if(_controlModeStack.Count > 1)
        {
            _controlModeStack.Pop();
        }
    }
}
