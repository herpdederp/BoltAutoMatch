using System;
using Bolt;
using UdpKit;
using UnityEngine;
using udpkit.platform.photon;
using udpkit.platform.photon.photon;
using Bolt.photon;
using Bolt.Utils;
public class Callbacks : Bolt.GlobalEventListener
{
    bool full;

    public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
    {

        if (BoltNetwork.IsServer)
        {
            if (full == false)
            {
                full = true;
                PhotonRoomProperties myToken = new PhotonRoomProperties
                {
                    IsOpen = false,
                    IsVisible = false
                };

                var matchName = Guid.NewGuid().ToString();

                BoltNetwork.SetServerInfo(matchName, myToken);

                BoltNetwork.Accept(endpoint);
            }
            else
            {
                BoltNetwork.Refuse(endpoint);
            }

        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
