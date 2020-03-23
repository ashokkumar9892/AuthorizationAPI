using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthorizationAPI.Models
{
    public enum PackageStateEnum
    {
        NeverScheduled = -9, //never scheduled during any available windows
        NoShow = -8, // scheduled, but did not create exam data during the allowed window. 
        DoNotDisplay = -1,

        InProgress = 0,

        WaitingForScheduleStartDate = 3, // Step 1.
        WaitingForScheduledDate = 5, // Step 3
        ReadyToSchedule = 2, // Step 2
        ReadyToLaunch = 1, // Step 4. 15 minutes before the start time

        NotReady = 4,

        PackageComplete = 9,
        LockDownBrowserRequired = 10
    }

    public class PackageState
    {
        public List<Action> Actions = new List<Action>();

        public List<LaunchRuleState> LaunchRuleStates = new List<LaunchRuleState>();

        public List<Message> Messages = new List<Message>();
        public PackageStateEnum State = PackageStateEnum.DoNotDisplay;
        public bool ReadyToLaunch => LaunchRuleStates.All(lrs => lrs.Complete);

        public LaunchRuleState GetLaunchRuleState(int ruleTypeId)
        {
            return LaunchRuleStates.FirstOrDefault(rule => rule.RuleTypeId == ruleTypeId);
        }
    }

    public class Action
    {
        public string Url { get; set; }
        public string ActionName { get; set; }
        public string Label { get; set; }
        public List<Message> Messages { get; set; }
    }

    public class Message
    {
        public string Data;
        public MessageTypeEnum MessageType;
        public string Text;
    }

    public enum MessageTypeEnum
    {
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public class LaunchRuleState
    {
        public bool Complete;
        public DateTime DateCompleted;
        public int RuleTypeId;
    }
}