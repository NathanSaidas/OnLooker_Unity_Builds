using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class UniqueNumberGenerator
    {
        private List<int> m_FreeNumbers = new List<int>();
        private int m_NextNumber = 0;

        public UniqueNumberGenerator()
        {

        }
        public UniqueNumberGenerator(int aStart)
        {
            m_NextNumber = aStart;
        }

        public int getUniqueNumber()
        {
            int unique = 0;
            if (m_FreeNumbers.Count > 0)
            {
                unique = m_FreeNumbers[0];
                m_FreeNumbers.RemoveAt(0);
                return unique;
            }

            unique = m_NextNumber;
            m_NextNumber++;
            return unique;
        }
        //Adds a number onto the free number List
        public void freeNumber(int aNumber)
        {
            if (m_FreeNumbers.Contains(aNumber))
            {
                return;
            }
            m_FreeNumbers.Add(aNumber);
        }

        public int getFreeNumberCount()
        {
            return m_FreeNumbers.Count;
        }
        public int getNextNumber()
        {
            return m_NextNumber;
        }
	}

}