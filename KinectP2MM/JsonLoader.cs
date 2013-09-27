using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.IO;

namespace KinectP2MM
{
    class JsonLoader
    {
        private Canvas canvas { get; set; }

        public JsonLoader(Canvas c)
        {
            this.canvas = c;
        }

        public List<Word> load()
        {
            // Exemple de creation de json et d'ecriture sur fichier
            List<JsonWord> list = new List<JsonWord>();
            list.Add(new JsonWord("valentino", 300, 100));
            list.Add(new JsonWord("clement", 50, 50));
            list.Add(new JsonWord("simon", 120, 460));
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(@"p2mm.json", json);

            List<Word> listWords = new List<Word>();
            //lecture du fichier json et creation des mots
            foreach (JsonWord jWord in JsonConvert.DeserializeObject<List<JsonWord>>(File.ReadAllText(@"p2mm.json")))
            {
                Word word = new Word(jWord.content, jWord.x, jWord.y);
                listWords.Add(word);
                this.canvas.Children.Add(word);
            }
            return listWords;
        }        
     }

    class JsonWord
    {
        public JsonWord(String content, int x, int y)
        {
            this.content = content;
            this.x = x;
            this.y = y;
        }
        public int x { get; set; }
        public int y { get; set; }
        public String content { get; set; }
    }

}
