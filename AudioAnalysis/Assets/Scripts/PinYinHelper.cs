﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Baidu;
//using Microsoft.Office.Interop.Excel;
using Microsoft.International.Converters.PinYinConverter;
public class PinYinHelper
{
    private static Encoding gb2312 = Encoding.GetEncoding("GB2312");

    /// <summary>
    /// 汉字转全拼
    /// </summary>
    /// <param name="strChinese"></param>
    /// <returns></returns>
    public static string ConvertToAllSpell(string strChinese, IDictionary<char, string> pinyinDic = null)
    {
        try
        {
            if (strChinese.Length != 0)
            {
                StringBuilder fullSpell = new StringBuilder();
                for (int i = 0; i < strChinese.Length; i++)
                {
                    var chr = strChinese[i];
                    string pinyin = string.Empty;
                    if (i == 0)
                    {
                        pinyin = GetFromPinYinDic(chr, pinyinDic);
                    }
                    if (pinyin.Length == 0)
                    {
                        pinyin = GetSpell(chr);
                    }
                    fullSpell.Append(pinyin);
                }

                return fullSpell.ToString().ToLower();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("全拼转化出错！" + e.Message);
        }

        return string.Empty;
    }

    /// <summary>
    /// 汉字转首字母
    /// </summary>
    /// <param name="strChinese"></param>
    /// <returns></returns>
    public static string GetFirstSpell(string strChinese)
    {
        //NPinyin.Pinyin.GetInitials(strChinese)  有Bug  洺无法识别
        //return NPinyin.Pinyin.GetInitials(strChinese);

        try
        {
            if (strChinese.Length != 0)
            {
                StringBuilder fullSpell = new StringBuilder();
                for (int i = 0; i < strChinese.Length; i++)
                {
                    var chr = strChinese[i];
                    fullSpell.Append(GetSpell(chr)[0]);
                }

                return fullSpell.ToString().ToUpper();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("首字母转化出错！" + e.Message);
        }

        return string.Empty;
    }

    private static string GetSpell(char chr)
    {
        var coverchr = NPinyin.Pinyin.GetPinyin(chr);
        bool isChineses = ChineseChar.IsValidChar(coverchr[0]);
        if (isChineses)
        {
            ChineseChar chineseChar = new ChineseChar(coverchr[0]);
            foreach (string value in chineseChar.Pinyins)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return value.Remove(value.Length - 1, 1);
                }
            }
        }

        return coverchr;

    }

    /// <summary>
    /// 从字典获取拼音
    /// </summary>
    /// <param name="c">字</param>
    /// <param name="pinyinDic">字典</param>
    /// <returns></returns>
    private static string GetFromPinYinDic(char c, IDictionary<char, string> pinyinDic)
    {
        if (pinyinDic == null
            || pinyinDic.Count == 0
            || !pinyinDic.ContainsKey(c))
        {
            return "";
        }

        return pinyinDic[c];
    }
}