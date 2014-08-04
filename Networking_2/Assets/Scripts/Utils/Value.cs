using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnLooker
{
    public class AccessFlag
    {
        public const byte NONE = 0;
        public const byte READ = 1;
        public const byte WRITE = 2;
    }

    public enum ValueType
    {
        VALUE,
        FLOAT,
        INT,
        SHORT,
        BYTE,
        VECTOR3,
        QUATERNION
    }
    [Serializable]
    public class Value
    {
        public static readonly Type[] ACCEPTED_TYPES = new Type[] {typeof(int),typeof(float),typeof(string)};
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private object m_Data;
        [SerializeField]
        private byte m_AccessFlag;


        public Value(string aName)
        {
            m_Name = aName;
            m_AccessFlag = AccessFlag.READ | AccessFlag.WRITE;
            
        }

        public string name
        {
            get { return m_Name; }
            protected set { m_Name = value; }
        }

        protected object hiddenData
        {
            get { return m_Data; }
            set { m_Data = value; }
        }
        public int accessFlag
        {
            get { return m_AccessFlag; }
        }
        public void setReadOnly()
        {
            m_AccessFlag = AccessFlag.READ;
        }
        public void setWriteOnly()
        {
            m_AccessFlag = AccessFlag.WRITE;
        }
        public void setReadAndWrite()
        {
            m_AccessFlag = AccessFlag.READ | AccessFlag.WRITE;
        }

        public virtual ValueType valueType
        {
            get { return ValueType.VALUE; }
        }
    }
    [Serializable]
    public class FloatValue : Value
    {
        public FloatValue(string aName) : base(aName)
        {

        }
        public FloatValue(string aName, float aData) : base(aName)
        {
            data = aData;
        }
        public float data
        {
            get { return (float)hiddenData; }
            set { hiddenData = value; }
        }
        public override ValueType valueType
        {
            get { return ValueType.FLOAT; }
        }
    }
    [Serializable]
    public class IntValue : Value
    {
        public IntValue(string aName)
            : base(aName)
        {

        }
        public IntValue(string aName, int aData)
            : base(aName)
        {
            data = aData;
        }
        public int data
        {
            get { return (int)hiddenData; }
            set { hiddenData = value; }
        }
        public override ValueType valueType
        {
            get { return ValueType.INT; }
        }
    }
    [Serializable]
    public class ShortValue : Value
    {
        public ShortValue(string aName)
            : base(aName)
        {

        }
        public ShortValue(string aName, short aData)
            : base(aName)
        {
            data = aData;
        }
        public short data
        {
            get { return (short)hiddenData; }
            set { hiddenData = value; }
        }
        public override ValueType valueType
        {
            get { return ValueType.SHORT; }
        }
    }
    [Serializable]
    public class ByteValue : Value
    {
        public ByteValue(string aName)
            : base(aName)
        {

        }
        public ByteValue(string aName, byte aData)
            : base(aName)
        {
            data = aData;
        }
        public byte data
        {
            get { return (byte)hiddenData; }
            set { hiddenData = value; }
        }
        public override ValueType valueType
        {
            get { return ValueType.BYTE; }
        }
    }
    [Serializable]
    public class VectorValue : Value
    {
        public VectorValue(string aName)
            : base(aName)
        {

        }
        public VectorValue(string aName, Vector3 aData)
            : base(aName)
        {
            data = aData;
        }
        public Vector3 data
        {
            get { return (Vector3)hiddenData; }
            set { hiddenData = value; }
        }
        public override ValueType valueType
        {
            get { return ValueType.VECTOR3; }
        }
    }
    [Serializable]
    public class QuaternionValue : Value
    {
        public QuaternionValue(string aName)
            : base(aName)
        {

        }
        public QuaternionValue(string aName, Quaternion aData)
            : base(aName)
        {
            data = aData;
        }
        public Quaternion data
        {
            get { return (Quaternion)hiddenData; }
            set { hiddenData = value; }
        }
        public override ValueType valueType
        {
            get { return ValueType.QUATERNION; }
        }
    }
}
