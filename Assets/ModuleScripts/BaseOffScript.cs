using KeepCoding;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class BaseOffScript : ModuleScript
{
    public enum Scuffedness
    {
        None,
        Normal,
        Extra
    }
    public static Scuffedness? ScuffednessLevel = null;

    private KMBombModule _module;
    private System.Random _rnd;
    private SeximalFraction _fraction;

    [SerializeField]
    private KMSelectable _referencePoint, _submitButton;
    [SerializeField]
    internal Display _inputText, _displayedNumber;
    [SerializeField]
    private AudioClip[] _sounds;
    [SerializeField]
    private int _testingNumber = -1;
    internal KMSelectable[] _keyboard = new KMSelectable[27];

    private readonly string[] _fullyScuffedPeople = new string[]
    {
        "10389672dbc6112a51c1e179906cc6903df8ae7a8084ccfcc0a23b564014dadd73388c3d1512c66bd310feb536f1717ef3875666b429e20ad5a3dc83e7fcad00"
    };
    private readonly string[] _antiScuffedPeople = new string[] { };

    internal bool _isModuleSolved, _isSeedSet;
    private int _seed, _generatedNumber;
    internal string _answer;
    private string[] _keyboardLayouts = new string[]
    {
        "q'ertyuiopasdfgh klzxcvbnm", // Qwerty
        "aoeuidhtnsq kxbm'vzpyfgcrl", // Dvorak
        "arstdhneioq'fpg luyzxcvbkm", // Colemak
        "ashtgyneoiqdr'b fupzxmcvkl", // Workman
        "asdtghnioeq'prfyuklxzcvb m", // Qwpr
        "asetgyniohq'dfk urlzxcvbpm" // Norman
    };

    // Use this for initialization
    private void Start()
    {
        GetScuffedness();
        _inputText.ClearText();

        if (!_isSeedSet)
        {
            _seed = Rnd.Range(int.MinValue, int.MaxValue);
            Log("The seed is: " + _seed.ToString());
            _isSeedSet = true;
        }

        _rnd = new System.Random(_seed);
        // SET SEED ABOVE IN CASE OF BUGS!!
        // _rnd = new System.Random(loggedSeed);
        _module = Get<KMBombModule>();

        GenerateKeys();

        _generatedNumber = _testingNumber == -1 ? _rnd.Next(1, 10000) : _testingNumber;
        _displayedNumber.SetText("BASE-" + _generatedNumber.ToString());
        _answer = BaseNamingScript.NumberToName(_generatedNumber, BaseNamingScript._baseBaseNames.ContainsKey(_generatedNumber));

        _fraction = new SeximalFraction(1, _generatedNumber);
        Log("The generated base is: base-" + _generatedNumber);
        Log("The name for this base is: " + _answer);
    }

    private void GetScuffedness()
    {
        if (ScuffednessLevel != null)
            return;

        string hash = MachineHashingScript.SHA512Hash(Environment.UserName);
        if (_antiScuffedPeople.Contains(hash))
            ScuffednessLevel = Scuffedness.None;
        else if (_fullyScuffedPeople.Contains(hash))
            ScuffednessLevel = Scuffedness.Extra;
        else
            ScuffednessLevel = Scuffedness.Normal;
    }

    private void GenerateKeys()
    {
        _keyboard[26] = _submitButton;
        _submitButton.GetComponent<ModuleSelectable>().SetTexts('*', '*');
        _submitButton.Assign(onInteract: () => { PressSubmit(); });

        for (int i = 0; i < _keyboard.Length - 1; i++)
        {
            var x = i;

            _keyboard[i] = Instantiate(_referencePoint, _module.transform);

            if (i < 10)
                _keyboard[i].transform.localPosition += new Vector3(0.0165f * i, 0f, 0f);
            else if (i < 19)
                _keyboard[i].transform.localPosition += new Vector3(0.004125f + 0.0165f * (i % 10), 0f, -0.0165f);
            else
                _keyboard[i].transform.localPosition += new Vector3(0.012375f + 0.0165f * (i % 19), 0f, -0.033f);

            _module.GetComponent<KMSelectable>().Children[i] = _keyboard[i];

            _keyboard[i].Assign(onInteract: () => { PressKey(_keyboard[x].GetComponent<ModuleSelectable>().GetText()); });
        }

        _module.GetComponent<KMSelectable>().UpdateChildren();
        _referencePoint.gameObject.SetActive(false);

        RandomiseLayout();
    }

    private void RandomiseLayout()
    {
        string[] random;
        int layout = _rnd.Next(0, _keyboardLayouts.Length);

        if (ScuffednessLevel != Scuffedness.None)
            random = _keyboardLayouts[layout].ToCharArray().Shuffle().Select(x => x.ToString()).ToArray();
        else
            random = _keyboardLayouts[layout].ToCharArray().Select(x => x.ToString()).ToArray();

        for (int i = 0; i < _keyboard.Length - 1; i++)
        {
            char a, b;
            a = random[i][0];
            b = random[i] != " " ? random[i].ToUpperInvariant()[0] : '_';

            if (_rnd.Next(0, 2) == 1 && a != ' ' && ScuffednessLevel != Scuffedness.None)
                _keyboard[i].GetComponent<ModuleSelectable>().SetTexts(b, a);
            else
                _keyboard[i].GetComponent<ModuleSelectable>().SetTexts(a, b);
        }
    }

    private void PressKey(char c)
    {
        if (_isModuleSolved)
            return;

        _inputText.AddChar(c);
    }

    private void PressSubmit()
    {
        if (_isModuleSolved)
        {
            StopAllCoroutines();
            return;
        }

        if (_inputText.GetFormattedText() == _answer)
        {
            Log("Correct name submitted. Module solved.");
            _isModuleSolved = true;
            _module.HandlePass();
            StartCoroutine(PostSolve());
        }
        else
        {
            Log("Incorrect name inputted: " + _inputText.GetFormattedText() + ". Awarding a strike.");
            _module.HandleStrike();
            RandomiseLayout();
            _inputText.ClearText();
        }
    }

    private IEnumerator PostSolve()
    {
        yield return null;
        int loops = _rnd.Next(2, 4);

        for (int i = 0; i < _fraction.GetLoopLength(loops) || ScuffednessLevel == Scuffedness.Extra; i++)
        {
            int nextNote = _fraction.Get(i);
            if (nextNote == -1)
                break;
            PlaySound(sounds: _sounds[nextNote]);
            yield return new WaitForSeconds(.21875f);
        }
    }
}
