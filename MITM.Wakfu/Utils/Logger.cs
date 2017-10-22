using MITM.Wakfu.Timers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITM.Wakfu.Utils
{
    public class Logger
    {
        #region Vars
        private Queue<Tuple<string, string, ConsoleColor, Tuple<string, string, int>>> _queue = new Queue<Tuple<string, string, ConsoleColor, Tuple<string, string, int>>>();
        private TimerCore _timer;
        private Task _task;
        #endregion

        #region Construtor
        public Logger()
        {
            if (!Directory.Exists(".\\Logs"))
                Directory.CreateDirectory(".\\Logs");
            _timer = new TimerCore(new Action(LetsGo), 100, 100);
        }
        #endregion

        #region Public Methods
        public void Debug(string text,
            [System.Runtime.CompilerServices.CallerMemberName] string name = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int numberLine = 0)
        {
            _queue.Enqueue(new Tuple<string, string, ConsoleColor, Tuple<string, string, int>>("Debug", text, ConsoleColor.Cyan, new Tuple<string, string, int>(name, sourceFilePath, numberLine)));
        }

        public void Receive(string text,
            [System.Runtime.CompilerServices.CallerMemberName] string name = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int numberLine = 0)
        {
            _queue.Enqueue(new Tuple<string, string, ConsoleColor, Tuple<string, string, int>>("Receive", text, ConsoleColor.Green, new Tuple<string, string, int>(name, sourceFilePath, numberLine)));
        }

        public void Send(string text,
            [System.Runtime.CompilerServices.CallerMemberName] string name = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int numberLine = 0)
        {
            _queue.Enqueue(new Tuple<string, string, ConsoleColor, Tuple<string, string, int>>("Send", text, ConsoleColor.DarkGreen, new Tuple<string, string, int>(name, sourceFilePath, numberLine)));
        }

        public void Error(string text,
            [System.Runtime.CompilerServices.CallerMemberName] string name = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int numberLine = 0)
        {
            _queue.Enqueue(new Tuple<string, string, ConsoleColor, Tuple<string, string, int>>("Error", text, ConsoleColor.Red, new Tuple<string, string, int>(name, sourceFilePath, numberLine)));
        }

        public void Infos(string text,
            [System.Runtime.CompilerServices.CallerMemberName] string name = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int numberLine = 0)
        {
            _queue.Enqueue(new Tuple<string, string, ConsoleColor, Tuple<string, string, int>>("Infos", text, ConsoleColor.White, new Tuple<string, string, int>(name, sourceFilePath, numberLine)));
        }

        public void Server(string text,
    [System.Runtime.CompilerServices.CallerMemberName] string name = "",
    [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
    [System.Runtime.CompilerServices.CallerLineNumber] int numberLine = 0)
        {
            _queue.Enqueue(new Tuple<string, string, ConsoleColor, Tuple<string, string, int>>("Server", text, ConsoleColor.DarkMagenta, new Tuple<string, string, int>(name, sourceFilePath, numberLine)));
        }

        public void Client(string text,
    [System.Runtime.CompilerServices.CallerMemberName] string name = "",
    [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
    [System.Runtime.CompilerServices.CallerLineNumber] int numberLine = 0)
        {
            _queue.Enqueue(new Tuple<string, string, ConsoleColor, Tuple<string, string, int>>("Client", text, ConsoleColor.DarkYellow, new Tuple<string, string, int>(name, sourceFilePath, numberLine)));
        }
        #endregion

        #region Private Methods
        private void LetsGo()
        {
            if (_task != null && !_task.IsCompleted)
                return;

            _task = Task.Run(() =>
            {
                var nextInfos = _queue.Count <= 0 ? null : _queue.Dequeue();

                if (nextInfos == null)
                    return;

                var baseName = nextInfos.Item4.Item2.Split('\\')[3];

                Console.ForegroundColor = nextInfos.Item3;
                Console.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}", DateTime.Now.ToString("hh:mm:ss"), nextInfos.Item1, baseName, nextInfos.Item4.Item1 + "()",
                    nextInfos.Item4.Item2.Split('\\').Last().Replace(".cs", ""), "Line:" + nextInfos.Item4.Item3,
                    nextInfos.Item2);
                Console.ForegroundColor = ConsoleColor.White;


                using (var _stream = File.AppendText(".\\Logs\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".log"))
                {
                    _stream.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}", DateTime.Now.ToString("hh:mm:ss"), nextInfos.Item1, baseName, nextInfos.Item4.Item1 + "()",
                    nextInfos.Item4.Item2.Split('\\').Last().Replace(".cs", ""), "Line:" + nextInfos.Item4.Item3,
                    nextInfos.Item2);
                }
            });
        }
        #endregion
    }
}
