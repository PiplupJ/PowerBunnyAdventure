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

    public abstract StatBase CloneBase();
    protected void CopyBaseTo(StatBase dst)
    {
        dst.baseValue = this.baseValue;
        dst.growthPerLevel = this.growthPerLevel;        
    }
}
//int型のステータス
[System.Serializable]
public class IntStat : StatBase
{
    public int value { get; protected set; } 

    public override void StatUpdate() {
        value = (int)(baseValue * (1.0f + (statLevel * growthPerLevel))+0.5f);
    }

    public override StatBase CloneBase()   
    {        
        var c = new IntStat();        
        CopyBaseTo(c);        
        return c;    
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
        public override StatBase CloneBase()   
        {        
            var c = new FloatStat();        
            CopyBaseTo(c);        
            return c;    
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
    public override StatBase CloneBase()   
    {        
        var c = new HpStat();        
        CopyBaseTo(c);        
        return c;    
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

        clone.hp           = (HpStat)   this.hp.CloneBase();
        clone.attack       = (IntStat)  this.attack.CloneBase();
        clone.defense      = (IntStat)  this.defense.CloneBase();
        clone.moveSpeed    = (FloatStat)this.moveSpeed.CloneBase();
        clone.criticalRate = (FloatStat)this.criticalRate.CloneBase();
        clone.attackSpeed  = (FloatStat)this.attackSpeed.CloneBase();
        clone.attackDist   = (FloatStat)this.attackDist.CloneBase();

        clone.InitStats(); 
        return clone;
    }
}
