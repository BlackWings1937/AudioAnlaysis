using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ArrayUtil<T>
{
    /*
     * 判断两个数组顺序散列相似度
     */
    public static float CaculateRegularSimilar(T[] a, T[] b)
    {
        float rate = 0;
        T[] arrLength = null;
        T[] arrShort = null;
        if (a.Length>b.Length) {
            arrLength = a;
            arrShort = b;
        } else {
            arrLength = b;
            arrShort = a;
        }
        int lengthStartIndex = 0;
        float grade = 0;
        for (int i = 0;i<arrShort.Length;++i) {
            for (int z = lengthStartIndex; z<arrLength.Length;++z) {
                if (arrShort[i].Equals(arrLength[z])) {
                    lengthStartIndex = z + 1;
                    grade += 1;
                    
                }
            }
        }
        rate = grade / (float)a.Length;
        return rate;
    }
}
