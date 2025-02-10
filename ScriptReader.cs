using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;
using Unity.VisualScripting;

public class ScriptReader : MonoBehaviour
{
    [SerializeField]
    private TextAsset inkJsonFile, badMSV, badQLV, badESV, PUVPlus, SUVPlus;
    private Story storyScript;
    public TMP_Text dialogueBox;
    public TMP_Text nameBox;
    public Image characterSprite;
    public GameObject currentBackground;
    public GameObject currentAudio;
    private bool hasEnded = false;
    private bool hasPlayed = false;
    public GameObject restartButton;
    public int PUV, ESV, QLV, MSV, SUV;
    [SerializeField]
    private VerticalLayoutGroup buttonBox;
    [SerializeField]
    private Button buttonPrefab;

    void Start()
    {
        hasPlayed = false;
        LoadStory();
        restartButton.SetActive(false);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            DisplayNextLine();
        }
    }

    void LoadStory()
    {
        if(hasPlayed == false)
        {
            storyScript = new Story(inkJsonFile.text);
        }
        else{
        if(Parameter.Instance.MSSlider.value <= 20)
            {
                storyScript = new Story(badMSV.text);
            }
        else if(Parameter.Instance.ESSlider.value <= 20)
            {
                storyScript = new Story(badESV.text);
            }
        else if(Parameter.Instance.QLSlider.value <= 20)
            {
                storyScript = new Story(badQLV.text);
            }            
        else if(Parameter.Instance.PUSlider.value > Parameter.Instance.SUSlider.value)
            {
                storyScript = new Story(PUVPlus.text);
            }
        else
            {
                storyScript = new Story(SUVPlus.text);
            }
        }
        hasPlayed = true;
        storyScript.BindExternalFunction("Name", (string charName) => ChangeName(charName));
        storyScript.BindExternalFunction("Image", (string charName) => CharacterImage(charName));
        storyScript.BindExternalFunction("Param", (string param, int value) => ParamValueChange(param, value));
        storyScript.BindExternalFunction("Back", (string backName) => ChangeBackground(backName));
        storyScript.BindExternalFunction("Ended", () => FlipBool());
        storyScript.BindExternalFunction("Audio", (string audioName) => PlayAudio(audioName));
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (storyScript.canContinue)
        {
            string text = storyScript.Continue();
            text = text?.Trim();
            dialogueBox.text = text;
        }
        else if (storyScript.currentChoices.Count > 0)
        {
            DisplayChoices();
        }
        else if (hasEnded == false)
        {
            LoadStory();
            hasEnded = true;
        }
        else
        {
            dialogueBox.text = "You have reached one of the many ends, thank you for playing. Feel free to try again to reach new endings. Utopia depends on you.";
            restartButton.SetActive(true);
        }
    }

    private void DisplayChoices()
    {
        if(buttonBox.GetComponentsInChildren<Button>().Length > 0) return;
        for (int i = 0; i < storyScript.currentChoices.Count; i++)
        {
            var choice = storyScript.currentChoices[i];
            var button = CreateChoiceButton(choice.text);
            button.onClick.AddListener(() => OnClickChoiceButton(choice));
        }
    }

    Button CreateChoiceButton(string choice)
    {
        var choiceButton = Instantiate(buttonPrefab);
        choiceButton.transform.SetParent(buttonBox.transform, false);
        var buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = choice;
        return choiceButton;
    }

    void OnClickChoiceButton(Choice choice)
    {
        storyScript.ChooseChoiceIndex(choice.index);
        RefreshChoiceView();
        DisplayNextLine();
        DisplayNextLine();
    }

    void RefreshChoiceView()
    {
        if(buttonBox != null)
        {
            foreach(var button in buttonBox.GetComponentsInChildren<Button>())
            {
                Destroy(button.gameObject);
            }
        }
    }

    public void ChangeName(string name)
    {
        string speakerName = name;
        nameBox.text = speakerName;
    }

    public void CharacterImage(string name)
    {
        if(name != "blank")
        {
            ShowPortrait();
            var charImage = Resources.Load<Sprite>("Sprites/"+name);
            characterSprite.sprite = charImage;
        }
        else HidePortrait();
    }

    public void HidePortrait()
    {
        characterSprite.gameObject.SetActive(false);
    }

    public void ShowPortrait()
    {
        characterSprite.gameObject.SetActive(true);
    }

    public void ParamValueChange(string param, int value)
    {
        if (param == "PUV" && value != 0)
        {
            PUV = value;
            Parameter.Instance.SetPUValue(PUV);
        }
        if (param == "ESV" && value != 0)
        {
            ESV = value;
            Parameter.Instance.SetESValue(ESV);
        }
        if (param == "QLV" && value != 0)
        {
            QLV = value;
            Parameter.Instance.SetQLValue(QLV);
        }
        if (param == "MSV" && value != 0)
        {
            MSV = value;
            Parameter.Instance.SetMSValue(MSV);
        }    
        Parameter.Instance.SetSUValue();
     }

     public void ChangeBackground(string title)
     {
        Destroy(currentBackground);
        var backImage = Resources.Load<GameObject>("Backgrounds/"+title);
        currentBackground = Instantiate(backImage);
     }

     public void FlipBool()
     {
        hasEnded = true;
     }

     public void PlayAudio(string sound)
     {
        Destroy(currentAudio);
        var playSound = Resources.Load<GameObject>("Audio/"+sound);
        currentAudio = Instantiate(playSound);
     }
}
