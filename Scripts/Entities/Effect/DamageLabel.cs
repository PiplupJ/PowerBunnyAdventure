using UnityEngine;
using TMPro;
using System.Collections;

//UIElementDBに保存
//id 6000000
public class DamageLabel : PoolableObject
{
    public TextMeshPro damageText;
    public float maxHeight = 0.5f;
    public float duration = 0.2f;
    public float radius = 0.2f;

    //テキスト・位置を初期化
    public void Init(int damage, bool wasCrit, Vector3 newPos)
    {
        if(wasCrit){
            damageText.fontStyle = FontStyles.Bold;
            damageText.color = Color.red;
            damageText.fontSize = 16f;
        }
        else{
            damageText.fontStyle = FontStyles.Normal;
            damageText.color = Color.white;
            damageText.fontSize = 12f;
        }
        damageText.text = damage.ToString();
        this.transform.position = newPos;

        transform.forward = Camera.main.transform.forward;
    
        StartCoroutine(PopUpRoutine());
    }

    public void InitAsStatus(int damage, StatusType type, Vector3 newPos)
    {
        damageText.fontStyle = FontStyles.Bold;    
        damageText.fontSize = 12f;

        switch(type)
        {
            case StatusType.Toxic :
                damageText.color = Color.purple;
                break;
            case StatusType.Burn :
                damageText.color = Color.orange;
                break;
            default :
                break;
        }

        damageText.text = damage.ToString();
        this.transform.position = newPos;
        transform.forward = Camera.main.transform.forward;

        StartCoroutine(PopUpRoutine());
    }

    //ポップアップルチン。終わったらReturnToPool()
    private IEnumerator PopUpRoutine()
    {
        float randomAngle = Random.Range(0f, 180f)*Mathf.Deg2Rad;
    
        Vector3 startPos = transform.position;

        Vector3 targetPos = new Vector3(
            transform.position.x + Mathf.Cos(randomAngle) * radius,
            1,
            transform.position.z + Mathf.Sin(randomAngle)*radius
        );

        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime/duration;

            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);

            float height = 4 * maxHeight * t * (1-t);

            this.transform.position = new Vector3(currentPos.x, currentPos.y + height, currentPos.z);

            yield return null;
        }
        ReturnToPool();
    }
}
