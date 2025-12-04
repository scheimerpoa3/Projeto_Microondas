using System;
using System.IO;
using System.Text;

namespace Microondas.Infrastructure.Logger
{
    public class ArquivoLogger
    {
        private readonly string _arquivoLog;

        public ArquivoLogger(string caminho = null)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _arquivoLog = caminho ?? Path.Combine(baseDir, "logs.txt");
        }

        public void LogErro(Exception ex)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("----- ERRO -----");
                sb.AppendLine($"Data: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"Mensagem: {ex.Message}");
                if (ex.InnerException != null)
                {
                    sb.AppendLine($"InnerException: {ex.InnerException.Message}");
                }
                sb.AppendLine($"StackTrace: {ex.StackTrace}");
                sb.AppendLine();
                File.AppendAllText(_arquivoLog, sb.ToString());
            }
            catch { }
        }

        public void LogInfo(string mensagem)
        {
            try
            {
                var txt = $"[INFO] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {mensagem}{Environment.NewLine}";
                File.AppendAllText(_arquivoLog, txt);
            }
            catch { }
        }
    }
}
