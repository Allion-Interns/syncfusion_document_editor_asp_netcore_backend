using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Syncfusion_document_editor.Services
{
    public class SpellCheckService
    {
        static HttpClient client = new HttpClient();
        List<Replacement> ary = new List<Replacement>();
        public async Task<List<Replacement>> CreateProductAsync(String text)
        {
            try
            {
                var response = await client.PostAsJsonAsync(
                    "https://api.languagetool.org/v2/check?text=" + text + "&language=sv", text);
                string returnValue = await response.Content.ReadAsStringAsync();
                var json = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseType>(returnValue);
                return json.matches[0].replacements;
               
            }
            catch (Exception ex)
            {
                return ary;
            }
            
        }

        public class Software
        {
            public string name { get; set; }
            public string version { get; set; }
            public string buildDate { get; set; }
            public int apiVersion { get; set; }
            public bool premium { get; set; }
            public string premiumHint { get; set; }
            public string status { get; set; }
        }

        public class Warnings
        {
            public bool incompleteResults { get; set; }
        }

        public class DetectedLanguage
        {
            public string name { get; set; }
            public string code { get; set; }
            public double confidence { get; set; }
        }

        public class Language
        {
            public string name { get; set; }
            public string code { get; set; }
            public DetectedLanguage detectedLanguage { get; set; }
        }

        public class Replacement
        {
            public string value { get; set; }
        }

        public class Context
        {
            public string text { get; set; }
            public int offset { get; set; }
            public int length { get; set; }
        }

        public class Type
        {
            public string typeName { get; set; }
        }

        public class Category
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Rule
        {
            public string id { get; set; }
            public string description { get; set; }
            public string issueType { get; set; }
            public Category category { get; set; }
            public bool isPremium { get; set; }
        }

        public class Match
        {
            public string message { get; set; }
            public string shortMessage { get; set; }
            public List<Replacement> replacements { get; set; }
            public int offset { get; set; }
            public int length { get; set; }
            public Context context { get; set; }
            public string sentence { get; set; }
            public Type type { get; set; }
            public Rule rule { get; set; }
            public bool ignoreForIncompleteSentence { get; set; }
            public int contextForSureMatch { get; set; }
        }

        public class ResponseType
        {
            public Software software { get; set; }
            public Warnings warnings { get; set; }
            public Language language { get; set; }
            public List<Match> matches { get; set; }
        }
    }
}
