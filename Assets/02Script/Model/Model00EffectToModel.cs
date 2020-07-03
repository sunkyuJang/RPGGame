using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public partial class Model : MonoBehaviour
{
    public List<StateEffecterManager.CoroutinForEffecter> listForStateEffecter { set; get; } = new List<StateEffecterManager.CoroutinForEffecter>();

    public IEnumerator GetProcessIncreaseEffect(StateEffecterSheet.Param data, StateEffecterManager.CoroutinForEffecter coroutinForEffecter)
    {
        nowHP += (int)(HP * (data.nowHp / HP));
        nowMP += (int)(MP * (data.nowMp / MP));

        ATK += data.ATK_point;
        DEF += data.DEF_point;
        SPD += data.SPD_point;

        coroutinForEffecter.increasePercentageATK = (int)(ATK * (data.ATK_percent / 100));
        coroutinForEffecter.increasePercentageDEF = (int)(ATK * (data.DEF_percent / 100));
        coroutinForEffecter.increasePercentageSPD = (int)(SPD * (data.SPD_percent / 100));

        ATK += coroutinForEffecter.increasePercentageATK;
        DEF += coroutinForEffecter.increasePercentageDEF;
        SPD += coroutinForEffecter.increasePercentageSPD;

        if(this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }

        if (data.During > 0)
        {
            coroutinForEffecter.time = 0f;
            while (coroutinForEffecter.time <= data.During)
            {
                coroutinForEffecter.time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            ATK -= data.ATK_point;
            DEF -= data.DEF_point;
            SPD -= data.SPD_point;

            ATK -= coroutinForEffecter.increasePercentageATK;
            DEF -= coroutinForEffecter.increasePercentageDEF;
            SPD -= coroutinForEffecter.increasePercentageSPD;

            RemoveCoroutine(data);
        }

        if (this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }
    }

    public IEnumerator GetProcessDeincreaseEffect(StateEffecterSheet.Param data, StateEffecterManager.CoroutinForEffecter coroutinForEffecter)
    {
        nowHP -= (int)(HP * (data.nowHp / HP));
        nowMP -= (int)(MP * (data.nowMp / MP));

        ATK -= data.ATK_point;
        DEF -= data.DEF_point;
        SPD -= data.SPD_point;

        var increasePercentATK = (int)(ATK * (data.ATK_percent / 100));
        var increasePercentDEF = (int)(ATK * (data.DEF_percent / 100));
        var increasePercentSPD = (int)(SPD * (data.SPD_percent / 100));

        ATK -= increasePercentATK;
        DEF -= increasePercentDEF;
        SPD -= increasePercentSPD;

        if (this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }

        if (data.During > 0)
        {
            coroutinForEffecter.time = 0;
            while (coroutinForEffecter.time <= data.During)
            {
                coroutinForEffecter.time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            ATK += data.ATK_point;
            DEF += data.DEF_point;
            SPD += data.SPD_point;
                
            ATK += increasePercentATK;
            DEF += increasePercentDEF;
            SPD += increasePercentSPD;
        }

        if (this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }
        RemoveCoroutine(data);
        print(listForStateEffecter.Count);
    }

    public void StopEffectToModel(StateEffecterManager.CoroutinForEffecter coroutinForEffecter)
    {
        ATK -= coroutinForEffecter.data.ATK_point;
        DEF -= coroutinForEffecter.data.DEF_point;
        SPD -= coroutinForEffecter.data.SPD_point;

        ATK -= coroutinForEffecter.increasePercentageATK;
        DEF -= coroutinForEffecter.increasePercentageDEF;
        SPD -= coroutinForEffecter.increasePercentageSPD;

        if (this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }
    }

    void RemoveCoroutine(StateEffecterSheet.Param data)
    {
        int index = 0;
        if(StateEffecterManager.CoroutinForEffecter.IsAlreadyRunning(this, data, out index))
        {
            StateEffecterManager.CoroutinForEffecter.RemoveCoroutine(this, index);
        }
    }

    public void GetHit(float damage, GameObject HitFX, bool isFXStartFromGround)
    {
        if (this is Monster) { (this as Monster).GetHit(damage, HitFX, isFXStartFromGround); }
        else if (this is Character) { (this as Character).GetHit(damage); }
    }
}
