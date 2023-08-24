using System;
using UnityEngine;

public class Display : MonoBehaviour
{
    public TextMesh DisplayText;
    public static float cap = 0.006556245f;

    public void AddChar(char c)
    {
        DisplayText.text += c;
        ResizeText();
    }

    public void SetText(string text)
    {
        DisplayText.text = text;
        if(text != "")
            ResizeText();
    }

    public void ClearText()
    {
        DisplayText.text = "";
    }

    public string GetFormattedText()
    {
        return DisplayText.text.ToLowerInvariant().Replace("_", " ");
    }

    private void ResizeText()
    {
        int multiplier = 1;
        if (BaseOffScript.ScuffednessLevel == BaseOffScript.Scuffedness.None)
            DisplayText.text = GetFormattedText();
        else if (BaseOffScript.ScuffednessLevel == BaseOffScript.Scuffedness.Extra)
        {
            cap = float.MaxValue;
            multiplier = -1;
        }

        Font font = DisplayText.font;
        CharacterInfo characterInfo = new CharacterInfo();
        int totalLength = 0;

        foreach (char x in DisplayText.text)
        {
            font.GetCharacterInfo(x, out characterInfo, DisplayText.fontSize);
            totalLength += characterInfo.advance;
        }


        DisplayText.transform.localScale = new Vector3(Math.Min(cap, 9.5f / totalLength), Math.Abs(DisplayText.transform.localScale.y) * multiplier, DisplayText.transform.localScale.z);
    }
}
