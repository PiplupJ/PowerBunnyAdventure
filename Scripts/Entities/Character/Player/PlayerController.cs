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
    //プレイヤーをセッティング
    public void SetPlayer(IPlayerControl selectedPlayer)
    {
        _currentPlayer = selectedPlayer;
    }
   //更新
    private void Update()
    {
        if(IsAuto) { return; }
        _directionInput = Move.ReadValue<Vector2>();
        bool selectInput = Select.WasPressedThisFrame();
        switch(_controlMode)
        {
            //ゲームモードならプレイヤー操作
            case ControlMode.GameMode :
                _currentPlayer.HandlePlayerInput(_directionInput);
                break;
            //カード選択モードならカード選択操作
            case ControlMode.CardDrawMode :
                cardDrawControl.HandleCardSelection( _directionInput, selectInput);
                break;
            default :
                break;
        }
    }
    //物理的な、プレイヤーの移動に関する操作
    private void FixedUpdate() 
    {
        if(_controlMode != ControlMode.GameMode || _currentPlayer == null) { return; }    
        
        float dt = Time.fixedDeltaTime;

        _currentPlayer.HandlePlayerAction(dt);
    }
    //操作モード切り替え
    public void PushControlMode(ControlMode newMode)
    {
        _controlModeStack.Push(newMode);
    }
    //操作モードを前の状態へ
    public void PopControlMode()
    {
        if(_controlModeStack.Count > 1)
        {
            _controlModeStack.Pop();
        }
    }
}
