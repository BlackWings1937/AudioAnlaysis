using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
public class StringUtil
{
    /*
     * 比较中文字符串a 与b 的相似程度
     * 参数:
     * a:主字符串
     * b:比较字符串
     * 返回
     * 相似度结果【0~1】
     */
    public static float CaculateStringSimilar(string a,string b) {
        float rate = 0;

        string chinaWordA = GetChinaWordFromString(a);
        string chinaWordB = GetChinaWordFromString(b);

        string[] pingYingA = GetPingYingArrayFromChinaString(chinaWordA);
        string[] pingYingB = GetPingYingArrayFromChinaString(chinaWordB);

        rate = ArrayUtil<string>.CaculateRegularSimilar(pingYingA, pingYingB);

        return rate;
    }

    /*
     * 比较两个拼音的相似度
     * 参数:
     * a:拼音a
     * b:拼音b
     * 返回
     * 相似度[0~1]
     */
    public static float CaculatePingYingSimilar(string a,string b) {
        float rate = 0;

        /*
         * 暂时以顺序散列相似度为标准计算
         */
        string[] arrA = GetStringArrayFromString(a);
        string[] arrB = GetStringArrayFromString(b);
        rate = ArrayUtil<string>.CaculateRegularSimilar(arrA,arrB);

        return rate;
    }

    

    /*
     * 将字符串转化为字符串数组
     * 参数:
     * a:目标字符串
     * 返回
     * 字符串数组
     */
    public static string[] GetStringArrayFromString(string a) {
        char[] arr = a.ToCharArray();
        string[] back = new string[arr.Length];
        for (int i = 0; i < arr.Length; ++i) back[i] = arr[i].ToString(); 
        return back;
    }

    /*
     * 将中文字符串转化成拼音数组
     * 参数:
     * a:中文字符串
     * 返回
     * 逐子拼音数组
     */
    public static string[] GetPingYingArrayFromChinaString(string a) {
        char[] arr = a.ToCharArray();
        string[] back = new string[arr.Length];
        for (int i = 0; i < arr.Length; ++i) back[i] = PinYinHelper.ConvertToAllSpell(arr[i].ToString());
        return back;
    }

    /*
     * 提取字符串中的中文
     * 参数:
     * a:目标字符串
     * 返回
     * 提纯后的中文字符串
     */
    public static string GetChinaWordFromString(string a) {
        string backStr = "";
        Regex reg = new Regex("[\u4e00-\u9fa5]+");
        foreach (Match v in reg.Matches(a))
            backStr = backStr + v;
        return backStr;
    }
}
