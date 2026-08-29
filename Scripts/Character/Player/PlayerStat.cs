using UnityEngine;
using System.Collections.Generic;
using System;

//成長するステータスの基本
public abstract class StatBase
{
    public float baseValue;
    public float growthPerLevel;
    protected int statLevel;

    public int level {
        get { return statLevel; } 
        set { 
            statLevel = value; 
            StatUpdate(); 
        }
    }
    public abstract void StatUpdate();
}
//int型のステータス
[System.Serializable]
public class IntStat : StatBase
{
    public int value { get; protected set; } 

    public override void StatUpdate() {
        value = (int)(baseValue * (1.0f + (statLevel * growthPerLevel))+0.5f);
    }
}
//float型のステータス
[System.Serializable]
public class FloatStat : StatBase
{
    public float value { get; protected set; } 

    public override void StatUpdate() {
        value = (baseValue * (1.0f + (statLevel * growthPerLevel)));
    }
}
//HP用の関数
[System.Serializable]
public class HpStat : IntStat
{   
    //現在のhp
    public int currentHp { get; private set;}
    
    //maxHpがvalueの役割をする
    public int maxHp {
        get { return value; }
    }

    //ステータスが更新された時の処理
    public override void StatUpdate()
    {
        int prevMaxHp = maxHp;

        //親の処理を使う
        base.StatUpdate();

        //hpが強化されたら、最大hpが増加した分現在のhpが増加
        int hpDiff = maxHp - prevMaxHp;
        if(hpDiff > 0){
            currentHp += hpDiff;
        }
    }
    //ダメージ処理
    public void TakeDamage(int damage)
    {
        if(currentHp == 0) { return; }
        currentHp -= damage;
        if(currentHp <= 0) { currentHp = 0; }
    }
    //回復処理
    public void Heal(int healAmount)
    {
        currentHp += healAmount;
        if(currentHp > maxHp){
            currentHp = maxHp;
        }
    }
    //uiのための処理
    public float GetHpRatio()
    {
        return (float)currentHp/maxHp;
    }
}
//プレイヤのステータス
//jsonファイルから読み込む
[System.Serializable]
public class PlayerStat
{
	public int id;
	public string name;
	
	public HpStat hp;
	public IntStat attack;
	public IntStat defense;
	
	public FloatStat moveSpeed;
	public FloatStat criticalRate;
	public FloatStat attackSpeed;
    public FloatStat attackDist;
	
	public int resurrectCount;
	
	public float rad;
	public float attackInterval; 
	
	public List<int> ShotIDs;

    //ファイルからロード後呼び出す
    //初期状態のレベル0基準のステータスになる
    public void InitStats()
    {
        hp.StatUpdate();
        attack.StatUpdate();
        defense.StatUpdate();
        moveSpeed.StatUpdate();
        criticalRate.StatUpdate();
        attackSpeed.StatUpdate();
        attackDist.StatUpdate();

        resurrectCount = 1;

        hp.Heal(hp.maxHp);
    }

    public PlayerStat Clone()
    {
        PlayerStat clone = new PlayerStat();

        clone.id             = this.id;
        clone.name           = this.name;
        clone.resurrectCount = this.resurrectCount;
        clone.rad            = this.rad;
        clone.attackInterval = this.attackInterval;
        clone.ShotIDs        = new List<int>(this.ShotIDs); 

        clone.hp          = new HpStat
        {
            baseValue      = this.hp.baseValue,
            growthPerLevel = this.hp.growthPerLevel
        };
        clone.attack      = new IntStat
        {
            baseValue      = this.attack.baseValue,
            growthPerLevel = this.attack.growthPerLevel
        };
        clone.defense     = new IntStat
        {
            baseValue      = this.defense.baseValue,
            growthPerLevel = this.defense.growthPerLevel
        };
        clone.moveSpeed   = new FloatStat
        {
            baseValue      = this.moveSpeed.baseValue,
            growthPerLevel = this.moveSpeed.growthPerLevel
        };
        clone.criticalRate = new FloatStat
        {
            baseValue      = this.criticalRate.baseValue,
            growthPerLevel = this.criticalRate.growthPerLevel
        };
        clone.attackSpeed  = new FloatStat
        {
            baseValue      = this.attackSpeed.baseValue,
            growthPerLevel = this.attackSpeed.growthPerLevel
        };
        clone.attackDist   = new FloatStat
        {
            baseValue      = this.attackDist.baseValue,
            growthPerLevel = this.attackDist.growthPerLevel
        };

        clone.InitStats(); 
        return clone;
    }
}
