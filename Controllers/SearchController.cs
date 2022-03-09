using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.IO;


namespace Syncfusion_document_editor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<string>> GetResult()
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



        static List<string> findAll()
        {

           
            string[] WordDocumentNames = Directory.GetFiles(@"Project01");

            // FileStream[] fileStreams = new FileStream[WordDocumentNames.Length];

            List<FileStream> fileStreams = new List<FileStream>();


            foreach (string WordDocumentName in WordDocumentNames)
            {
                fileStreams.Add(new FileStream(WordDocumentName, FileMode.Open, FileAccess.ReadWrite));
            }

            List<string> resultFilePath= new List<string>();

            foreach (FileStream fileStream in fileStreams)
            {
                using(WordDocument document = new WordDocument(fileStream,FormatType.Automatic))
                {
                    TextSelection[] textSelection =  document.FindAll("apple", false, true);
                    if (textSelection == null)
                    {

                    }
                    else
                    {
                        resultFilePath.Add(fileStream.Name);
                    }

                }


            }

            return resultFilePath;
        }


    }
}