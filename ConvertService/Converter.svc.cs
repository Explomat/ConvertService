using ConvertService.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace ConvertService
{
    public class Converter : IConverter
    {
        #region Common Methods
        /// <summary>
        /// проверка соединения
        /// </summary>
        /// <returns> OK </returns>
        public string TestConnection()
        {
            return "OK";
        }

        #endregion

        #region File Methods

        public RemoteFileInfo DownloadFile(DownloadRequest request)
        {
            RemoteFileInfo result = new RemoteFileInfo(); //D:\repos\VS2013\Projects\ConvertService\ConvertService\WRecors\cabehok@inbox.ru\76e0ca8f36f524b2e8db91a7dac8ee2c\video
            try
            {
                string filePath = System.Web.HttpContext.Current.Server.MapPath(request.Path);
                FileInfo fileInfo = new FileInfo(filePath);

                // check if exists
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException("File not found", request.Path);
                }

                // open stream
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                OperationContext clientContext = OperationContext.Current;
                clientContext.OperationCompleted += (o, args) =>
                {
                    if (stream != null) stream.Dispose();
                };
                // return result 
                result.Length = fileInfo.Length;
                result.FileByteStream = stream;
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public ResponseFileInfo ConvertFile(UploadFileInfo request)
        {
            string hash = "";
            string destDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WRecords");
            Stream sourceStream = new MemoryStream(request.ByteArray);
            ResponseFileInfo fileInfo = new ResponseFileInfo();
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    hash = Hash.GetMD5Hash(md5, request.ByteArray);
                    string destFullDirectory = Path.Combine(destDirectory, request.Email, hash);
                    string videoDirectory = Path.Combine(destFullDirectory, "video");

                    if (!Directory.Exists(destDirectory))
                    {
                        Directory.CreateDirectory(destDirectory);
                    }

                    if (!Directory.Exists(destFullDirectory))
                    {
                        Directory.CreateDirectory(destFullDirectory);
                        Directory.CreateDirectory(videoDirectory);
                        Zip.Unzip(sourceStream, destFullDirectory);

                    }

                    VideoConverter converter = new VideoConverter(destFullDirectory, videoDirectory);
                    string archiveError = converter.CheckArchiveCorrect();
                    if (archiveError != null)
                    {
                        throw new Exception(archiveError);
                    }

                    string outFilePath = converter.Start();
                    converter.Dispose();
                    fileInfo.Path = string.Format("WRecords/{0}/{1}/video/{2}", request.Email, hash, Path.GetFileName(outFilePath));
                    fileInfo.Hash = hash;
                }
            }
            catch (Exception e)
            {
                File.WriteAllText(Path.Combine(destDirectory, request.Email, hash, (DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + "__" + DateTime.Now.TimeOfDay) + "__catch.txt"), e.Message);
            }
            finally
            {
                sourceStream.Close();
            }
            return fileInfo;
        }
        #endregion
    }
}
