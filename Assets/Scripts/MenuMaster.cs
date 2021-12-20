using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
/// <summary>
/// this class has all the logic of the menu looks and trading functions
/// ir receives the selected section of the wheel from "WheelBehaviour" and acts acordingly
/// </summary>
public class MenuMaster : MonoBehaviour
{
    [TextArea]
    [SerializeField]
    string[] fusionDescriptions,discoverDescriptions,forgeDescriptions,mixDescriptions;//text descriptions of each section
    [Header("UI components")]
    [SerializeField]
    Button mainBtn;//main button UI
    [SerializeField] Button upBtn,downBtn;// section changing buttons
    [SerializeField] TextMeshProUGUI MainBtnTxt;//text shown in the main button
    [SerializeField] TextMeshProUGUI DescriptionsTxt,titleTxt;//info UI elements
    [SerializeField] Slider mainSlider;//trading slider element
    [SerializeField] TextMeshProUGUI sliderTxt, AnimationTxt, SphereAnimationTxt, SphereAnimationTitleTxt, MixAnimationTxt, MixAnimationTitleTxt;//informative texts
    [SerializeField] TextMeshProUGUI WOrbeesTxt, ROrbeesTxt, YOrbeesTxt, GOrbeesTxt, COrbeesTxt, BOrbeesTxt, MOrbeesTxt;//shows the quantity of each orb in the trades
    [SerializeField] GameObject FusionAnimPanel, SphereAnimPanel,MixAnimPanel;//panels with the animation of each trade
    [SerializeField] GameObject wheel, outerwheel;//wheel menu sections
    [SerializeField] Sprite[] powerIcons;//list of the logos of each family

    string[] powernames= {"Familia Roja","Familia Amarilla","Familia Verde","Familia Cyan","Familia azul","Familia Magenta"};//families names

    [Header("Other Components")]
    [SerializeField] GameObject[] OuterPanels,middlePanels,centerPanels,innerPanels,controlPanels;//panels to show in each section
    
    Variables var;//users data base
    public enum section { fusion, discover, forge, mix,subsection};//available sections of the wheel("WheelBehaviour" uses it)
    
    public section currentSection=section.fusion;//section we are currently at(fusion,discover,forge,mix and the subsections)
    public int sectionLevel=0;//leve
    public int currentSubsection = 0;
    public int auxSubsection = 0;
    // Start is called before the first frame update
    void Start()
    {
        //var is the class where all the users variables are stored
        var = Transform.FindObjectOfType<Variables>();

        showOrbeesValues();//shows how many orbs we have
        switchSection(currentSection);//enables all the panels of the current section
    }


