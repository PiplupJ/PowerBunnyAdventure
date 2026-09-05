/*
26/04/12 Update
無限モード追加のために、GameManagerへの依存性を切った
レベルアップエフェクトを配置する時、GameManagerを呼び出さずに、生成からPlayer座標を持っている
*/
using UnityEngine;
using System.Collections.Generic;
using System;

//JSONファイル用の宣言
[System.Serializable] 
public class ExpData
{
    public List<float> expList;
}
//経験値更新機能のみinterface化
public interface ILevelUpSystem
{
    void ExpCalculation(float gainedExp);
}

public class LevelManager : MonoBehaviour , ILevelUpSystem
{
    public int level;
    public float currentExp;//現在の経験値
    public List<float>expTable = new List<float>();
    private int maxLevel; //最大レベル
    private float targetExp;//レベル別目標経験値
    public event Action LevelUpEvent;

    private Transform _playerTransform;

    //ドラッグ＆ドロップ。シーンに配置はLevelManagerの子供としてすること
    [SerializeField] private LevelSystemUI _levelSystemUI;

    public void Init()
    {
        level = 1;
        currentExp = 0;
        LoadExpTable();
        SetTargetExp();
        _levelSystemUI.Init();
    }

    public void SetPlayer(Transform playerTransform)
    {
        _playerTransform = playerTransform;
    }

    //レベルによる目標expを設定
    private void LoadExpTable()
    {
        //Resourcesフォルダからjsonファイルを開く
        TextAsset jsonText = Resources.Load<TextAsset>("Data/ExpTable");

        if(jsonText == null)
        {
            throw new System.IO.FileNotFoundException(
                "経験値テーブルが見つかりません:Resources/Data/ExpTable"
            );
        }
        ExpData data = JsonUtility.FromJson<ExpData>(jsonText.text);
        if(data?.expList == null || data.expList.Count < 2)
        {
            throw new System.Exception("経験値テーブルのパースに失敗、または要素不足です。:Data/ExpTable");
        }
                        
        //GameManagerのListを更新
        expTable = data.expList;
            
        //配列の[0]はDummyDataなので配列の大きさ-1
        maxLevel = expTable.Count - 1; 
    }

    //現在のレベルの目標経験値を設定
    private void SetTargetExp()
    {
        if(level < 0 || level >= expTable.Count)
        {
            Debug.LogError($"[LevelManager]レベル{level}が経験値テーブルの範囲外です");
            return;
        }
        targetExp = expTable[level];
    }

    public void ExpCalculation(float gainedExp)
    {
        if(level < maxLevel)
        {
            currentExp += gainedExp;
            bool isLevelUp = currentExp >= targetExp;
            float targetExpRatio = currentExp/targetExp;

            if(targetExpRatio>0){
                _levelSystemUI.UpdateExpBar(targetExpRatio, () => {
                    if(isLevelUp){
                        HandleLevelUp();
                        //次の処理はGameManager・PlayerControllerに任せる
                        LevelUpEvent?.Invoke();
                    }
                });
            }
        }        
    }

    private void HandleLevelUp()
    {
        level++;
        currentExp -= targetExp;
        SetTargetExp();
        //満タンになったEXPバーを見せる
        _levelSystemUI.OnLevelUp(level);
        ShowLevelUpEffect();
    }
    
    private void ShowLevelUpEffect()
    {
        if(ObjectPool.Instance.TryGetObject<HitEffect>(IDRegistry.EFFECT_LEVELUP, out HitEffect levelUp))
        {
            levelUp.transform.position = _playerTransform.position;            
        }
    }
}
