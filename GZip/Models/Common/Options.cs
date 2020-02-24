namespace GZip.Models.Common
{
    public class Options
    {
        /// <summary>
        /// Путь к файлу, который будет заархивирован
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Путь к заархивированному файлу
        /// </summary>
        public string TargetFilePath { get; set; }

        /// <summary>
        /// Размер буфера в байтах
        /// </summary>
        public uint BufferSize { get; set; }
    }
}
