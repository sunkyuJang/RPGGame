using UnityEngine;
using UnityEngine.UI;
using GLip;

public class SimpleDiscriptionBox : MonoBehaviour
{
    public static SimpleDiscriptionBox instance { set; get; }
    public GameObject frame;
    public GameObject imageFrame;

    public Image icon;
    public GameObject emptyForSpace;
    public Button btn;
    public Text btnText;
    public Text descriptionText;
    public bool isBtnPressed = false;
    public Vector2 StartPosition { set; get; }
    public enum Form { Text, withImage, withBtn }

    public void Awake()
    {
        StartPosition = frame.transform.position;
    }
    public void ShowDescription(string text) => ShowDiscriptionBox(Form.Text, null, null, text);
    public void ShowDescriptionWithImage(string text, Image icon) => ShowDiscriptionBox(Form.Text, icon, null, text);
    public void ShowDescriptionWithImageAndBtn(string text, Image icon, string btnText) => ShowDiscriptionBox(Form.Text, icon, btnText, text);
    void ShowDiscriptionBox(Form form, Image icon, string btnText, string text)
    {
        this.icon = icon;
        descriptionText.text = text;

        frame.transform.position = GPosition.GetInputPosition().x < Screen.width * 0.5f ? StartPosition : new Vector2(StartPosition.x * -1, StartPosition.y);

        switch (form)
        {
            case Form.Text:
                imageFrame.SetActive(false);
                break;
            case Form.withImage:
                imageFrame.SetActive(true);
                icon.gameObject.SetActive(true);
                emptyForSpace.SetActive(true);
                break;
            case Form.withBtn:
                imageFrame.SetActive(true);
                icon.gameObject.SetActive(true);
                btn.gameObject.SetActive(true);
                this.btnText.text = btnText;
                break;
        }
    }

    public void HideDiscriptionBox()
    {
        emptyForSpace.SetActive(false);
        btn.gameObject.SetActive(false);
        frame.SetActive(false);
    }

    public void SetIsBtnPressed() => isBtnPressed = true;
}
