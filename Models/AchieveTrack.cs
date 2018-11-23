using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;


namespace AchievementTracker.Models{

    public class AchieveTrack{

        //DATABASE CONNECTIVITY VARIABLES
        private MySqlConnection dbConnection;
        //private static string connectionString = "Database=dotnetcoreSamples;Data Source=localhost;User Id=root;Password=password;SslMode=None;";
        private static string connectionString = "Database=[dbName];Data Source=localhost;User Id=root;Password=[yourPassword];SslMode=None;";
        private MySqlCommand dbCommand;
        private MySqlDataReader dbReader;

        //PROPERTY VARIBLES
        private int _count;
        private List<Game> _games;
        //the selected game object, for when editing the game
        public Game selectedGame;

        public AchieveTrack(){
            //initalize properties
            _count = 0;
            _games = new List<Game>();
            selectedGame = new Game();

            //construct required DB objects for use
            dbConnection = new MySqlConnection(connectionString);
            dbCommand = new MySqlCommand("", dbConnection);
        }

        //------------------------------------------------GETS/SETS
        public int count{
            get{
                return _count;
            }
        }

        public List<Game> games{
            get{
                return _games;
            }
        }

        //properties to be handed into the database
        [Key]
        public int gameID {get;set;} = 0;
        [Required]
        [Display(Name="Name")]
        [MaxLength(100, ErrorMessage="Whoa, let's keep this simple. 100 Character Max")]
        public string name {get;set;}
        [Required]
        public int achievementsEarned {get;set;}
        [Required]
        public int maxAchievements {get;set;}

        //grand total properties
        public double earnedTotal {get;set;} = 0;
        public double earnedMax {get;set;} = 0;
        public double earnedPercent {get;set;}

        //------------------------------------------------PUBLIC METHODS
        public void setupMe(){
            //call private methods for setting up the app
            getGameData();
        }

        public bool submit(){
            try{
                dbConnection.Open();

                //clear parameters from before and create new ones with the command
                dbCommand.Parameters.Clear();
                dbCommand.CommandText = "INSERT INTO tblAchievements (name,achievementsEarned,maxAchievements) VALUE (?name,?earned,?max)";
                dbCommand.Parameters.AddWithValue("?name", name);
                dbCommand.Parameters.AddWithValue("?earned",achievementsEarned);
                dbCommand.Parameters.AddWithValue("?max",maxAchievements);

                //execute the command
                dbCommand.ExecuteNonQuery();

            } catch (Exception e){
                Console.WriteLine("\n\n>>Error at submitting the new game!");
                Console.WriteLine(e.Message + "\n\n");
                return false;
            } finally{
                dbConnection.Close();
            }

            return true;
        }

        public bool delete(){
            try{
                //open the connection
                dbConnection.Open();

                //create and issue the request
                dbCommand.CommandText = "DELETE FROM tblAchievements WHERE gameID = ?gameID";
                dbCommand.Parameters.AddWithValue("?gameID",gameID);
                dbCommand.ExecuteNonQuery();

            } catch (Exception e){
                Console.WriteLine("\n\n>>ERROR!<<");
                Console.WriteLine(">>Message : " + e.Message + " <<\n\n");
            } finally{
                dbConnection.Close();
            }
            return true;
        }

        public void editGet(){
            try{
                //open the connection
                dbConnection.Open();

                //create and issue the request
                dbCommand.Parameters.Clear();
                dbCommand.CommandText = "SELECT * FROM tblAchievements WHERE gameID = ?gameID";
                dbCommand.Parameters.AddWithValue("?gameID", gameID);
                dbReader = dbCommand.ExecuteReader();

                while (dbReader.Read()){
                    //populate the selected game data
                    selectedGame.gameID = Convert.ToInt32(dbReader["gameID"]);
                    selectedGame.name = dbReader["name"].ToString();
                    selectedGame.achievementsEarned = Convert.ToInt32(dbReader["achievementsEarned"]);
                    selectedGame.maxAchievements = Convert.ToInt32(dbReader["maxAchievements"]);
                }

            }catch (Exception e){
                Console.WriteLine("\n\n>>ERROR!<<");
                Console.WriteLine(">>Message : " + e.Message + " <<\n\n");
            }finally{
                //close the connection once done
                dbConnection.Close();
            }
        }

