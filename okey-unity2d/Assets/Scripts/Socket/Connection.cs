using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

// SignalR socket connection
// singleton class
public class Connection : MonoBehaviour
{
    private static Connection instance;
    private readonly string hubUrl = "http://localhost:5034/chatHub";
    private readonly string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI3OWRlM2VhMC1iOWVkLTQzMjgtODA5Zi1mMjQzMjlmNTExM2IiLCJuYmYiOjE2OTQ3MDMyMjYsImV4cCI6MTY5NDczMzIyNiwiaXNzIjoiSXNzdWVySW5mb3JtYXRpb24iLCJhdWQiOiJBdWRpZW5jZUluZm9ybWF0aW9uIn0.Ktf-gvbGjLtID48pnb-kIOEfWn5ztld4RWwIWxwqqow";
    public GameData latestData = null;
    public GameData previousData = null;
    public HubConnection hubConnection;
    private TurnOfPlayer turnOfPlayerGameObject;

    private readonly string user = Guid.NewGuid().ToString();

    private async void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.Headers["Authorization"] = "Bearer " + jwtToken;
            })
            .Build();

        hubConnection.On<string, string>("ReceiveMessage", async (user, message) => await Task.Run(() =>
           {
               latestData = JsonSerializer.Deserialize<GameData>(message);
           }));

        hubConnection.On<string>("ReceivePlayerNamesAndPositionsAtTheGroup", async (message) =>
            {
                await ReceivePlayerNamesAndPositionsAtTheGroup(message);
            });

        await hubConnection.StartAsync();

        UnityEngine.Debug.Log("Connection started");

        await hubConnection.InvokeAsync("JoinOrCreateGroup", user);

        previousData = new GameData();

        // set the turn UI according to the data
        turnOfPlayerGameObject = GameObject.Find("TurnOfPlayer").GetComponent<TurnOfPlayer>();

        Invoke(nameof(StartTheCoroutine), 0.2f);
    }

    private async Task ReceivePlayerNamesAndPositionsAtTheGroup(string message)
    {

        UnityEngine.Debug.LogWarning("ReceivePlayerNamesAndPositionsAtTheGroup()");

        Room.Instance.PlayerNames = JsonSerializer.Deserialize<List<string>>(message);

        // stop listening this function
        hubConnection.Remove("ReceivePlayerNamesAndPositionsAtTheGroup");

        UnityEngine.Debug.LogWarning("Rqwelfwlekfwlekflwkef");

        Room.Instance.MyIndexInGroup = Room.Instance.PlayerNames.IndexOf(user);

        UnityEngine.Debug.LogWarning("ReceivePlayerNamesAndPositionsAtTheGroup: " + message);
        UnityEngine.Debug.LogWarning("MyIndexInGroup: " + Room.Instance.MyIndexInGroup);
    }

    private void StartTheCoroutine()
    {
        StartCoroutine(TickForUpdate());
    }

    public static Connection GetInstance()
    {
        return instance;
    }

    private IEnumerator TickForUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            if (latestData != null)
            {
                previousData.PlayerPieces ??= new List<string>();

                if (previousData.PlayerPieces != latestData.PlayerPieces) // if the player pieces has changed the update the UI
                {
                    Room.Instance.OkeyPiece = latestData.OkeyPiece;

                    previousData = latestData;
                    SetPieces.SetPiecesOnWoodGrid16x2();
                }

                if (latestData.IsTurnOfPlayer == true) // if it is turn of player
                {
                    // enable the buttons
                    turnOfPlayerGameObject.SetUITurnOfPlayer(true);
                }
                else
                {
                    // disable the buttons
                    turnOfPlayerGameObject.SetUITurnOfPlayer(false);
                }

            }
        }
    }

    public async void ButtonClick(int code)
    {
        switch (code)
        {
            case 06:
                await SortSeries();
                break;
            case 07:
                await SortPairs();
                break;
        }
    }

    private async System.Threading.Tasks.Task SortSeries()
    {
        UnityEngine.Debug.Log("SortSeries");
        string message = "06";
        await SendMessage(user, message);
    }

    private async System.Threading.Tasks.Task SortPairs()
    {
        UnityEngine.Debug.Log("SortPairs");
        string message = "07";
        await SendMessage(user, message);
    }

    private async System.Threading.Tasks.Task SendMessage(string user, string message)
    {
        await hubConnection.InvokeAsync("SendMessage", user, null, null, message);
    }

    public async void SendOpenRequest(bool isSeriesNotPairs, List<List<string>> validGroups)
    {
        // Todo: send the open request

        await hubConnection.InvokeAsync("OpenRequest", user, isSeriesNotPairs, validGroups);
    }



}
