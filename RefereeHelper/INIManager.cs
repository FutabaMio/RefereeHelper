using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper
{
    //Класс для чтения/записи INI-файлов
    public class INIManager
    {
        /// <summary>
        /// Конструктор, принимающий путь к INI-файлу
        /// </summary>
        /// <param name="aPath"> путь к INI-файлу </param>
        public INIManager(string aPath)
        {
            path = aPath;
        }

        /// <summary>
        /// Конструктор без аргументов (путь к INI-файлу нужно будет задать отдельно)
        /// </summary>
        public INIManager() : this("") { }

        /// <summary>
        /// Возвращает значение из INI-файла (по указанным секции и ключу)
        /// </summary>
        /// <param name="aSection"> Название секции в текстововм формате </param>
        /// <param name="aKey"> Название ключа в текстовом формате </param>
        /// <returns> Возвращает значение привязвное к ключю в нужной секции в текстововм формате </returns>
        public string GetPrivateString(string aSection, string aKey)
        {
            StringBuilder buffer = new StringBuilder(SIZE);

            GetPrivateString(aSection, aKey, null, buffer, SIZE, path);

            return buffer.ToString();
        }

        //Пишет значение в INI-файл (по указанным секции и ключу) 
        /// <summary>
        /// Записывает значение в INI-файл (по указанным секции и ключу) 
        /// </summary>
        /// <param name="aSection"> Название секции в текстововм формате </param>
        /// <param name="aKey"> Название ключа в текстовом формате </param>
        /// <param name="aValue"> Записуемое значение в текстовом формате </param>
        public void WritePrivateString(string aSection, string aKey, string aValue)
        {
            WritePrivateString(aSection, aKey, aValue, path);
        }

        /// <summary>
        /// Возвращает или устанавливает путь к INI файлу
        /// </summary>
        public string Path { get { return path; } set { path = value; } }

        //Поля класса
        private const int SIZE = 1024; //Максимальный размер (для чтения значения из файла)
        private string path = null; //Для хранения пути к INI-файлу

        //Импорт функции GetPrivateProfileString (для чтения значений) из библиотеки kernel32.dll
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateString(string section, string key, string def, StringBuilder buffer, int size, string path);

        //Импорт функции WritePrivateProfileString (для записи значений) из библиотеки kernel32.dll
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        private static extern int WritePrivateString(string section, string key, string str, string path);
    }
}
