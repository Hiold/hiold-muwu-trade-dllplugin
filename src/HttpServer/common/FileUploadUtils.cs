using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.common
{
    /*
     RFC1867协议 举例     
     ------WebKitFormBoundaryKcbJOyftlttL1JBB
    Content-Disposition: form-data; name="name"

    zq
    ------WebKitFormBoundaryKcbJOyftlttL1JBB
    Content-Disposition: form-data; name="myfile"; filename="IMG_20130219_181308.jpg"
    Content-Type: image/jpeg
    [二进制数据]

    ------WebKitFormBoundaryKcbJOyftlttL1JBB--
     */

    /// <summary>
    /// 解析RFC1867协议
    /// </summary>
    public class FileUploadUtils
    {
        private string boundary;
        private byte[] _boundary;
        private byte[] _data;
        private int _length;
        private int _lineLength = -1;
        private int _lineStart = -1;
        private int _pos;
        private bool _lastBoundaryFound;
        private string _partContentType;
        private int _partDataLength = -1;
        private int _partDataStart = -1;
        private string _partFilename;
        private string _partName;
        private Encoding _encoding;

        public FileUploadUtils(HttpListenerRequest request, Encoding encoding)
        {
            this._encoding = encoding;
            //Content-Type: multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW
            Regex regex = new Regex("boundary=(.*)$");

            Match match = regex.Match(request.ContentType);

            if (match.Success)
            {
                boundary = match.Groups[1].Value;
                _boundary = _encoding.GetBytes("--" + boundary);
            }

            Stream input = request.InputStream;

            //将上传文件保存到内存
            BufferedStream br = new BufferedStream(input);

            MemoryStream ms = new MemoryStream();

            byte[] buffer = new byte[4096];

            int len = 0;

            while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, len);
            }

            _data = ms.ToArray();

            _length = _data.Length;

            ms.Close();
        }

        /// <summary>
        /// 获取每一行数据
        /// </summary>
        /// <returns></returns>
        private bool GetNextLine()
        {
            int num = this._pos;

            this._lineStart = -1;

            while (num < this._length)
            {
                if (this._data[num] == 10)
                { // '\n'
                    this._lineStart = this._pos;
                    this._lineLength = num - this._pos;
                    this._pos = num + 1;

                    // ignore \r
                    if ((this._lineLength > 0) && (this._data[num - 1] == 13))
                    {
                        this._lineLength--;
                    }

                    break;
                }

                if (++num == this._length)
                {
                    this._lineStart = this._pos;
                    this._lineLength = num - this._pos;
                    this._pos = this._length;
                }
            }

            return (this._lineStart >= 0);
        }

        /// <summary>
        /// 当前行是否是分隔符行
        /// </summary>
        /// <returns></returns>
        private bool AtBoundaryLine()
        {
            int length = this._boundary.Length;

            if ((this._lineLength != length) && (this._lineLength != (length + 2)))
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                if (this._data[this._lineStart + i] != this._boundary[i])
                {
                    return false;
                }
            }

            if (this._lineLength != length)
            {
                // last boundary line? (has to end with "--")
                if ((this._data[this._lineStart + length] != 0x2d) || (this._data[(this._lineStart + length) + 1] != 0x2d))
                {
                    return false;
                }

                this._lastBoundaryFound = true;
            }

            return true;
        }

        /// <summary>
        /// 是否解析完毕
        /// </summary>
        /// <returns></returns>
        private bool AtEndOfData()
        {
            if (this._pos < this._length)
            {
                return this._lastBoundaryFound;
            }

            return true;
        }

        /// <summary>
        /// 从Content-Disposition:行抽取""中的内容
        /// </summary>
        /// <param name="l">行内容</param>
        /// <param name="pos"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string ExtractValueFromContentDispositionHeader(string l, int pos, string name)
        {
            String pattern = name + "=\"";

            //:所在行位置+1
            int i1 = CultureInfo.InvariantCulture.CompareInfo.IndexOf(l, pattern, pos, CompareOptions.IgnoreCase);

            if (i1 < 0)
                return null;
            i1 += pattern.Length;

            int i2 = l.IndexOf('"', i1);
            if (i2 < 0)
                return null;
            if (i2 == i1)
                return String.Empty;

            return l.Substring(i1, i2 - i1);
        }

        /// <summary>
        /// 读取头部信息
        /// </summary>
        private void ParsePartHeaders()
        {
            _partName = null;
            _partFilename = null;
            _partContentType = null;

            while (GetNextLine())
            {
                if (_lineLength == 0)
                    break;  // empty line signals end of headers ->\r\n

                // get line as String 
                byte[] lineBytes = new byte[_lineLength];

                Array.Copy(_data, _lineStart, lineBytes, 0, _lineLength);

                String line = _encoding.GetString(lineBytes);

                // parse into header and value
                int ic = line.IndexOf(':');
                if (ic < 0)
                    continue;   // not a header

                // remeber header 
                String header = line.Substring(0, ic);

                if (header.Equals("Content-Disposition"))
                {
                    // parse name and filename
                    _partName = ExtractValueFromContentDispositionHeader(line, ic + 1, "name");
                    _partFilename = ExtractValueFromContentDispositionHeader(line, ic + 1, "filename");
                }
                else if (header.Equals("Content-Type"))
                {
                    _partContentType = line.Substring(ic + 1).Trim();
                }
            }
        }

        /// <summary>
        /// 处理数据部分
        /// </summary>
        private void ParsePartData()
        {
            _partDataStart = _pos;
            _partDataLength = -1;

            while (GetNextLine())
            {
                if (AtBoundaryLine())
                {
                    // calc length: adjust to exclude [\r]\n before the separator
                    int iEnd = _lineStart - 1;
                    if (_data[iEnd] == 10)   // \n 
                        iEnd--;
                    if (_data[iEnd] == 13)   // \r 
                        iEnd--;

                    _partDataLength = iEnd - _partDataStart + 1;
                    break;
                }
            }
        }

        /// <summary>
        /// 解析数据为对象列表
        /// </summary>
        /// <returns></returns>
        public List<MultipartFormItem> ParseIntoElementList()
        {
            List<MultipartFormItem> itemList = new List<MultipartFormItem>();

            while (GetNextLine())
            {
                if (AtBoundaryLine())
                    break;
            }

            if (AtEndOfData())
                return itemList;

            do
            {
                // Parse current part's headers 
                ParsePartHeaders();

                if (AtEndOfData())
                    break;          // cannot stop after headers

                // Parse current part's data
                ParsePartData();

                if (_partDataLength == -1)
                    break;          // ending boundary not found

                // Remember the current part (if named)
                if (_partName != null)
                {
                    MultipartFormItem item = new MultipartFormItem();
                    item.Name = _partName;
                    item.Data = new byte[_partDataLength];

                    Buffer.BlockCopy(_data, _partDataStart, item.Data, 0, _partDataLength);

                    item.ContentType = _partContentType;

                    if (item.ContentType != null)
                    {
                        item.ItemType = FormItemType.File;
                    }

                    itemList.Add(item);
                }
            }
            while (!AtEndOfData());

            return itemList;
        }
    }

    public class FormItemType
    {
        public static readonly string File = "file";
    }

    public class MultipartFormItem
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public string ItemType { get; set; }
    }
}
