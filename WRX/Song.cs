using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace TransitServer
{
    public class SongWMP
    {
        private WindowsMediaPlayer wplayer;
        private bool isPlay;
        private bool isMute;
        public SongWMP(string URL)
        {
            wplayer = new WindowsMediaPlayer();
            wplayer.URL = URL;
            wplayer.controls.stop();
            isPlay = false;
            isMute = false;
        }
        public void Play()
        {
            wplayer.controls.play();
            isPlay = true;
        }
        public void Stop()
        {
            wplayer.controls.stop();
            isPlay = false;
        }
        public bool IsPlay()
        {
            return isPlay;
        }
        public void Mute()
        {
            wplayer.settings.volume = (isMute) ? 100 : 0;
            isMute = !isMute;
        }
        public bool IsMute()
        {
            return isMute;
        }
    }
}
