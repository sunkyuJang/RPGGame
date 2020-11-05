using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public partial class Model : MonoBehaviour
{
    public GameObject AlertBox { private set; get; }
    public Vector3 AlertBoxStartPoint { get { return Camera.main.WorldToScreenPoint(transform.position + (Vector3.up * 2)); } }
    List<Text> AlertBoxTexts { set; get; } = new List<Text>();
    void AwakeInAlert()
    {
        AlertBox = Resources.Load<GameObject>("AlretText");
    }
    public void ShowAlert(string text, Color color)
    {
        Text alertText = Instantiate(AlertBox, AlertBoxStartPoint, Quaternion.identity, GameManager.mainCanvas).GetComponent<Text>();
        alertText.text = text;
        alertText.color = color;
        AlertBoxTexts.Add(alertText);
        StartCoroutine(StartAlertTextMove(alertText));
    }
    IEnumerator StartAlertTextMove(Text text)
    {
        var limitDist = text.transform.position.y + 50f;

        if (AlertBoxTexts.Count > 1)
        {
            var beforeText = AlertBoxTexts[AlertBoxTexts.Count - 2];
            text.gameObject.SetActive(false);

            while (beforeText != null)
            {
                if (beforeText.transform.position.y >= limitDist - 10)
                    break;
                yield return new WaitForFixedUpdate();
            }

            text.gameObject.SetActive(true);
        }

        var movePosition = (Vector3.up * 2);
        while (text.transform.position.y < limitDist)
        {
            text.transform.position += movePosition;
            yield return new WaitForFixedUpdate();
        }

        Destroy(text.gameObject);
        yield return new WaitForFixedUpdate();

        if (AlertBoxTexts[AlertBoxTexts.Count - 1] == null)
            AlertBoxTexts.Clear();
    }
}
