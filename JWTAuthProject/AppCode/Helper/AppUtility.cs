using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using System.Drawing;
using JWTAuthProject.Models;
using Newtonsoft.Json;
using JWTAuthProject.AppCode.Data;
using System.Drawing.Imaging;
using JWTAuthProject.AppCode.Interface;
using JWTAuthProject.AppCode.Enums;
using OfficeOpenXml;
using System.Net.Http.Headers;

namespace JWTAuthProject.AppCode.Helper
{
    public class AppUtility
    {
        public static AppUtility O => instance.Value;
        private static Lazy<AppUtility> instance = new Lazy<AppUtility>(() => new AppUtility());
        private AppUtility() { }
        public Response UploadFile(FileUploadModel request)
        {
            var response = Validate.O.IsFileValid(request.file);
            if (response.StatusCode == ResponseStatus.Success)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(request.FilePath);
                    if (!Directory.Exists(sb.ToString()))
                    {
                        Directory.CreateDirectory(sb.ToString());
                    }
                    var filename = ContentDispositionHeaderValue.Parse(request.file.ContentDisposition).FileName.Trim('"');
                    string originalExt = Path.GetExtension(filename).ToLower();
                    string[] Extensions = { ".png", ".jpeg", ".jpg" };
                    if (Extensions.Contains(originalExt))
                    {
                        //originalExt = ".jpg";
                    }
                    //string originalFileName = Path.GetFileNameWithoutExtension(filename).ToLower() + originalExt;
                    if (string.IsNullOrEmpty(request.FileName))
                    {
                        request.FileName = filename;//Path.GetFileNameWithoutExtension(request.FileName).ToLower() + originalExt;
                    }
                    //request.FileName = string.IsNullOrEmpty(request.FileName) ? originalFileName.Trim() : request.FileName;
                    sb.Append(request.FileName);
                    using (FileStream fs = File.Create(sb.ToString()))
                    {
                        request.file.CopyTo(fs);
                        fs.Flush();
                        if (request.IsThumbnailRequired)
                        {
                            GenrateThumbnail(request.file, request.FileName, 20L);
                        }
                    }
                    response.StatusCode = ResponseStatus.Success;
                    response.ResponseText = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    response.ResponseText = "Error in file uploading. Try after sometime...";
                }
            }
            return response;
        }
        public bool GenrateThumbnail(IFormFile file, string fileName, long quality = 20L)
        {
            string tempImgNameWithPath = string.Concat(FileDirectories.Thumbnail, fileName);
            var newimg = new Bitmap(file.OpenReadStream());
            ImageCodecInfo jgpEncoder = GetEncoderInfo("image/jpeg");
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            try
            {
                if (File.Exists(tempImgNameWithPath))
                {
                    File.Delete(tempImgNameWithPath);
                }
                newimg.Save(tempImgNameWithPath, jgpEncoder, myEncoderParameters);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        public string GetErrorDescription(int errorCode)
        {
            string error = ((Errorcodes)errorCode).DescriptionAttribute();
            return error;
        }

        public string GetRole(int roleId)
        {
            string error = ((Role)roleId).ToString();
            return error;
        }

        public IResponse<byte[]> ExportToExcel(DataTable dataTable, string[] removableCol = null)
        {
            IResponse<byte[]> response = new Response<byte[]>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Something went wrong"
            };
            try
            {
                if (removableCol != null)
                {
                    foreach (string str in removableCol)
                    {
                        if (dataTable.Columns.Contains(str))
                        {
                            dataTable.Columns.Remove(str);
                        }
                    }
                }

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, PrintHeaders: true);
                    worksheet.Row(1).Height = 20;
                    worksheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Row(1).Style.Font.Bold = true;
                    for (var col = 1; col < dataTable.Columns.Count + 1; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }
                    var exportToExcel = new InMemoryFile
                    {
                        Content = package.GetAsByteArray()
                    };
                    response.Result = exportToExcel.Content;
                    response.StatusCode = ResponseStatus.Success;
                    response.ResponseText = string.Empty;
                }
            }
            catch (Exception ex)
            {
                response.ResponseText = ex.Message;
            }
            return response;
        }
        public IResponse<byte[]> ExportToExcel<T>(IEnumerable<T> records)
        {
            var dataTable = records.ToDataTable();
            IResponse<byte[]> response = new Response<byte[]>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Something went wrong"
            };
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, PrintHeaders: true);
                    worksheet.Row(1).Height = 20;
                    worksheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Row(1).Style.Font.Bold = true;
                    for (var col = 1; col < dataTable.Columns.Count + 1; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }
                    var exportToExcel = new InMemoryFile
                    {
                        Content = package.GetAsByteArray()
                    };
                    response.Result = exportToExcel.Content;
                    response.StatusCode = ResponseStatus.Success;
                    response.ResponseText = string.Empty;
                }
            }
            catch (Exception ex)
            {
                response.ResponseText = ex.Message;
            }
            return response;
        }

        public Dictionary<string, dynamic> ConvertToDynamicDictionary(object someObject)
        {
            var res = someObject.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => (dynamic)prop.GetValue(someObject, null));
            return res;
        }

        public Dictionary<string, string> ConvertToDictionary(object someObject)
        {
            var res = someObject.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(someObject, null));
            return res;
        }

        public string GenrateRandom(int length, bool isNumeric = false)
        {
            string valid = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ23456789";
            if (isNumeric)
            {
                valid = "1234567890";
            }
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public byte[] ConvertBitmapToBytes(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

        public string GenerateOrderIdId(int orederId)
        {
            string o = orederId.ToString();
            return $"TID{o.PadLeft(7)}A";
        }

        public async Task SaveJsonAsFile(string jsonStr, string Action = "misc")
        {
            var fileData = new List<JsonAsFileModel>();
            try
            {
                if (!Directory.Exists(FileDirectories.JsonDoc))
                {
                    Directory.CreateDirectory(FileDirectories.JsonDoc);
                }
                var jsonFile = $"{FileDirectories.JsonDoc}jsonFile.json";
                if (!File.Exists(jsonFile))
                {
                    using (File.Create(jsonFile)) ;
                }
                if (File.Exists(jsonFile))
                {
                    var json = File.ReadAllText(jsonFile);
                    if (!string.IsNullOrEmpty(json))
                    {
                        fileData = JsonConvert.DeserializeObject<List<JsonAsFileModel>>(json);
                        //string.Concat(json, ",", jsonStr);
                    }
                    fileData.Add(new JsonAsFileModel
                    {
                        DateTime = DateTime.Now.ToString("dd MMM yyyy hh:mm:ss tt"),
                        Action = Action,
                        Data = jsonStr
                    });
                    File.WriteAllText(jsonFile, JsonConvert.SerializeObject(fileData, Newtonsoft.Json.Formatting.Indented));
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<HttpContextRequest> ExtractHttpContextRequestAsync(HttpRequest request, bool isUrlDecodeNeeded = true)
        {
            var response = new HttpContextRequest
            {
                Method = request.Method,
                Path = request.Path,
                Scheme = request.Scheme
            };
            StringBuilder resp = new StringBuilder("");
            try
            {
                if (request.Method == "POST")
                {
                    if (request.HasFormContentType)
                    {
                        if (request.Form.Keys.Count > 0)
                        {
                            foreach (var item in request.Form.Keys)
                            {
                                request.Form.TryGetValue(item, out StringValues strVal);
                                if (resp.Length == 0)
                                {
                                    resp.AppendFormat("{0}={1}", item, strVal);
                                }
                                else
                                {
                                    resp.AppendFormat("&{0}={1}", item, strVal);
                                }
                            }
                        }
                    }
                    else
                    {
                        resp = new StringBuilder(await request.GetRawBodyStringAsync().ConfigureAwait(false));
                    }
                }
                else
                {
                    if (request.HasFormContentType)
                    {
                        if (request.Form.Keys.Count > 0)
                        {
                            foreach (var item in request.Form.Keys)
                            {
                                request.Form.TryGetValue(item, out StringValues strVal);
                                if (resp.Length == 0)
                                {
                                    resp.AppendFormat("{0}={1}", item, strVal);
                                }
                                else
                                {
                                    resp.AppendFormat("&{0}={1}", item, strVal);
                                }
                            }
                        }
                    }
                    else
                    {
                        resp = new StringBuilder(await request.GetRawBodyStringAsync().ConfigureAwait(false));
                    }
                }
                if (resp.Length == 0)
                {
                    resp = new StringBuilder(request.QueryString.ToString());
                }
                response.Content = isUrlDecodeNeeded ? WebUtility.UrlDecode(resp.ToString()) : resp.ToString();
            }
            catch (Exception ex)
            {
                response.Content = ex.Message;
            }
            return response;
        }

        public Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;
            //Convert Base64 string to byte[]
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
            {
                memoryStream.Position = 0;
                bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);
                byteBuffer = null;
            }
            return bmpReturn;
        }
    }

    public class HttpContextRequest
    {
        public string Method { get; set; }
        public string Content { get; set; }
        public string Scheme { get; set; }
        public string Path { get; set; }
        public string RemoteIP { get; set; }
    }
}
