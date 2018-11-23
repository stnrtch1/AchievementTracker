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
            //check if form fields are valid
            //ERRORCHECK 1: Name is not null
            if (achieveTrack.name != null){
                //ERRORCHECK 2: Achievements Earned Are Above Or Zero 
                if (achieveTrack.achievementsEarned >= 0){
                    //ERRORCHECK 3: Achievement Earned Are Below Or Equal To The Max Count
                    if (achieveTrack.achievementsEarned <= achieveTrack.maxAchievements){
                        //ERRORCHECK 4: Max Achievments Are Above Zero
                        if (achieveTrack.maxAchievements > 0){
                            //All fields have passed, submit the game
                            bool result = achieveTrack.submit();
                            if (result == true){
                                //Game has been submitted
                                TempData["feedback"] = "Game has been submitted";
                            } else{
                                //GAME HAS FAILED SUBMISSION
                                TempData["feedback"] = "ERROR: GAME FAILED TO SUBMIT!!";
                            }
                        } else{ TempData["feedback"] = "ERROR: MAX ACHIEVEMENTS CAN NOT BE BELOW OR AT ZERO"; }
                    } else{ TempData["feedback"] = "ERROR: ACHIEVEMENTS EARNED CAN NOT BE MORE THAN MAX ACHIEVEMENTS";}
                } else{ TempData["feedback"] = "ERROR: ACHIEVEMENTS EARNED CAN NOT BE BELOW ZERO"; }
            } else{ TempData["feedback"] = "ERROR: NO NAME SUBMITTED"; }
        
            return RedirectToAction("Index",achieveTrack);
        }

        [HttpPost]
        public IActionResult Delete(AchieveTrack achieveTrack){
            return View(achieveTrack);
        }

        [HttpPost]
        public IActionResult DeleteSubmit(AchieveTrack achieveTrack){
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
            Console.WriteLine("\n\n>>Editing Game: " + achieveTrack.gameID + "\n\n");

            //check if form fields are valid
            //ERRORCHECK 1: Achievements Earned Are Above Or Zero 
            if (achieveTrack.achievementsEarned >= 0){
                //ERRORCHECK 2: Achievement Earned Are Below Or Equal To The Max Count
                if (achieveTrack.achievementsEarned <= achieveTrack.maxAchievements){
                    //ERRORCHECK 3: Max Achievments Are Above Zero
                    if (achieveTrack.maxAchievements > 0){
                        //All fields have passed, submit the game
                        bool result = achieveTrack.editSubmit();
                        if (result == true){
                            //Game has been edited
                            TempData["feedback"] = "Game has been edited";
                        } else{
                            //GAME HAS FAILED EDITING
                            TempData["feedback"] = "ERROR: GAME FAILED TO EDIT!!";
                        }
                    } else{ TempData["feedback"] = "ERROR: MAX ACHIEVEMENTS CAN NOT BE BELOW OR AT ZERO"; }
                } else{ TempData["feedback"] = "ERROR: ACHIEVEMENTS EARNED CAN NOT BE MORE THAN MAX ACHIEVEMENTS";}
            } else{ TempData["feedback"] = "ERROR: ACHIEVEMENTS EARNED CAN NOT BE BELOW ZERO"; }
            
            return RedirectToAction("Index",achieveTrack);
        }
    }
}
