using KeepCoding;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ABaseNeutralSystemForNamingNumberingSystemsTPScript : TPScript<ABaseNeutralSystemForNamingNumberingSystemsScript>
{
#pragma warning disable 414
    private string TwitchHelpMessage = "'!{0} <[a-z]*>' to press the location of the keys on the module on a QWERTY keyboard and submit the answer. '!{0} mute' before submitting (in a separate command) if you do not want to have the solve sound playing after solve.";
#pragma warning restore 414

    private bool _isMute = false;
    private string _qwertyLayout = "qwertyuiopasdfghjklzxcvbnm";

    public override IEnumerator ForceSolve()
    {
        yield return null;
        Module._inputText.ClearText();

        for (int i = 0; i < Module._answer.Length; i++)
        {
            for(int j = 0; j < _qwertyLayout.Length; j++)
            {
                if (Module._keyboard[j].GetComponent<Selectable>().GetText().ToString().ToLowerInvariant().Replace("_", " ") == Module._answer[i].ToString())
                    Module._keyboard[j].OnInteract();
            }
            yield return new WaitForSeconds(.1f);
        }
        yield return null;
        Module._keyboard[26].OnInteract();
        Module._keyboard[26].OnInteract();
    }

    public override IEnumerator Process(string command)
    {
        if (command.ToLowerInvariant() == "mute")
            _isMute = true;

        char[] split = command.ToLowerInvariant().ToCharArray();

        if (split.Any(x => !_qwertyLayout.Contains(x)))
        {
            yield return "sendtochaterror Key not found in QWERTY keyboard detected. Cancelling command.";
            yield break;
        }

        yield return null;
        for (int i = 0; i < split.Length; i++)
        {
            Module._keyboard[_qwertyLayout.IndexOf(split[i])].OnInteract();
            yield return new WaitForSeconds(.1f);
        }
        Module._keyboard[26].OnInteract();

        yield return null;
        if(Module._isModuleSolved && _isMute)
            Module._keyboard[26].OnInteract();
    }
}
