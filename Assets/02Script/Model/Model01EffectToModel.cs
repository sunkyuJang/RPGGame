using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public partial class Model : MonoBehaviour
{
    public List<StateSaver> StateSavers { set; get; } = new List<StateSaver>();

    public void SetEffect(StateEffecterSheet.Param data, bool isDeEffect)
    {
        for (int i = 0; i < StateSavers.Count; i++)
        {
            var nowState = StateSavers[i];
            if (nowState.data == data)
            {
                StopCoroutine(nowState.coroutine);
                StopEffectToModel(nowState);
                StateSavers.RemoveAt(i);
                break;
            }
        }

        var StateSaver = new StateSaver(data);
        StateSaver.coroutine = StartCoroutine(isDeEffect ? GetProcessDeincreaseEffect(data, StateSaver) : GetProcessIncreaseEffect(data, StateSaver));
        if (data.During > 0)
            StateSavers.Add(StateSaver);
    }

    public IEnumerator GetProcessIncreaseEffect(StateEffecterSheet.Param data, StateSaver saver)
    {
        nowHP += (int)(HP * (data.nowHp / HP));
        nowMP += (int)(MP * (data.nowMp / MP));

        nowHP = nowHP < HP ? nowHP : HP;
        nowMP = nowMP < MP ? nowMP : MP;

        ATK += data.ATK_point;
        DEF += data.DEF_point;
        SPD += data.SPD_point;

        saver.increasePercentageATK = (int)(ATK * (data.ATK_percent / 100));
        saver.increasePercentageDEF = (int)(ATK * (data.DEF_percent / 100));
        saver.increasePercentageSPD = (int)(SPD * (data.SPD_percent / 100));

        ATK += saver.increasePercentageATK;
        DEF += saver.increasePercentageDEF;
        SPD += saver.increasePercentageSPD;

        if (this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }

        if (data.During > 0)
        {
            RefreshedHPBar();
            saver.time = 0f;
            while (saver.time <= data.During)
            {
                saver.time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            ATK -= data.ATK_point;
            DEF -= data.DEF_point;
            SPD -= data.SPD_point;

            ATK -= saver.increasePercentageATK;
            DEF -= saver.increasePercentageDEF;
            SPD -= saver.increasePercentageSPD;

            StateSavers.Remove(saver);
        }

        if (this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }
        RefreshedHPBar();
    }

    public IEnumerator GetProcessDeincreaseEffect(StateEffecterSheet.Param data, StateSaver saver)
    {
        nowHP -= (int)(HP * (data.nowHp / HP));
        nowMP -= (int)(MP * (data.nowMp / MP));

        nowHP = nowHP > 0 ? nowHP : 1;
        nowMP = nowMP > 0 ? nowMP : 0;

        ATK -= data.ATK_point;
        DEF -= data.DEF_point;
        SPD -= data.SPD_point;

        saver.increasePercentageATK = (int)(ATK * (data.ATK_percent / 100));
        saver.increasePercentageDEF = (int)(ATK * (data.DEF_percent / 100));
        saver.increasePercentageSPD = (int)(SPD * (data.SPD_percent / 100));

        ATK -= saver.increasePercentageATK;
        DEF -= saver.increasePercentageDEF;
        SPD -= saver.increasePercentageSPD;

        if (this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }

        if (data.During > 0)
        {
            RefreshedHPBar();
            saver.time = 0;
            while (saver.time <= data.During)
            {
                saver.time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            ATK += data.ATK_point;
            DEF += data.DEF_point;
            SPD += data.SPD_point;

            ATK += saver.increasePercentageATK;
            DEF += saver.increasePercentageDEF;
            SPD += saver.increasePercentageSPD;

            StateSavers.Remove(saver);
        }

        if (this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }
        RefreshedHPBar();
    }

    public void StopEffectToModel(StateSaver saver)
    {
        ATK -= saver.data.ATK_point;
        DEF -= saver.data.DEF_point;
        SPD -= saver.data.SPD_point;

        ATK -= saver.increasePercentageATK;
        DEF -= saver.increasePercentageDEF;
        SPD -= saver.increasePercentageSPD;

        if (this is Character) { (this as Character).EquipmentView.LoadCharacterState(); }
    }
    public void GetHit(float damage, GameObject HitFX, bool isFXStartFromGround)
    {
        if (nowHP > 0)
        {
            damage -= DEF;
            damage = damage > 0 ? damage : 0;
            nowHP -= (int)damage;

            ShowAlert(((int)damage).ToString(), Color.red);
            if (this is Monster) { (this as Monster).GetHit(HitFX, isFXStartFromGround); }
            else if (this is Character) { (this as Character).GetHit(); }
            RefreshedHPBar();
        }
    }

    public class StateSaver
    {
        public StateEffecterSheet.Param data;
        public Coroutine coroutine;

        public int increasePercentageATK { set; get; }
        public int increasePercentageDEF { set; get; }
        public int increasePercentageSPD { set; get; }
        public float time = 0;
        public StateSaver(StateEffecterSheet.Param data)
        {
            this.data = data;
        }
    }
}
