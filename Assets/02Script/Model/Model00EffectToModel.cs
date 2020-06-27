using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Model : MonoBehaviour
{
    public static StateEffecterSheet sheet;

    List<StateEffecterSheet.Param> runningEffect { set; get; } = new List<StateEffecterSheet.Param>();
    public void SetEffecterToModel(StateEffecterSheet.Param data)
    {
        StartCoroutine(ProcessEffect(data));
    }
    IEnumerator ProcessEffect(StateEffecterSheet.Param data)
    {
        nowHP += (int)(HP * (data.nowHp / HP));
        nowMP += (int)(MP * (data.nowMp / MP));

        ATK += data.ATK_point;
        DEF += data.DEF_point;
        SPD += data.SPD_point;

        var increasePercentATK = (int)(ATK * (data.ATK_percent / 100));
        var increasePercentDEF = (int)(ATK * (data.DEF_percent / 100));
        var increasePercentSPD = (int)(SPD * (data.SPD_percent / 100));

        ATK += increasePercentATK;
        DEF += increasePercentDEF;
        SPD += increasePercentSPD;

        if (data.During > 0)
        {
            float time = 0;
            while (time <= data.During)
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            ATK -= data.ATK_point;
            DEF -= data.DEF_point;
            SPD -= data.SPD_point;

            ATK -= increasePercentATK;
            DEF -= increasePercentDEF;
            SPD -= increasePercentSPD;
        }
    }
}
