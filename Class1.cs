using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

/* This code was written for the TFRP Dev Challenge by FrontLineBanana.
 * This is the CLIENT side code for the challenge.*/

/* To do:
 * Set up 3 instances: Overworld, InstanceA, InstanceB
 * Set up chat commands: JOIN to join A or B, LEAVE to leave current instance (If A or B), SHOW to show current players in instance.
 * Make it so A and B don't overlap (Players cannot hear, see, text chat, or bump into players in opposite instance)
 * Sync state of player to server.
 */

namespace DevChallenge
{
    public class Class1 : BaseScript
    {
        //Variables
        bool isInA_ = false;
        bool isInB_ = false;
        bool isInOverworld_ = true;

        string conditions = "";

        //Instance Creation
        //Reminder: Instances are just lists of player ids
        List<string> Overworld = new List<string>();
        List<string> InstanceA = new List<string>();
        List<string> InstanceB = new List<string>();

        //Event Handler
        public void RegisterEventHandler(string name, Delegate action)
        {
            try
            {
                EventHandlers[name] += action;
            }
            catch (Exception ex)
            {
                CitizenFX.Core.Debug.WriteLine("Error trying to register event");
            }
        }

        //Start Resource and Client
        public Class1()
        {
            //Start Resource
            CitizenFX.Core.Debug.WriteLine("Instancing started!");
            StartResource();

            //Add everyone to Overworld
            JoinInstance("Over");

            //Provide warning if not in overworld instance
            if (!isInOverworld_)
            {
                CitizenFX.Core.Debug.WriteLine("WARNING: NOT IN OVERWORLD. PLEASE RELOG TO FIX IF YOU CANNOT SEE/HEAR ANYONE.");
                isInOverworld_ = true;
            }

        }

        //Instance Syncing
        void JoinInstance(string instanceToJoin)
        {
            string playerString = PlayerPedId().ToString();

            if (instanceToJoin == "A")
            {
                InstanceA.Add(playerString);
            }
            if (instanceToJoin == "Over")
            {
                Overworld.Add(playerString);
            }
            else if (instanceToJoin == "B")
            {
                InstanceB.Add(playerString);
            }

        }

        void LeaveInstance()
        {
            string playerString = PlayerPedId().ToString();

            if (isInA_)
            {
                InstanceA.Remove(playerString);
            }
            else if (isInB_)
            {
                InstanceB.Remove(playerString);
            }
        }


        //Chat Commands

        public void StartResource()
        {
            RegisterCommand("join", new Action<int, List<object>, string>((source, args, raw) =>
            {
                conditions = Convert.ToString(args[0]);
                conditions.TrimEnd();

                //Determine which instance to join or if already in the instance
                if (args.Count() > 0)
                {
                    if (conditions == "A" || conditions == "a")
                    {
                        if (!isInA_)
                        {
                            // put player in A
                            JoinInstance("A");
                            isInA_ = true;
                            isInB_ = false;
                        }
                        else if (isInA_)
                        {
                            TriggerEvent("chat:addMessage", new
                            {
                                color = new[] { 255, 0, 0 },
                                args = new[] { "[Instances]", $"You are already in Instance A!" }
                            });
                        }
                    }
                    else if (conditions == "B" || conditions == "b")
                    {
                        if (!isInB_)
                        {
                            // put player in B
                            JoinInstance("B");
                            isInB_ = true;
                            isInA_ = false;
                        }
                        else if (isInB_)
                        {
                            TriggerEvent("chat:addMessage", new
                            {
                                color = new[] { 255, 0, 0 },
                                args = new[] { "[Instances]", $"You are already in Instance B!" }
                            });
                        }
                    }
                    else
                    {
                        TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 0, 0 },
                            args = new[] { "[Instances]", $"Proper usage: /join [A/B]" }
                        });
                    }
                }
            }), false);

            RegisterCommand("leave", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (isInA_ || isInB_)
                {
                    LeaveInstance();
                    isInA_ = false;
                    isInB_ = false; 
                    
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] { "[Instances]", $"Left Instance." }
                    });
                }
                else if (!isInA_ || !isInB_)
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] { "[Instances]", $"You are currently not in an instance!" }
                    });
                }
            }), false);

            RegisterCommand("show", new Action<int, List<object>, string>((source, args, raw) =>
            {
                
                TriggerEvent("chat:addMessage", new
                {
                    color = new[] { 255, 0, 0 },
                    args = new[] { "[Instances]", $"Players in Instance: " + InstanceA.ToString() }
                });
                CitizenFX.Core.Debug.WriteLine("EAT MY ASS");

            }), false);
        }

        //Instance Collision Handling

        void InstanceRules()
        {
            //Set player ped
            var playerPed = Game.PlayerPed;

            //Cannot see, touch, or hear other instance
            if (isInA_)
            {
                // turn B invis, turn B walk-through, turn B mute
                for (int i = 0; i < InstanceB.Count; i++)
                {
                    //Set player to not see anyone in Instance B
                    //Set player to not hear anyone in Instance B
                    //Set player to not collide with anyone in Instance B
                    //Set player to not see text chat with anyone in Instance B
                }
            }
            
            if (isInB_)
            {
                // turn A invis, turn A walk-through, turn A mute
                for (int i = 0; i < InstanceA.Count; i++)
                {
                    //Set player to not see anyone in Instance A
                    //Set player to not hear anyone in Instance A
                    //Set player to not collide with anyone in Instance A
                    //Set player to not see text chat with anyone in Instance A
                }
            }
        }
    }
}
