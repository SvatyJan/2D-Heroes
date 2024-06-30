using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class DayNightCycleController : MonoBehaviour
{
    [SerializeField] public Light2D globalLight;

    [SerializeField] private float dayDurationSec = 50f;
    [SerializeField] private float duskDurationSec = 10f;
    [SerializeField] private float nightDurationSec = 20f;
    [SerializeField] private float dawnDurationSec = 10f;

    [SerializeField] private float dayIntensity = 1f;
    [SerializeField] private float nightIntensity = 0.2f;

    [SerializeField] public TextMeshProUGUI timeDisplay;

    private float currentTime = 0f;
    private float currentDayTime = 0f;
    private float currentNightTime = 0f;
    private bool isDay = true;

    public enum DayPhase { Day, Nightfall, Night, Dawn }
    public DayPhase currentDayPhase;

    void Update()
    {
        CalculateDayNightCycle();
        UpdateTimeDisplay();
    }

    private void CalculateDayNightCycle()
    {
        // Aktualizace èasu
        currentTime += Time.deltaTime;

        if (isDay)
        {
            if (currentTime <= dayDurationSec)
            {
                // Bìhem dne
                globalLight.intensity = dayIntensity;
                currentDayTime = currentTime;
                currentDayPhase = DayPhase.Day;
            }
            else if (currentTime <= dayDurationSec + duskDurationSec)
            {
                // Bìhem stmívání
                float t = (currentTime - dayDurationSec) / duskDurationSec;
                globalLight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, t);
                currentDayTime = currentTime;
                currentDayPhase = DayPhase.Nightfall;
            }
            else
            {
                // Konec dne, zaèíná noc
                currentTime = 0f;
                isDay = false;
            }
        }
        else
        {
            if (currentTime <= nightDurationSec)
            {
                // Bìhem noci
                globalLight.intensity = nightIntensity;
                currentNightTime = currentTime;
                currentDayPhase = DayPhase.Night;
            }
            else if (currentTime <= nightDurationSec + dawnDurationSec)
            {
                // Bìhem svítání
                float t = (currentTime - nightDurationSec) / dawnDurationSec;
                globalLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, t);
                currentNightTime = currentTime;
                currentDayPhase = DayPhase.Dawn;
            }
            else
            {
                // Konec noci, zaèíná den
                currentTime = 0f;
                isDay = true;
            }
        }
    }

    void UpdateTimeDisplay()
    {
        // Aktualizace textu dle fáze dne
        switch (currentDayPhase)
        {
            case DayPhase.Day:
                timeDisplay.text = string.Format("Day {0:0.0}/{1:0.0}", currentTime, dayDurationSec);
                break;
            case DayPhase.Nightfall:
                timeDisplay.text = string.Format("Nightfall {0:0.0}/{1:0.0}", currentTime - dayDurationSec, duskDurationSec);
                break;
            case DayPhase.Night:
                timeDisplay.text = string.Format("Night {0:0.0}/{1:0.0}", currentTime, nightDurationSec);
                break;
            case DayPhase.Dawn:
                timeDisplay.text = string.Format("Dawn {0:0.0}/{1:0.0}", currentTime - nightDurationSec, dawnDurationSec);
                break;
        }
    }
}
