using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace KinectP2MM
{
    public enum Casse
    {
        MAJUSCULE = 0,
        MINUSCULE = 1
    }

    public enum Procede
    {
        coupable_min_haut = 0,
        coupable_min_bas = 1,
        coupable_maj_haut = 2,
        coupable_maj_bas = 3
    }

    class APIManager
    {
        HttpClient client;
        public Procede procede { get; set; }
        public Casse casse { get; set; }
        String currentWord;
        List<XMLWord> wordsList;

        public APIManager()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://192.185.52.237/~lasepa/beta/");
            procede = Procede.coupable_min_haut;
            casse = Casse.MINUSCULE;
            currentWord = "";
            wordsList = new List<XMLWord>();
        }

        public async Task<String> getCompatibleWord(String word)
        {
            //on ne refait pas de requete et on passe le prochain mot
            if (!word.Equals(currentWord))
            {
                currentWord = word;
                wordsList = new List<XMLWord>();
                await requestAPI(word);
            }

            if (wordsList.Count > 0)
            {
                XMLWord w  = wordsList.First();
                wordsList.RemoveAt(0);
                return w.content;
            }
            else
                return "";
        }

        private async Task requestAPI(String word)
        {
            if (word.Equals(String.Empty)) return;
            try
            {
                var response = await client.GetAsync("words.php?procedes=" + procede + "&word=" + word + "&casse=" + Convert.ToInt32(casse));
                response.EnsureSuccessStatusCode(); // Throw on error code.
                var content = await response.Content.ReadAsStringAsync();
                
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(content);
                XmlNodeList elemList = xmlDoc.GetElementsByTagName("word");
                for (int i = 0; i < elemList.Count; i++)
                {
                    var name = elemList[i].Attributes.GetNamedItem("name").Value;
                    var font = elemList[i].Attributes.GetNamedItem("font").Value;
                    var code = elemList[i].Attributes.GetNamedItem("code").Value;
                    var frequence = Convert.ToInt32(elemList[i].Attributes.GetNamedItem("freq").Value);
                    wordsList.Add(new XMLWord(name, font, code, frequence));
                }
                //trie par sequence
                wordsList = wordsList.OrderByDescending(w => w.frequence).ToList<XMLWord>();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }
    }

    class XMLWord
    {
        public XMLWord(String content, String font, String code, int frequence)
        {
            this.content = content;
            this.font = font;
            this.code = code;
            this.frequence = frequence;
        }
        public String content { get; set; }
        public String font { get; set; }
        public String code { get; set; }
        public int frequence { get; set; }
    }
}
