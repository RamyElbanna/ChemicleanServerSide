using Microsoft.AspNetCore.Mvc;
using ChemicleanProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.Sockets;
using System.Net;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace ChemicleanProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // should be in repository layer
        private readonly dbContext _db;
        private static HttpContext _context;
        private static WebSocket _socket;
        public ProductsController(dbContext db)
        {
            _db = db;
        }

        // for socket
        [HttpGet]
        [Route("/ws")]
        public async Task Get()
        {
            _context = ControllerContext.HttpContext;
            var isSocketRequest = _context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {
                _socket = await _context.WebSockets.AcceptWebSocketAsync();
            }
            else
            {
                _context.Response.StatusCode = 400;
            }
        }
        [HttpGet]
        [Route("/products")]
        public List<Product> GetAllProducts()
        {
            var allProducts = _db.Products.ToList();
            return allProducts;
        }

        [HttpGet]
        [Route("/products/{id}")]
        public Product GetProductById(int id)
        {
            return _db.Products.FirstOrDefault(p => p.Id == id);
        }

        [HttpGet]
        [Route("/products/save/{productId}")]
        public async Task DownloadLocalCopy(int productId)
        {
            // get file name from url
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            string remoteFile = product.Url;
            string fileName = remoteFile.Split('/').LastOrDefault();
            if(fileName.Contains("?"))
            {
                fileName = fileName.Split('?')[0];
            }
            string fileLocalPath = @"R:\Projects\chemi-clean\ChemicleanProject\ChemicleanProject\DataSheets\" + fileName;
            using (HttpClient client = new HttpClient())
            {
                var res = await client.GetAsync(remoteFile);

                HttpContent content = res.Content;
                using (var file = System.IO.File.Create(fileLocalPath))
                {
                    var contentStream = await content.ReadAsStreamAsync();
                    await contentStream.CopyToAsync(file);
                }
            }
            // save in database
            SaveBytesInDatabase(fileLocalPath, productId, fileName);
            
            // should implement errors handling and rolling back if one of the layer has failed.
        }


        [HttpGet]
        [Route("/products/view")]
        public string ViewDataSheet()
        {
            // should handle the different file extensions
            StringBuilder text = new StringBuilder();
            // using PdfBig to parse pdf
            using (UglyToad.PdfPig.PdfDocument document = UglyToad.PdfPig.PdfDocument.Open(@"R:\Projects\chemi-clean\ChemicleanProject\ChemicleanProject\DataSheets\hylomar_universal_blue.pdf"))
            {
                for (int i = 1; i <= document.NumberOfPages; i++)
                {
                    text.Append(string.Join(" ", document.GetPage(i).GetWords()));
                }
            }
            return text.ToString();
        }

        private void SaveBytesInDatabase(string filePath, int productId, string fileName)
        {
            string fileLocalPath = @"R:\Projects\chemi-clean\ChemicleanProject\ChemicleanProject\DataSheets\" + fileName;
            byte[] fileBytes;
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);
            fileBytes = reader.ReadBytes((int)stream.Length);
            // compare files before saving
            var product = _db.Products.FirstOrDefault(p => p.Id == productId);
            if(product.Bytes == null)
            {
                // save in database
                product.Bytes = fileBytes;
            }
            else
            {
                bool fileChanged = CompareFiles(fileBytes,product.Bytes);
                if(fileChanged)
                {
                    product.Bytes = fileBytes;
                    product.UpdatedAt = DateTime.Now;
                    // notify users
                    //var bytes = Encoding.ASCII.GetBytes(product.UpdatedAt.ToString());
                    //var arraySegment = new ArraySegment<byte>(bytes);
                    //await _socket.SendAsync(arraySegment, WebSocketMessageType.Text, false, CancellationToken.None);
                    // Need more time to complete realtime
                }

            }
            // save in database
            _db.Products.Update(product);
            _db.SaveChanges();

        }

        private bool CompareFiles(byte[] oldFile, byte[] original)
        {
            var hash1 = Encoding.Default.GetString(MD5.Create().ComputeHash(oldFile));
            var hash2 = Encoding.Default.GetString(MD5.Create().ComputeHash(original));
            return hash1 == hash2;
        }
    }
}
