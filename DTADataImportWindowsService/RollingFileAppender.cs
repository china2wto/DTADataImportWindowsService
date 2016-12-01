using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace DTADataImportWindowsService
{
    class RollingFileAppender : log4net.Appender.RollingFileAppender
    {
        private DateTime lastcheckDate = DateTime.MinValue;

        protected override void AdjustFileBeforeAppend()
        {
            base.AdjustFileBeforeAppend();

            // 清理历史文件
            bool _isok = this.needToclear();
            if (!_isok)
            {
                return;
            }

            System.IO.FileInfo _fileinfo = new FileInfo(File);

            DateTime _maxdate = System.IO.File.GetLastWriteTime(File).AddDays(-this.MaxSizeRollBackups + 1);
            IList _files = System.IO.Directory.GetFiles(_fileinfo.DirectoryName, _fileinfo.Name + "*");
            foreach (string _file in _files)
            {
                _fileinfo = new FileInfo(_file);
                DateTime _date = System.IO.File.GetLastWriteTime(_file);
                if (_date < _maxdate)
                {
                    _fileinfo.Delete();
                }
            }
        }

        private bool needToclear()
        {
            bool _isok = true;
            System.IO.FileInfo _fileinfo = new FileInfo(File);
            DateTime _fileDate = _fileinfo.LastWriteTime.Date;
            if (!lastcheckDate.Equals(DateTime.MinValue))
            {
                if (lastcheckDate >= _fileDate)
                {
                    _isok = false;
                }
            }
            if (this.MaxSizeRollBackups <= 0)
            {
                _isok = false;
            }
            if (_isok)
            {
                this.lastcheckDate = _fileDate;
            }
            return _isok;
        }
    }
}
