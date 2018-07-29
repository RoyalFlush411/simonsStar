using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class simonsStarScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;

    //buttons
    public starButtons[] buttons;
    public List<int> selectedColours = new List<int>();
    public List<String> flashedColours = new List<string>();
    public Color[] colourOptions;
    public string[] colourNames;
    public MeshRenderer[] actualButtonRend;
    public AudioClip[] flashNotes;
    string pressedButtonName;

    //levelIndicator
    public Renderer[] levelIndicator;
    public Light[] indicatorLight;
    private bool moduleSolved = false;

    //number
    public TextMesh numberDisplay;
    public int[] selectedNumber;
    private int cyclingNumber = 0;
    private int stage = 0;
    private int substage = 0;
    private int cycle = 0;
    private bool numbersSelected = false;
    private bool solving = false;
    private bool striking = false;

    //coroutines
    Coroutine flashRoutine;

    //database
    public string[] locationOptions;

    //firstFlashInfo
    private int firstFlashIndex = 0;
    public string firstFlashLocation;
    public string firstFlashColour;
    private bool stageOnePass = false;
    private starButtons firstCorrectButton;
    private int basePositionIndex;

    //secondFlashInfo
    private int secondFlashIndex = 0;
    public string secondFlashLocation;
    public string secondFlashColour;
    private bool stageTwoPass = false;
    private starButtons secondCorrectButton;

    //thirdFlashInfo
    private int thirdFlashIndex = 0;
    public string thirdFlashLocation;
    public string thirdFlashColour;
    private bool stageThreePass = false;
    private starButtons thirdCorrectButton;

    //fourthFlashInfo
    private int fourthFlashIndex = 0;
    public string fourthFlashLocation;
    public string fourthFlashColour;
    private bool stageFourPass = false;
    private starButtons fourthCorrectButton;
    private starButtons fourthHolderButton1;
    private starButtons fourthHolderButton2;
    private starButtons fourthHolderButton3;
    private starButtons fourthHolderButton4;

    //fifthFlashInfo
    private int fifthFlashIndex = 0;
    public string fifthFlashLocation;
    public string fifthFlashColour;
    private bool stageFivePass = false;
    private starButtons fifthCorrectButton;
    private starButtons fifthHolderButton4;
    private starButtons fifthHolderButton5;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (starButtons selectable in buttons)
        {
            starButtons pressedButton = selectable;
            selectable.selectable.OnInteract += delegate () { buttonPress(pressedButton); return false; };
        }
    }

    void Start()
    {
        foreach(Light indicLight in indicatorLight)
        {
            indicLight.enabled = false;
        }
        foreach(starButtons spot in buttons)
        {
            spot.buttonLight.enabled = false;
        }
        pickInitialColours();
        StartCoroutine(generateNumber());
        pickFlashes();
        flashRoutine = StartCoroutine(flashes());
        stageOneLogic();
        stageTwoLogic();
        stageThreeLogic();
        stageFourLogic();
        stageFiveLogic();
        Debug.LogFormat("[Simon's Star #{0}] The colours (clockwise from north) are: {1}, {2}, {3}, {4} and {5}.", moduleId, buttons[0].colourName, buttons[1].colourName, buttons[2].colourName, buttons[3].colourName, buttons[4].colourName);
        Debug.LogFormat("[Simon's Star #{0}] The flashes are: {1}, {2}, {3}, {4}, {5}.", moduleId, firstFlashColour, secondFlashColour, thirdFlashColour, fourthFlashColour, fifthFlashColour);
        Debug.LogFormat("[Simon's Star #{0}] The central digits are: {1}, {2}, {3}, {4}, {5}.", moduleId, selectedNumber[0], selectedNumber[1], selectedNumber[2], selectedNumber[3], selectedNumber[4]);
        Debug.LogFormat("[Simon's Star #{0}] The button presses are: {1}, {2}, {3}, {4}, {5}.", moduleId, firstCorrectButton.colourName, secondCorrectButton.colourName, thirdCorrectButton.colourName, fourthCorrectButton.colourName, fifthCorrectButton.colourName);
    }

    void pickInitialColours()
    {
        foreach(starButtons buttonMat in buttons)
        {
            int matIndex = UnityEngine.Random.Range(0,5);
            while(selectedColours.Contains(matIndex))
            {
                matIndex = UnityEngine.Random.Range(0,5);
            }
            selectedColours.Add(matIndex);
            buttonMat.buttonColour.color = colourOptions[matIndex];
            buttonMat.buttonColourIndex = matIndex;
            buttonMat.colourName = colourNames[matIndex];
        }
        int matchIndex = 0;
        foreach(MeshRenderer buttonMat in actualButtonRend)
        {
            buttonMat.material.color = buttons[matchIndex].buttonColour.color;
            matchIndex++;
        }
    }

    IEnumerator generateNumber()
    {
        if(numbersSelected == false)
        {
            int selectedDigitA = UnityEngine.Random.Range(1,5);
            selectedNumber[0] = selectedDigitA;
            int selectedDigitB = UnityEngine.Random.Range(1,5);
            selectedNumber[1] = selectedDigitB;
            int selectedDigitC = UnityEngine.Random.Range(1,5);
            selectedNumber[2] = selectedDigitC;
            int selectedDigitD = UnityEngine.Random.Range(1,5);
            selectedNumber[3] = selectedDigitD;
            int selectedDigitE = UnityEngine.Random.Range(1,5);
            selectedNumber[4] = selectedDigitE;
            numbersSelected = true;
        }

        while(cycle < 15)
        {
            yield return new WaitForSeconds(0.1f);
            cyclingNumber = UnityEngine.Random.Range(0,5);
            numberDisplay.text = cyclingNumber.ToString();
            cycle++;
        }
        cycle = 0;
        if(stage < 5)
        {
            numberDisplay.text = selectedNumber[stage].ToString();
        }
        else
        {
            numberDisplay.text = "0";
        }
    }

    void pickFlashes()
    {
        firstFlashIndex = UnityEngine.Random.Range(0,5);
        firstFlashLocation = locationOptions[firstFlashIndex];
        firstFlashColour = buttons[firstFlashIndex].colourName;
        secondFlashIndex = UnityEngine.Random.Range(0,5);
        secondFlashLocation = locationOptions[secondFlashIndex];
        secondFlashColour = buttons[secondFlashIndex].colourName;
        thirdFlashIndex = UnityEngine.Random.Range(0,5);
        thirdFlashLocation = locationOptions[thirdFlashIndex];
        thirdFlashColour = buttons[thirdFlashIndex].colourName;
        fourthFlashIndex = UnityEngine.Random.Range(0,5);
        fourthFlashLocation = locationOptions[fourthFlashIndex];
        fourthFlashColour = buttons[fourthFlashIndex].colourName;
        fifthFlashIndex = UnityEngine.Random.Range(0,5);
        fifthFlashLocation = locationOptions[fifthFlashIndex];
        fifthFlashColour = buttons[fifthFlashIndex].colourName;
        flashedColours.Add(firstFlashColour);
        flashedColours.Add(secondFlashColour);
        flashedColours.Add(thirdFlashColour);
        flashedColours.Add(fourthFlashColour);
        flashedColours.Add(fifthFlashColour);
    }

    IEnumerator flashes()
    {
        foreach(starButtons flash in buttons)
        {
            flash.buttonLight.enabled = false;
        }
        yield return new WaitForSeconds(2f);
        while(stageOnePass == false)
        {
            buttons[firstFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[firstFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(4f);
        }
        while(stageTwoPass == false)
        {
            Audio.PlaySoundAtTransform(flashNotes[firstFlashIndex].name, transform);
            buttons[firstFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[firstFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[secondFlashIndex].name, transform);
            buttons[secondFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[secondFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(4f);
        }
        while(stageThreePass == false)
        {
          Audio.PlaySoundAtTransform(flashNotes[firstFlashIndex].name, transform);
            buttons[firstFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[firstFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[secondFlashIndex].name, transform);
            buttons[secondFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[secondFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[thirdFlashIndex].name, transform);
            buttons[thirdFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[thirdFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(4f);
        }
        while(stageFourPass == false)
        {
            Audio.PlaySoundAtTransform(flashNotes[firstFlashIndex].name, transform);
            buttons[firstFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[firstFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[secondFlashIndex].name, transform);
            buttons[secondFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[secondFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[thirdFlashIndex].name, transform);
            buttons[thirdFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[thirdFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[fourthFlashIndex].name, transform);
            buttons[fourthFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[fourthFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(4f);
        }
        while(stageFivePass == false)
        {
            Audio.PlaySoundAtTransform(flashNotes[firstFlashIndex].name, transform);
            buttons[firstFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[firstFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[secondFlashIndex].name, transform);
            buttons[secondFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[secondFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[thirdFlashIndex].name, transform);
            buttons[thirdFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[thirdFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[fourthFlashIndex].name, transform);
            buttons[fourthFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[fourthFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(0.4f);
            Audio.PlaySoundAtTransform(flashNotes[fifthFlashIndex].name, transform);
            buttons[fifthFlashIndex].buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            buttons[fifthFlashIndex].buttonLight.enabled = false;
            yield return new WaitForSeconds(4f);
        }
    }

    void stageOneLogic()
    {
        if(firstFlashColour == "red")
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "green")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            firstCorrectButton = buttons[(basePositionIndex + 2) % 5];
        }
        else if (firstFlashColour == "blue")
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "yellow")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            firstCorrectButton = buttons[(basePositionIndex + (5 - selectedNumber[0])) % 5];
        }
        else if (firstFlashColour == "yellow")
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "purple")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            firstCorrectButton = buttons[(basePositionIndex + selectedNumber[0]) % 5];
        }
        else if (firstFlashColour == "green")
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "red")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            firstCorrectButton = buttons[(basePositionIndex + 4) % 5];
        }
        else
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "blue")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            firstCorrectButton = buttons[(basePositionIndex + 3) % 5];
        }
    }

    void stageTwoLogic()
    {
        if(secondFlashColour == "green" && firstFlashColour != "purple" && firstFlashColour != "red")
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "blue")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            secondCorrectButton = buttons[(basePositionIndex + (5 - selectedNumber[1])) % 5];
        }
        else if(secondFlashColour == "red" && firstCorrectButton.buttonColourIndex != 1 && firstCorrectButton.buttonColourIndex != 0)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "yellow")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            secondCorrectButton = buttons[(basePositionIndex + 3) % 5];
        }
        else if(secondFlashColour == "blue" && (firstCorrectButton.buttonColourIndex == 2 || firstCorrectButton.buttonColourIndex == 4))
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "green")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            secondCorrectButton = buttons[basePositionIndex];
        }
        else if(secondFlashColour == "yellow" && firstFlashColour != "red")
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "red")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            secondCorrectButton = buttons[(basePositionIndex + 3) % 5];
        }
        else if(secondFlashColour == "purple" && (firstFlashColour == "red" || firstFlashColour == "green"))
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "purple")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            secondCorrectButton = buttons[(basePositionIndex + selectedNumber[1]) % 5];
        }
        else
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == firstFlashColour)
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            secondCorrectButton = buttons[basePositionIndex];
        }
    }

    void stageThreeLogic()
    {
        if(firstFlashColour != secondFlashColour && firstFlashColour != thirdFlashColour && secondFlashColour != thirdFlashColour)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "yellow")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            thirdCorrectButton = buttons[(basePositionIndex + (5 - selectedNumber[2])) % 5];
        }
        else if(firstCorrectButton.buttonColourIndex != secondCorrectButton.buttonColourIndex)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "blue")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            thirdCorrectButton = buttons[(basePositionIndex + 2) % 5];
        }
        else if(firstCorrectButton.buttonColourIndex != 1 && firstCorrectButton.buttonColourIndex != 2 && secondCorrectButton.buttonColourIndex != 1 && secondCorrectButton.buttonColourIndex != 2)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "red")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            thirdCorrectButton = buttons[(basePositionIndex + selectedNumber[2]) % 5];
        }
        else if(thirdFlashColour == "blue" || thirdFlashColour == "red")
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "purple")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            thirdCorrectButton = buttons[(basePositionIndex + 4) % 5];
        }
        else
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == firstCorrectButton.colourName)
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            thirdCorrectButton = buttons[basePositionIndex];
        }
    }

    void stageFourLogic()
    {
        foreach(starButtons button in buttons)
        {
            if(button.colourName == thirdFlashColour)
            {
                basePositionIndex = button.buttonPosition;
            }
        }
        fourthHolderButton1 = buttons[(basePositionIndex + selectedNumber[3]) % 5];

        foreach(starButtons button in buttons)
        {
            if(button.colourName == fourthFlashColour)
            {
                basePositionIndex = button.buttonPosition;
            }
        }
        fourthHolderButton2 = buttons[(basePositionIndex + 3) % 5];

        foreach(starButtons button in buttons)
        {
            if(button.colourName == secondCorrectButton.colourName)
            {
                basePositionIndex = button.buttonPosition;
            }
        }
        fourthHolderButton3 = buttons[(basePositionIndex + (5 - selectedNumber[3])) % 5];

        foreach(starButtons button in buttons)
        {
            if(button.colourName == firstCorrectButton.colourName)
            {
                basePositionIndex = button.buttonPosition;
            }
        }
        fourthHolderButton4 = buttons[(basePositionIndex + 2) % 5];



        if(fourthHolderButton1.buttonColourIndex == firstCorrectButton.buttonColourIndex || fourthHolderButton1.buttonColourIndex == secondCorrectButton.buttonColourIndex || fourthHolderButton1.buttonColourIndex == thirdCorrectButton.buttonColourIndex)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == secondCorrectButton.colourName)
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fourthCorrectButton = buttons[basePositionIndex];
        }

        else if(fourthHolderButton2.buttonColourIndex != firstCorrectButton.buttonColourIndex && fourthHolderButton2.buttonColourIndex != secondCorrectButton.buttonColourIndex && fourthHolderButton2.buttonColourIndex != thirdCorrectButton.buttonColourIndex)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == thirdFlashColour)
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fourthCorrectButton = buttons[(basePositionIndex + (5 - selectedNumber[3])) % 5];
        }

        else if(fourthHolderButton3.colourName == firstFlashColour || fourthHolderButton3.colourName == secondFlashColour || fourthHolderButton3.colourName == thirdFlashColour || fourthHolderButton3.colourName == fourthFlashColour)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == firstFlashColour)
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fourthCorrectButton = buttons[basePositionIndex];
        }

        else if(fourthHolderButton4.buttonColourIndex != firstCorrectButton.buttonColourIndex && fourthHolderButton4.buttonColourIndex != secondCorrectButton.buttonColourIndex && fourthHolderButton4.buttonColourIndex != thirdCorrectButton.buttonColourIndex)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == thirdCorrectButton.colourName)
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fourthCorrectButton = buttons[(basePositionIndex + selectedNumber[3]) % 5];
        }

        else
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == secondFlashColour)
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fourthCorrectButton = buttons[basePositionIndex];
        }
    }

    void stageFiveLogic()
    {
        foreach(starButtons button in buttons)
        {
            if(button.colourName == "red")
            {
                basePositionIndex = button.buttonPosition;
            }
        }
      fifthHolderButton4 = buttons[(basePositionIndex + (5 - selectedNumber[4])) % 5];

      foreach(starButtons button in buttons)
      {
          if(button.colourName == "blue")
          {
              basePositionIndex = button.buttonPosition;
          }
      }
    fifthHolderButton5 = buttons[(basePositionIndex + selectedNumber[4]) % 5];

        if(flashedColours.Contains("red") && flashedColours.Contains("green") && flashedColours.Contains("blue") && flashedColours.Contains("yellow") && flashedColours.Contains("purple"))
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "green")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fifthCorrectButton = buttons[basePositionIndex];
        }
        else if (firstCorrectButton.buttonColourIndex != 2 && secondCorrectButton.buttonColourIndex != 2 && thirdCorrectButton.buttonColourIndex != 2 && fourthCorrectButton.buttonColourIndex != 2)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "red")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fifthCorrectButton = buttons[basePositionIndex];
        }
        else if (firstFlashColour != "yellow" && secondFlashColour != "yellow" && thirdFlashColour != "yellow" && fourthFlashColour != "yellow" && fifthFlashColour != "yellow")
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "blue")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fifthCorrectButton = buttons[basePositionIndex];
        }
        else if (fifthHolderButton4.colourName != firstCorrectButton.colourName && fifthHolderButton4.colourName != secondCorrectButton.colourName && fifthHolderButton4.colourName != thirdCorrectButton.colourName && fifthHolderButton4.colourName != fourthCorrectButton.colourName)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "purple")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fifthCorrectButton = buttons[basePositionIndex];
        }
        else if (fifthHolderButton5.colourName != firstFlashColour && fifthHolderButton5.colourName != secondFlashColour && fifthHolderButton5.colourName != thirdFlashColour && fifthHolderButton5.colourName != fourthFlashColour && fifthHolderButton5.colourName != fifthFlashColour)
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "yellow")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fifthCorrectButton = buttons[basePositionIndex];
        }
        else if ((firstFlashColour == "green" || secondFlashColour == "green" || thirdFlashColour == "green" || fourthFlashColour == "green" || fifthFlashColour == "green") && (firstCorrectButton.colourName == "green" || secondCorrectButton.colourName == "green" || thirdCorrectButton.colourName == "green" || fourthCorrectButton.colourName == "green"))
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "red")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fifthCorrectButton = buttons[(basePositionIndex + selectedNumber[4]) % 5];
        }
        else
        {
            foreach(starButtons button in buttons)
            {
                if(button.colourName == "blue")
                {
                    basePositionIndex = button.buttonPosition;
                }
            }
            fifthCorrectButton = buttons[(basePositionIndex + (5 - selectedNumber[4])) % 5];
        }
    }

    void stageOnePassMeth()
    {
        StartCoroutine(indicLightFlicker());
        stageOnePass = true;
        Debug.LogFormat("[Simon's Star #{0}] You pressed {1}. That is correct.", moduleId, firstCorrectButton.colourName);
        stage++;
        StartCoroutine(generateNumber());
        flashRoutine = StartCoroutine(flashes());
        solving = false;
    }

    void stageTwoPassMeth()
    {
        StartCoroutine(indicLightFlicker());
        stageTwoPass = true;
        Debug.LogFormat("[Simon's Star #{0}] You pressed {1}. That is correct.", moduleId, secondCorrectButton.colourName);
        stage++;
        StartCoroutine(generateNumber());
        flashRoutine = StartCoroutine(flashes());
        substage = 0;
        solving = false;
    }

    void stageThreePassMeth()
    {
        StartCoroutine(indicLightFlicker());
        stageThreePass = true;
        Debug.LogFormat("[Simon's Star #{0}] You pressed {1}. That is correct.", moduleId, thirdCorrectButton.colourName);
        stage++;
        StartCoroutine(generateNumber());
        flashRoutine = StartCoroutine(flashes());
        substage = 0;
        solving = false;
    }

    void stageFourPassMeth()
    {
        StartCoroutine(indicLightFlicker());
        stageFourPass = true;
        Debug.LogFormat("[Simon's Star #{0}] You pressed {1}. That is correct.", moduleId, fourthCorrectButton.colourName);
        stage++;
        StartCoroutine(generateNumber());
        flashRoutine = StartCoroutine(flashes());
        substage = 0;
        solving = false;
    }

    void stageFivePassMeth()
    {
        StartCoroutine(indicLightFlicker());
        stageFivePass = true;
        Debug.LogFormat("[Simon's Star #{0}] You pressed {1}. That is correct. Module disarmed.", moduleId, fifthCorrectButton.colourName);
        stage++;
        StartCoroutine(generateNumber());
        StartCoroutine(solvedRoutine());
        substage = 0;
        solving = false;
        moduleSolved = true;
        GetComponent<KMBombModule>().HandlePass();
    }

    void stageOneStrike()
    {
        Debug.LogFormat("[Simon's Star #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedButtonName);
        GetComponent<KMBombModule>().HandleStrike();
        StartCoroutine(strikeOneRoutine());
    }

    void stageTwoStrike()
    {
        Debug.LogFormat("[Simon's Star #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedButtonName);
        substage = 0;
        GetComponent<KMBombModule>().HandleStrike();
        StartCoroutine(strikeTwoRoutine());
    }

    void stageThreeStrike()
    {
        Debug.LogFormat("[Simon's Star #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedButtonName);
        substage = 0;
        GetComponent<KMBombModule>().HandleStrike();
        StartCoroutine(strikeThreeRoutine());
    }

    void stageFourStrike()
    {
        Debug.LogFormat("[Simon's Star #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedButtonName);
        substage = 0;
        GetComponent<KMBombModule>().HandleStrike();
        StartCoroutine(strikeFourRoutine());
    }

    void stageFiveStrike()
    {
        Debug.LogFormat("[Simon's Star #{0}] Strike! You pressed {1}. That is incorrect.", moduleId, pressedButtonName);
        substage = 0;
        GetComponent<KMBombModule>().HandleStrike();
        StartCoroutine(strikeFiveRoutine());
    }

    IEnumerator strikeOneRoutine()
    {
        foreach(Light indicLight in indicatorLight)
        {
            indicLight.enabled = true;
        }
        foreach(Renderer indic in levelIndicator)
        {
            indic.material.color = colourOptions[3];
        }
        Audio.PlaySoundAtTransform("strikeSound", transform);
        yield return new WaitForSeconds(0.5f);
        foreach(Light indicLight in indicatorLight)
        {
            indicLight.enabled = false;
        }
        foreach(Renderer indic in levelIndicator)
        {
            indic.material.color = colourOptions[5];
        }
        flashRoutine = StartCoroutine(flashes());
        striking = false;
    }

    IEnumerator strikeTwoRoutine()
    {
        indicatorLight[1].enabled = true;
        indicatorLight[2].enabled = true;
        indicatorLight[3].enabled = true;
        indicatorLight[4].enabled = true;
        levelIndicator[1].material.color = colourOptions[3];
        levelIndicator[2].material.color = colourOptions[3];
        levelIndicator[3].material.color = colourOptions[3];
        levelIndicator[4].material.color = colourOptions[3];
        Audio.PlaySoundAtTransform("strikeSound", transform);
        yield return new WaitForSeconds(0.5f);
        indicatorLight[1].enabled = false;
        indicatorLight[2].enabled = false;
        indicatorLight[3].enabled = false;
        indicatorLight[4].enabled = false;
        levelIndicator[1].material.color = colourOptions[5];
        levelIndicator[2].material.color = colourOptions[5];
        levelIndicator[3].material.color = colourOptions[5];
        levelIndicator[4].material.color = colourOptions[5];
        flashRoutine = StartCoroutine(flashes());
        striking = false;
    }

    IEnumerator strikeThreeRoutine()
    {
        indicatorLight[2].enabled = true;
        indicatorLight[3].enabled = true;
        indicatorLight[4].enabled = true;
        levelIndicator[2].material.color = colourOptions[3];
        levelIndicator[3].material.color = colourOptions[3];
        levelIndicator[4].material.color = colourOptions[3];
        Audio.PlaySoundAtTransform("strikeSound", transform);
        yield return new WaitForSeconds(0.5f);
        indicatorLight[2].enabled = false;
        indicatorLight[3].enabled = false;
        indicatorLight[4].enabled = false;
        levelIndicator[2].material.color = colourOptions[5];
        levelIndicator[3].material.color = colourOptions[5];
        levelIndicator[4].material.color = colourOptions[5];
        flashRoutine = StartCoroutine(flashes());
        striking = false;
    }

    IEnumerator strikeFourRoutine()
    {
        indicatorLight[3].enabled = true;
        indicatorLight[4].enabled = true;
        levelIndicator[3].material.color = colourOptions[3];
        levelIndicator[4].material.color = colourOptions[3];
        Audio.PlaySoundAtTransform("strikeSound", transform);
        yield return new WaitForSeconds(0.5f);
        indicatorLight[3].enabled = false;
        indicatorLight[4].enabled = false;
        levelIndicator[3].material.color = colourOptions[5];
        levelIndicator[4].material.color = colourOptions[5];
        flashRoutine = StartCoroutine(flashes());
        striking = false;
    }

    IEnumerator strikeFiveRoutine()
    {
        indicatorLight[4].enabled = true;
        levelIndicator[4].material.color = colourOptions[3];
        Audio.PlaySoundAtTransform("strikeSound", transform);
        yield return new WaitForSeconds(0.5f);
        indicatorLight[4].enabled = false;
        levelIndicator[4].material.color = colourOptions[5];
        flashRoutine = StartCoroutine(flashes());
        striking = false;
    }

    void buttonPress(starButtons selectable)
    {
        if(moduleSolved || solving || striking)
        {
            return;
        }
        GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
        pressedButtonName = selectable.colourName;
        StopCoroutine(flashRoutine);
        foreach(starButtons flash in buttons)
        {
            flash.buttonLight.enabled = false;
        }
        StartCoroutine(pressFlash(selectable));
        switch (stage)
        {
            case 0:
            if(selectable.selectable == buttons[0].selectable)
            {
                Audio.PlaySoundAtTransform("1stnote", transform);
                if(selectable == firstCorrectButton)
                {
                    solving = true;
                    stageOnePassMeth();
                }
                else
                {
                    striking = true;
                    stageOneStrike();
                }
            }
            else if(selectable.selectable == buttons[1].selectable)
            {
              Audio.PlaySoundAtTransform("2ndnote", transform);
                if(selectable== firstCorrectButton)
                {
                    solving = true;
                    stageOnePassMeth();
                }
                else
                {
                    striking = true;
                    stageOneStrike();
                }
            }
            else if(selectable.selectable == buttons[2].selectable)
            {
                Audio.PlaySoundAtTransform("3rdnote", transform);
                if(selectable == firstCorrectButton)
                {
                    solving = true;
                    stageOnePassMeth();
                }
                else
                {
                    striking = true;
                    stageOneStrike();
                }
            }
            else if(selectable.selectable == buttons[3].selectable)
            {
                Audio.PlaySoundAtTransform("4thnote", transform);
                if(selectable == firstCorrectButton)
                {
                    solving = true;
                    stageOnePassMeth();
                }
                else
                {
                    striking = true;
                    stageOneStrike();
                }
            }
            else
            {
                Audio.PlaySoundAtTransform("5thnote", transform);
                if(selectable == firstCorrectButton)
                {
                    solving = true;
                    stageOnePassMeth();
                }
                else
                {
                    striking = true;
                    stageOneStrike();
                }
            }
            //Debug.LogFormat("[Simon's Star #{0}] You pressed {1}.", moduleId, selectable.colourName);
            break;


            case 1:
            if(selectable.selectable == buttons[0].selectable)
            {
                Audio.PlaySoundAtTransform("1stnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    solving = true;
                    stageTwoPassMeth();
                }
                else
                {
                    striking = true;
                    stageTwoStrike();
                }
            }
            else if(selectable.selectable == buttons[1].selectable)
            {
                Audio.PlaySoundAtTransform("2ndnote", transform);
                if(substage == 0 && selectable== firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    solving = true;
                    stageTwoPassMeth();
                }
                else
                {
                    striking = true;
                    stageTwoStrike();
                }
            }
            else if(selectable.selectable == buttons[2].selectable)
            {
                Audio.PlaySoundAtTransform("3rdnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    solving = true;
                    stageTwoPassMeth();
                }
                else
                {
                    striking = true;
                    stageTwoStrike();
                }
            }
            else if(selectable.selectable == buttons[3].selectable)
            {
                Audio.PlaySoundAtTransform("4thnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    solving = true;
                    stageTwoPassMeth();
                }
                else
                {
                    striking = true;
                    stageTwoStrike();
                }
            }
            else
            {
                Audio.PlaySoundAtTransform("5thnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    solving = true;
                    stageTwoPassMeth();
                }
                else
                {
                    striking = true;
                    stageTwoStrike();
                }
            }
            //Debug.LogFormat("[Simon's Star #{0}] You pressed {1}.", moduleId, selectable.colourName);
            break;


            case 2:
            if(selectable.selectable == buttons[0].selectable)
            {
                Audio.PlaySoundAtTransform("1stnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    solving = true;
                    stageThreePassMeth();
                }
                else
                {
                    striking = true;
                    stageThreeStrike();
                }
            }
            else if(selectable.selectable == buttons[1].selectable)
            {
                Audio.PlaySoundAtTransform("2ndnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    solving = true;
                    stageThreePassMeth();
                }
                else
                {
                    striking = true;
                    stageThreeStrike();
                }
            }
            else if(selectable.selectable == buttons[2].selectable)
            {
                Audio.PlaySoundAtTransform("3rdnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    solving = true;
                    stageThreePassMeth();
                }
                else
                {
                    striking = true;
                    stageThreeStrike();
                }
            }
            else if(selectable.selectable == buttons[3].selectable)
            {
                Audio.PlaySoundAtTransform("4thnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    solving = true;
                    stageThreePassMeth();
                }
                else
                {
                    striking = true;
                    stageThreeStrike();
                }
            }
            else
            {
                Audio.PlaySoundAtTransform("5thnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    solving = true;
                    stageThreePassMeth();
                }
                else
                {
                    striking = true;
                    stageThreeStrike();
                }
            }
            //Debug.LogFormat("[Simon's Star #{0}] You pressed {1}.", moduleId, selectable.colourName);
            break;

            case 3:
            if(selectable.selectable == buttons[0].selectable)
            {
                Audio.PlaySoundAtTransform("1stnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    solving = true;
                    stageFourPassMeth();
                }
                else
                {
                    striking = true;
                    stageFourStrike();
                }
            }
            else if(selectable.selectable == buttons[1].selectable)
            {
                Audio.PlaySoundAtTransform("2ndnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    solving = true;
                    stageFourPassMeth();
                }
                else
                {
                    striking = true;
                    stageFourStrike();
                }
            }
            else if(selectable.selectable == buttons[2].selectable)
            {
                Audio.PlaySoundAtTransform("3rdnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    solving = true;
                    stageFourPassMeth();
                }
                else
                {
                    striking = true;
                    stageFourStrike();
                }
            }
            else if(selectable.selectable == buttons[3].selectable)
            {
                Audio.PlaySoundAtTransform("4thnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    solving = true;
                    stageFourPassMeth();
                }
                else
                {
                    striking = true;
                    stageFourStrike();
                }
            }
            else
            {
                Audio.PlaySoundAtTransform("5thnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    solving = true;
                    stageFourPassMeth();
                }
                else
                {
                    striking = true;
                    stageFourStrike();
                }
            }
            //Debug.LogFormat("[Simon's Star #{0}] You pressed {1}.", moduleId, selectable.colourName);
            break;

            case 4:
            if(selectable.selectable == buttons[0].selectable)
            {
                Audio.PlaySoundAtTransform("1stnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    substage++;
                }
                else if(substage == 4 && selectable == fifthCorrectButton)
                {
                    solving = true;
                    stageFivePassMeth();
                }
                else
                {
                    striking = true;
                    stageFiveStrike();
                }
            }
            else if(selectable.selectable == buttons[1].selectable)
            {
                Audio.PlaySoundAtTransform("2ndnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    substage++;
                }
                else if(substage == 4 && selectable == fifthCorrectButton)
                {
                    solving = true;
                    stageFivePassMeth();
                }
                else
                {
                    striking = true;
                    stageFiveStrike();
                }
            }
            else if(selectable.selectable == buttons[2].selectable)
            {
                Audio.PlaySoundAtTransform("3rdnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    substage++;
                }
                else if(substage == 4 && selectable == fifthCorrectButton)
                {
                    solving = true;
                    stageFivePassMeth();
                }
                else
                {
                    striking = true;
                    stageFiveStrike();
                }
            }
            else if(selectable.selectable == buttons[3].selectable)
            {
                Audio.PlaySoundAtTransform("4thnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    substage++;
                }
                else if(substage == 4 && selectable == fifthCorrectButton)
                {
                    solving = true;
                    stageFivePassMeth();
                }
                else
                {
                    striking = true;
                    stageFiveStrike();
                }
            }
            else
            {
                Audio.PlaySoundAtTransform("5thnote", transform);
                if(substage == 0 && selectable == firstCorrectButton)
                {
                    substage++;
                }
                else if(substage == 1 && selectable == secondCorrectButton)
                {
                    substage++;
                }
                else if(substage == 2 && selectable == thirdCorrectButton)
                {
                    substage++;
                }
                else if(substage == 3 && selectable == fourthCorrectButton)
                {
                    substage++;
                }
                else if(substage == 4 && selectable == fifthCorrectButton)
                {
                    solving = true;
                    stageFivePassMeth();
                }
                else
                {
                    striking = true;
                    stageFiveStrike();
                }
            }
            //Debug.LogFormat("[Simon's Star #{0}] You pressed {1}.", moduleId, selectable.colourName);
            break;


            default:
            break;
        }
    }

    IEnumerator pressFlash(starButtons selectable)
    {
        if(selectable.selectable == buttons[0])
        {
            yield return new WaitForSeconds(0.1f);
            selectable.buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            if(stage > 5)
            selectable.buttonLight.enabled = false;
        }
        else if(selectable.selectable == buttons[1])
        {
            yield return new WaitForSeconds(0.1f);
            selectable.buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            selectable.buttonLight.enabled = false;
        }
        else if(selectable.selectable == buttons[2])
        {
            yield return new WaitForSeconds(0.1f);
            selectable.buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            if(stage > 5)
            selectable.buttonLight.enabled = false;
        }
        else if(selectable.selectable == buttons[3])
        {
            yield return new WaitForSeconds(0.1f);
            selectable.buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            selectable.buttonLight.enabled = false;
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            selectable.buttonLight.enabled = true;
            yield return new WaitForSeconds(0.4f);
            selectable.buttonLight.enabled = false;
        }
    }

    IEnumerator indicLightFlicker()
    {
        int stageCo = stage;
        int flashRoutine = 0;
        indicatorLight[stageCo].enabled = true;
        while(flashRoutine < 2)
        {
            levelIndicator[stageCo].material.color = colourOptions[selectedColours[stageCo]];
            yield return new WaitForSeconds(0.1f);
            levelIndicator[stageCo].material.color = colourOptions[5];
            yield return new WaitForSeconds(0.1f);
            levelIndicator[stageCo].material.color = colourOptions[selectedColours[(stageCo + 1) % 5]];
            yield return new WaitForSeconds(0.1f);
            levelIndicator[stageCo].material.color = colourOptions[5];
            yield return new WaitForSeconds(0.1f);
            levelIndicator[stageCo].material.color = colourOptions[selectedColours[(stageCo + 2) % 5]];
            yield return new WaitForSeconds(0.1f);
            levelIndicator[stageCo].material.color = colourOptions[5];
            yield return new WaitForSeconds(0.1f);
            levelIndicator[stageCo].material.color = colourOptions[selectedColours[(stageCo + 3) % 5]];
            yield return new WaitForSeconds(0.1f);
            levelIndicator[stageCo].material.color = colourOptions[5];
            yield return new WaitForSeconds(0.1f);
            levelIndicator[stageCo].material.color = colourOptions[selectedColours[(stageCo + 4) % 5]];
            yield return new WaitForSeconds(0.1f);
            levelIndicator[stageCo].material.color = colourOptions[5];
            yield return new WaitForSeconds(0.1f);
            flashRoutine++;
        }
        levelIndicator[stageCo].material.color = colourOptions[selectedNumber[stageCo]];
        flashRoutine = 0;
    }

    IEnumerator solvedRoutine()
    {
        yield return new WaitForSeconds(1f);
        Audio.PlaySoundAtTransform("solvedSound", transform);
        foreach(starButtons flash in buttons)
        {
            flash.buttonLight.enabled = true;
        }
        yield return new WaitForSeconds(0.5f);
        foreach(starButtons flash in buttons)
        {
            flash.buttonLight.enabled = false;
        }
        buttons[0].buttonLight.enabled = true;
        yield return new WaitForSeconds(0.3f);
        buttons[1].buttonLight.enabled = true;
        yield return new WaitForSeconds(0.3f);
        buttons[2].buttonLight.enabled = true;
        yield return new WaitForSeconds(0.3f);
        buttons[3].buttonLight.enabled = true;
        yield return new WaitForSeconds(0.5f);
        buttons[4].buttonLight.enabled = true;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Submit yellow, green and purple with “!{0} press yellow green purple”.";
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToLowerInvariant();
        string[] parts = command.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        if (!command.StartsWith("press")) yield break;
        string[] valid = new[] { "r", "g", "b", "y", "p", "red", "green", "blue", "yellow", "purple" };
        foreach (string part in parts.Skip(1))
        {
            if (!valid.Contains(part)) yield break;
        }
        yield return null;
        foreach (string part in parts.Skip(1))
        {
            starButtons correctButton = buttons.Where(x => x.colourName.StartsWith(part)).First();
            yield return "trycancel";
            yield return new[] { correctButton.selectable };
            yield return new WaitForSeconds(.4f);
        }
    }
}