using UnityEngine;
using System.Collections.Generic;

public enum BGMType 
{ 
    Stage_Green_Normal, Stage_Green_Boss, 
    Stage_Desert_Normal, Stage_Desert_Boss,
    BonusStage,
    Title
}

[System.Serializable]
public struct BGMData
{
    public BGMType bgmType;
    public AudioClip clip;
}

public class BGMController : MonoBehaviour
{
    public static BGMController Instance;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private BGMData[] _bgmTable;
    private Dictionary <BGMType, AudioClip> _bgmDict;

    private void Awake()
    {
        if(Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Cache_Init();
    }

    private void Cache_Init()
    {
        _bgmDict = new Dictionary<BGMType, AudioClip>();
        foreach(var bgm in _bgmTable)
        {
            if(_bgmDict.ContainsKey(bgm.bgmType)){
                Debug.LogWarning($"同じタイプのBGMがあります。:{bgm.bgmType}");
                continue;
            }
            _bgmDict[bgm.bgmType] = bgm.clip;
        }
    }
    
    //StageManagerで実行。StageTypeに相当するBGMを再生
    public void PlayByStageMusic(string stageMusic)
    {
        BGMType bgmType = StringToBGMType(stageMusic);
        PlayByBGMType(bgmType);
    }

    private BGMType StringToBGMType(string stageMusic)
    {
        return stageMusic switch 
        {
            "Green_Normal" => BGMType.Stage_Green_Normal,
            "Green_Boss" => BGMType.Stage_Green_Boss,
            "Desert_Normal" => BGMType.Stage_Desert_Normal,
            "Desert_Boss" => BGMType.Stage_Desert_Boss,
            "Bonus" => BGMType.BonusStage,
            _               => BGMType.Stage_Green_Normal
        };
    }
    //タイトルや結果シーンで実行
    public void PlayByBGMType(BGMType bgmType)
    {
         if (!_bgmDict.TryGetValue(bgmType, out AudioClip clip))
        {
            Debug.LogWarning($"BGMが見つかりません: {bgmType}");
            return;
        }
        if (_audioSource.clip == clip) return; //もう同じBGMを再生中ならreturn
        
        _audioSource.clip = clip;
        _audioSource.loop = true;
        _audioSource.Play();

    }
}
