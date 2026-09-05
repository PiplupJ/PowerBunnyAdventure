using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private TextMeshProUGUI _healthState;
    private Player _player;
    
    public void BindPlayer(Player player)
    {
        Unbind();

        _player = player;
        _player.OnHpUpdate += UpdateHealthBar;
        
        UpdateHealthBar();
    }

    public void Unbind()
    {
        if (_player == null) return;
        _player.OnHpUpdate -= UpdateHealthBar;
        _player = null;
    }

    private void OnDestroy() {
        Unbind();    
    }

    private void UpdateHealthBar()
    {
        string currHp = _player.stat.hp.currentHp.ToString(); 
        string maxHp = _player.stat.hp.maxHp.ToString(); 
        _healthState.text = currHp +"/"+maxHp;

        _healthBar.fillAmount = _player.stat.hp.GetHpRatio();
    }
}
