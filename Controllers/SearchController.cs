using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion_document_editor.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace Syncfusion_document_editor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public  ActionResult<List<SearchFindResponceDto>> GetResult()
        {

            //FindandHighlight();
            var result = findAll();
            return Ok(result);
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



        static List<SearchFindResponceDto> findAll()
        {
            var timer = new Stopwatch();

            
        

        string[] WordDocumentNames = Directory.GetFiles(@"Project01");


            List<FileStream> fileStreams = new List<FileStream>();


            foreach (string WordDocumentName in WordDocumentNames)
            {
                //timer.Start();
                //fileStreams.Add(new FileStream(WordDocumentName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));

                //Console.WriteLine(timer.Elapsed.TotalMilliseconds);
                //timer.Stop();

                try
                {



                    timer.Start();
                    fileStreams.Add(new FileStream(WordDocumentName, FileMode.Open, FileAccess.ReadWrite));

                    Console.WriteLine("file open time "+ timer.Elapsed.TotalMilliseconds);
                    timer.Stop();
                }
                catch (IOException )
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
                    TextSelection[] textSelection = document.FindAll("pasindu", false, true);
                    Console.WriteLine("file read time "+ timer.Elapsed.TotalMilliseconds);
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
}