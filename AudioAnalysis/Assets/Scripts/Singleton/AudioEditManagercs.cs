using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AudioPart
{
    public long startIndex;
    public long endIndex;
    public string[] textArr;
    public string[] textPingYinArr;
    public string audioPath;
    public bool mark = false;
}

class AudioEditManagercs:IDispose
{
    private static AudioEditManagercs instance_;

    public static AudioEditManagercs Instance
    {
        get
        {
            if (instance_ == null) { instance_ = new AudioEditManagercs(); }
            return instance_;
        }
    }

    private void cutAudio() { }

    //-------------对外接口------------------

    public void initWithNew(string strAudioPath) {

    }

    public void initWithOld(string outPutPath) {

    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
