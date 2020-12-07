using System;
using System.Diagnostics;

namespace SeSkarptEnumClasses
{
    public class Command // An implementation of Enum class.
    {
        public static Command Disconnect { get; } = new Command(0, "Disconnect");
        public static Command SayHello { get; } = new Command(1, "SayHello");
        public static Command GetData { get; } = new Command(2, "GetData");
        public static Command SetData { get; } = new Command(3, "SetData");
        public static Command None { get; } = new Command(4, "None");

        public string Name { get; private set; }
        public int Value { get; private set; }

        private Command(int val, string name)
        {
            Value = val;
            Name = name;
        }

        /// <summary>
        /// Method to find and return Command object based on a string with same name.
        /// </summary>
        /// <param name="contender"></param>
        /// <returns Command></returns>

        public Command FindCommand(string contender) 
        {
            if (Command.Disconnect.Name.Equals(contender))
            {
                return Command.Disconnect;
            }
            else if (Command.GetData.Name.Equals(contender))
            {
                return Command.GetData;
            }
            else if (Command.SetData.Name.Equals(contender))
            {
                return Command.SetData;
            }
            else if (Command.SayHello.Name.Equals(contender))
            {
                return Command.SayHello;
            }
            else if (Command.None.Name.Equals(contender))
            {
                return Command.None;
            }
            else
            {
                Debug.WriteLine("Invalid command, no matches! returned 'None' command");
                return Command.None;
            }

        }

    }
    public class Ledcolor // Enum implementation
    {
        public static Ledcolor Red { get; } = new Ledcolor(0, "Red");
        public static Ledcolor Blue { get; } = new Ledcolor(1, "Blue");
        public static Ledcolor Green { get; } = new Ledcolor(2, "Green");
        public static Ledcolor Magenta { get; } = new Ledcolor(3, "Magenta");
        public static Ledcolor Cyan { get; } = new Ledcolor(4, "Cyan");
        public static Ledcolor Yellow { get; } = new Ledcolor(5, "Yellow");
        public static Ledcolor White { get; } = new Ledcolor(6, "White");
        public static Ledcolor Off { get; } = new Ledcolor(7, "Off");
        public string Name { get; private set; }
        public int Value { get; private set; }

        private Ledcolor(int val, string name)
        {
            Value = val;
            Name = name;
        }

        /// <summary>
        /// Method to find and return Ledcolor object based on the name
        /// </summary>
        /// <param name="command"></param>
        /// <returns Ledcolor ></returns>
        public Ledcolor FindLedcolor(string command)
        {
            if (Ledcolor.Red.Name.Equals(command))
            {
                return Ledcolor.Red;
            }
            else if (Ledcolor.Green.Name.Equals(command))
            {
                return Ledcolor.Green;
            }
            else if (Ledcolor.Blue.Name.Equals(command))
            {
                return Ledcolor.Blue;       
            }
            else if (Ledcolor.Magenta.Name.Equals(command))
            {
                return Ledcolor.Magenta;
            }
            else if (Ledcolor.Cyan.Name.Equals(command))
            {
                return Ledcolor.Cyan;
            }
            else if (Ledcolor.Yellow.Name.Equals(command))
            {
                return Ledcolor.Yellow;
            }
            else if (Ledcolor.White.Name.Equals(command))
            {
                return Ledcolor.White;
            }
            else if (Ledcolor.Off.Name.Equals(command))
            {
                return Ledcolor.Off;
            }
            else
            {
                Debug.WriteLine("Invalid color, no matches! returned 'Off' Ledcolor");
                return Ledcolor.Off;
            }

        }

    }
}
