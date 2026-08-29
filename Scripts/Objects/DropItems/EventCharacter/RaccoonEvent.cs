using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaccoonEvent : EventChracter
{
    private bool canSelect = false;

    [SerializeField] private GameObject [] _choices;
    [SerializeField] private GameObject model;
    //[SerializeField] private int _prizeID;
    [SerializeField] private int _effectID;

    [SerializeField] private CardRarity rarity = CardRarity.Epic;

    public EventCharacterAnimationController animationController;

    [SerializeField] private float itemSpacing = 3.0f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correct;
    [SerializeField] private AudioClip wrong;

    private int _correctIndex;

    public override void ApplyEffect()
    {
        if(!canSelect) return;
        CheckSelection();
    }

    public override void HandleItemAction(float deltaTime)
    {
        Vector3 dir = (receiver.playerPos - model.transform.position).normalized;
        Vector2 rotDir = new Vector2(dir.x, dir.z);
        model.transform.rotation = MovementHelper.GetRoation(rotDir);
    }


    public override void StartEvent()
    {
        StartCoroutine(RaccoonRoutine());
    }

    private IEnumerator RaccoonRoutine()
    {

        Debug.Log("Routine Start");
        animationController.PlayAction("Start");

        yield return new WaitForSeconds(0.5f);

        Debug.Log("animation ends");
        CreateItems();
        canSelect = true;
    }

    private void CreateItems()
    {
        _correctIndex = Random.Range(0, _choices.Length);
        Debug.Log("当たりは:"+_correctIndex+1);

        Vector3[] positions = GetSpawnPositions();

        for(int i = 0; i < _choices.Length; i++)
        {
            Vector3 spawnPos = positions[i];
            HitEffect trickEffect = ObjectPool.Instance.GetObject<HitEffect>(_effectID);
            trickEffect.transform.position = spawnPos;
            
            _choices[i].transform.position = spawnPos;
            _choices[i].SetActive(true);
        }
    }

    private Vector3[] GetSpawnPositions()
    {
        Vector3 center = transform.position;

        return new Vector3[]
        {
            center + new Vector3(-itemSpacing, 0f,  -itemSpacing),
            center + new Vector3(0f, 0f,  -itemSpacing),
            center + new Vector3(itemSpacing, 0f,  -itemSpacing)
        };

    }

    private void CheckSelection()
    {
        for(int i = 0; i < _choices.Length; i++)
        {
            Vector3 itemPos = _choices[i].transform.position;

            if(DistanceHelper.IsInRange(receiver.playerPos, itemPos, 1.0f))
            {
                ConfirmSelection(i);
                canSelect = false;
                break;
            }
        }
    }

    private void ConfirmSelection(int choice)
    {
        bool wasCorrect;

        if(choice == _correctIndex)
        {
            wasCorrect = true;
            audioSource.PlayOneShot(correct);
            Debug.Log("当たり!");
            //itemSystem.CreateItemAtPosition(_prizeID, _choices[choice].transform.position);

            
        }
        else
        {
            wasCorrect = false;
            audioSource.PlayOneShot(wrong);
            Debug.Log("ハズレ!");
        }

        StartCoroutine(EndRoutine(wasCorrect));
    }

    private IEnumerator EndRoutine(bool wasCorrect)
    {

        animationController.PlayAction("Finish");

        yield return new WaitForSeconds(3.0f);

        HitEffect trickEffect = ObjectPool.Instance.GetObject<HitEffect>(_effectID);
        trickEffect.transform.position = transform.position;

        if(wasCorrect){
            GameManager.Instance.StartCardDrawEventWithRarity(rarity);
        }
        OnUse();
    }

    private void OnDisable()
    {
        for(int i = 0; i < _choices.Length; i++)
        {
            _choices[i].SetActive(false);
        }
    }
    
}
