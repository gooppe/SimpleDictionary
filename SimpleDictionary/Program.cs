using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleDictionary
{
    //Класс программы. Это базовы класс приложения
    class Program
    {
        //Данный метод вызывается самым первым, после запуска программы, в него передается список аргументов -
        //параметры запуска приложения
        static void Main(string[] args)
        {
            PrintHello();

            //Создадим объект словаря, для дальнейшей работы с ним
            MyDictionary mydict = new MyDictionary();

            while(true)
            {
                //С помощью метода ParseCommands() получим введеную команду
                MyDictCommand command = ParseCommands();
                
                //В зависимости от команды выполним определенные действия
                switch (command)
                {
                    case MyDictCommand.Exit:
                        //Для выхода завершим работу метода оператором return
                        return;
                    case MyDictCommand.AddWord:
                        mydict.AddNewWordToDictionary();
                        break;
                    case MyDictCommand.WirteDictation:
                        mydict.WriteDictation();
                        break;
                    case MyDictCommand.Unknown:
                    default:
                        Console.WriteLine("I don't understand you, man.");
                        break;
                }
            }
        }

        //Данный метод производит разбор и определение введеной команды
        private static MyDictCommand ParseCommands()
        {
            //Считаем строку из консоли
            string cmd = Console.ReadLine();

            //Обработаем строки: удалим лишние пробелы и приведем все к нижнему регистру.
            //Вернем нужные перечисления в зависимости от введеной команды
            switch (cmd.Trim().ToLower())
            {
                case "exit":
                case "close":
                case "goodbye":
                case "bye":
                    return MyDictCommand.Exit;
                case "add word":
                    return MyDictCommand.AddWord;
                case "start test":
                    return MyDictCommand.WirteDictation;
                default :
                    return MyDictCommand.Unknown;    
            }
        }

        //Метод выведет приветствующее сообщение в консоль
        private static void PrintHello()
        {
            //Изменим цвет. Для этого укажем консоли, какой цвет текста ей следует использовать.
            Console.ForegroundColor = ConsoleColor.Yellow;
            //Выведем текст
            Console.WriteLine("Hello! It's a simple dictionary. You can prove your language skills!");
            Console.WriteLine("How it works:");
            Console.WriteLine("Write \"Add word\" to add new word into the dictionary");
            Console.WriteLine("Write \"Start test\" to write a dictation");
            //Восстановим стандартный цвет консоли
            Console.ResetColor();
        }

        //Вывести ошибку
        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        //Вывести сообщение об успехе
        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    //Класс нашего словаря
    class MyDictionary
    {   
        //Имя файла, в котором будет храниться наш словарь
        //Данное поле является константой
        const string DictionaryFileName = "dictionary.txt";
        //Число слов, из которого будет состоять диктант
        const int DictationWordCount = 10;

        //Поле, содержащее в себе список всех записей словаря
        List<Record> tmpDictionary;

        //Конструктор класса словаря
        public MyDictionary()
        {
            //Сперва создадим список записей.
            tmpDictionary = new List<Record>();
            //Проверяем наличие файла словаря
            //Если его нет, то создадим новый
            if (!File.Exists(DictionaryFileName))
                File.Create(DictionaryFileName);
            //Данная конструкция обрабатывает исключения, вызываемые в коде
            try
            {
                //Считываем в массив все строки из файла словаря
                string[] lines = File.ReadAllLines(DictionaryFileName);
                foreach (string line in lines)
                {
                    //Разделяем строку на два слова
                    string[] words = line.Split(',');
                    //Если слов оказалось меньше или больше, то пропускаем строку так как она некорректна.
                    if (words.Length != 2)
                        continue;
                    //Создаем новую запись и добавляем ее в словарь в памяти
                    Record rec = new Record(words[0].Trim(), words[1].Trim());
                    tmpDictionary.Add(rec);
                }
            }
            catch
            {
                //Если было перехвачено какое-то исключение, то выведем сообщение об этом.
                Program.PrintError("Couldn't open dictionary!");
            }
        }

        //Метод добавляющий новое слово в словарь 
        public void AddNewWordToDictionary()
        {
            //Считываем слово и перевод
            Console.Write("Word: ");
            string word = Console.ReadLine().Trim().ToLower();
            Console.Write("Translation: ");
            string translation = Console.ReadLine().Trim().ToLower();

            //Проверяем введеные данные на корректность
            if (word.Length == 0 || translation.Length == 0)
            {
                Console.WriteLine("Incorrect word or translation!");
                return;
            }
            try
            {
                //Записываем слово в словарь.
                File.AppendAllText(DictionaryFileName, string.Format("{0}, {1}\n", word, translation));
                //Также добавим слово в наш список, чтобы не обращаться каждый раз к файлу
                tmpDictionary.Add(new Record(word, translation));
                Console.WriteLine("Word was successfully added");
            }
            catch
            {
                Program.PrintError("Cannot add word into the dicionary");
                return;
            }
        }

        //Метод, позволяющий написать диктант
        public void WriteDictation()
        {
            //Создадим объект, позвляющий генерировать псевдослучайные числа
            Random rnd = new Random(DateTime.Now.Millisecond);
            //Число определим число слов как минимум из числа всех слов и слов, которые мы будем писать в диктанте
            int wordCount = Math.Min(DictationWordCount, tmpDictionary.Count);
            //Определим число слов, которые были написаны правильно
            int trueAnswerCount = 0;
            for (int i = 0; i < wordCount; i++)
            {
                //Выберем случайный номер слова
                int wordNumber = rnd.Next(tmpDictionary.Count);
                //Выведем его
                Console.Write(tmpDictionary[wordNumber].Translation + " - ");
                //Считаем ответ
                string answer = Console.ReadLine();
                //Проверим
                if (answer.Trim().ToLower() == tmpDictionary[wordNumber].Word)
                {
                    //Если все верно, то увеличиваем счетчик верных ответов на единицу
                    trueAnswerCount++;
                    Program.PrintSuccess("True!");
                }
                else
                {
                    //Если неверно, то напишем об этом.
                    Program.PrintError(string.Format("You'r wrong: true answer is \"{0}\"", tmpDictionary[wordNumber].Word));
                }
            }
            //Выведем общий результат
            Program.PrintSuccess(string.Format("Your score is {0:0.00}", (float)trueAnswerCount / wordCount * 100));
        }
    }

    //Структура записи
    struct Record
    {
        //Слово
        public string Word;
        //Перевод
        public string Translation;
        //Конструктор структуры
        public Record(string Word, string Translation)
        {
            //Здесь с помощью ключевого слова this мы обращаемся не к локальной переменной,
            //а к глобальной переменной объявленной в теле структуры.
            this.Word = Word;
            this.Translation = Translation;
        }
    }

    //Перечисление, содержащее возможные команды
    enum MyDictCommand
    {
        Unknown,
        Exit,
        AddWord,
        WirteDictation
    }
}