    //*******************************************actualizes the  texts values in the UI
    public void showOrbeesValues()
    {
        if (currentSection == section.fusion)//fusion section information
        {
            sliderTxt.text = mainSlider.value.ToString();//slider value
            //orbs values of each color
            WOrbeesTxt.text = "X" + Kformating((int)(var.whiteOrbs + mainSlider.value));
            ROrbeesTxt.text = Kformating(var.redOrbs) + "/" + Kformating((int)mainSlider.value);
            YOrbeesTxt.text = Kformating(var.yellowOrbs) + "/" + Kformating((int)mainSlider.value);
            GOrbeesTxt.text = Kformating(var.greenOrbs) + "/" + Kformating((int)mainSlider.value);
            COrbeesTxt.text = Kformating(var.cyanOrbs) + "/" + Kformating((int)mainSlider.value);
            BOrbeesTxt.text = Kformating(var.blueOrbs) + "/" + Kformating((int)mainSlider.value);
            MOrbeesTxt.text = Kformating(var.magentaOrbs) + "/" + Kformating((int)mainSlider.value);
        }
        else if (currentSection == section.discover) //discover section information
        {
            sliderTxt.text = mainSlider.value.ToString();
            spherefamilies orbInfo = GetSpherefamilies(auxSubsection)[currentSubsection];//info of the current selected rune 
            controlPanels[2].transform.GetChild(0).GetChild(0).transform.GetComponent<TextMeshProUGUI>().text 
                = Kformating(var.whiteOrbs)+"/"+Kformating(orbInfo.whiteCost* (int)mainSlider.value);
            controlPanels[2].transform.GetChild(1).GetChild(0).transform.GetComponent<TextMeshProUGUI>().text
                = Kformating(getVarByIndex(auxSubsection)) + "/" + Kformating(orbInfo.colorCost * (int)mainSlider.value);
            innerPanels[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (orbInfo.unlocked+(int)mainSlider.value).ToString() + "%";
            
        }
        else if (currentSection == section.mix)//mix section information
        {
            sliderTxt.text = mainSlider.value.ToString();
            spherefamilies orbInfo = GetSpherefamilies(auxSubsection)[currentSubsection];
            controlPanels[3].transform.GetChild(0).GetChild(0).transform.GetComponent<TextMeshProUGUI>().text
                = Kformating(var.whiteOrbs) + "/" + Kformating(orbInfo.whiteCost * (int)mainSlider.value);
            controlPanels[3].transform.GetChild(1).GetChild(0).transform.GetComponent<TextMeshProUGUI>().text
                = Kformating(getVarByIndex(auxSubsection)) + "/" + Kformating(orbInfo.colorCost * (int)mainSlider.value);
            controlPanels[3].transform.GetChild(2).GetChild(0).transform.GetComponent<TextMeshProUGUI>().text
                = Kformating(getVarByIndex(orbInfo.mixcolorIndex)) + "/" + Kformating(orbInfo.mixCost * (int)mainSlider.value);

            controlPanels[3].transform.GetChild(1).GetComponent<Image>().color
                = controlPanels[1].transform.GetChild(auxSubsection).transform.GetComponent<Image>().color;

            controlPanels[3].transform.GetChild(2).GetComponent<Image>().color
                = controlPanels[1].transform.GetChild(orbInfo.mixcolorIndex).transform.GetComponent<Image>().color;
            innerPanels[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ("x" + Kformating((int)mainSlider.value+ orbInfo.quantity));
            if (orbInfo.unlocked < 100)
            {
                innerPanels[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }

    }

    
    //********************************gives the k format to the int  higher than 10000 just for visuals
    string Kformating(int x)
    {
        string xs=x.ToString();
        if (x >= 1000000)
        {
            xs = (x / 1000000).ToString("N1") + "M";
        }
        else if(x >= 10000)
        {
            xs = (x / 1000).ToString("N1") + "K";
        }
        return xs;
    }

    //***********************************function of increment and decrement slider buttons
    public void SliderButtons(int btn)
    {
        if (btn == 0)
            mainSlider.value -= mainSlider.maxValue / 10;
        else
            mainSlider.value += mainSlider.maxValue / 10;
    }


    //*******************************************manages all the initializations when we change section(hide/show panels, init functions, etc)
    public void switchSection(section sectionSet)
    {
        if(sectionSet!=section.subsection)
            currentSection = sectionSet;
        switch (sectionSet)
        {
            //fusion menu init configs
            case section.fusion:
                mainSlider.interactable = true;
                DescriptionsTxt.text = fusionDescriptions[0];
                MainBtnTxt.text = "Fusionar";
                titleTxt.text = "Fusionar";
                mainBtn.interactable = true;
                upBtn.interactable = false;
                downBtn.interactable = false;
                setSliderMaxValue();
                hideSectionPanels();
                middlePanels[0].SetActive(true);
                innerPanels[0].SetActive(true);
                controlPanels[0].SetActive(true);
                controlPanels[1].SetActive(true);
                break;

            //discover menu init configs
            case section.discover:
                DescriptionsTxt.text = discoverDescriptions[0];
                MainBtnTxt.text = "Descubrir";
                titleTxt.text = "Descubrir";
                mainBtn.interactable = false;
                upBtn.interactable = false;
                downBtn.interactable = true;
                setSliderMaxValue();
                hideSectionPanels();
                middlePanels[0].SetActive(true);
                centerPanels[0].SetActive(true);
                break;
            //forge menu init configs
            case section.forge:
                DescriptionsTxt.text = forgeDescriptions[0];
                MainBtnTxt.text = "Forjar";
                titleTxt.text = "Forjar";
                mainBtn.interactable = false;
                upBtn.interactable = false;
                downBtn.interactable = false;
                setSliderMaxValue();
                hideSectionPanels();
                middlePanels[0].SetActive(true);
                centerPanels[0].SetActive(true);
                break;
            //mix menu init configs
            case section.mix:
                DescriptionsTxt.text = mixDescriptions[0];
                MainBtnTxt.text = "Mezclar";
                titleTxt.text = "Mezclar";
                mainBtn.interactable = false;
                upBtn.interactable = false;
                downBtn.interactable = true;
                setSliderMaxValue();
                hideSectionPanels();
                middlePanels[0].SetActive(true);
                centerPanels[0].SetActive(true);
                break;

            //subsections menu init configs
            case section.subsection:
                switch (currentSection)
                {
                    case section.discover:
                        setSliderMaxValue();
                        showOrbeesValues();

                        discoverSwitchFamily();
                        break;
                    case section.mix:
                        if (sectionLevel == 1)
                        {
                            discoverSwitchFamily(1);
                            setSliderMaxValue();
                            showOrbeesValues();
                        }
                        if (sectionLevel == 2)
                        {

                            discoverSwitchFamily(1);
                            setSliderMaxValue();
                            showOrbeesValues();
                            
                            
                        }
                        break;
                }
                break;
        }
    }


    //************************************manages all the configurations when we dig lower or higher in the same section
    public void switchLevel(int level)
    {
        sectionLevel = level;
        hideSectionPanels();
        switch (currentSection)
        {
            ///////////////////////discover configs
            case section.discover:
                if (sectionLevel == 0)
                {
                    switchSection(currentSection);
                    currentSubsection = 0;
                }
                else if (sectionLevel == 1)
                {
                    upBtn.interactable = true;
                    downBtn.interactable = true;
                    mainBtn.interactable = false;
                    OuterPanels[0].SetActive(true);
                    middlePanels[1].SetActive(true);
                    centerPanels[1].SetActive(true);
                    innerPanels[1].SetActive(true);
                    currentSubsection = auxSubsection;
                    auxSubsection = 0;
                    discoverSwitchFamily();
                }
                else if (sectionLevel == 2)
                {
                    auxSubsection = currentSubsection;
                    currentSubsection = 0;
                    upBtn.interactable = true;
                    downBtn.interactable = false;
                    mainBtn.interactable = true;
                    OuterPanels[1].SetActive(true);
                    middlePanels[2].SetActive(true);
                    innerPanels[2].SetActive(true);
                    controlPanels[0].SetActive(true);
                    controlPanels[2].SetActive(true);
                    controlPanels[2].transform.GetChild(1).GetComponent<Image>().color =
                        controlPanels[1].transform.GetChild(auxSubsection).GetComponent<Image>().color;
                    mainSlider.value = mainSlider.minValue;
                    setSliderMaxValue();
                    showOrbeesValues();
                }
                break;
            case section.forge:
                break;
            //////////////////////////////////////discover configs
            case section.mix:
                if (sectionLevel == 0)
                {
                    switchSection(currentSection);
                    currentSubsection = 0;
                }
                else if (sectionLevel == 1)
                {
                    upBtn.interactable = true;
                    downBtn.interactable = true;
                    mainBtn.interactable = false;
                    OuterPanels[0].SetActive(true);
                    middlePanels[1].SetActive(true);
                    centerPanels[1].SetActive(true);
                    innerPanels[1].SetActive(true);
                    currentSubsection = auxSubsection;
                    auxSubsection = 0;
                    discoverSwitchFamily(1);
                }
                else if (sectionLevel == 2)
                {
                    auxSubsection = currentSubsection;
                    currentSubsection = 0;

                    upBtn.interactable = true;
                    downBtn.interactable = false;
                    mainBtn.interactable = true;
                    OuterPanels[1].SetActive(true);
                    middlePanels[2].SetActive(true);
                    innerPanels[2].SetActive(true);
                    controlPanels[0].SetActive(true);
                    controlPanels[3].SetActive(true);
                    controlPanels[3].transform.GetChild(1).GetComponent<Image>().color =
                        controlPanels[1].transform.GetChild(currentSubsection).GetComponent<Image>().color;
                    mainSlider.value = mainSlider.minValue;
                    setSliderMaxValue();
                    showOrbeesValues();
                    discoverSwitchFamily(1);
                }
                break;
        }
    }

    //****************************************************sets the main slider max value acordingly to the selected trade logic and orbs available
    void setSliderMaxValue()
    {
        if (currentSection == section.fusion)
        {
            int min1 = Math.Min(var.redOrbs, Math.Min(var.yellowOrbs, var.greenOrbs));
            int min2 = Math.Min(var.cyanOrbs, Math.Min(var.blueOrbs, var.magentaOrbs));
            int min3 = Math.Min(min1, min2);
            if (min3 > 0)
            {
                mainSlider.maxValue = min3;
                mainSlider.interactable = true;
            }
                
            else
            {
                mainSlider.maxValue = 1;
                mainSlider.minValue = 0;
                mainSlider.interactable = false;
                mainSlider.value = mainSlider.minValue;
            }
        }
        else if(currentSection == section.discover && sectionLevel==2)
        {
            int orb = getVarByIndex(auxSubsection);
            int whiteOrb=var.whiteOrbs;
            spherefamilies orbInfo = GetSpherefamilies(auxSubsection)[currentSubsection];

            int min =Mathf.Clamp(Math.Min(whiteOrb/orbInfo.whiteCost, orb/orbInfo.colorCost),0,100-orbInfo.unlocked);
            print(orbInfo.name);
            print(min);
            mainSlider.interactable = true;
            if (min > 0)
            {
                mainBtn.interactable = true;
                mainSlider.maxValue = min;
            }
            else
            {
                mainSlider.maxValue = 1;
                mainSlider.minValue = 0;
                mainSlider.interactable = false;
                mainSlider.value = mainSlider.minValue;
                mainBtn.interactable = false;
            }
        }
        else if (currentSection == section.mix)
        {
            int orb = getVarByIndex(auxSubsection);
            int whiteOrb = var.whiteOrbs;
            spherefamilies orbInfo = GetSpherefamilies(auxSubsection)[currentSubsection];

            int mixorb = getVarByIndex(orbInfo.mixcolorIndex);
            int mini = Math.Min(whiteOrb / orbInfo.whiteCost, orb / orbInfo.colorCost);
            int min = Math.Min(mini, mixorb / orbInfo.mixCost);
            mainSlider.interactable = true;
            if (min > 0)
            {
                mainSlider.interactable = true;
                mainSlider.maxValue = min;
            }
            else
            {
                mainSlider.maxValue = 1;
                mainSlider.minValue = 0;
                mainSlider.interactable = false;
                mainSlider.value = mainSlider.minValue;
            }
        }
    }

    //********************************returns the selected orb큦 value by index
    int getVarByIndex(int index)
    {
        int value = 0;
        switch (index)
        {
            case 0: value = var.redOrbs;break;
            case 1: value = var.yellowOrbs;break;
            case 2: value = var.greenOrbs;break;
            case 3: value = var.cyanOrbs;break;
            case 4: value = var.blueOrbs;break;
            case 5: value = var.magentaOrbs;break;
        }
        return value;
    }

    //********************************returns the selected family array by index
    spherefamilies[] GetSpherefamilies(int index)
    {
        spherefamilies[] value=new spherefamilies[1];
        switch (index)
        {
            case 0: value = var.redFamily; break;
            case 1: value = var.yellowFamily; break;
            case 2: value = var.greenFamily; break;
            case 3: value = var.cyanFamily; break;
            case 4: value = var.blueFamily; break;
            case 5: value = var.magentaFamily; break;
        }
        return value;
    }

    //*********************************main button function accordingly to the section(trade actions)
    public void mainButtonAction()
    {
        spherefamilies orbInfo = GetSpherefamilies(auxSubsection)[currentSubsection];
        switch (currentSection)
        {

            ////////////////////////////////////////////////////////////////////fusion trading section
            case section.fusion:
                if(var.redOrbs >= mainSlider.value && var.yellowOrbs >= mainSlider.value && var.greenOrbs >= mainSlider.value &&
                   var.cyanOrbs >= mainSlider.value && var.blueOrbs >= mainSlider.value && var.magentaOrbs >= mainSlider.value
                   && mainSlider.value>0)
                {
                    var.whiteOrbs += (int)mainSlider.value;
                    var.redOrbs -= (int)mainSlider.value;
                    var.yellowOrbs -= (int)mainSlider.value;
                    var.greenOrbs -= (int)mainSlider.value;
                    var.cyanOrbs -= (int)mainSlider.value;
                    var.blueOrbs -= (int)mainSlider.value;
                    var.magentaOrbs -= (int)mainSlider.value;
                    setFusionAnim(true);
                    mainSlider.value = mainSlider.minValue;
                    setSliderMaxValue();
                    showOrbeesValues();
                }
                
                break;



            ////////////////////////////////////////////////////////////////////discover trading section
            case section.discover:
                int colorOrb = getVarByIndex(auxSubsection);
                if  (var.whiteOrbs>=orbInfo.whiteCost*(int)mainSlider.value && 
                    colorOrb >= orbInfo.colorCost * (int)mainSlider.value)
                {
                   
                    var.whiteOrbs -= (int)mainSlider.value * orbInfo.whiteCost;
                    switch (auxSubsection)
                    {
                        case 0: 
                            var.redOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.redFamily[currentSubsection].unlocked += (int)mainSlider.value;
                            break;
                        case 1: 
                            var.yellowOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.yellowFamily[currentSubsection].unlocked += (int)mainSlider.value; 
                            break;
                        case 2: 
                            var.greenOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.greenFamily[currentSubsection].unlocked += (int)mainSlider.value; 
                            break;
                        case 3: 
                            var.cyanOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.cyanFamily[currentSubsection].unlocked += (int)mainSlider.value; 
                            break;
                        case 4: 
                            var.blueOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.blueFamily[currentSubsection].unlocked += (int)mainSlider.value; 
                            break;
                        case 5: 
                            var.magentaOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.magentaFamily[currentSubsection].unlocked += (int)mainSlider.value;
                            break;
                    }
                    setSphereAnim(true);
                    setSliderMaxValue();
                    showOrbeesValues();
                    discoverSwitchFamily();
                }
                break;

                ////////////////////////////////////////////////////////////////////mix trading section
            case section.mix:
                
                int colorOrb1 = getVarByIndex(auxSubsection);//first colored orb
                int colorOrb2 = getVarByIndex(orbInfo.mixcolorIndex);//second colored orb

                //if we have enough orbs we make the trading
                if (var.whiteOrbs >= orbInfo.whiteCost * (int)mainSlider.value &&
                    colorOrb1 >= orbInfo.colorCost * (int)mainSlider.value &&
                    colorOrb2 >= orbInfo.mixCost * (int)mainSlider.value) 
                {

                    var.whiteOrbs -= (int)mainSlider.value * orbInfo.whiteCost;
                    //substracts the main color cost of the database
                    switch (auxSubsection)
                    {
                        case 0:
                            var.redOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.redFamily[currentSubsection].quantity += (int)mainSlider.value;
                            break;
                        case 1:
                            var.yellowOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.yellowFamily[currentSubsection].quantity += (int)mainSlider.value;
                            break;
                        case 2:
                            var.greenOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.greenFamily[currentSubsection].quantity += (int)mainSlider.value;
                            break;
                        case 3:
                            var.cyanOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.cyanFamily[currentSubsection].quantity += (int)mainSlider.value;
                            break;
                        case 4:
                            var.blueOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.blueFamily[currentSubsection].quantity += (int)mainSlider.value;
                            break;
                        case 5:
                            var.magentaOrbs -= (int)mainSlider.value * orbInfo.colorCost;
                            var.magentaFamily[currentSubsection].quantity += (int)mainSlider.value;
                            break;
                    }
                    //substracts the mix cost of the data base
                    switch (orbInfo.mixcolorIndex)
                    {
                        case 0:
                            var.redOrbs -= (int)mainSlider.value * orbInfo.mixCost;
                            break;
                        case 1:
                            var.yellowOrbs -= (int)mainSlider.value * orbInfo.mixCost;
                            break;
                        case 2:
                            var.greenOrbs -= (int)mainSlider.value * orbInfo.mixCost;
                            break;
                        case 3:
                            var.cyanOrbs -= (int)mainSlider.value * orbInfo.mixCost;
                            break;
                        case 4:
                            var.blueOrbs -= (int)mainSlider.value * orbInfo.mixCost;
                            break;
                        case 5:
                            var.magentaOrbs -= (int)mainSlider.value * orbInfo.mixCost;
                            break;
                    }
                    setMixAnim(true);
                    setSliderMaxValue();
                    showOrbeesValues();
                    discoverSwitchFamily(1);
                }
                break;
        }
        
    }

    //**************************************************************function to navigate between section큦 levels 
    public Quaternion[] levelrotation= new Quaternion[4];
    public void buttonLevelSwitch(int lvl)//0=down 1=up
    {
        if (lvl == 0)//lows sections
        {
            sectionLevel--;
            if (sectionLevel > 0)
            {
                
                outerwheel.transform.rotation = levelrotation[sectionLevel-1];
                
                Transform.FindObjectOfType<WheelBehaviour>().dropSectionRotation = levelrotation[sectionLevel];
                wheel.transform.rotation = levelrotation[sectionLevel];
                
            }
            else
            {
                Transform.FindObjectOfType<WheelBehaviour>().dropSectionRotation = levelrotation[sectionLevel];
                wheel.transform.rotation = levelrotation[sectionLevel];
                switchSection(currentSection);
            }

        }
        else if (lvl == 1) //high section
        {
            levelrotation[sectionLevel] = wheel.transform.rotation;
            outerwheel.transform.rotation = wheel.transform.rotation;
            Transform.FindObjectOfType<WheelBehaviour>().dropSectionRotation = Quaternion.identity;
            wheel.transform.rotation = Quaternion.identity;
            sectionLevel++;
        }
        switchLevel(sectionLevel);

    }

    //****************************************enables/disables the fusion  animation
    public void setFusionAnim(bool setting)
    {
        FusionAnimPanel.SetActive(setting);
        AnimationTxt.text = "Cantidad: " + mainSlider.value.ToString();
    }

    //****************************************enables/disables the discover animation
    public void setSphereAnim(bool setting)
    {
        SphereAnimPanel.SetActive(setting);
        SphereAnimationTxt.text = "Descubierto: " + mainSlider.value.ToString()+"%";
        SphereAnimationTitleTxt.text= "Runa " + GetSpherefamilies(auxSubsection)[currentSubsection].name;
        SphereAnimPanel.transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = powerIcons[auxSubsection];
    }

    //****************************************enables/disables the mix  animation
    public void setMixAnim(bool setting)
    {
        MixAnimPanel.SetActive(setting);
        MixAnimationTxt.text = "Cantidad: " + mainSlider.value.ToString();
        MixAnimationTitleTxt.text = "Runa " + GetSpherefamilies(auxSubsection)[currentSubsection].name;
        MixAnimPanel.transform.GetChild(2).GetChild(0).GetComponent<Renderer>().material.color = GetSpherefamilies(auxSubsection)[currentSubsection].color;
        MixAnimPanel.transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = powerIcons[auxSubsection];
    }

    //*****************************************opens selected scene
    public void openScene(int scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    //******************************************hides all the section panels
    void hideSectionPanels()
    {
        foreach(GameObject panel in OuterPanels){ panel.SetActive(false);}
        foreach (GameObject panel in middlePanels) { panel.SetActive(false); }
        foreach (GameObject panel in centerPanels) { panel.SetActive(false); }
        foreach (GameObject panel in innerPanels) { panel.SetActive(false); }
        foreach (GameObject panel in controlPanels) { panel.SetActive(false); }
    }

    //******************************************manages all the runes, orbs and family information to shown in the different levels of the sections
    void discoverSwitchFamily()
    {
        discoverSwitchFamily(0);
    }
    void discoverSwitchFamily(int mode)
    {
        spherefamilies[] family = var.redFamily;
        int tempsubsection =currentSubsection;
        if (sectionLevel == 1)
        {
            tempsubsection = currentSubsection;
        }
        else if (sectionLevel == 2)
        {
            tempsubsection = auxSubsection;
        }
            
        family = GetSpherefamilies(tempsubsection);//the family we are currently at

        innerPanels[1].transform.GetChild(0).GetComponent<Image>().sprite = powerIcons[tempsubsection];//shows the family logo at the center
        innerPanels[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = powernames[currentSubsection];//shows the family name at the center

        if(family[currentSubsection].unlocked < 100)//if the rune is not unlocked shows only the shadow
        {
            innerPanels[2].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);//hides image
            innerPanels[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";//hides name
            if (mode == 1)//mode 1 is for the mix section
            {
                innerPanels[2].transform.GetChild(0).gameObject.SetActive(false);//hides shadow
                innerPanels[2].transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text="";//hides quantity
                innerPanels[2].transform.GetChild(2).transform.GetComponent<TextMeshProUGUI>().text = "";//hides title
                controlPanels[0].transform.parent.gameObject.SetActive(false);//hides slider
            }
            
        }
        else//if the rune is unlocked shows all the info
        {
            if (mode == 1)//for the mix section
            {
                innerPanels[2].transform.GetChild(0).gameObject.SetActive(true);//shadow
                controlPanels[0].transform.parent.gameObject.SetActive(true);//slider panel
                setSliderMaxValue();//sets the slider max value
            }
            innerPanels[2].transform.GetChild(0).gameObject.SetActive(true);//hides image and leaves the shadow
            innerPanels[2].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            innerPanels[2].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = family[currentSubsection].color;// rune color
            innerPanels[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = powerIcons[tempsubsection];//family logo
            innerPanels[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = family[currentSubsection].unlocked.ToString()+"%";//shows the unlocked level
            innerPanels[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = family[currentSubsection].name;//rune큦 name
            if (mode == 1)//for the mix section
            {
                innerPanels[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x "+family[currentSubsection].quantity.ToString();//show runes quantity

                if (family[currentSubsection].unlocked < 100)//if the rune is locked hides it큦 name
                {
                    innerPanels[2].transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = "";
                }
            }
        }
        for (int i = 1; i <=6; i++)// for each rune in the selected family we hide it if locked
        {
            if (family[i-1].unlocked < 100)
            {
                middlePanels[2].transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                centerPanels[1].transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                if(mode==1)//mix mode
                {
                    middlePanels[2].transform.GetChild(i).gameObject.SetActive(false);
                    centerPanels[1].transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else//if unlocked show all the runes info
            {
                middlePanels[2].transform.GetChild(i).gameObject.SetActive(true);
                centerPanels[1].transform.GetChild(i).gameObject.SetActive(true);
                middlePanels[2].transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                middlePanels[2].transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().color = family[i - 1].color;
                middlePanels[2].transform.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = powerIcons[tempsubsection];
                centerPanels[1].transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                centerPanels[1].transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().color = family[i-1].color;
                centerPanels[1].transform.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = powerIcons[currentSubsection];
            }

        }
    }

}
