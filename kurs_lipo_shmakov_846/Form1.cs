using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace kurs_lipo_shmakov_846
{
    public partial class Form1 : Form
    {
        // Директория в которой находится исполняемая программа
        readonly string CurrentDirectory = Directory.GetCurrentDirectory();
        // Расположение считываемого файла
        string DirectoryReadFile;
        public Form1() => InitializeComponent();

        // Чтение исходных текстов программ на языке Паскаль
        public void OpenFile()
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CurrentDirectory;
                openFileDialog.Filter = "Исходные тексты программ (*.pas)|*.pas";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Расположение считываемого файла
                DirectoryReadFile = openFileDialog.FileName;

                // Считывание содержимого файла в поток
                var fileStream = openFileDialog.OpenFile();

                using StreamReader reader = new StreamReader(fileStream);
                string fileContent = reader.ReadToEnd();
                textBox.Text = fileContent;
            }

        }

        // Переопределение стандартной функции поиска слова в строке, для расширения возможности поиска без учета регистра
        public static bool Contains( string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }

        // Алгоритм 
        public void Algorithm()
        {
            // Счетчик строки
            int counter = 1;
            // Сообщение об ошибке
            string Warning = "Ошибок не найдено!";
            // Список всех анализируемых лексем
            List<string> lexemes = new List<string>() { "repeat", "case", "begin", "record" };
            // Стек лексем, найденных в тексте программы
            Stack<string> tokens = new Stack<string>();

            // Считываем текст программы по строкам
            foreach (string line in System.IO.File.ReadLines(DirectoryReadFile))
            {
                // В каждой строке ищем все анализируемые лексемы
                foreach (string lexeme in lexemes)
                {
                    // Если лексема найдена, добавляем ее в стек
                    if (line.Contains(lexeme, StringComparison.OrdinalIgnoreCase))
                        tokens.Push(lexeme);

                }

                // Если встретили правую скобку типа until
                if (line.Contains("until", StringComparison.OrdinalIgnoreCase))
                {
                    // Если при найденной правой скобке пустой стек
                    if (tokens.Count == 0)
                    {
                        Warning = "(" + DateTime.Now + ") " + "Отсутствует левая скобка: [Строка] = " + counter;
                        tokens.Clear();
                        break;
                    }

                    // Если левая скобка из стека не совпадает с найденной правой по типу
                    if (tokens.Pop() != lexemes[0])
                    {
                        Warning = "(" + DateTime.Now + ") " +  "Не совпадают типы скобок: [Строка] = " + counter;
                        tokens.Clear();
                        break;
                    }
                }

                // Если встретили правую скобку типа end
                if (line.Contains("end", StringComparison.OrdinalIgnoreCase))
                {
                    // Если при найденной правой скобке пустой стек
                    if (tokens.Count == 0)
                    {
                        Warning = "(" + DateTime.Now + ") " + "Отсутствует левая скобка: " + "[Строка] = " + counter;
                        tokens.Clear();
                        break;
                    }

                    // Проверяем совпадают ли по типу найденная правая скобка с левыми
                    if ((tokens.Peek() != lexemes[1]) && (tokens.Peek() != lexemes[2]) && (tokens.Peek() != lexemes[3]))
                    {
                        Warning = "(" + DateTime.Now + ") " + "Не совпадают типы скобок: [Строка] = " + counter;
                        tokens.Clear();
                        break;
                    }
                    tokens.Pop();
                }
                counter++;
            }
            
            // Если после прохода алготима стек оказался не пустой
            if (tokens.Count != 0)
            {
                Warning = "(" + DateTime.Now + ") " +  "Отсутствует правая скобка: [Строка] = " + counter;

            }

            textBoxWarnings.Text = Warning + "\r\n" + "Отчет об ошибках сохранен:" + Directory.GetCurrentDirectory() + "/Warnings.txt";
            SaveWarningFile(Warning);
        }

        // Сохранение результатов анализа в файл
        public void SaveWarningFile(string Warning)
        {
            using FileStream fstream = new FileStream(Directory.GetCurrentDirectory() + "/Warnings.txt", FileMode.OpenOrCreate);
                // Преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(Warning);
                // Запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
        }

        // Клик меню открыть
        private void MenuOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
            Algorithm();
        }

        // Клик меню о программе
        private void MenuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Дисциплина: Лингвистическое и программное обеспечение САПР." +
                "Тема курсового проекта: Синтаксический анализ программ.\n" +
                "Вариант 7: СИНТАКСИЧЕСКИЙ АНАЛИЗ ПАРНЫХ ЭЛЕМЕНТОВ ПРОГРАММ\n" +
                "Выполнил: Студент 4 курса, Кафедра САПР ВС, Группа 846, Шмаков А.Ю., Проверил Скворцов С.В.");
        }

        // Клик меню выйти
        private void MenuClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