        public bool editSubmit(){
            try{
                dbConnection.Open();

                //clear parameters from before and create new ones with the command
                dbCommand.Parameters.Clear();
                dbCommand.CommandText = "UPDATE tblAchievements SET achievementsEarned=?earned,maxAchievements=?max WHERE gameID = ?gameID";
                dbCommand.Parameters.AddWithValue("?earned",achievementsEarned);
                dbCommand.Parameters.AddWithValue("?max",maxAchievements);
                dbCommand.Parameters.AddWithValue("?gameID",gameID);

                //execute the command
                dbCommand.ExecuteNonQuery();

            } catch (Exception e){
                Console.WriteLine("\n\n>>Error at editing the game!");
                Console.WriteLine(e.Message + "\n\n");
                return false;
            } finally{
                dbConnection.Close();
            }

            return true;
        }

        //------------------------------------------------PRIVATE METHODS
        private void getGameData(){
            try{
                //open the connection
                dbConnection.Open();

                //create and issue the request
                dbCommand.CommandText = "SELECT * FROM tblAchievements ORDER BY name ASC";
                dbReader = dbCommand.ExecuteReader();

                while (dbReader.Read()){
                    //populate the games list
                    Game game = new Game();
                    game.gameID = Convert.ToInt32(dbReader["gameID"]);
                    game.name = dbReader["name"].ToString();
                    game.achievementsEarned = Convert.ToInt32(dbReader["achievementsEarned"]);
                    game.maxAchievements = Convert.ToInt32(dbReader["maxAchievements"]);
                    //get the percentage of achievements earned using the earned and max values
                    game.achievementPercent = getPercentage(Convert.ToDouble(dbReader["achievementsEarned"]),Convert.ToDouble(dbReader["maxAchievements"]));
                    _games.Add(game);

                    //add the achievement values to the grand totals
                    earnedTotal = earnedTotal + game.achievementsEarned;
                    earnedMax = earnedMax + game.maxAchievements;
                }

                //get the number of games in the list, just to see if the database is empty
                _count = _games.Count;

                //calculate the percentage of achievements earned in the max total
                earnedPercent = getPercentage(earnedTotal,earnedMax);

            }catch (Exception e){
                Console.WriteLine("\n\n>>ERROR!<<");
                Console.WriteLine(">>Message : " + e.Message + " <<\n\n");
            }finally{
                //close the connection once done
                dbConnection.Close();
            }
        }

        private double getPercentage(double earned, double max){
            double percent;
            //round the percentage value to two spaces
            percent = Math.Round(((earned/max)*100),2);
            return percent;
        }

        //------------------------------------------------------------------------------------STATIC METHODS
        public static List<SelectListItem> getList() {

            List<SelectListItem> list = new List<SelectListItem>();
            // connectionString is set to private static above so I have access - everything else I redeclare in the static scope
            MySqlConnection dbConnection = new MySqlConnection(connectionString);

            try {
                dbConnection.Open();
                MySqlCommand dbCommand = new MySqlCommand("SELECT * FROM tblAchievements", dbConnection);
                MySqlDataReader dbReader = dbCommand.ExecuteReader();
                while (dbReader.Read()) {
                    SelectListItem item = new SelectListItem();
                    item.Text = dbReader["name"] + " " + dbReader["achievementsEarned"] + "/" + dbReader["maxAchievements"];
                    item.Value = dbReader["gameID"].ToString();
                    list.Add(item);
                }

            } finally {
                dbConnection.Close();
            }

            return list;
        }

    }


}