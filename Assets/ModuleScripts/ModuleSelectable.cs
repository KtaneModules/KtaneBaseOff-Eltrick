using KeepCoding;
using System;
using UnityEngine;

public class ModuleSelectable : MonoBehaviour
{
    public KMSelectable Button { get; private set; }
    public TextMesh ButtonText;

    private char[] _possibleTexts;

    public void SetTexts(char x, char y)
    {
        _possibleTexts = new char[2];
        _possibleTexts[0] = x;
        _possibleTexts[1] = y;
        ButtonText.text = x.ToString();

        if (BaseOffScript.ScuffednessLevel == BaseOffScript.Scuffedness.Extra)
            ButtonText.transform.localScale = new Vector3(ButtonText.transform.localScale.x, -1 * Math.Abs(ButtonText.transform.localScale.y), ButtonText.transform.localScale.z);
    }

    private void Awake()
    {
        Button = GetComponent<KMSelectable>();
        Button.Assign(onHighlight: () =>
        {
            ButtonText.text = _possibleTexts[1].ToString();
            ButtonText.color = new Color(0.25f, 0.75f, 1f, 1);
        });
        Button.Assign(onHighlightEnded: () =>
        {
            ButtonText.text = _possibleTexts[0].ToString();
            ButtonText.color = new Color(1, 1, 1, 1);
        });
    }

    public char GetText()
    {
        return _possibleTexts[1];
    }
}