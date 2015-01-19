using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numfuscator
{
    class Program
    {
        static void Main(string[] args)
        {
            var fuscator = new Obfuscator();
            fuscator.Find();
        }
    }

    class Fuscator
    {
        private Dictionary<IEnumerable<int>, Func<int, int, int>> _methods = new Dictionary
            <IEnumerable<int>, Func<int, int, int>>()
        {
            {new[] {0, 1, 2}, Add},
            {new[] {3, 4}, Subtract},
            {new[] {5, 6, 7}, Multiply},
            {new[] {8, 9}, Divide}
        };

        private static int Add(int one, int two)
        {
            return one + two;
        }

        private static int Subtract(int one, int two)
        {
            return one - two;
        }

        private static int Multiply(int one, int two)
        {
            return one*two;
        }

        private static int Divide(int one, int two)
        {
            return one/two;
        }

        
        public void Run()
        {
            var input = "";
            while (true)
            {
                Options();
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) || input.Length%3 != 0)
                    continue;
                if (input == "q")
                    break;

                var response = "";

                for (var i = 0; i < input.Length; i+=3)
                {
                    var one = int.Parse(input[i].ToString());
                    var two = int.Parse(input[i + 1].ToString());
                    var selector = int.Parse(input[i + 2].ToString());
                    var action = _methods.First(x => x.Key.Contains(selector));
                    var result = action.Value(one, two);
                    if (result < 0)
                        result += 10;
                    if (result >= 10)
                        result -= 10;
                    response += result.ToString();
                }
                Console.WriteLine(response);
            }
        }

        private void Options()
        {
            Console.WriteLine("Type a code or q to exit.");
        }
    }

    public class Obfuscator
    {

        private readonly Dictionary<string, IEnumerable<string>> _list = new Dictionary<string, IEnumerable<string>>();

        public Obfuscator()
        {
            GenerateNumbers();
        }
        private void Options()
        {
            Console.WriteLine("Type a number or q to exit.");
        }

        public void Find()
        {
            var input = "";
            while (true)
            {
                Options();
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    continue;
                if (input == "q")
                    break;

                var response = "";
                foreach (var t in input)
                {
                    var list = _list[t.ToString()];
                    response += list.PickRandom();
                }
                Console.WriteLine(response);
            }

        }

        private IEnumerable<string> FindDigits(int digit, Func<int, int, int> action, params int[] parts)
        {
            for (int i = 1; i < 11; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    var result = GetNormalizedInt(action(i, j));
                    if (result == GetNormalizedInt(digit))
                    {
                        foreach (var part in parts)
                            yield return NormalizeDigits(i, j, part);
                    }
                }
            }
        }

        private string NormalizeDigits(int one, int two, int three)
        {
            one = GetNormalizedInt(one);
            two = GetNormalizedInt(two);

            return string.Format("{0}{1}{2}", one, two, three);
        }

        private int GetNormalizedInt(int digit)
        {
            if (digit >= 10)
                return digit - 10;
            if (digit < 0)
                return digit + 10;
            return digit;
        }

        private IEnumerable<string> FindNumbers(int digit)
        {
            if (digit == 0)
                digit = 10;
            
            // Additions
            foreach (var num in FindDigits(digit, (i, i1) =>
            {
                return i + i1;
            }, 0, 1, 2))
                yield return num;

            // Subtracts
            foreach (var num in FindDigits(digit, (i, i1) =>
            {
                return i - i1;
            }, 3, 4))
                yield return num;

            // Multiplies
            foreach (var num in FindDigits(digit, (i, i1) =>
            {
                return i*i1;
            }, 5, 6, 7))
                yield return num;

            // Divides
            foreach (var num in FindDigits(digit, (i, i1) =>
            {
                return i/i1;
            }, 8, 9))
                yield return num;
        }

        private void GenerateNumbers()
        {
            for (int i = 0; i < 10; i++)
            {
                _list.Add(i.ToString(), FindNumbers(i));
            }
        }
    }
}
