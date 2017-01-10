using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConvertService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IConverter" in both code and config file together.
    [ServiceContract(Namespace = "http://service.weblms.ru")]
    public interface IConverter
    {
        [OperationContract]
        RemoteFileInfo DownloadFile(DownloadRequest request);

        //[OperationContract]
        //void ConvertFile(UploadFileInfo request);
        [OperationContract]
        Task<ResponseFileInfo> ConvertFile(UploadFileInfo request);
    }

    [MessageContract]
    public class DownloadRequest
    {
        [MessageBodyMember]
        public string Path;
    }


    [DataContract(Namespace = "http://service.weblms.ru")]
    public class ResponseFileInfo
    {
        [DataMember]
        public string Path;

        [DataMember]
        public string Hash;
    }


    [DataContract(Namespace = "http://service.weblms.ru")]
    public class UploadFileInfo
    {
        [DataMember]
        public string Email;

        [DataMember]
        public byte[] ByteArray;
    }


    [MessageContract]
    public class RemoteFileInfo// : IDisposable
    {
        //[MessageHeader(MustUnderstand = true)]
        //public string FileName;

        [MessageHeader(MustUnderstand = true)]
        public long Length;

        [MessageBodyMember(Order = 1)]
        public System.IO.Stream FileByteStream;

        //public void Dispose()
        //{
        //    if (FileByteStream != null)
        //    {
        //        FileByteStream.Close();
        //        FileByteStream = null;
        //    }
        //}
    }
}
