namespace AchievementTracker.Models{

    public class Game{

        public int gameID;
        public string name;
        public int achievementsEarned;
        public int maxAchievements;
        //this value is not part of the database
        //it will be calculated while adding the data to the app
        public double achievementPercent;


    }
}