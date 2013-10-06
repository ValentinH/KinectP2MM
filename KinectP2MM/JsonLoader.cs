﻿using System;
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

        private String file;

        public JsonLoader(String file = "sequences.p2mm")
        {
            this.file = file;
        }

        public List<Sequence> load()
        {
            List<Sequence> listSequences = new List<Sequence>();
            //lecture du fichier json et creation des mots
            foreach (JsonSequence jSeq in JsonConvert.DeserializeObject<List<JsonSequence>>(File.ReadAllText(file)))
            {
                List<Word> words = new List<Word>();
                foreach (var jWord in jSeq.words)
                {
                    words.Add(new Word(jWord.content, jWord.x, jWord.y));
                }
                listSequences.Add(new Sequence(words, jSeq.canZoom, jSeq.canRotate));
            }
            return listSequences;
        }

        public void save(List<Sequence> listSequences)
        {
            // Exemple de creation de json et d'ecriture sur fichier
            List<JsonSequence> listJsonSeq = new List<JsonSequence>();

            foreach (Sequence sequence in listSequences)
            {
                List<JsonWord> list = new List<JsonWord>();
                foreach (Word word in sequence.words)
                {
                    list.Add(new JsonWord((String)word.wordTop.Content, (int)word.x, (int)word.y));
                }
                listJsonSeq.Add(new JsonSequence(list, sequence.canZoom, sequence.canRotate));
            }

            string json = JsonConvert.SerializeObject(listJsonSeq, Formatting.Indented);
            File.WriteAllText(@"test.p2mm", json);   
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


    class JsonSequence
    {
        public JsonSequence(List<JsonWord> words, bool canZoom, bool canRotate)
        {
            this.words = words;
            this.canZoom = canZoom;
            this.canRotate = canRotate;
        }
        public List<JsonWord> words { get; set; }
        public bool canZoom { get; set; }
        public bool canRotate { get; set; }
    }

}
