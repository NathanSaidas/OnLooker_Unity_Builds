using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    //Network Component will be the base component for all future components
    //This is what will be used when handling NetworkHandle ids and such
	public class NetworkComponent : MonoBehaviour
    {
        private NetworkHandle m_Handle = new NetworkHandle();
        private NetworkConnectionInfo m_ConnectionInfo;
        private bool m_IsServerActive = true;
        //Used to determine if the despawn flag was recieved - All NetworkComponents should have this
        //flag raised before a despawn occur properly. Sometimes they will be forced though
        //private bool m_DespawnFlagRecieved = false;

        //Used to determine if the flag was recieved
        private bool m_SetHandleFlagRecieved = false;

        //NetworkValue - class
        //type
        //name
        //Shader Program Example:
        //addFloat
        //addInt
        //addString
        //getFloat
        //getInt
        //getString

        private List<Value> m_ServerData = new List<Value>();
        private List<INetworkListener> m_Listeners = new List<INetworkListener>();

        private bool valueExists(string aName)
        {
            for (int i = 0; i < m_ServerData.Count; i++)
            {
                if (m_ServerData[i].name == aName)
                {
                    return true;
                }
            }
            return false;
        }

        public void registerListener(INetworkListener aListener)
        {
            if (aListener != null)
            {
                m_Listeners.Add(aListener);
            }
        }
        public void unregisterListener(INetworkListener aListener)
        {
            if (aListener != null)
            {
                m_Listeners.Remove(aListener);
            }
        }
        #region SHADER_DESIGN
        public void addFloat(string aName, float aValue)
        {
            if (valueExists(aName) == false)
            {
                m_ServerData.Add(new FloatValue(aName, aValue));
            }
        }
        public void addInt(string aName, int aValue)
        {
            if (valueExists(aName) == false)
            {
                m_ServerData.Add(new IntValue(aName, aValue));
            }
        }
        public void addShort(string aName, short aValue)
        {
            if (valueExists(aName) == false)
            {
                m_ServerData.Add(new ShortValue(aName, aValue));
            }
        }
        public void addByte(string aName, byte aValue)
        {
            if (valueExists(aName) == false)
            {
                m_ServerData.Add(new ByteValue(aName, aValue));
            }
        }
        public void addVector3(string aName, Vector3 aValue)
        {
            if (valueExists(aName) == false)
            {
                m_ServerData.Add(new VectorValue(aName, aValue));
            }
        }
        public void addQuaternion(string aName, Quaternion aValue)
        {
            if (valueExists(aName) == false)
            {
                m_ServerData.Add(new QuaternionValue(aName, aValue));
            }
        }

        public void addValue(Value aValue)
        {
            if (aValue == null)
            {
                return;
            }
            if (valueExists(aValue.name) == false)
            {
                m_ServerData.Add(aValue);
            }
        }

        public void removeValue(string aName)
        {
            for (int i = 0; i < m_ServerData.Count; i++)
            {
                if (m_ServerData[i].name == aName)
                {
                    m_ServerData.Remove(m_ServerData[i]);
                    break;
                }
            }
        }

        public Value getValue(string aName)
        {
            for (int i = 0; i < m_ServerData.Count; i++)
            {
                if (m_ServerData[i].name == aName)
                {
                    return m_ServerData[i];
                }
            }
            return null;
        }

        public float getFloat(string aName)
        {
            Value floatValue = getValue(aName);
            if (floatValue != null)
            {
                if (floatValue.valueType == ValueType.FLOAT)
                {
                    return (floatValue as FloatValue).data;
                }
            }
            return 0.0f;
        }

        public int getInt(string aName)
        {
            Value intValue = getValue(aName);
            if (intValue != null)
            {
                if (intValue.valueType == ValueType.INT)
                {
                    return (intValue as IntValue).data;
                }
            }
            return 0;
        }
        public int getShort(string aName)
        {
            Value shortValue = getValue(aName);
            if (shortValue != null)
            {
                if (shortValue.valueType == ValueType.SHORT)
                {
                    return (shortValue as ShortValue).data;
                }
            }
            return 0;
        }

        public int getByte(string aName)
        {
            Value byteValue = getValue(aName);
            if (byteValue != null)
            {
                if (byteValue.valueType == ValueType.BYTE)
                {
                    return (byteValue as ByteValue).data;
                }
            }
            return 0;
        }

        public Vector3 getVector(string aName)
        {
            Value vectorValue = getValue(aName);
            if (vectorValue != null)
            {
                if (vectorValue.valueType == ValueType.VECTOR3)
                {
                    return (vectorValue as VectorValue).data;
                }
            }
            return Vector3.zero;
        }
        public Quaternion getQuaternion(string aName)
        {
            Value quaternionValue = getValue(aName);
            if (quaternionValue != null)
            {
                if (quaternionValue.valueType == ValueType.QUATERNION)
                {
                    return (quaternionValue as QuaternionValue).data;
                }
            }
            return Quaternion.identity;
        }
        #endregion

        public void setOwner(NetworkPlayer aOwner, string aUsername, int aHandle)
        {
            networkView.RPC("setHandle", RPCMode.AllBuffered, aOwner, aUsername, aHandle);
        }
        //RPC Notes:
        //Requires NetworkView to use
        //If a NetworkView is being used just for RPC calls, state sync should be turned off
        //and observed can be set to none
        //RPC function names should be unique across the scene, should there be two functions in different scripts
        //only one of them is invoked
        //Gets called when the object gets instantiated on the server to all clients from the server instance
        [RPC]
        private void setHandle(NetworkPlayer aOwner, string aUsername, int aHandle)
        {
            if (aOwner == null)
            {
                Debug.LogWarning("Set Handle - aOwner was found null");
            }
            if (aUsername == string.Empty)
            {
                Debug.LogWarning("Set Handle - aUsername was found empty");
            }
            if (aHandle == 0)
            {
                Debug.LogWarning("Set Handle - aHandle was found as null handle");
            }
            if (m_Handle != null)
            {
                m_Handle.networkPlayer = aOwner;
                m_Handle.username = aUsername;
                m_Handle.id = aHandle;
            }
            m_SetHandleFlagRecieved = true;
        }
        //Called when the server wants to make this component on all clients active on all clients
        //The handle ID is passed so that it may be checked with for a specific instance call
        //The user who sent this request has their name passed in as a string as well
        
        //Think of a scenario where the player has disconnected and you dont want to make any moves immediately.
        //Wait 5 seconds and then despawn this object from the server or free it from the server or run something
        [RPC]
        private void activate(int aHandle,string aSender)
        {
            if (m_Handle != null && aHandle == m_Handle.id)
            {
                m_IsServerActive = true;
                //TODO: Call onActivateInstance
            }
            //TODO: Call onActivate
        }
        //Called when the server wants to make this component on all clients deactivate
        //The handle ID is passed so that it may be checked with for a specific instance
        //The user who sent this request has their name passed in as a string as well
        [RPC]
        private void deactivate(int aHandle,string aSender)
        {
            if (m_Handle != null && aHandle == m_Handle.id)
            {
                m_IsServerActive = false;
                //TODO: Call onDeactivateInstance
            }
            //TODO: Call onDeactivate
        }

        //This will get called at the server start as well as when the player reconnects to the game
        //If the player were to disconnect we might run a routine and wait for them to connect back
        //Say if they did this RPC would get invoked and we can do a username check to see if they are the same user
        //(And apply a password for security)
        [RPC]
        private void playerConnected(NetworkPlayer aPlayer, string aUsername)
        {
            if (m_Handle != null)
            {
                if (m_Handle.username == aUsername)
                {
                    if (m_Handle.networkPlayer == aPlayer)
                    {
                        //This is the same player at the same computer
                    }
                    else
                    {
                        //This is possibly the same player however they might be at a different computer
                    }
                }
            }
        }

        //This gets called when the player has disconnected
        [RPC]
        private void playerDisconnected(NetworkPlayer aPlayer, string aUsername)
        {
            if (m_Handle != null)
            {
                if (m_Handle.username == aUsername)
                {
                    if (m_Handle.networkPlayer == aPlayer)
                    {
                        //This is the same player at the same computer
                    }
                    else
                    {
                        //This is possibly the same player however they might be at a different computer
                    }
                }
            }
        }


        //Called when the server initialially spawns the object (This is not the same as activate / deactivate)
        //Nor is this the same as Start or Awake or Enable
        
        //An Object will be instantiated on the server. The first thing that will need to happen is the handle will need to be
        //set to state object player ownership as well as instance ownership on the server.
        //Once that state has been set a serverSpawned call will be made
        [RPC] 
        private void serverSpawned()
        {
        
        }

        public void networkDestroy()
        {
            networkView.RPC("netDestroy", RPCMode.AllBuffered);
        }
        [RPC]
        private void netDestroy()
        {
            NetworkServer.instance.objectDestroyed(handle.id);
            Network.RemoveRPCs(networkView.viewID);
            Network.Destroy(gameObject);
        }

		// Use this for initialization
		void Start ()
        {
		    
		}
		
		// Update is called once per frame
		void Update () {
		
		}

        public void OnSerializeNetworkView(BitStream aStream, NetworkMessageInfo aInfo)
        {
            if (m_ServerData.Count == 0)
            {
                return;
            }

            if (aStream.isWriting)
            {
                for (int i = 0; i < m_ServerData.Count; i++)
                {
                    if((m_ServerData[i].accessFlag & AccessFlag.WRITE) == AccessFlag.WRITE)
                    {
                        switch (m_ServerData[i].valueType)
                        {
                            case ValueType.FLOAT:
                                {
                                    float value = (m_ServerData[i] as FloatValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as FloatValue).data = value;
                                }
                                break;
                            case ValueType.INT:
                                {
                                    int value = (m_ServerData[i] as IntValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as IntValue).data = value;
                                }
                                break;
                            case ValueType.SHORT:
                                {
                                    short value = (m_ServerData[i] as ShortValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as ShortValue).data = value;
                                }
                                break;
                            case ValueType.BYTE:
                                {
                                    char value = (char)(m_ServerData[i] as ByteValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as ByteValue).data = (byte)value;
                                }
                                break;

                            case ValueType.VECTOR3:
                                {
                                    Vector3 value = (m_ServerData[i] as VectorValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as VectorValue).data = value;
                                }
                                break;
                            case ValueType.QUATERNION:
                                {
                                    Quaternion value = (m_ServerData[i] as QuaternionValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as QuaternionValue).data = value;
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_ServerData.Count; i++)
                {
                    if ((m_ServerData[i].accessFlag & AccessFlag.READ) == AccessFlag.READ)
                    {
                        switch (m_ServerData[i].valueType)
                        {
                            case ValueType.FLOAT:
                                {
                                    float value = (m_ServerData[i] as FloatValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as FloatValue).data = value;
                                }
                                break;
                            case ValueType.INT:
                                {
                                    int value = (m_ServerData[i] as IntValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as IntValue).data = value;
                                }
                                break;
                            case ValueType.SHORT:
                                {
                                    short value = (m_ServerData[i] as ShortValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as ShortValue).data = value;
                                }
                                break;
                            case ValueType.BYTE:
                                {
                                    char value = (char)(m_ServerData[i] as ByteValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as ByteValue).data = (byte)value;
                                }
                                break;

                            case ValueType.VECTOR3:
                                {
                                    Vector3 value = (m_ServerData[i] as VectorValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as VectorValue).data = value;
                                }
                                break;
                            case ValueType.QUATERNION:
                                {
                                    Quaternion value = (m_ServerData[i] as QuaternionValue).data;
                                    aStream.Serialize(ref value);
                                    (m_ServerData[i] as QuaternionValue).data = value;
                                }
                                break;
                        }
                    }
                }
            }
        }

        [RPC]
        public void callMethodString(string aMethodName, string aData)
        {

        }
        [RPC]
        public void callMethodInt(string aMethodName, int aData)
        {

        }
        [RPC]
        public void callMethodVector(string aMethodName, Vector3 aData)
        {

        }

        [RPC]
        public void setPosition(Vector3 aPosition)
        {
            for (int i = 0; i < m_Listeners.Count; i++)
            {
                m_Listeners[i].callMethod("setPosition", new object[] {aPosition});
            }
        }

        [RPC]
        public void setState(int aState)
        {
            for (int i = 0; i < m_Listeners.Count; i++)
            {
                m_Listeners[i].callMethod("setState", new object[] { aState });
            }
        }
        
        public NetworkHandle handle
        {
            get { return handle; }
        }
        public bool isServerActive
        {
            get { return m_IsServerActive; }
            set
            {
                if (m_IsServerActive == true)
                {
                    networkView.RPC("activate", RPCMode.AllBuffered, m_Handle.id, m_Handle.username);
                }
                else
                {
                    networkView.RPC("deactivate", RPCMode.AllBuffered, m_Handle.id, m_Handle.username);
                }
            }
        }
	}

}