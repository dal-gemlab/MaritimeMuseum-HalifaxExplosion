using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentationHelper
{
    public static List<byte[]> FragmentPackage(byte[] msg,  int PAYLOAD_SIZE)
    {
        int msgSize = msg.Length;
        List<byte[]> fragments = new List<byte[]>();

        int numFragments = Mathf.CeilToInt(((float)msgSize / PAYLOAD_SIZE));
        for (int i = 0; i < numFragments; i++)
        {

            if (i == numFragments - 1 && msgSize % PAYLOAD_SIZE != 0)
            {
                int pgkSize = msgSize % PAYLOAD_SIZE;
                byte[] frag = new byte[pgkSize];
                Array.Copy(msg, PAYLOAD_SIZE * i, frag, 0, pgkSize);
                fragments.Add(frag);
            }
            else
            {
                byte[] frag = new byte[PAYLOAD_SIZE];
                Array.Copy(msg, PAYLOAD_SIZE * i, frag, 0, PAYLOAD_SIZE);
                fragments.Add(frag);
            }
        }

        return fragments;
    }

    public static byte[] DefragmentPackage(List<byte[]> fragments, int msgSize, int PAYLOAD_SIZE)    
    {
        byte[] result = new byte[msgSize];

        for (int i = 0; i < fragments.Count; i++)
        {
            int pgkSize;
            if (i == fragments.Count - 1 && msgSize % PAYLOAD_SIZE != 0)
                pgkSize = msgSize % PAYLOAD_SIZE;
            else
                pgkSize = PAYLOAD_SIZE;

            Array.Copy(fragments[i], 0, result, i * PAYLOAD_SIZE, pgkSize);
            
        }
        return result;
    }

}
