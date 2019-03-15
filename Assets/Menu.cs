using System;
using Bolt;
using UdpKit;
using UnityEngine;


public class Menu : Bolt.GlobalEventListener
{
    bool noServersFound;
    float timer = 0;

    private void FixedUpdate()
    {
        if (BoltNetwork.IsClient == true)
        {
            timer += Time.fixedDeltaTime;

            if (timer > 5f)
            {
                noServersFound = true;
                BoltLauncher.Shutdown();
            }

        }
    }

    public override void BoltShutdownBegin(AddCallback registerDoneCallback)
    {
        registerDoneCallback(Test0);
    }

    void Test0()
    {
        BoltLauncher.StartServer();
    }


    private void OnGUI()
    {
        if (BoltNetwork.IsRunning == false)
        {

            GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

            if (GUILayout.Button("Start Server", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                // START SERVER
                BoltLauncher.StartServer();
            }

            if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                // START CLIENT
                BoltLauncher.StartClient();
            }

            GUILayout.EndArea();
        }
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            var matchName = Guid.NewGuid().ToString();

            BoltNetwork.SetServerInfo(matchName, null);
            BoltNetwork.LoadScene("game");
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        foreach (var session in sessionList)
        {
            var photonSession = session.Value;
            if (photonSession.Source == UdpSessionSource.Photon)
                BoltNetwork.Connect(photonSession);
        }
    }
}

