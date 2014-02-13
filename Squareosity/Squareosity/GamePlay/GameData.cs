using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squareosity.GamePlay
{
     class GameData
    {
       public static  int totalScore = 0;
        public static int totalDeaths = 0;
        public static int hightestLevelcompleted = -1;
        public static int totalKills = 0;
        public static double totalGameTime = 0;

         public static int score
         {
             set { score += value; }
             get { return score; }
         }
    }

}
