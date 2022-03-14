using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.DocumentEditor;
using Syncfusion.EJ2.SpellChecker;
using Syncfusion_document_editor.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Syncfusion_document_editor.Controllers
{
    [Route("api/[controller]")]
    public class DocumentEditorController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        List<DictionaryData> spellDictionary;
        string personalDictPath;
        string path;
        private SpellCheckService spellCheckService = new SpellCheckService();


        public DocumentEditorController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            spellDictionary = Startup.spellDictCollection;
            path = Startup.path;
            personalDictPath = Startup.personalDictPath;

        }

        [AcceptVerbs("Post")]
        [HttpPost]
       // [EnableCors("AllowAllOrigins")]
        [Route("SpellCheck")]
        public  string SpellCheck([FromBody] SpellCheckJsonData spellChecker)
        {
            var timer = new System.Diagnostics.Stopwatch();
            

            try
            {
                // SpellCheckDictionary ss = new SpellCheckDictionary(spellDictionary[0]);
                List<SpellCheckDictionary> ss = new List<SpellCheckDictionary>();
                

                foreach (var spell in spellDictionary)
                {
                    if(spell.LanguadeID == spellChecker.LanguageID)
                    {
                        ss.Add(new SpellCheckDictionary(spell));
                    }
                    
                }
                SpellChecker spellCheck = new SpellChecker(ss);


                if (spellChecker.LanguageID == 1053) 
                {
                    timer.Start();
                    spellCheck.CheckSpelling(spellChecker.LanguageID, spellChecker.TexttoCheck);

                    var x = Newtonsoft.Json.JsonConvert.SerializeObject(spellCheck);

                    var replacearray = spellCheckService.CreateProductAsync(spellChecker.TexttoCheck);

                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SpellResponse>(x);

                    foreach (var item in replacearray.Result)
                    {
                        result.Suggestions.Add(item.value);
                    }
                    Debug.WriteLine("file open time=================================================================================== " + timer.ElapsedMilliseconds);
                    timer.Stop();
                    return Newtonsoft.Json.JsonConvert.SerializeObject(result);
                    
                }
                else
                {
                    timer.Start();
                    spellCheck.GetSuggestions(spellChecker.LanguageID, spellChecker.TexttoCheck, spellChecker.CheckSpelling, spellChecker.CheckSuggestion, spellChecker.AddWord);

                    return Newtonsoft.Json.JsonConvert.SerializeObject(spellCheck);
                    Console.WriteLine("file open time " + timer.Elapsed.TotalMilliseconds);
                    timer.Stop();
                }

            }
            catch(Exception e)
            { 
                return "{\"SpellCollection\":[],\"HasSpellingError\":false,\"Suggestions\":null}";
                
            }
        }


        [AcceptVerbs("Post")]
        [HttpPost]
        [EnableCors("AllowAllOrigins")]
        [Route("SpellCheckByPage")]
        public string SpellCheckByPage([FromBody] SpellCheckJsonData spellChecker)
        {
            try
            {
                SpellChecker spellCheck = new SpellChecker(spellDictionary, personalDictPath);
                spellCheck.CheckSpelling(spellChecker.LanguageID, spellChecker.TexttoCheck);
                return Newtonsoft.Json.JsonConvert.SerializeObject(spellCheck);
            }
            catch
            {
                return "{\"SpellCollection\":[],\"HasSpellingError\":false,\"Suggestions\":null}";
            }
        }

        public class SpellCheckJsonData
        {
            public int LanguageID { get; set; }
            public string TexttoCheck { get; set; }
            public bool CheckSpelling { get; set; }
            public bool CheckSuggestion { get; set; }
            public bool AddWord { get; set; }

        }
        public class SpellCollection
        {
            public string Text { get; set; }
            public bool HasSpellError { get; set; }
        }

        public class SpellResponse
        {
            public List<SpellCollection> SpellCollection { get; set; }
            public bool HasSpellingError { get; set; }
            public List<string> Suggestions { get; set; }
        }



        [AcceptVerbs("Post")]
        [HttpPost]
        [EnableCors("AllowAllOrigins")]
        [Route("SystemClipboard")]
        public string SystemClipboard([FromBody] CustomParameter param)
        {
            if (param.content != null && param.content != "")
            {
                try
                {
                    WordDocument document = WordDocument.LoadString(param.content, GetFormatType(param.type.ToLower()));
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                    document.Dispose();
                    return json;
                }
                catch (Exception)
                {
                    return "";
                }
            }
            return "";
        }

        public class CustomParameter
        {
            public string content { get; set; }
            public string type { get; set; }
        }


        internal static FormatType GetFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            switch (format.ToLower())
            {
                case ".dotx":
                case ".docx":
                case ".docm":
                case ".dotm":
                    return FormatType.Docx;
                case ".dot":
                case ".doc":
                    return FormatType.Doc;
                case ".rtf":
                    return FormatType.Rtf;
                case ".txt":
                    return FormatType.Txt;
                case ".xml":
                    return FormatType.WordML;
                case ".html":
                    return FormatType.Html;
                default:
                    throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            }
        }




    }


}
