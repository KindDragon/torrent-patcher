using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace TorrentPatcher.TorrentLoader
{
    class Scrape
    {
        private PeersRecievedCallback _Callback;
        private int _index;
        private ISynchronizeInvoke _syncObject;
        public string _URL;
        private Thread _Worker;

        public Scrape(int index, string AnnounceURL, byte[] Hash, PeersRecievedCallback Callback, ISynchronizeInvoke syncObject)
        {
            _index = index;
            _Callback = Callback;
            _syncObject = syncObject;
            BeginGetScrape(AnnounceURL, Hash);
        }

        private void BeginGetScrape(string AnnounceURL, byte[] Hash)
        {
            int length = AnnounceURL.LastIndexOf("announce");
            string str = AnnounceURL.Substring(0, length) + "scrape" + AnnounceURL.Substring(length + 8);
            string str2 = HttpUtility.UrlEncode(Hash);
            _URL = str + "?info_hash=" + str2;
            _Worker = new Thread(new ThreadStart(ScrapeThread));
            _Worker.Name = AnnounceURL;
            _Worker.Start();
        }

        public void EndProcess()
        {
            _Worker.Abort();
        }

        private void ScrapeThread()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_URL);
                request.UserAgent = "TorrentPatcher/" + Application.ProductVersion;
                ScrapeInfo info = new ScrapeInfo(new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd());
                _syncObject.Invoke(new dCallback(ThreadCallBack), new object[] { _index, info });
            }
            catch
            {
                Thread.CurrentThread.Abort();
            }
        }

        private void ThreadCallBack(int index, ScrapeInfo scrape)
        {
            _Callback(index, scrape, this);
        }

        private delegate void dCallback(int index, ScrapeInfo scrape);

        public delegate void PeersRecievedCallback(int index, ScrapeInfo scrape, Scrape worker);
    }
}

