using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlotManager : MonoBehaviour
{
    //switch buttons
    public Toggle toggle1;
    public Toggle toggle2;

    //array of the gameObjects which will turn into plots
    public GameObject[] plotsGameObjects;

    //VERY IMPORTANT: The maximum frequency in Unity for the spectrum data is 24000 hz.
    //Lets say we allocate 1024 bins as resolution
    //If we want to use only a range from 0 - 8000 hz for example, we would have to use only 
    //the first 1/3 of the bins as the line of pixels we want to update, at the top of the plot
    //I haven't thought about how to automate this in an intuitive matter yet, so for now we will plug
    //the correct values
    //This whould only bee needed for the spectrogram type of plots however

    //specifications for each plot
    private float[] heights = {100, 100, 100, 100};
    private float[] widths = {100, 100, 100, 100};
    private int[] resolutionPixelsY = {1024, 1024, 512, 512};

    //keep in mind that for spectrograms,
    // the number of pixels in the X axis should be smaller or equal to the number of bins
    // ! Important ! Need to fix in the future, right now the order of these elements in the array, and their contents
    //are important. It has to be changed perhaps, but for now there should be put care if
    //they are to be modified since some classess and methods access the positions directly
    //And the number of seconds should be less than hald of the number of pixels in the Y dimension
    private int[] resolutionPixelsX = {512, 512, 512, 512};
    private int[] numbSecondsDisplayed = {30, 240, 30, 240};
    private string[] plotTypes = {"spectrogram", "spectrogram", "bearing",  "bearing"};


    //---------------------------------------------------------

    //number of bins used to gather spectrum data for each plot;
    private int[] numberOfBins = {1024, 1024, 1024, 1024};

    //refrences to classes
    public Utilities utilities;
    public UserControls userControls;
    public EntityManager entityManager;


    // to update
    [SerializeField]
    private float[] frequenciesDetectableRange;
    [SerializeField]
    private float[] frequencyNumberArr;

    private GameObject[] soundSourcesArr;
    public GameObject noise;

    //Labels for the time axis spectrogram
    public TextMeshProUGUI  timeTextAxisSpectrogram1;
    public TextMeshProUGUI  timeTextAxisSpectrogram2;   
    public TextMeshProUGUI  timeTextAxisSpectrogram3;
    public TextMeshProUGUI  timeTextMeasureSpectrogram;

    //Labels for the time axis bearing
    public TextMeshProUGUI  timeTextAxisBearing1;
    public TextMeshProUGUI  timeTextAxisBearing2;   
    public TextMeshProUGUI  timeTextAxisBearing3;
    public TextMeshProUGUI  timeTextMeasureBearing;


    void Start(){
        //Create all the plots
        for(int i = 0; i < plotTypes.Length; i++){
            PlotGenerator plot = plotsGameObjects[i].GetComponent<PlotGenerator>();
            plot.Initialise(heights[i], widths[i], resolutionPixelsY[i], resolutionPixelsX[i], numbSecondsDisplayed[i], numberOfBins[i], plotTypes[i]);
        }

        soundSourcesArr = entityManager.getSoundSourcesArray();
        frequencyNumberArr = utilities.createFreqArray(resolutionPixelsX[0]);
        frequenciesDetectableRange = utilities.CreateDetectableRangesArray(frequencyNumberArr);

        UpdateSpectrogramTimeAxis(1);
        UpdateBearingTimeAxis(1);

        toggle1.onValueChanged.AddListener(OnToggle1ValueChanged);
        toggle2.onValueChanged.AddListener(OnToggle2ValueChanged);
    }

    void Update(){
        float lowPassCutoffValue = utilities.maxSonarFrequency;
        float highPassCutoffValue = 10;
        int arrayLen = resolutionPixelsX[0];
        for(int i = 0; i < soundSourcesArr.Length; i++){
            //Debug.Log("Hello");
            float rot =  utilities.getRelativeAngleAtSoundSource(i);
            //Debug.Log("Rot: " + rot);
            //Debug.Log("userControls.getSonarBeamWidth() / 2: " + (userControls.getSonarBeamWidth() / 2));
            //Debug.Log("(frequenciesDetectableRange[j] * Mathf.Rad2Deg / 2): " + (frequenciesDetectableRange[0] * Mathf.Rad2Deg / 2));
            lowPassCutoffValue = 10;
            highPassCutoffValue = utilities.maxSonarFrequency;

            /*
            for (int j = 0; j < frequenciesDetectableRange.Length; j++){
                 if ((rot < (userControls.getSonarBeamWidth() / 2) || (rot < (frequenciesDetectableRange[j] * Mathf.Rad2Deg / 2)))){
                    highPassCutoffValue = utilities.getFrequencyBin(arrayLen, utilities.getFrequencyArrayIndexLowBound(arrayLen));
                    lowPassCutoffValue = utilities.getFrequencyBin(arrayLen, utilities.getFrequencyArrayIndexUpperBound(arrayLen));
                    Debug.Log(highPassCutoffValue);
                    Debug.Log(lowPassCutoffValue);
                }
            }
            */

            if ((rot < (userControls.getSonarBeamWidth() / 2))){
                    highPassCutoffValue = utilities.getFrequencyBin(arrayLen, utilities.getFrequencyArrayIndexLowBound(arrayLen));
                    lowPassCutoffValue = utilities.getFrequencyBin(arrayLen, utilities.getFrequencyArrayIndexUpperBound(arrayLen));
                    //Debug.Log(highPassCutoffValue);
                    //Debug.Log(lowPassCutoffValue);
            }
            
        
            SetCutoffValues(soundSourcesArr[i], lowPassCutoffValue, highPassCutoffValue);
        }
        highPassCutoffValue = utilities.getFrequencyBin(arrayLen, utilities.getFrequencyArrayIndexLowBound(arrayLen));
        lowPassCutoffValue = utilities.getFrequencyBin(arrayLen, utilities.getFrequencyArrayIndexUpperBound(arrayLen));
        SetCutoffValues(noise, lowPassCutoffValue, highPassCutoffValue);
    }

    public void SetCutoffValues(GameObject source, float lp, float hp){
        AudioLowPassFilter lowPassFilter = source.GetComponent<AudioLowPassFilter>();
        AudioHighPassFilter highPassFilter = source.GetComponent<AudioHighPassFilter>();
        lowPassFilter.cutoffFrequency = lp;
        highPassFilter.cutoffFrequency = hp;
    }


    //toggles the (1 minute) plots
    private void OnToggle1ValueChanged(bool isOn)
    {
        UpdateSpectrogramTimeAxis(1);
        UpdateBearingTimeAxis(1);
        plotsGameObjects[0].GetComponent<MeshRenderer>().enabled = true;
        plotsGameObjects[1].GetComponent<MeshRenderer>().enabled = false;

        plotsGameObjects[2].GetComponent<MeshRenderer>().enabled = true;
        plotsGameObjects[3].GetComponent<MeshRenderer>().enabled = false;
    }

    //toggles the (5 minute) plots
    private void OnToggle2ValueChanged(bool isOn)
    {
        UpdateSpectrogramTimeAxis(2);
        UpdateBearingTimeAxis(2);
        plotsGameObjects[1].GetComponent<MeshRenderer>().enabled = true;
        plotsGameObjects[0].GetComponent<MeshRenderer>().enabled = false;

        plotsGameObjects[3].GetComponent<MeshRenderer>().enabled = true;
        plotsGameObjects[2].GetComponent<MeshRenderer>().enabled = false;
    }

    //pass it as argument which one of the plot's number of seconds which
    //the plot displays (E.g. spectrogram 1, spectrogram 2 and return 60 seconds and 300 seconds)
    //along with its type
    public int getNumberOfSecondsPlot(int plotNumber, string plotType){
        for(int i = 0; i < numbSecondsDisplayed.Length; i++){
            if(plotTypes[i] == plotType){
                plotNumber--;
                if(plotNumber == 0){
                    return numbSecondsDisplayed[i];
                }
            }
        }
        return 0;

    }

    public void UpdateSpectrogramTimeAxis(int spectrogramPlotNumber){
        int numbSeconds = getNumberOfSecondsPlot(spectrogramPlotNumber, "spectrogram");
        timeTextMeasureSpectrogram.text = "TIME (m)";
        timeTextAxisSpectrogram1.text = "0";
        timeTextAxisSpectrogram2.text = (numbSeconds / 60f / 2f).ToString("0.##");
        timeTextAxisSpectrogram3.text = (numbSeconds / 60f).ToString("0.##");
    }

    public void UpdateBearingTimeAxis(int bearingPlotNumber){
        int numbSeconds = getNumberOfSecondsPlot(bearingPlotNumber, "bearing");
        timeTextMeasureBearing.text = "TIME (m)";
        timeTextAxisBearing1.text = "0";
        timeTextAxisBearing2.text = (numbSeconds / 60f / 2f).ToString("0.##");
        timeTextAxisBearing3.text = (numbSeconds / 60f).ToString("0.##");
    }


    

}