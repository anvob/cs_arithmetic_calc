using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_arithmetic_calc
{
    class Program
    {
        /// <summary>
        /// Вычисление выражения
        /// </summary>
        /// <param name="s">Текстовое выражение</param>
        /// <returns>Результат вычисления</returns>
        public static double Calculation(string s)
        {
            s = '(' + s + ')';
            Stack<double> Operands = new Stack<double>();
            Stack<char> Functions = new Stack<char>();
            string stageSolution = "\nРезультат:\n";
            int pos = 0;
            object symb;
            object prevSymb = '(';

            do
            {
                symb = getSymb(s, ref pos);

                //Проверка на повтор операторов которые не должны идти подряд
                if (symb is char && prevSymb is char && "*/+-".Contains((char)prevSymb) && "*/+-".Contains((char)symb))
                {
                    throw new Exception("\nНеправильная запись выражения: Неверная запись операторов! \"" + (char)prevSymb + (char)symb + "\"\n");
                }

                // Унарный + и -
                if (symb is char && prevSymb is char && (char)prevSymb == '(' && ((char)symb == '+' || (char)symb == '-'))
                {
                    Operands.Push(0); // Добавляем нулевой элемент
                }

                if (symb is double) // Если очередной объект операнд
                {
                    // Если операнд стоит после ")" 
                    if (prevSymb is char && (char)prevSymb==')')
                    {
                        throw new Exception("Неправильная запись выражения: \"" + (char)prevSymb + (double)symb + "\"");
                    }
                    // Если операнд стоит после операнда
                    if(symb is double && prevSymb is double)
                    {
                        throw new Exception("Неправильная запись выражения: Неверная запись операндов! " + (double)prevSymb + " " + (double)symb);
                    }
                    Operands.Push((double)symb); 
                }
                else if (symb is char) // Если очередной собъект оператор
                {
                    if ((char)symb == ')')
                    {
                        while (Functions.Count > 0 && Functions.Peek() != '(')
                            popFunction(Operands, Functions, ref stageSolution);
                        try
                        { 
                            Functions.Pop(); // Удаляем "("
                        }
                        catch
                        {
                            throw new Exception("Неправильная запись выражения: Не найдена открывающая скобка!");
                        }
                    }
                    else
                    {
                        if (prevSymb is double && (char)symb == '(')// Если перед "(" стоит операнд
        				{
                            throw new Exception("Неправильная запись выражения: \"" + prevSymb + symb + "\"");
                        }
                        while (canPop((char)symb, Functions)) // Если можно вытолкнуть
                        {
                            popFunction(Operands, Functions, ref stageSolution); // то 
                        }
                        Functions.Push((char)symb); // Кидаем новую операцию в стек
                    }
                }
                prevSymb = symb;
            }
            while (symb != null);

            if (Operands.Count > 1 || Functions.Count > 0)
            {
                throw new Exception("Неправильная запись выражения");
                return 0;
            }
            else
            {
                Console.WriteLine(stageSolution);
            }

            return Operands.Pop();
        }

        //Вычисление простой операции
        private static void popFunction(Stack<double> Operands, Stack<char> Functions, ref string stageSolution)
        {
            double B = Operands.Pop();
            double A = Operands.Pop();
            double res = -1; ;
            char F = Functions.Pop();

            switch (F)
            {
                case '+':
                    res = (A + B);
                    break;
                case '-':
                    res = (A - B);
                    break;
                case '*':
                    res = (A * B);
                    break;
                case '/':
                    res = (A / B);
                    break;
            }
            Operands.Push(res);
            stageSolution += A + F.ToString() + B + " = " + res + "\n";
        }

        //Проверка приорететов операторов
        private static bool canPop(char op1, Stack<char> Functions)
        {
            if (Functions.Count == 0)
                return false;
            int p1 = getPriority(op1);
            int p2 = getPriority(Functions.Peek());

            return p1 >= 0 && p2 >= 0 && p1 >= p2;
        }

        //Получаем приоритет оператора
        private static int getPriority(char op)
        {
            switch (op)
            {
                case '(':
                    return -1; 
                case '*':
                case '/':
                    return 1;
                case '+':
                case '-':
                    return 2;
                default:
                    throw new Exception("Неправильная запись выражения: Не найден оператор \""+op+ "\"");
            }
        }

        //Получаем очередной символ
        private static object getSymb(string s, ref int pos)
        {
            readWhiteSpace(s, ref pos);
            if (pos == s.Length) // конец строки
                return null;
            if (char.IsDigit(s[pos]))
                return Convert.ToDouble(readDouble(s, ref pos));
            else
                return readFunction(s, ref pos);
        }

        //Считываем оператор
        private static char readFunction(string s, ref int pos)
        {
            return s[pos++];
        }

        //Считываем операнд
        private static string readDouble(string s, ref int pos)
        {
            string res = "";
            while (pos < s.Length && (char.IsDigit(s[pos]) || s[pos] == '.'))
                res += s[pos++];
            return res;
        }

        //Читаем пробелы
        private static void readWhiteSpace(string s, ref int pos)
        {
            while (pos < s.Length && char.IsWhiteSpace(s[pos]))
                pos++;
        }

        static void Main(string[] args)
        {
            bool run = true;
            do
            {
                Console.WriteLine("Введите математическое выражение и нажмите Enter.\nДля выхода из программы введите end и нажмите Enter.\n");
                string text = Console.ReadLine();
                if (text.Trim() == "end")
                {
                    run = false;
                }
                else
                {
                    try
                    {
                        Console.WriteLine("\nОтвет: " + Calculation(text) + "\n");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "\n");
                    }
                }
            }
            while (run);
            Console.WriteLine("Для выхода нажмите любую клавишу");
            Console.ReadLine();
        }
    }


}
