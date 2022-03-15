using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion_document_editor.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion_document_editor.Controllers
{

    
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {

       

        [HttpPost]
        public ActionResult<List<SearchFindResponceDto>> GetResult(string keyword)
        {

            //FindandHighlight();
            var result = findAll(keyword);
            return Ok(result);
        }


        [HttpPost]
        [Route("GetallDocuments")]
        public ActionResult<List<SearchFindResponceDto>> GetallDocuments()
        {

            List<SearchFindResponceDto> resultFilePath = new List<SearchFindResponceDto>();
            string[] WordDocumentNames = Directory.GetFiles(@"Project01");

            List<FileStream> fileStreams = new List<FileStream>();

            foreach (string WordDocumentName in WordDocumentNames)
            {
                try
                {
                  
                    fileStreams.Add(new FileStream(WordDocumentName, FileMode.Open, FileAccess.ReadWrite));
                   
                }

                catch (Exception)
                {

                    throw;
                }
            }

            foreach (FileStream fileStream in fileStreams)
            {
                resultFilePath.Add(new SearchFindResponceDto { name = Path.GetFileName(fileStream.Name), path = fileStream.Name });

            }
            return resultFilePath;
        }



        [HttpPost]
        [Route("getdocument")]
        public ActionResult<string> getDocinSdft(string docPath)
        {
            FileStream stream = new FileStream(docPath, FileMode.Open, FileAccess.ReadWrite);

            Syncfusion.EJ2.DocumentEditor.WordDocument document = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(stream, Syncfusion.EJ2.DocumentEditor.FormatType.Docx);

            string sfdtJson = Newtonsoft.Json.JsonConvert.SerializeObject(document);

            return Ok(sfdtJson);

        }



        [HttpPost]
        [Route("getdocumentbysfdt")]
        public async Task<ActionResult<string>> getdocumentbysdftAsync()
        {

            var sfdt = await Request.GetRawBodyStringAsync();
            Stream document = Syncfusion.EJ2.DocumentEditor.WordDocument.Save(sfdt, Syncfusion.EJ2.DocumentEditor.FormatType.Doc);

            FileStream file = new FileStream("Result.doc", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            document.CopyTo(file);
            file.Dispose();
            document.Dispose();

            FileStream file1 = new FileStream("Result.doc", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            Syncfusion.EJ2.DocumentEditor.WordDocument document1 = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(file1, Syncfusion.EJ2.DocumentEditor.FormatType.Doc);

            string sfdtJson = Newtonsoft.Json.JsonConvert.SerializeObject(document1);

            file1.Dispose();
            document1.Dispose();
            //string sfdtJson =  Newtonsoft.Json.JsonConvert.SerializeObject(document);

            return Ok(sfdtJson);

        }




        static void FindandHighlight()
        {
            using (FileStream fileStream = new FileStream(Path.GetFullPath(@"test.docx"), FileMode.Open, FileAccess.ReadWrite))
            {
                //Loads an existing Word document into DocIO instance.
                using (WordDocument document = new WordDocument(fileStream, FormatType.Automatic))
                {
                    //Finds the occurrence of the Word "sample" in the document.

                    TextSelection[] textSelection = document.FindAll("sample", false, true);

                    //Iterates through each occurrence and highlights it.
                    foreach (TextSelection selection in textSelection)
                    {
                        IWTextRange textRange = selection.GetAsOneRange();

                        textRange.CharacterFormat.HighlightColor = Syncfusion.Drawing.Color.Yellow;
                    }
                    //Creates file stream.
                    using (FileStream outputStream = new FileStream(Path.GetFullPath(@"Result.docx"), FileMode.Create, FileAccess.ReadWrite))
                    {
                        //Saves the Word document to file stream.
                        document.Save(outputStream, FormatType.Docx);
                    }
                }
            }
        }





        static List<SearchFindResponceDto> findAll(string keyword)
        {
            var timer = new Stopwatch();

            string[] WordDocumentNames = Directory.GetFiles(@"Project01");

            List<FileStream> fileStreams = new List<FileStream>();

            foreach (string WordDocumentName in WordDocumentNames)
            {
                try
                {
                    timer.Start();
                    fileStreams.Add(new FileStream(WordDocumentName, FileMode.Open, FileAccess.ReadWrite));

                    Console.WriteLine("file open time " + timer.Elapsed.TotalMilliseconds);
                    timer.Stop();
                }
                catch (IOException)
                {
                    throw;
                }


                catch (Exception)
                {

                    throw;
                }
            }

            List<SearchFindResponceDto> resultFilePath = new List<SearchFindResponceDto>();

            foreach (FileStream fileStream in fileStreams)
            {
                using (WordDocument document = new WordDocument(fileStream, FormatType.Automatic))
                {
                    timer.Start();
                    TextSelection[] textSelection = document.FindAll(keyword, false, true);
                    Console.WriteLine("file read time " + timer.Elapsed.TotalMilliseconds);
                    timer.Stop();
                    if (textSelection == null)
                    {

                    }
                    else
                    {
                        resultFilePath.Add(new SearchFindResponceDto { name = Path.GetFileName(fileStream.Name), path = fileStream.Name });
                        //resultFilePath.Add(fileStream.Name); 
                    }

                    document.Dispose();
                    fileStream.Dispose();
                }

            }
            return resultFilePath;
        }


    }

    public static class HttpRequestExtensions
    {

        /// <summary>
        /// Retrieve the raw body as a string from the Request.Body stream
        /// </summary>
        /// <param name="request">Request instance to apply to</param>
        /// <param name="encoding">Optional - Encoding, defaults to UTF8</param>
        /// <returns></returns>
        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            using (StreamReader reader = new StreamReader(request.Body, encoding))
                return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Retrieves the raw body as a byte array from the Request.Body stream
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetRawBodyBytesAsync(this HttpRequest request)
        {
            using (var ms = new MemoryStream(2048))
            {
                await request.Body.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }

}