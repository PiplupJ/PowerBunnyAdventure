using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StatusType
{
    Burn, Toxic, Slow, Freeze, Stun, Confusion
}

//プレイヤ・敵が使うため
public interface IStatusReceiver
{
    int MaxHP { get; }
    void OnHit(int damage, bool wasCrit);
    Vector3 DamageAnchorPos  { get; }
}

public class StatusSystem : MonoBehaviour
{
    private IStatusReceiver _owner;
    [SerializeField] private Animator _ownerAnimator;
    
    private Dictionary<StatusType, Coroutine> _activeStatuses
        = new Dictionary<StatusType, Coroutine>();

    public float speedModifier { get; private set; } = 1.0f;
    public float attackModifier { get; private set; } = 1.0f;

    [SerializeField] private StatusConfig _config;


    public bool HasStatus(StatusType type)
    {
        return _activeStatuses.ContainsKey(type);
    }
    
    public void Init(IStatusReceiver owner)
    {
        _owner = owner;
    }

    public void ApplyStatus(StatusType type, float duration)
    {
        if(HasStatus(type))
        {
            //延長できるステータス異常は延長
            if(!_config.Get(type).canRefresh) { return; }
            StopCoroutine(_activeStatuses[type]);
            _activeStatuses.Remove(type);
        }

        Coroutine c = StartCoroutine(StatusRoutine(type, duration));
        _activeStatuses[type] = c;
    }

    private IEnumerator StatusRoutine(StatusType type, float duration)
    {
        OnStatusEnter(type);
        CalculateModifier();

        StatusData data = _config.Get(type);

        float elapsedTime = 0.0f;
        float tickTimer = 0.0f;
        
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            tickTimer += Time.deltaTime;

            if(data.hasDOT && tickTimer >= data.tickInterval)
            {
                tickTimer = 0.0f;
                
                int damage = Mathf.Max(1, (int)(_owner.MaxHP * data.dotPercent));
                
                ApplyDOT(type, damage);
            }

            yield return null;
        }

        _activeStatuses.Remove(type);
        OnStatusExit(type);
        CalculateModifier();
    }

    private void OnStatusEnter(StatusType type)
    {
        ShowStatusIcon(type);
    }

    private void OnStatusExit(StatusType type)
    {

    }

    private void CalculateModifier()
    {
        speedModifier = 1f;
        attackModifier = 1f;

        foreach(StatusType s in new List<StatusType>(_activeStatuses.Keys))
        {
            StatusData data = _config.Get(s);
            if(data == null) continue;

            speedModifier *= data.moveModifier;
            attackModifier += data.attackModifier;
        }
    }

    private void ApplyDOT(StatusType type, int damage)
    {
        _owner.OnHit(damage, false);
        DamageLabel damageLabel = ObjectPool.Instance.GetObject<DamageLabel>(IDRegistry.DAMAGE_LABEL);
        damageLabel.InitAsStatus(damage, type, _owner.DamageAnchorPos);
    }

    
    //ui

    [SerializeField] private GameObject _burnIcon;
    [SerializeField] private GameObject _toxicIcon;
    [SerializeField] private GameObject _freezeIcon;
    [SerializeField] private GameObject _stunIcon;
    [SerializeField] private GameObject _slowIcon;
    [SerializeField] private GameObject _confusionIcon;
    [SerializeField] private Transform  _statusBar;

    private List<GameObject> _activeIcons = new List<GameObject>();

    private void ShowStatusIcon(StatusType type)
    {
        GameObject icon = GetIcon(type);
        if(icon == null) return;

        icon.SetActive(true);

        if (!_activeIcons.Contains(icon))
        {
            _activeIcons.Add(icon);
        }
         
        AlignStatusBar();
    }

    private void HideStatusIcon(StatusType type)
    {
        GameObject icon = GetIcon(type);
        if(icon == null) return;

        icon.SetActive(false);
        _activeIcons.Remove(icon);
        AlignStatusBar();
    }

    private GameObject GetIcon(StatusType type) => type switch
    {
        StatusType.Burn   => _burnIcon,
        StatusType.Toxic  => _toxicIcon,
        StatusType.Freeze => _freezeIcon,
        StatusType.Stun   => _stunIcon,
        StatusType.Slow   => _slowIcon,
        StatusType.Confusion   => _confusionIcon,
        _                 => null
    };

    private void AlignStatusBar()
    {

    }

}
