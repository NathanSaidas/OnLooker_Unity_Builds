using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{
    public class NetState
    {
        public float timeStamp = 0.0f;
        public Vector3 pos = Vector3.zero;
        public Quaternion rot = Quaternion.identity;


        public NetState()
        {

        }
        public NetState(float time, Vector3 aPos, Quaternion aRot)
        {
            timeStamp = time;
            pos = aPos;
            rot = aRot;
        }

    }


	public class NetPredictor : MonoBehaviour {

        //The transform that will be observed the data here will be sent over the network
        public Transform observedTransform;
        //This is the client side player manager that is receiving the new server data.
        public NetClientInput receiver;
        //This is the top margin which is added to the current average player ping to counter lagspikes during movement prediction of remote clients.
        public float pingMargin;


        private float clientPing;
        
        //1st is latest
        //Last is oldest
        private NetState[] serverStateBuffer = new NetState[20];

        public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            Vector3 pos = observedTransform.position;
            Quaternion rot = observedTransform.rotation;

            if (stream.isWriting)
            {
                stream.Serialize(ref pos);
                stream.Serialize(ref rot);
            }
            else
            {
                stream.Serialize(ref pos);
                stream.Serialize(ref rot);

                receiver.serverPos = pos;
                receiver.serverRot = rot;

                receiver.lerpToTarget();

                for (int i = serverStateBuffer.Length - 1; i >= 1; i--)
                {
                    serverStateBuffer[i] = serverStateBuffer[i - 1];
                }

                serverStateBuffer[0] = new NetState((float)info.timestamp, pos, rot);
            }
        }

        public void Update()
        {
            if (Network.player == receiver.getOwner() || Network.isServer)
            {
                return;
            }


            clientPing = (Network.GetAveragePing(Network.connections[0]) / 100.0f) + pingMargin;
            float interpolationTime = (float)Network.time - clientPing;

            if (serverStateBuffer[0] == null)
            {
                serverStateBuffer[0] = new NetState(0, transform.position, transform.rotation);
            }

            if (serverStateBuffer[0].timeStamp > interpolationTime)
            {
                for (int i = 0; i < serverStateBuffer.Length; i++)
                {
                    if (serverStateBuffer[i] == null)
                    {
                        continue;
                    }

                    if (serverStateBuffer[i].timeStamp <= interpolationTime || i == serverStateBuffer.Length - 1)
                    {
                        NetState bestTarget = serverStateBuffer[Mathf.Max(i - 1, 0)];
                        NetState bestStart = serverStateBuffer[i];

                        float timediff = bestTarget.timeStamp - bestStart.timeStamp;
                        float lerpTime = 0.0f;

                        if (timediff > 0.0001f)
                        {
                            lerpTime = ((interpolationTime - bestStart.timeStamp) / timediff);
                        }

                        transform.position = Vector3.Lerp(bestStart.pos, bestTarget.pos, lerpTime);
                        transform.rotation = Quaternion.Slerp(bestStart.rot, bestTarget.rot, lerpTime);
                        return;
                    }
                }
            }
            else
            {
                NetState latest = serverStateBuffer[0];
                transform.position = Vector3.Lerp(transform.position, latest.pos, 0.5f);
                transform.rotation = Quaternion.Lerp(transform.rotation, latest.rot, 0.5f);
            }

        }
	}

}