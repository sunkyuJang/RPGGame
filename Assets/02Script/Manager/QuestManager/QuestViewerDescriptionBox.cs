using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestViewerDescriptionBox : MonoBehaviour
{
    QuestNameBtn Btn { set; get; } = null;
    public Text Description;
    public Transform requireTransform;
    public Transform rewardTransform;

    public void ShowDescription(QuestNameBtn questNameBtn)
    {
        gameObject.SetActive(true);
        if (Btn != null) ReturnItemViewPerent();

        Btn = questNameBtn;
        Description.text = Btn.questTable.data.Name + "\r\n" + Btn.questTable.data.Description;
        SetItemViewPerent();
    }

    void SetItemViewPerent()
    {
        for (int i = 0; i < 2; i++)
        {
            var nowList = i == 0 ? Btn.requireItemGroup : Btn.rewardItemGroup;
            /*            foreach (Transform nowTransform in nowList)
                        {
                            print(nowList.childCount);
                            nowTransform.SetParent(i == 0 ? requireTransform : rewardTransform);
                            nowTransform.gameObject.SetActive(true);
                        }*/

            for (int j = 0; j < nowList.childCount; j++)
            {
                var nowTransform = nowList.GetChild(j--);
                nowTransform.SetParent(i == 0 ? requireTransform : rewardTransform);
                //nowTransform.GetComponent<ItemView>().frame.transform.localPosition = Vector3.zero;
                nowTransform.gameObject.SetActive(true);
            }
        }
    }

    void ReturnItemViewPerent()
    {
        for (int i = 0; i < 2; i++)
        {
            var nowList = i == 0 ? requireTransform : rewardTransform;

            for (int j = 0; j < nowList.childCount; j++)
            {
                var nowTransform = nowList.GetChild(j--);
                nowTransform.SetParent(i == 0 ? Btn.requireItemGroup : Btn.rewardItemGroup);
                nowTransform.gameObject.SetActive(true);
            }
        }
    }

    public void HideDescription()
    {
        ReturnItemViewPerent();
        gameObject.SetActive(false);
    }
}
