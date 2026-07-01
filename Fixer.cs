using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace CSVFixer
{
    public class Fixer
    {
        public static void FixCsv(string inputPath, string outputPath)
        {
            // Кодировка Revit 2024 - Windows-1251,
            // хотя на форуме написана UTF-18 LE
            Encoding encoding = Encoding.GetEncoding(1251);
            string[] lines;

            try
            {
                lines = File.ReadAllLines(inputPath, encoding);
                if (lines.Length == 0) return;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            string[] fixedLines = new string[lines.Length];

            // Первая строка — заголовок, не трогаем
            fixedLines[0] = lines[0].TrimStart();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) // Если строка пустая - пропуск
                {
                    fixedLines[i] = lines[i];
                    continue;
                }

                // Разделение строки по запятым, после которых нет пробела
                string[] fields = Regex.Split(lines[i], @",(?!\s)");

                for (int j = 0; j < fields.Length; j++)
                {
                    string field = fields[j].Trim(); // Удаление лишних пробелов

                    // Всё что состоит из двух и более слов и не число - в кавычки
                    if (!double.TryParse(field, NumberStyles.Any, CultureInfo.InvariantCulture, out _)
                        && !field.StartsWith("\""))
                    {
                        fields[j] = $"\"{field}\"";
                    }
                }

                // Собираем строку обратно через стандартную запятую
                fixedLines[i] = string.Join(",", fields);
            }

            // Сохраняем исправленный файл
            File.WriteAllLines(outputPath, fixedLines, encoding);
        }
    }
}
