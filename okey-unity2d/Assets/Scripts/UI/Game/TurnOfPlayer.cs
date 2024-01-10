using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOfPlayer : MonoBehaviour
{
    public GameObject[] toDisableGameObjectsOnTurn;
    public GameObject[] toEnableGameObjectsOnTurn;
    public GameObject toEnableTimerOnTurn; // it is a slider
    bool isTurnOfPlayer = false;

    public void SetUITurnOfPlayer(bool isTurnOfPlayer)
    {
        if (this.isTurnOfPlayer == isTurnOfPlayer)
        {
            return;
        }

        this.isTurnOfPlayer = isTurnOfPlayer;

        Debug.Log("SetUITurnOfPlayer: " + isTurnOfPlayer);

        foreach (var gameObject in toDisableGameObjectsOnTurn)
        {
            gameObject.SetActive(!isTurnOfPlayer);
        }

        foreach (var gameObject in toEnableGameObjectsOnTurn)
        {
            gameObject.SetActive(isTurnOfPlayer);
        }

        toEnableTimerOnTurn.SetActive(isTurnOfPlayer);

        if (isTurnOfPlayer)
        {
            StartCoroutine(StartTimer());
        }
    }

    IEnumerator StartTimer()
    {
        float time = 0;
        float maxTime = 30;
        while (time < maxTime)
        {
            time += Time.deltaTime;
            toEnableTimerOnTurn.GetComponent<UnityEngine.UI.Slider>().value = time / maxTime;
            yield return null;
        }
    }
}
