using System;
using System.IO;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers
{
    public class LineReader: IDisposable
    {

        public LineReader(string fileName)
        {
            Reader = LocationStorage.Reader(fileName);
        }

        public LineReader(FileStream stream)
        {
            Reader = new StreamReader(stream, TxtEncoder.GetEncoding(stream));
        }

        public LineReader(StreamReader reader)
        {
            Reader = reader;
        }


        private readonly StreamReader Reader;
        private int LastLineNo = -1;
        private string? LastLine;
        private bool MoveNextStop;

        public int LineNo => LastLineNo;

        public string? ReadLine()
        {
            if (MoveNextStop)
            {
                MoveNextStop = false;
                return LastLine;
            }
            LastLine = Reader.ReadLine();
            LastLineNo++;
            return LastLine;
        }

        public void Back()
        {
            MoveNextStop = true;
        }

        public void Dispose()
        {
            Reader?.Dispose();
        }
    }
}
