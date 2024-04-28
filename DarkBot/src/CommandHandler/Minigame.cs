using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkBot.src.CommandHandler
{
	internal class Minigame
	{
        //rps fucntion returns 0 [Player Won] 1[tie] 2 [Player lost]
        public static int HandleRpsResult(string userChoice, string compChoice) //param: usrChoice of type string 
        {
            userChoice = userChoice.ToUpper();
            compChoice = compChoice.ToUpper();

            if (userChoice == "ROCK" && compChoice == "SCISSORS")
            {
                return 0;
            }
            else if (userChoice == "ROCK" && compChoice == "PAPER")
            {
                return 2;
            }
            else if (userChoice == "PAPER" && compChoice == "ROCK")
            {
                return 0;
            }
            else if (userChoice == "PAPER" && compChoice == "SCISSORS")
            {
                return 2;
            }
            else if (userChoice == "SCISSORS" && compChoice == "ROCK")
            {
                return 2;
            }
            else if (userChoice == "SCISSORS" && compChoice == "PAPER")
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
