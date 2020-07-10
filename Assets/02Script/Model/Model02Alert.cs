using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public partial class Model : MonoBehaviour
{
    public GameObject AlertBox { private set; get; }
    public Vector3 AlertBoxStartPoint { get { return Camera.main.WorldToScreenPoint(transform.position + (Vector3.up * 2)); } }
    void AwakeInAlert()
    {
        AlertBox = Resources.Load<GameObject>("AlretText");
    }
    public void ShowAlert(string text, Color color)
    {
        Text alertText = Instantiate(AlertBox, AlertBoxStartPoint, Quaternion.identity, StaticManager.canvasTrasform).GetComponent<Text>();
        alertText.text = text;
        alertText.color = color;
        StartCoroutine(StartAlertTextMove(alertText));
    }
    IEnumerator StartAlertTextMove(Text text)
    {
        var limitDist = text.transform.position.y + 200f;
        var movePosition = (Vector3.up * 2);
        while (text.transform.position.y < limitDist)
        {
            text.transform.position += movePosition;
            yield return new WaitForFixedUpdate();
        }

        Destroy(text.gameObject);
    }
}
