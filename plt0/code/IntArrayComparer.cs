using System.Collections.Generic;

public class IntArrayComparer : IComparer<int[]>
{
    public int Compare(int[] ba, int[] bb)
    {
        int n = ba.Length;  //fetch the length of the first array
        int ci = n.CompareTo(bb.Length); //compare to the second
        if (ci != 0)
        { //if not equal return the compare result
            return ci;
        }
        else
        { //else elementwise comparer
            for (int i = 0; i < n; i++)
            {
                if (ba[i] != bb[i])
                { //if not equal element, return compare result
                    return bb[i].CompareTo(ba[i]);
                }
            }
            return 0; //if all equal, return 0
        }
    }
}