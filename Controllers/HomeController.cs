using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AchievementTracker.Models;

namespace AchievementTracker.Controllers{

    public class HomeController : Controller{

        public IActionResult Index(){
            // INDEX: Construct model and show database
            AchieveTrack achieveTrack = new AchieveTrack();
            achieveTrack.setupMe();
            return View(achieveTrack);
        }

        public IActionResult Add(AchieveTrack achieveTrack){
            return View(achieveTrack);
        }

        [HttpPost]
        public IActionResult AddSubmit(AchieveTrack achieveTrack){
            //check if the model state is still valid
            if(!ModelState.IsValid) return View("index");

            //check if the user has more achievements Earned than Max Achievements
            if (achieveTrack.achievementsEarned > achieveTrack.maxAchievements){
                //if they did, notify them and keep them on the view
                TempData["feedback"] = "HEY! What did I say about putting more achievements earned than max achievements.";
                return View("Add");
            }

            //All fields have passed, submit the game
            bool result = achieveTrack.submit();
            if (result == true){
                //Game has been submitted
                TempData["feedback"] = "Game has been submitted";
            } else{
                //GAME HAS FAILED SUBMISSION
                TempData["feedback"] = "ERROR: GAME FAILED TO SUBMIT!!";
            }
                        
        
            return RedirectToAction("Index",achieveTrack);
        }

        [HttpPost]
        public IActionResult Delete(AchieveTrack achieveTrack){
            return View(achieveTrack);
        }

        [HttpPost]
        public IActionResult DeleteSubmit(AchieveTrack achieveTrack){
            //check if the model state is still valid
            //if(!ModelState.IsValid) return View("index");

            //grab the dropdown data and delete the game
            achieveTrack.delete();
            return RedirectToAction("Index",achieveTrack);
        }

        [Route("Home/Edit/{gameID}")]
        public IActionResult Edit(AchieveTrack achieveTrack, string gameID){
            Console.WriteLine("Editing Game: " + gameID);
            //get the details of the selected game
            achieveTrack.editGet();
            return View(achieveTrack);
        }

        [HttpPost]
        public IActionResult EditSubmit(AchieveTrack achieveTrack){
            //check if the model state is still valid
            //if(!ModelState.IsValid) return View("index");

            //Console.WriteLine("\n\n>>Editing Game: " + achieveTrack.gameID + "\n\n");

            //check if the user has more achievements Earned than Max Achievements
            if (achieveTrack.achievementsEarned > achieveTrack.maxAchievements){
                //if they did, notify them and keep them on the view
                TempData["feedback"] = "HEY! What did I say about putting more achievements earned than max achievements.";
                string gameID = achieveTrack.gameID.ToString();
                achieveTrack.editGet();
                return View("Edit",achieveTrack);
            }

            //All fields have passed, submit the game
            bool result = achieveTrack.editSubmit();
            if (result == true){
                //Game has been edited
                TempData["feedback"] = "Game has been edited";
            } else{
                //GAME HAS FAILED EDITING
                TempData["feedback"] = "ERROR: GAME FAILED TO EDIT!!";
            }
                    
            
            return RedirectToAction("Index",achieveTrack);
        }
    }
}
