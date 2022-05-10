
namespace UIElements
{
    using System.Text;


    static class UI
    {
        public const int ESCAPE = -1;

        public static void ChangeEncoding(Encoding input, Encoding output)
        {
            Console.OutputEncoding = output;
            Console.InputEncoding = input;
        }

        public static void MySetColor(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        static int ManageChoice(ref ushort choice, ushort answerCount, bool IsEscape)
        {
            // Uİ'da dəyərlərin keyboardla idarəsini təmin edir.

            ConsoleKey kb = Console.ReadKey().Key;

            switch (kb)
            {
                case ConsoleKey.W:
                case ConsoleKey.A:
                case ConsoleKey.UpArrow:
                case ConsoleKey.LeftArrow:
                    if (choice == 0) choice = answerCount;
                    choice--;
                    return 1;
                case ConsoleKey.S:
                case ConsoleKey.D:
                case ConsoleKey.DownArrow:
                case ConsoleKey.RightArrow:
                    choice++;
                    choice %= answerCount;
                    return 1;
                case ConsoleKey.Enter:
                    return 0;
                case ConsoleKey.Escape:
                    if (IsEscape) return -1;
                    return 1;
                default:
                    return 1;
            }
        }

        public static int GetChoice<T>(string question, T[] answers, bool IsEscape = false)
        {
            // KEYBOARD - CONTROLLED Menu ilə MOD Seçimi

            ushort answerCount = Convert.ToUInt16(answers.Length);
            sbyte notFound = 1;
            ushort choice = 0;

            while (notFound > 0)
            {
                Console.Clear();
                Console.WriteLine(question);
                for (ushort i = 0; i < answerCount; i++)
                {
                    char prefix = ' ';
                    if (i == choice) { MySetColor(ConsoleColor.DarkGreen, ConsoleColor.Gray); prefix = '◙'; }
                    Console.WriteLine($" {prefix} << {answers[i]} >>");
                    Console.ResetColor();
                }
                if (IsEscape) Console.WriteLine("Press ESC To Exit...");
                notFound = Convert.ToSByte(ManageChoice(ref choice, answerCount, IsEscape));
                if (notFound == ESCAPE) return notFound;
            }

            return choice;
        }

    }
}